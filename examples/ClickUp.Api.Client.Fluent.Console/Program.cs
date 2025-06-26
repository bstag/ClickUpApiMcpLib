using ClickUp.Api.Client.Fluent;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO; // For Stream
using ClickUp.Api.Client.Models.RequestModels.Docs; // For ParentDocIdentifier
using ClickUp.Api.Client.Models.RequestModels.UserGroups; // For UserGroupMembersUpdate

Console.WriteLine("Hello, ClickUp Fluent API!");

// You'll need a logger factory. For simple cases, you can use NullLoggerFactory.Instance.
var loggerFactory = NullLoggerFactory.Instance;

// Create a client with your API token
// IMPORTANT: Replace "YOUR_API_TOKEN" with your actual ClickUp API token.
var client = ClickUpClient.Create("YOUR_API_TOKEN", loggerFactory);

// Example: Get all workspaces
try
{
    Console.WriteLine("\nFetching workspaces...");
    var workspacesResponse = await client.Workspaces.GetAsync();
    foreach (var workspace in workspacesResponse.Workspaces)
    {
        Console.WriteLine($"- Workspace: {workspace.Name} (ID: {workspace.Id})");

        // Example: Get tasks from a specific list within the workspace
        // IMPORTANT: Replace "YOUR_LIST_ID" with an actual list ID from your workspace.
        const string listId = "YOUR_LIST_ID";
        if (listId != "YOUR_LIST_ID")
        {
            Console.WriteLine($"\n  Fetching tasks from list '{listId}'...");
            var tasksResponse = await client.Tasks.Get(listId)
                                       .WithArchived(false)
                                       .WithPage(0)
                                       .WithOrderBy("created")
                                       .WithReverse(true)
                                       .WithSubtasks(true)
                                       .WithStatuses(new[] { "open", "in progress" })
                                       .GetAsync();

            if (tasksResponse.Tasks.Any())
            {
                foreach (var task in tasksResponse.Tasks)
                {
                    Console.WriteLine($"    - Task: {task.Name} (ID: {task.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No tasks found in this list with the specified filters.");
            }
        }

        // Example: Get filtered team tasks
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID" with an actual workspace ID.
        const string workspaceId = "YOUR_WORKSPACE_ID";
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching filtered team tasks from workspace '{workspaceId}'...");
            var filteredTasksResponse = await client.Tasks.GetFilteredTeamTasks(workspaceId)
                                                .WithSubtasks(true)
                                                .WithIncludeClosed(false)
                                                .GetAsync();
            if (filteredTasksResponse.Tasks.Any())
            {
                foreach (var task in filteredTasksResponse.Tasks)
                {
                    Console.WriteLine($"    - Filtered Task: {task.Name} (ID: {task.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No filtered tasks found in this workspace.");
            }
        }

        // Example: Attachments API - Create a task attachment
        // IMPORTANT: Replace "YOUR_TASK_ID" and "PATH_TO_YOUR_FILE.txt"
        const string taskIdForAttachment = "YOUR_TASK_ID";
        const string filePath = "PATH_TO_YOUR_FILE.txt";
        if (taskIdForAttachment != "YOUR_TASK_ID" && File.Exists(filePath))
        {
            Console.WriteLine($"\n  Attaching file '{filePath}' to task '{taskIdForAttachment}'...");
            using var fileStream = File.OpenRead(filePath);
            var attachmentResponse = await client.Attachments.Create(taskIdForAttachment, fileStream, Path.GetFileName(filePath))
                                                    .CreateAsync();
            Console.WriteLine($"    Attachment created: {attachmentResponse.Id}");
        }
        else if (taskIdForAttachment != "YOUR_TASK_ID")
        {
            Console.WriteLine($"    Skipping attachment example: File '{filePath}' not found.");
        }

        // Example: Authorization API - Get authorized user
        Console.WriteLine("\n  Fetching authorized user...");
        var authorizedUser = await client.Authorization.GetAuthorizedUserAsync();
        Console.WriteLine($"    Authorized User: {authorizedUser.Username} (ID: {authorizedUser.Id})");

        // Example: Chat API - Get chat channels
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching chat channels for workspace '{workspaceId}'...");
            var chatChannelsResponse = await client.Chat.GetChatChannels(workspaceId)
                                                    .WithLimit(10)
                                                    .GetAsync();
            if (chatChannelsResponse.Channels.Any())
            {
                foreach (var channel in chatChannelsResponse.Channels)
                {
                    Console.WriteLine($"    - Chat Channel: {channel.Name} (ID: {channel.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No chat channels found in this workspace.");
            }
        }

        // Example: Comments API - Get task comments
        // IMPORTANT: Replace "YOUR_TASK_ID"
        const string taskIdForComments = "YOUR_TASK_ID";
        if (taskIdForComments != "YOUR_TASK_ID")
        {
            Console.WriteLine($"\n  Fetching comments for task '{taskIdForComments}'...");
            await foreach (var comment in client.Comments.GetTaskComments(taskIdForComments).GetStreamAsync())
            {
                Console.WriteLine($"    - Comment: {comment.CommentText} (ID: {comment.Id})");
            }
        }

        // Example: Custom Fields API - Get accessible custom fields
        // IMPORTANT: Replace "YOUR_LIST_ID"
        const string listIdForCustomFields = "YOUR_LIST_ID";
        if (listIdForCustomFields != "YOUR_LIST_ID")
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

        // Example: Docs API - Search docs
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Searching docs in workspace '{workspaceId}'...");
            var searchDocsResponse = await client.Docs.SearchDocs(workspaceId)
                                                .WithQuery("test")
                                                .WithLimit(5)
                                                .SearchAsync();
            if (searchDocsResponse.Docs.Any())
            {
                foreach (var doc in searchDocsResponse.Docs)
                {
                    Console.WriteLine($"    - Doc: {doc.Name} (ID: {doc.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No docs found with the specified query.");
            }
        }

        // Example: Folders API - Get folders in a space
        // IMPORTANT: Replace "YOUR_SPACE_ID"
        const string spaceIdForFolders = "YOUR_SPACE_ID";
        if (spaceIdForFolders != "YOUR_SPACE_ID")
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

        // Example: Goals API - Get goals
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching goals for workspace '{workspaceId}'...");
            var goalsResponse = await client.Goals.GetGoals(workspaceId)
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

        // Example: Guests API - Get guest
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID" and "YOUR_GUEST_ID"
        const string guestId = "YOUR_GUEST_ID";
        if (workspaceId != "YOUR_WORKSPACE_ID" && guestId != "YOUR_GUEST_ID")
        {
            Console.WriteLine($"\n  Fetching guest '{guestId}' from workspace '{workspaceId}'...");
            try
            {
                var guestResponse = await client.Guests.GetGuestAsync(workspaceId, guestId);
                Console.WriteLine($"    Guest: {guestResponse.Guest.Username} (ID: {guestResponse.Guest.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Error fetching guest: {ex.Message}");
            }
        }

        // Example: Lists API - Get lists in folder (already covered by tasks example, but showing direct call)
        // IMPORTANT: Replace "YOUR_FOLDER_ID"
        const string folderIdForLists = "YOUR_FOLDER_ID";
        if (folderIdForLists != "YOUR_FOLDER_ID")
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

        // Example: Members API - Get task members
        // IMPORTANT: Replace "YOUR_TASK_ID"
        const string taskIdForMembers = "YOUR_TASK_ID";
        if (taskIdForMembers != "YOUR_TASK_ID")
        {
            Console.WriteLine($"\n  Fetching members for task '{taskIdForMembers}'...");
            var taskMembers = await client.Members.GetTaskMembersAsync(taskIdForMembers);
            if (taskMembers.Any())
            {
                foreach (var member in taskMembers)
                {
                    Console.WriteLine($"    - Task Member: {member.Username} (ID: {member.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No members found for this task.");
            }
        }

        // Example: Roles API - Get custom roles
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching custom roles for workspace '{workspaceId}'...");
            var customRoles = await client.Roles.GetCustomRolesAsync(workspaceId);
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

        // Example: Shared Hierarchy API - Get shared hierarchy
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching shared hierarchy for workspace '{workspaceId}'...");
            var sharedHierarchy = await client.SharedHierarchy.GetSharedHierarchyAsync(workspaceId);
            Console.WriteLine($"    Shared Tasks Count: {sharedHierarchy.Tasks.Count}");
            Console.WriteLine($"    Shared Lists Count: {sharedHierarchy.Lists.Count}");
            Console.WriteLine($"    Shared Folders Count: {sharedHierarchy.Folders.Count}");
        }

        // Example: Spaces API - Get spaces
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching spaces for workspace '{workspaceId}'...");
            var spaces = await client.Spaces.GetSpacesAsync(workspaceId);
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

        // Example: Tags API - Get space tags
        // IMPORTANT: Replace "YOUR_SPACE_ID"
        const string spaceIdForTags = "YOUR_SPACE_ID";
        if (spaceIdForTags != "YOUR_SPACE_ID")
        {
            Console.WriteLine($"\n  Fetching tags for space '{spaceIdForTags}'...");
            var tags = await client.Tags.GetSpaceTagsAsync(spaceIdForTags);
            if (tags.Any())
            {
                foreach (var tag in tags)
                {
                    Console.WriteLine($"    - Tag: {tag.Name} (Color: {tag.TagForegroundColor})");
                }
            }
            else
            {
                Console.WriteLine("    No tags found in this space.");
            }
        }

        // Example: Task Checklists API - Delete checklist (assuming you have one to delete)
        // IMPORTANT: Replace "YOUR_CHECKLIST_ID"
        const string checklistIdToDelete = "YOUR_CHECKLIST_ID";
        if (checklistIdToDelete != "YOUR_CHECKLIST_ID")
        {
            Console.WriteLine($"\n  Attempting to delete checklist '{checklistIdToDelete}'...");
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

        // Example: Task Relationships API - Add dependency
        // IMPORTANT: Replace "YOUR_TASK_ID_1" and "YOUR_TASK_ID_2"
        const string taskId1 = "YOUR_TASK_ID_1";
        const string taskId2 = "YOUR_TASK_ID_2";
        if (taskId1 != "YOUR_TASK_ID_1" && taskId2 != "YOUR_TASK_ID_2")
        {
            Console.WriteLine($"\n  Adding dependency: Task '{taskId1}' depends on Task '{taskId2}'...");
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

        // Example: Templates API - Get task templates
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching task templates for workspace '{workspaceId}'...");
            var taskTemplatesResponse = await client.Templates.GetTaskTemplates(workspaceId)
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

        // Example: Time Tracking API - Get time entries
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching time entries for workspace '{workspaceId}'...");
            var timeEntries = await client.TimeTracking.GetTimeEntries(workspaceId)
                                                    .WithStartDate(DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds())
                                                    .WithEndDate(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
                                                    .GetAsync();
            if (timeEntries.Any())
            {
                foreach (var entry in timeEntries)
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
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching user groups for workspace '{workspaceId}'...");
            var userGroups = await client.UserGroups.GetUserGroupsAsync(workspaceId);
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

        // Example: Users API - Get user from workspace
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID" and "YOUR_USER_ID"
        const string userIdForUsers = "YOUR_USER_ID";
        if (workspaceId != "YOUR_WORKSPACE_ID" && userIdForUsers != "YOUR_USER_ID")
        {
            Console.WriteLine($"\n  Fetching user '{userIdForUsers}' from workspace '{workspaceId}'...");
            try
            {
                var user = await client.Users.GetUserFromWorkspaceAsync(workspaceId, userIdForUsers);
                Console.WriteLine($"    User: {user.Username} (ID: {user.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Error fetching user: {ex.Message}");
            }
        }

        // Example: Views API - Get workspace views
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching views for workspace '{workspaceId}'...");
            var viewsResponse = await client.Views.GetWorkspaceViewsAsync(workspaceId);
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

        // Example: Webhooks API - Get webhooks
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        if (workspaceId != "YOUR_WORKSPACE_ID")
        {
            Console.WriteLine($"\n  Fetching webhooks for workspace '{workspaceId}'...");
            var webhooks = await client.Webhooks.GetWebhooksAsync(workspaceId);
            if (webhooks.Any())
            {
                foreach (var webhook in webhooks)
                {
                    Console.WriteLine($"    - Webhook: {webhook.Url} (ID: {webhook.Id})");
                }
            }
            else
            {
                Console.WriteLine("    No webhooks found in this workspace.");
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\nAn error occurred: {ex.Message}");
    Console.WriteLine("Please ensure you have replaced 'YOUR_API_TOKEN' and other placeholder IDs with your actual data.");
}