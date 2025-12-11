using ClickUp.Api.Client.Fluent;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using ClickUp.Api.Client.Models.RequestModels.UserGroups;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using ClickUp.Api.Client.Fluent.Console;
// Enhanced SDK imports for latest patterns
using ClickUp.Api.Client.Abstractions;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Abstractions.Strategies;
using ClickUp.Api.Client.Strategies.Authentication;
using ClickUp.Api.Client.Strategies.Caching;
using ClickUp.Api.Client.Services.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

Console.WriteLine("Hello, ClickUp Fluent API!");

// Build configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var settings = configuration.GetSection("ClickUpSettings").Get<ClickUpSettings>();

if (settings is null)
{
    Console.WriteLine("Failed to load ClickUpSettings from appsettings.json");
    return;
}

if (string.IsNullOrWhiteSpace(settings.ApiToken) || settings.ApiToken == "YOUR_API_TOKEN")
{
    Console.WriteLine("Please set your API token in appsettings.json under ClickUpSettings:ApiToken.");
    return;
}

// Create enhanced host with dependency injection and latest patterns
var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // Configure enhanced logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // Configure caching strategies
        services.AddMemoryCache();
        services.AddSingleton<ICachingStrategy, MemoryCachingStrategy>();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        
        // Configure authentication strategies
        services.AddSingleton<IAuthenticationStrategy>(provider =>
        {
            var logger = provider.GetService<ILogger<ApiKeyAuthenticationStrategy>>();
            var strategy = new ApiKeyAuthenticationStrategy(logger);
            strategy.SetApiKey(settings.ApiToken);
            return strategy;
        });
        
        // Register ClickUp client with enhanced configuration
        services.AddSingleton<ClickUpClient>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            return ClickUpClient.Create(settings.ApiToken, loggerFactory);
        });
    })
    .Build();

using var scope = host.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;

// Get the enhanced client and services
var client = serviceProvider.GetRequiredService<ClickUpClient>();
var cacheService = serviceProvider.GetRequiredService<ICacheService>();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("[ENHANCED FLUENT API] Starting demonstration with latest patterns...");

