using System;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using ClickUp.Api.Client.Models.RequestModels.Lists;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using Xunit.Abstractions;

namespace ClickUp.Api.Client.IntegrationTests.TestInfrastructure
{
    public class TestHierarchyContext
    {
        public string SpaceId { get; set; } = null!;
        public string FolderId { get; set; } = null!;
        public string ListId { get; set; } = null!;
        public string TaskId { get; set; } = null!;
        // Add other relevant entities if needed, e.g., user, statuses from list
    }

    public static class TestHierarchyHelper
    {
        public static async Task<TestHierarchyContext> CreateFullTestHierarchyAsync(
            ISpacesService spacesService,
            IFoldersService foldersService,
            IListsService listsService,
            ITaskCrudService taskCrudService,
            string workspaceId,
            string baseName,
            ITestOutputHelper output)
        {
            var context = new TestHierarchyContext();

            // 1. Create Space
            var spaceName = $"{baseName}_Space_{Guid.NewGuid()}";
            output.LogInformation($"[HierarchyHelper] Creating test space: {spaceName} in Workspace ID: {workspaceId}");
            var createSpaceReq = new CreateSpaceRequest(spaceName, null, null);
            var space = await spacesService.CreateSpaceAsync(workspaceId, createSpaceReq);
            context.SpaceId = space.Id;
            output.LogInformation($"[HierarchyHelper] Test space created. Space ID: {context.SpaceId}");
            await Task.Delay(500); // Delay after space creation

            // 2. Create Folder
            var folderName = $"{baseName}_Folder_{Guid.NewGuid()}";
            output.LogInformation($"[HierarchyHelper] Creating test folder: {folderName} in Space ID: {context.SpaceId}");
            var createFolderReq = new CreateFolderRequest(folderName);
            var folder = await foldersService.CreateFolderAsync(context.SpaceId, createFolderReq);
            context.FolderId = folder.Id;
            output.LogInformation($"[HierarchyHelper] Test folder created. Folder ID: {context.FolderId}");
            await Task.Delay(500); // Delay after folder creation

            // 3. Create List
            var listName = $"{baseName}_List_{Guid.NewGuid()}";
            output.LogInformation($"[HierarchyHelper] Creating test list: {listName} in Folder ID: {context.FolderId}");
            var createListReq = new CreateListRequest(
                Name: listName,
                Content: null,
                MarkdownContent: null,
                DueDate: null,
                DueDateTime: null,
                Priority: null,
                Assignee: null,
                Status: null);
            var list = await listsService.CreateListInFolderAsync(context.FolderId, createListReq);
            context.ListId = list.Id;
            output.LogInformation($"[HierarchyHelper] Test list created. List ID: {context.ListId}");
            await Task.Delay(500); // Delay after list creation

            // 4. Create Task
            var taskName = $"{baseName}_Task_{Guid.NewGuid()}";
            output.LogInformation($"[HierarchyHelper] Creating test task: {taskName} in List ID: {context.ListId}");
            var createTaskReq = new CreateTaskRequest(
                Name: taskName,
                Description: null,
                Assignees: null,
                GroupAssignees: null,
                Tags: null,
                Status: null,
                Priority: null,
                DueDate: null,
                DueDateTime: null,
                TimeEstimate: null,
                StartDate: null,
                StartDateTime: null,
                NotifyAll: null,
                Parent: null,
                LinksTo: null,
                CheckRequiredCustomFields: null,
                CustomFields: null,
                CustomItemId: null,
                ListId: null);
            var task = await taskCrudService.CreateTaskAsync(context.ListId, createTaskReq);
            context.TaskId = task.Id;
            output.LogInformation($"[HierarchyHelper] Test task created. Task ID: {context.TaskId}");
            // No delay needed after the last creation step in this helper method

            return context;
        }

        public static async Task<TestHierarchyContext> CreateSpaceFolderListHierarchyAsync(
            ISpacesService spacesService,
            IFoldersService foldersService,
            IListsService listsService,
            string workspaceId,
            string baseName,
            ITestOutputHelper output)
        {
            var context = new TestHierarchyContext();

            var spaceName = $"{baseName}_Space_{Guid.NewGuid()}";
            output.LogInformation($"[HierarchyHelper] Creating test space: {spaceName} in Workspace ID: {workspaceId}");
            var createSpaceReq = new CreateSpaceRequest(spaceName, null, null);
            var space = await spacesService.CreateSpaceAsync(workspaceId, createSpaceReq);
            context.SpaceId = space.Id;
            output.LogInformation($"[HierarchyHelper] Test space created. Space ID: {context.SpaceId}");
            await Task.Delay(500); // Delay after space creation

            var folderName = $"{baseName}_Folder_{Guid.NewGuid()}";
            output.LogInformation($"[HierarchyHelper] Creating test folder: {folderName} in Space ID: {context.SpaceId}");
            var createFolderReq = new CreateFolderRequest(folderName);
            var folder = await foldersService.CreateFolderAsync(context.SpaceId, createFolderReq);
            context.FolderId = folder.Id;
            output.LogInformation($"[HierarchyHelper] Test folder created. Folder ID: {context.FolderId}");
            await Task.Delay(500); // Delay after folder creation

            var listName = $"{baseName}_List_{Guid.NewGuid()}";
            output.LogInformation($"[HierarchyHelper] Creating test list: {listName} in Folder ID: {context.FolderId}");
            var createListReq = new CreateListRequest(
                Name: listName,
                Content: null,
                MarkdownContent: null,
                DueDate: null,
                DueDateTime: null,
                Priority: null,
                Assignee: null,
                Status: null);
            var list = await listsService.CreateListInFolderAsync(context.FolderId, createListReq);
            context.ListId = list.Id;
            output.LogInformation($"[HierarchyHelper] Test list created. List ID: {context.ListId}");
            // No delay needed after the last creation step in this helper method as it's the end of this specific helper.

            return context;
        }

        public static async Task TeardownHierarchyAsync(
            ISpacesService spacesService, // Only need spaces service if we delete from top-down
            TestHierarchyContext context, // The context containing IDs to delete
            ITestOutputHelper output)
        {
            // In current integration tests, task, list, folder cleanup is often implicit
            // by deleting the parent space. So, this focuses on space deletion.
            // If more granular cleanup is needed here, add logic for tasks, lists, folders.

            if (!string.IsNullOrWhiteSpace(context?.SpaceId))
            {
                try
                {
                    output.LogInformation($"[HierarchyHelper] Deleting test space: {context.SpaceId}");
                    await spacesService.DeleteSpaceAsync(context.SpaceId);
                    output.LogInformation($"[HierarchyHelper] Test space {context.SpaceId} deleted.");
                }
                catch (Exception ex)
                {
                    output.LogError($"[HierarchyHelper] Error deleting space {context.SpaceId}: {ex.Message}", ex);
                }
            }
            else
            {
                output.LogInformation("[HierarchyHelper] No SpaceId provided in context for teardown, or context was null.");
            }
        }
    }
}
