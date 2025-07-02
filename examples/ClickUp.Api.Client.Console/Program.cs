using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Extensions;
// using Microsoft.Extensions.Configuration; // Duplicate
// using Microsoft.Extensions.DependencyInjection; // Duplicate
// using Microsoft.Extensions.Hosting; // Duplicate
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Exceptions;
using ClickUp.Api.Client.Models.RequestModels.Tasks; // For GetTasksRequest, CreateTaskRequest, UpdateTaskRequest
using ClickUp.Api.Client.Models.RequestModels.Comments; // For GetTaskCommentsRequest, CreateCommentRequest, CreateTaskCommentRequest
using System.Collections.Generic;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Abstractions.Options;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Comments;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.Entities.WorkSpaces;
// using ClickUp.Api.Client.Models.RequestModels.Tasks; // Duplicate
// using ClickUp.Api.Client.Models.RequestModels.Comments; // Duplicate


// Helper class to hold example settings
public class ExampleSettings
{
    public string? WorkspaceIdForExamples { get; set; }
    public string? SpaceIdForExamples { get; set; }
    public string? FolderIdForExamples { get; set; }
    public string? ListIdForExamples { get; set; }
    public string? TaskIdForComments { get; set; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddClickUpClient(options =>
                {
                    context.Configuration.GetSection("ClickUpApiOptions").Bind(options);
                    if (string.IsNullOrEmpty(options.PersonalAccessToken))
                    {
                        Log.Warning("ClickUp API PersonalAccessToken not configured. Please check appsettings.json.");
                    }
                });
                services.Configure<ExampleSettings>(context.Configuration.GetSection("ExampleSettings"));
            })
            .UseSerilog()
            .Build();

        Log.Information("Console Example Application Starting...");

        using var serviceScope = host.Services.CreateScope();
        var sp = serviceScope.ServiceProvider;
        var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExampleSettings>>().Value;

        string? listIdForTaskOps = settings.ListIdForExamples;
        string? taskIdForCommentOps = settings.TaskIdForComments;
        string? newTaskId = null;

        try
        {
            var authService = sp.GetRequiredService<IAuthorizationService>();
            var taskService = sp.GetRequiredService<ITasksService>();
            var commentService = sp.GetRequiredService<ICommentsService>();
            var listService = sp.GetRequiredService<IListsService>();
            var spaceService = sp.GetRequiredService<ISpacesService>();
            var folderService = sp.GetRequiredService<IFoldersService>();
            var workspaceService = sp.GetRequiredService<IWorkspacesService>();


            Log.Information("[INIT] Performing initial setup and fetching IDs if necessary...");
            User? authorizedUser = await authService.GetAuthorizedUserAsync();
            if (authorizedUser == null) { Log.Error("Authorization failed: User object is null. Exiting."); return; }
            Log.Information("[INIT] Authorized as {User}", authorizedUser.Username);

            if (string.IsNullOrWhiteSpace(listIdForTaskOps) || listIdForTaskOps.Contains("YOUR_"))
            {
                Log.Warning("[INIT] ListIdForExamples not configured. Attempting to find a default list...");
                IEnumerable<ClickUpWorkspace>? workspaces = await authService.GetAuthorizedWorkspacesAsync();
                if (workspaces != null && workspaces.Any())
                {
                    IEnumerable<Space>? spaces = await spaceService.GetSpacesAsync(workspaces.First().Id);
                    if (spaces != null && spaces.Any())
                    {
                        IEnumerable<Folder>? folders = await folderService.GetFoldersAsync(spaces.First().Id);
                        if (folders != null && folders.Any())
                        {
                            IEnumerable<ClickUpList>? lists = await listService.GetListsInFolderAsync(folders.First().Id);
                            if (lists != null && lists.Any()) listIdForTaskOps = lists.First().Id;
                        }
                        if (string.IsNullOrWhiteSpace(listIdForTaskOps))
                        {
                            IEnumerable<ClickUpList>? folderlessLists = await listService.GetFolderlessListsAsync(spaces.First().Id);
                            if (folderlessLists != null && folderlessLists.Any()) listIdForTaskOps = folderlessLists.First().Id;
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(listIdForTaskOps) || listIdForTaskOps.Contains("YOUR_"))
                { Log.Error("[INIT] Could not automatically determine a List ID. Please configure ListIdForExamples in appsettings.json. Exiting task examples."); return; }
                else { Log.Information("[INIT] Using automatically determined List ID for task operations: {ListId}", listIdForTaskOps); }
            }
            else { Log.Information("[INIT] Using List ID from settings for task operations: {ListId}", listIdForTaskOps); }

            Log.Information("\n--- Starting Task Examples for List ID: {ListId} ---", listIdForTaskOps);

            var createTaskRequest = new CreateTaskRequest(
                Name: "My New SDK Task " + DateTime.Now.Ticks,
                Description: "This is a task created by the SDK example.",
                Assignees: null, GroupAssignees: null, Tags: null, Status: null, Priority: null,
                DueDate: null, DueDateTime: null, TimeEstimate: null, StartDate: null, StartDateTime: null,
                NotifyAll: null, Parent: null, LinksTo: null, CheckRequiredCustomFields: null, CustomFields: null,
                CustomItemId: null, ListId: null
            );

            CuTask? createdTask = await taskService.CreateTaskAsync(listIdForTaskOps, createTaskRequest);
            if (createdTask != null)
            {
                newTaskId = createdTask.Id;
                taskIdForCommentOps = newTaskId;
                Log.Information("[TASKS] Successfully created task ID: {TaskId}, Name: {TaskName}", newTaskId, createdTask.Name);

                CuTask? fetchedTask = await taskService.GetTaskAsync(newTaskId, new GetTaskRequest());
                if (fetchedTask != null) Log.Information("[TASKS] Got task: {TaskName}, Status: {Status}", fetchedTask.Name, fetchedTask.Status?.StatusValue);
                else Log.Warning("[TASKS] Failed to get task {TaskId}.", newTaskId);

                var updateTaskRequest = new UpdateTaskRequest(
                    Name: createdTask.Name + " (Updated)",
                    Description: "Description updated via SDK.",
                    Status: null, Priority: null, DueDate: null, DueDateTime: null, Parent: null, TimeEstimate: null,
                    StartDate: null, StartDateTime: null, Assignees: null, GroupAssignees: null, Archived: null, CustomFields: null
                );
                CuTask? updatedTask = await taskService.UpdateTaskAsync(newTaskId, updateTaskRequest);
                if (updatedTask != null) Log.Information("[TASKS] Updated task: {TaskName}", updatedTask.Name);
                else Log.Warning("[TASKS] Failed to update task {TaskId}.", newTaskId);
            }
            else Log.Warning("[TASKS] Failed to create task.");

            Log.Information("[TASKS] 4. Getting tasks for list ID: {ListId}...", listIdForTaskOps);
            GetTasksResponse? getTasksResponse = await taskService.GetTasksAsync(listIdForTaskOps, new GetTasksRequest { IncludeClosed = true }); // Ensure GetTasksRequest is used directly
            if (getTasksResponse != null && getTasksResponse.Tasks.Any())
            {
                Log.Information("[TASKS] Found {TaskCount} tasks in list {ListId}:", getTasksResponse.Tasks.Count, listIdForTaskOps);
                foreach (var task in getTasksResponse.Tasks.Take(5))
                {
                    Log.Information("- Task ID: {TaskId}, Name: {TaskName}, Status: {Status}", task.Id, task.Name, task.Status?.StatusValue);
                }
                if (getTasksResponse.Tasks.Count > 5) Log.Information("... and more tasks not listed here.");
            } else Log.Warning("[TASKS] No tasks found in list {ListId}", listIdForTaskOps);
            // Removed filtered tasks example as it's no longer relevant or has been moved to another example.
            // TODO add back.
            if (!string.IsNullOrWhiteSpace(taskIdForCommentOps) && !taskIdForCommentOps.Contains("YOUR_") && authorizedUser != null)
            {
                Log.Information("\n--- Starting Comment Examples for Task ID: {TaskId} ---", taskIdForCommentOps);

                var createTaskCommentRequest = new CreateTaskCommentRequest( // Changed to CreateTaskCommentRequest
                    CommentText: "Hello from the SDK! This is a test comment.",
                    Assignee: authorizedUser.Id,
                    NotifyAll: false,
                    GroupAssignee: null // Added missing GroupAssignee from CreateTaskCommentRequest
                );

                CreateCommentResponse? createCommentResponse = await commentService.CreateTaskCommentAsync(taskIdForCommentOps, createTaskCommentRequest);
                if (createCommentResponse != null && !string.IsNullOrEmpty(createCommentResponse.Id))
                {
                    Log.Information("[COMMENTS] Successfully created comment. ID: {CommentId}", createCommentResponse.Id);

                    var taskCommentsEnumerable = await commentService.GetTaskCommentsAsync(new GetTaskCommentsRequest(taskIdForCommentOps));

                    if (taskCommentsEnumerable != null && taskCommentsEnumerable.Any())
                    {
                        Log.Information("[COMMENTS] Found {CommentCount} comments for task {TaskId}:", taskCommentsEnumerable.Count(), taskIdForCommentOps);
                        foreach (var comment in taskCommentsEnumerable.Take(3))
                        {
                            string commentTextToDisplay = comment.CommentText ?? string.Empty;
                            Log.Information("- Comment ID: {CommentId}, Text: {CommentText}", comment.Id ?? "UnknownId", commentTextToDisplay.Substring(0, Math.Min(50, commentTextToDisplay.Length)) + "...");
                        }
                    } else Log.Warning("[COMMENTS] No comments found for task {TaskId}", taskIdForCommentOps);
                }
                else Log.Warning("[COMMENTS] Failed to create comment on task {TaskId}.", taskIdForCommentOps);
            } else Log.Warning("\n[COMMENTS] TaskIdForComments not available, user not found, or task creation failed. Skipping comment examples.");

            if (!string.IsNullOrWhiteSpace(newTaskId))
            {
                Log.Information("\n[CLEANUP] Attempting to delete created task ID: {TaskId}...", newTaskId);
                await taskService.DeleteTaskAsync(newTaskId, new DeleteTaskRequest());
                Log.Information("[CLEANUP] Delete call for task ID: {TaskId} completed (assuming success if no exception).", newTaskId);
            }
        }
        catch (ClickUpApiException ex)
        {
            Log.Error(ex, "A ClickUp API error occurred: {ErrorMessage}", ex.Message);
            if (ex is ClickUpApiValidationException validationEx)
            {
                foreach (var error in validationEx.Errors)
                {
                    Log.Error("Validation Error: {Field} - {Message}", error.Key, string.Join(", ", error.Value));
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected error occurred.");
        }
        finally
        {
            Log.Information("\nConsole Example Application Shutting Down...");
            await host.StopAsync();
        }
    }
}