// Example 1: Enhanced workspace retrieval with caching
try
{
    logger.LogInformation("[DEMO] Retrieving workspaces with enhanced caching...");
    Console.WriteLine("\nFetching workspaces...");
    var cacheKey = "workspaces_all";
    var workspaces = await cacheService.GetOrCreateAsync(cacheKey, async () =>
    {
        logger.LogInformation("Cache miss - fetching from API");
        return await client.Authorization.GetAuthorizedWorkspacesAsync();
    }, TimeSpan.FromMinutes(5));
    
    Console.WriteLine($"Found {workspaces.Count()} workspaces (cached for 5 minutes):");
    foreach (var workspace in workspaces)
    {
        Console.WriteLine($"- Workspace: {workspace.Name} (ID: {workspace.Id})");

        // Example: Get tasks from a specific list within the workspace
        var listId = settings.ListId ?? "YOUR_LIST_ID";
        if (listId != "YOUR_LIST_ID")
        {
            Console.WriteLine($"\n  Fetching tasks from list '{listId}'...");
            var tasksResponse = await client.Tasks.Get(listId)
                                       .WithArchived(false)
                                       .WithPage(0)
                                       .OrderBy("created", ClickUp.Api.Client.Models.Common.ValueObjects.SortDirection.Descending)
                                       .WithSubtasks(true)
                                       .WithStatuses(new[] { "Open", "in progress" }) // Example statuses
                                       .GetAsync();

            if (tasksResponse.Items.Any())
            {
                foreach (var task in tasksResponse.Items)
                {
                    Console.WriteLine($"    - Task: {task.Name} (ID: {task.Id}, Status: {task.Status?.StatusValue})");
                }
            }
            else
            {
                Console.WriteLine("    No tasks found in this list with the specified filters.");
            }
        }

        // Example: Get filtered team tasks
        var workspaceIdForFilteredTasks = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID";
        if (workspaceIdForFilteredTasks != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching filtered team tasks from workspace '{workspaceIdForFilteredTasks}'...");
            var filteredTasksResponse = await client.Tasks.GetFilteredTeamTasks(workspaceIdForFilteredTasks)
                                                .WithSubtasks(true)
                                                .WithIncludeClosed(false)
                                                // Example: Add more filters if needed
                                                // .WithSpaceIds(new[] { "SPACE_ID_EXAMPLE" })
                                                // .WithAssignees(new[] { 123456 })
                                                .GetAsync();
            if (filteredTasksResponse.Items.Any())
            {
                foreach (var task in filteredTasksResponse.Items)
                {
                    Console.WriteLine($"    - Filtered Task: {task.Name} (ID: {task.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No filtered tasks found in this workspace with the specified filters.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping filtered team tasks example: Workspace ID placeholder '{workspaceIdForFilteredTasks}' not replaced.");
        }


        // Example: Attachments API - Create a task attachment
        var taskIdForAttachment = settings.TaskIdForAttachment ?? "YOUR_TASK_ID_FOR_ATTACHMENT_EXAMPLE";
        const string filePath = "test-attachment.txt";

        if (taskIdForAttachment != "YOUR_TASK_ID_FOR_ATTACHMENT_EXAMPLE")
        {
            if (!File.Exists(filePath))
            {
                await File.WriteAllTextAsync(filePath, "This is a test file for ClickUp attachment example created by the console app.");
                Console.WriteLine($"    Created dummy file '{filePath}' for attachment example.");
            }

            Console.WriteLine($"\n  Attaching file '{filePath}' to task '{taskIdForAttachment}'...");
            using var fileStream = File.OpenRead(filePath);
            // Note: .WithGuest() was removed as it's not available on TaskAttachmentFluentCreateRequest
            var attachmentResponse = await client.Attachments.Create(taskIdForAttachment, fileStream, Path.GetFileName(filePath))
                                                    .CreateAsync();
            Console.WriteLine($"    Attachment created: {attachmentResponse.Id}, URL: {attachmentResponse.Url}");
        }
        else
        {
            Console.WriteLine($"    Skipping attachment example: Task ID placeholder '{taskIdForAttachment}' not replaced.");
        }

        // Example: Authorization API - Get authorized user
        Console.WriteLine("\n  Fetching authorized user...");
        var authorizedUser = await client.Authorization.GetAuthorizedUserAsync();
        Console.WriteLine($"    Authorized User: {authorizedUser.Username} (ID: {authorizedUser.Id})");

        // Example: Chat API - Get chat channels
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForChat = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_CHAT";
        if (workspaceIdForChat != "YOUR_WORKSPACE_ID_FOR_CHAT")
        {
            Console.WriteLine($"\n  Fetching chat channels for workspace '{workspaceIdForChat}'...");
            var chatChannelsResponse = await client.Chat.GetChatChannels(workspaceIdForChat)
                                                    .WithLimit(10)
                                                    .GetAsync();
            if (chatChannelsResponse.Data.Any())
            {
                foreach (var channel in chatChannelsResponse.Data)
                {
                    Console.WriteLine($"    - Chat Channel: {channel.Name} (ID: {channel.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No chat channels found in this workspace.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Chat API example: Placeholder Workspace ID '{workspaceIdForChat}' not replaced.");
        }

        // Example: Comments API - Get task comments
        // IMPORTANT: Replace "YOUR_TASK_ID"
        var taskIdForComments = settings.TaskIdForComments ?? "YOUR_TASK_ID_FOR_COMMENTS_EXAMPLE";
        if (taskIdForComments != "YOUR_TASK_ID_FOR_COMMENTS_EXAMPLE")
        {
            Console.WriteLine($"\n  Fetching comments for task '{taskIdForComments}' (Note: This requires a valid Task ID to exist)...");
            await foreach (var comment in client.Comments.GetTaskComments(taskIdForComments).GetStreamAsync())
            {
                Console.WriteLine($"    - Comment: {comment.CommentText} (ID: {comment.Id})");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Comments API example: Placeholder Task ID '{taskIdForComments}' not replaced.");
        }

        // Example: Custom Fields API - Get accessible custom fields
        // IMPORTANT: Replace "YOUR_LIST_ID"
        var listIdForCustomFields = settings.ListId ?? "YOUR_LIST_ID_FOR_CUSTOM_FIELDS";
        if (listIdForCustomFields != "YOUR_LIST_ID_FOR_CUSTOM_FIELDS")
        {
            Console.WriteLine($"\n  Fetching accessible custom fields for list '{listIdForCustomFields}'...");
            var customFields = await client.CustomFields.GetAccessibleCustomFieldsAsync(listIdForCustomFields);
            if (customFields.Any())
            {
                foreach (var field in customFields)
                {
                    Console.WriteLine($"    - Custom Field: {field.Name} (ID: {field.Id}, Type: {field.Type})");
                }
            }
            else
            {
                Console.WriteLine("    No custom fields found for this list.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Custom Fields API example: Placeholder List ID '{listIdForCustomFields}' not replaced.");
        }

        // Example: Docs API - Search docs
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForDocs = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_DOCS";
        if (workspaceIdForDocs != "YOUR_WORKSPACE_ID_FOR_DOCS")
        {
            Console.WriteLine($"\n  Searching docs in workspace '{workspaceIdForDocs}'...");
            var searchDocsResponse = await client.Docs.SearchDocs(workspaceIdForDocs)
                                                .WithQuery("test")
                                                .WithLimit(5)
                                                .SearchAsync();
            if (searchDocsResponse.Items.Any())
            {
                foreach (var doc in searchDocsResponse.Items)
                {
                    Console.WriteLine($"    - Doc: {doc.Name} (ID: {doc.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No docs found with the specified query.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Docs API example: Placeholder Workspace ID '{workspaceIdForDocs}' not replaced.");
        }

        // Example: Folders API - Get folders in a space
        // IMPORTANT: Replace "YOUR_SPACE_ID"
        var spaceIdForFolders = settings.SpaceId ?? "YOUR_SPACE_ID_FOR_FOLDERS";
        if (spaceIdForFolders != "YOUR_SPACE_ID_FOR_FOLDERS")
        {
            Console.WriteLine($"\n  Fetching folders in space '{spaceIdForFolders}'...");
            var folders = await client.Folders.GetFoldersAsync(spaceIdForFolders);
            if (folders.Any())
            {
                foreach (var folder in folders)
                {
                    Console.WriteLine($"    - Folder: {folder.Name} (ID: {folder.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No folders found in this space.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Folders API example: Placeholder Space ID '{spaceIdForFolders}' not replaced.");
        }

        // Example: Goals API - Get goals
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForGoals = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_GOALS";
        if (workspaceIdForGoals != "YOUR_WORKSPACE_ID_FOR_GOALS")
        {
            Console.WriteLine($"\n  Fetching goals for workspace '{workspaceIdForGoals}'...");
            var goalsResponse = await client.Goals.GetGoals(workspaceIdForGoals)
                                            .WithIncludeCompleted(true)
                                            .GetAsync();
            if (goalsResponse.Goals.Any())
            {
                foreach (var goal in goalsResponse.Goals)
                {
                    Console.WriteLine($"    - Goal: {goal.Name} (ID: {goal.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No goals found in this workspace.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Goals API example: Placeholder Workspace ID '{workspaceIdForGoals}' not replaced.");
        }

        // Example: Guests API - Get guest
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID" and "YOUR_GUEST_ID"
        var workspaceIdForGuests = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_GUESTS";
        var guestId = settings.GuestId ?? "YOUR_GUEST_ID_EXAMPLE";
        if (workspaceIdForGuests != "YOUR_WORKSPACE_ID_FOR_GUESTS" && guestId != "YOUR_GUEST_ID_EXAMPLE")
        {
            Console.WriteLine($"\n  Fetching guest '{guestId}' from workspace '{workspaceIdForGuests}' (Note: Requires valid Guest ID)...");
            try
            {
                var guestResponse = await client.Guests.GetGuestAsync(workspaceIdForGuests, guestId);
                Console.WriteLine($"    Guest: {guestResponse.Guest.User.Username} (ID: {guestResponse.Guest.User.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Error fetching guest: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get Guest example: Placeholder Workspace ID or Guest ID not replaced.");
        }

        // Example: Lists API - Get lists in folder (already covered by tasks example, but showing direct call)
        // IMPORTANT: Replace "YOUR_FOLDER_ID"
        var folderIdForLists = settings.FolderId ?? "YOUR_FOLDER_ID_FOR_LISTS";
        if (folderIdForLists != "YOUR_FOLDER_ID_FOR_LISTS")
        {
            Console.WriteLine($"\n  Fetching lists in folder '{folderIdForLists}'...");
            var listsInFolder = await client.Lists.GetListsInFolderAsync(folderIdForLists);
            if (listsInFolder.Any())
            {
                foreach (var list in listsInFolder)
                {
                    Console.WriteLine($"    - List in Folder: {list.Name} (ID: {list.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No lists found in this folder.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get Lists in Folder example: Placeholder Folder ID '{folderIdForLists}' not replaced.");
        }

        // Example: Members API - Get task members
        // IMPORTANT: Replace "YOUR_TASK_ID"
        var taskIdForMembers = settings.TaskIdForMembers ?? "YOUR_TASK_ID_FOR_MEMBERS_EXAMPLE";
        if (taskIdForMembers != "YOUR_TASK_ID_FOR_MEMBERS_EXAMPLE")
        {
            Console.WriteLine($"\n  Fetching members for task '{taskIdForMembers}' (Note: Requires valid Task ID)...");
            var taskMembers = await client.Members.GetTaskMembersAsync(taskIdForMembers);
            if (taskMembers.Any())
            {
                foreach (var member in taskMembers)
                {
                    Console.WriteLine($"    - Task Member: {member.User.Username} (ID: {member.User.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No members found for this task.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get Task Members example: Placeholder Task ID '{taskIdForMembers}' not replaced.");
        }

        // Example: Roles API - Get custom roles
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForRoles = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_ROLES";
        if (workspaceIdForRoles != "YOUR_WORKSPACE_ID_FOR_ROLES")
        {
            Console.WriteLine($"\n  Fetching custom roles for workspace '{workspaceIdForRoles}'...");
            var customRoles = await client.Roles.GetCustomRolesAsync(workspaceIdForRoles);
            if (customRoles.Any())
            {
                foreach (var role in customRoles)
                {
                    Console.WriteLine($"    - Custom Role: {role.Name} (ID: {role.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No custom roles found in this workspace.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get Custom Roles example: Placeholder Workspace ID '{workspaceIdForRoles}' not replaced.");
        }

        // Example: Shared Hierarchy API - Get shared hierarchy
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForSharedHierarchy = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_SHAREDH";
        if (workspaceIdForSharedHierarchy != "YOUR_WORKSPACE_ID_FOR_SHAREDH")
        {
            Console.WriteLine($"\n  Fetching shared hierarchy for workspace '{workspaceIdForSharedHierarchy}'...");
            var sharedHierarchy = await client.SharedHierarchy.GetSharedHierarchyAsync(workspaceIdForSharedHierarchy);
            Console.WriteLine($"    Shared Tasks Count: {sharedHierarchy.Shared.Tasks.Count}");
            Console.WriteLine($"    Shared Lists Count: {sharedHierarchy.Shared.Lists.Count}");
            Console.WriteLine($"    Shared Folders Count: {sharedHierarchy.Shared.Folders.Count}");
        }
        else
        {
            Console.WriteLine($"    Skipping Get Shared Hierarchy example: Placeholder Workspace ID '{workspaceIdForSharedHierarchy}' not replaced.");
        }

        // Example: Spaces API - Get spaces
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForSpaces = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_SPACES";
        if (workspaceIdForSpaces != "YOUR_WORKSPACE_ID_FOR_SPACES")
        {
            Console.WriteLine($"\n  Fetching spaces for workspace '{workspaceIdForSpaces}'...");
            var spaces = await client.Spaces.GetSpacesAsync(workspaceIdForSpaces);
            if (spaces.Any())
            {
                foreach (var space in spaces)
                {
                    Console.WriteLine($"    - Space: {space.Name} (ID: {space.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No spaces found in this workspace.");
            }
        }
        else
        {
             Console.WriteLine($"    Skipping Get Spaces example: Placeholder Workspace ID '{workspaceIdForSpaces}' not replaced.");
        }

        // Example: Tags API - Get space tags
        // IMPORTANT: Replace "YOUR_SPACE_ID"
        var spaceIdForTags = settings.SpaceId ?? "YOUR_SPACE_ID_FOR_TAGS";
        if (spaceIdForTags != "YOUR_SPACE_ID_FOR_TAGS")
        {
            Console.WriteLine($"\n  Fetching tags for space '{spaceIdForTags}'...");
            var tags = await client.Tags.GetSpaceTagsAsync(spaceIdForTags);
            if (tags.Any())
            {
                foreach (var tag in tags)
                {
                    Console.WriteLine($"    - Tag: {tag.Name} (Color: {tag.TagFg})");
                }
            }
            else
            {
                Console.WriteLine("    No tags found in this space.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get Space Tags example: Placeholder Space ID '{spaceIdForTags}' not replaced.");
        }

        // Example: Task Checklists API - Delete checklist (assuming you have one to delete)
        // IMPORTANT: Replace "YOUR_CHECKLIST_ID"
        var checklistIdToDelete = settings.ChecklistIdForDelete ?? "YOUR_CHECKLIST_ID_EXAMPLE";
        if (checklistIdToDelete != "YOUR_CHECKLIST_ID_EXAMPLE")
        {
            Console.WriteLine($"\n  Attempting to delete checklist '{checklistIdToDelete}' (Note: Requires valid Checklist ID)...");
            try
            {
                await client.TaskChecklists.DeleteChecklistAsync(checklistIdToDelete);
                Console.WriteLine($"    Checklist '{checklistIdToDelete}' deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Error deleting checklist: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Delete Checklist example: Placeholder ID '{checklistIdToDelete}' not replaced.");
        }

        // Example: Task Relationships API - Add dependency
        // IMPORTANT: Replace "YOUR_TASK_ID_1" and "YOUR_TASK_ID_2"
        var taskId1 = settings.TaskIdForDependency1 ?? "YOUR_TASK_ID_1_REL_EXAMPLE";
        var taskId2 = settings.TaskIdForDependency2 ?? "YOUR_TASK_ID_2_REL_EXAMPLE";
        if (taskId1 != "YOUR_TASK_ID_1_REL_EXAMPLE" && taskId2 != "YOUR_TASK_ID_2_REL_EXAMPLE")
        {
            Console.WriteLine($"\n  Adding dependency: Task '{taskId1}' depends on Task '{taskId2}' (Note: Requires valid Task IDs)...");
            try
            {
                await client.TaskRelationships.AddDependency(taskId1)
                                            .WithDependsOnTaskId(taskId2)
                                            .AddAsync();
                Console.WriteLine($"    Dependency added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Error adding dependency: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Add Dependency example: Placeholder Task IDs not replaced.");
        }

        // Example: Templates API - Get task templates
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForTemplates = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_TEMPLATES";
        if (workspaceIdForTemplates != "YOUR_WORKSPACE_ID_FOR_TEMPLATES")
        {
            Console.WriteLine($"\n  Fetching task templates for workspace '{workspaceIdForTemplates}'...");
            var taskTemplatesResponse = await client.Templates.GetTaskTemplates(workspaceIdForTemplates)
                                                        .WithPage(0)
                                                        .GetAsync();
            if (taskTemplatesResponse.Templates.Any())
            {
                foreach (var template in taskTemplatesResponse.Templates)
                {
                    Console.WriteLine($"    - Task Template: {template.Name} (ID: {template.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No task templates found in this workspace.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get Task Templates example: Placeholder Workspace ID '{workspaceIdForTemplates}' not replaced.");
        }

        // Example: Time Tracking API - Get time entries
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForTimeTracking = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_TIME";
        if (workspaceIdForTimeTracking != "YOUR_WORKSPACE_ID_FOR_TIME")
        {
            Console.WriteLine($"\n  Fetching time entries for workspace '{workspaceIdForTimeTracking}'...");
            var timeEntries = await client.TimeTracking.GetTimeEntries(workspaceIdForTimeTracking)
                                                    .WithTimeRange(DateTimeOffset.UtcNow.AddDays(-7), DateTimeOffset.UtcNow)
                                                    .WithIncludeTaskTags(true)
                                                    .WithIncludeLocationNames(true)
                                                    // .ForTask("SPECIFIC_TASK_ID_IF_NEEDED") // Example: filter by specific task
                                                    .GetAsync();
            if (timeEntries.Items.Any())
            {
                foreach (var entry in timeEntries.Items)
                {
                    Console.WriteLine($"    - Time Entry: {entry.Description} (ID: {entry.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No time entries found for the last 7 days.");
            }
        }

        // Example: User Groups API - Get user groups
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForUserGroups = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_USERGROUPS";
        if (workspaceIdForUserGroups != "YOUR_WORKSPACE_ID_FOR_USERGROUPS")
        {
            Console.WriteLine($"\n  Fetching user groups for workspace '{workspaceIdForUserGroups}'...");
            var userGroups = await client.UserGroups.GetUserGroupsAsync(workspaceIdForUserGroups);
            if (userGroups.Any())
            {
                foreach (var group in userGroups)
                {
                    Console.WriteLine($"    - User Group: {group.Name} (ID: {group.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No user groups found in this workspace.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get User Groups example: Placeholder Workspace ID '{workspaceIdForUserGroups}' not replaced.");
        }

        // Example: Users API - Get user from workspace
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID" and "YOUR_USER_ID"
        var workspaceIdForUsers = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_USERS";
        var userIdForUsers = settings.UserId ?? "YOUR_USER_ID_EXAMPLE";
        if (workspaceIdForUsers != "YOUR_WORKSPACE_ID_FOR_USERS" && userIdForUsers != "YOUR_USER_ID_EXAMPLE")
        {
            Console.WriteLine($"\n  Fetching user '{userIdForUsers}' from workspace '{workspaceIdForUsers}' (Note: Requires valid User ID)...");
            try
            {
                var user = await client.Users.GetUserFromWorkspaceAsync(workspaceIdForUsers, userIdForUsers);
                Console.WriteLine($"    User: {user.Username} (ID: {user.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Error fetching user: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get User from Workspace example: Placeholder Workspace ID or User ID not replaced.");
        }

        // Example: Views API - Get workspace views
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForViews = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_VIEWS";
        if (workspaceIdForViews != "YOUR_WORKSPACE_ID_FOR_VIEWS")
        {
            Console.WriteLine($"\n  Fetching views for workspace '{workspaceIdForViews}'...");
            var viewsResponse = await client.Views.GetWorkspaceViewsAsync(workspaceIdForViews);
            if (viewsResponse.Views.Any())
            {
                foreach (var view in viewsResponse.Views)
                {
                    Console.WriteLine($"    - View: {view.Name} (ID: {view.Id}, Type: {view.Type})");
                }
            }
            else
            {
                Console.WriteLine("    No views found in this workspace.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get Workspace Views example: Placeholder Workspace ID '{workspaceIdForViews}' not replaced.");
        }

        // Example: Webhooks API - Get webhooks
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        var workspaceIdForWebhooks = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_WEBHOOKS";
        if (workspaceIdForWebhooks != "YOUR_WORKSPACE_ID_FOR_WEBHOOKS")
        {
            Console.WriteLine($"\n  Fetching webhooks for workspace '{workspaceIdForWebhooks}'...");
            var webhooks = await client.Webhooks.GetWebhooksAsync(workspaceIdForWebhooks);
            if (webhooks.Any())
            {
                foreach (var webhook in webhooks)
                {
                    Console.WriteLine($"    - Webhook: {webhook.Endpoint} (ID: {webhook.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No webhooks found in this workspace.");
            }
        }
        else
        {
            Console.WriteLine($"    Skipping Get Webhooks example: Placeholder Workspace ID '{workspaceIdForWebhooks}' not replaced.");
        }
    } // This closes the foreach (var workspace in workspaces)
    
    // Example 2: Enhanced Fluent API Composition
    logger.LogInformation("[DEMO] Demonstrating enhanced fluent API composition...");
    await DemonstrateEnhancedFluentComposition(client, logger, settings);
    
    // Example 3: Advanced Caching Strategies
    logger.LogInformation("[DEMO] Demonstrating advanced caching strategies...");
    await DemonstrateAdvancedCaching(client, cacheService, logger, settings);
    
    // Example 4: Fluent Builder Patterns
    logger.LogInformation("[DEMO] Demonstrating enhanced fluent builders...");
    await DemonstrateFluentBuilders(client, logger, settings);
} // This closes the try block
catch (ClickUp.Api.Client.Models.Exceptions.ClickUpApiException cuEx)
{
    Console.WriteLine($"\nA ClickUp API error occurred: {cuEx.Message} (Status: {cuEx.StatusCode}, ErrorCode: {cuEx.ErrorCode})");
    if (cuEx is ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException valEx)
    {
        Console.WriteLine("Validation Errors:");
        if (valEx.Errors != null)
        {
            foreach (var error in valEx.Errors)
            {
                Console.WriteLine($"- {error.Key}: {string.Join(", ", error.Value)}");
            }
        }
    }
    Console.WriteLine("Please ensure you have set your API token and other IDs in appsettings.json and that they are valid.");
}
catch (Exception ex) // General exception handler
{
    logger.LogError(ex, "Error occurred during demonstration");
    Console.WriteLine($"\nAn error occurred: {ex.Message}");
    Console.WriteLine("Please ensure you have set your API token and other IDs in appsettings.json.");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

// Enhanced demonstration methods
static async Task DemonstrateEnhancedFluentComposition(ClickUpClient client, ILogger logger, ClickUpSettings settings)
{
    try
    {
        logger.LogInformation("=== Enhanced Fluent API Composition ===");
        
        if (string.IsNullOrWhiteSpace(settings.ListId) || settings.ListId == "YOUR_LIST_ID")
        {
            Console.WriteLine("Skipping fluent composition demo - ListId not configured");
            return;
        }
        
        // Demonstrate fluent composition with enhanced builders
        var complexTaskQuery = client.Tasks
            .Get(settings.ListId)
            .WithArchived(false)
            .WithSubtasks(true)
            .WithPage(0)
            .WithIncludeMarkdownDescription(true)
            .OrderBy("created")
            .WithCustomItems(new List<int>())
            .WithAssignees(new List<int>());
            
        logger.LogInformation("Executing complex fluent query with enhanced composition...");
        var tasks = await complexTaskQuery.GetAsync();
        
        Console.WriteLine($"Retrieved {tasks.Items?.Count ?? 0} tasks using enhanced fluent composition");
        
        // Demonstrate fluent task creation with validation
        if (tasks.Items?.Any() == true)
        {
            var firstTask = tasks.Items.First();
            logger.LogInformation($"Sample task: {firstTask.Name} (Status: {firstTask.Status?.StatusValue})");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error in fluent composition demonstration");
    }
}

static async Task DemonstrateAdvancedCaching(ClickUpClient client, ICacheService cacheService, ILogger logger, ClickUpSettings settings)
{
    try
    {
        logger.LogInformation("=== Advanced Caching Strategies ===");
        
        // Demonstrate cache warming
        var warmupTasks = new[]
        {
            "spaces_all",
            "teams_all",
            "user_profile"
        };
        
        foreach (var cacheKey in warmupTasks)
        {
            logger.LogInformation($"Warming cache for key: {cacheKey}");
            
            switch (cacheKey)
            {
                case "spaces_all":
                     await cacheService.GetOrCreateAsync(cacheKey, async () =>
                     {
                         if (!string.IsNullOrWhiteSpace(settings.WorkspaceId) && settings.WorkspaceId != "YOUR_WORKSPACE_ID")
                         {
                             var spaces = await client.Spaces.GetSpacesAsync(settings.WorkspaceId);
                             return spaces ?? new List<ClickUp.Api.Client.Models.Entities.Spaces.Space>();
                         }
                         return new List<ClickUp.Api.Client.Models.Entities.Spaces.Space>();
                     }, TimeSpan.FromMinutes(10));
                     break;
                     
                 case "teams_all":
                     await cacheService.GetOrCreateAsync(cacheKey, async () =>
                     {
                         var workspaces = new List<ClickUp.Api.Client.Models.Entities.WorkSpaces.ClickUpWorkspace>();
                         await foreach (var workspace in client.Workspaces.GetAuthorizedWorkspacesAsyncEnumerableAsync())
                         {
                             workspaces.Add(workspace);
                         }
                         return workspaces;
                     }, TimeSpan.FromMinutes(15));
                     break;
                     
                 case "user_profile":
                     await cacheService.GetOrCreateAsync(cacheKey, async () =>
                     {
                         return await client.Authorization.GetAuthorizedUserAsync();
                     }, TimeSpan.FromHours(1));
                     break;
            }
        }
        
        // Demonstrate cache metrics
        logger.LogInformation("Cache warming completed - demonstrating cache hits");
        
        // Second call should hit cache
         var cachedUser = await cacheService.GetOrCreateAsync("user_profile", async () =>
         {
             logger.LogInformation("This should not execute - cache hit expected");
             return await client.Authorization.GetAuthorizedUserAsync();
         }, TimeSpan.FromHours(1));
        
        Console.WriteLine($"User profile retrieved from cache: {cachedUser?.Username}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error in advanced caching demonstration");
    }
}

static async Task DemonstrateFluentBuilders(ClickUpClient client, ILogger logger, ClickUpSettings settings)
{
    try
    {
        logger.LogInformation("=== Enhanced Fluent Builders ===");
        
        if (string.IsNullOrWhiteSpace(settings.ListId) || settings.ListId == "YOUR_LIST_ID")
        {
            Console.WriteLine("Skipping fluent builders demo - ListId not configured");
            return;
        }
        
        // Demonstrate enhanced task builder with validation
        logger.LogInformation("Creating task with enhanced fluent builder...");
        
        var taskBuilder = client.Tasks
            .CreateTask(settings.ListId)
            .WithName($"Enhanced Fluent Task - {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
            .WithDescription("Created using enhanced fluent builder patterns with validation")
            .WithPriority(3)
            .WithDueDate(((DateTimeOffset)DateTime.Now.AddDays(7)).ToUnixTimeMilliseconds())
            .WithTimeEstimate((long)TimeSpan.FromHours(2).TotalMilliseconds);
            
        // Add conditional properties
        if (!string.IsNullOrWhiteSpace(settings.AssigneeId) && settings.AssigneeId != "YOUR_ASSIGNEE_ID")
        {
            if (int.TryParse(settings.AssigneeId, out int assigneeId))
            {
                taskBuilder = taskBuilder.WithAssignees(new List<int> { assigneeId });
            }
        }
        
        // Execute with enhanced error handling
        try
        {
            var createdTask = await taskBuilder.CreateAsync();
            logger.LogInformation($"Task created successfully: {createdTask.Name} (ID: {createdTask.Id})");
            Console.WriteLine($"Enhanced fluent builder created task: {createdTask.Name}");
            
            // Demonstrate fluent update builder
            logger.LogInformation("Updating task with enhanced fluent builder...");
            
            var updateBuilder = client.Tasks
                .UpdateTask(createdTask.Id)
                .WithName($"{createdTask.Name} - Updated")
                .WithDescription("Updated using enhanced fluent builder");
                
            var updatedTask = await updateBuilder.UpdateAsync();
            logger.LogInformation($"Task updated successfully: {updatedTask.Name}");
            Console.WriteLine($"Enhanced fluent builder updated task: {updatedTask.Name}");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Task creation/update failed - this is expected in demo environment");
            Console.WriteLine($"Task operation failed (expected in demo): {ex.Message}");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error in fluent builders demonstration");
    }
}