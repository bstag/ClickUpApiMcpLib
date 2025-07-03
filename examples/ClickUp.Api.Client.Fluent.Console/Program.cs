using ClickUp.Api.Client.Fluent;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO; // For Stream
using ClickUp.Api.Client.Models.RequestModels.Docs; // For ParentDocIdentifier
using ClickUp.Api.Client.Models.RequestModels.UserGroups; // For UserGroupMembersUpdate
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using ClickUp.Api.Client.Fluent.Console;

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

// You'll need a logger factory. For simple cases, you can use NullLoggerFactory.Instance.
var loggerFactory = NullLoggerFactory.Instance;

// Create a client with your API token
if (string.IsNullOrWhiteSpace(settings.ApiToken) || settings.ApiToken == "YOUR_API_TOKEN")
{
    Console.WriteLine("Please set your API token in appsettings.json under ClickUpSettings:ApiToken.");
    return;
}
var client = ClickUpClient.Create(settings.ApiToken, loggerFactory);

// Example: Get all workspaces
try
{
    Console.WriteLine("\nFetching workspaces...");
    var workspaceIdForSpaces = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID";
    var workspaces = await client.Authorization.GetAuthorizedWorkspacesAsync();
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
                                       // .WithOrderBy("created") // TODO: Re-enable/update when Fluent API supports GetTasksRequestParameters (Step 6.7)
                                       // .WithReverse(true)   // TODO: Re-enable/update
                                       .WithSubtasks(true)
                                       // .WithStatuses(new[] { "open", "in progress" }) // TODO: Re-enable/update
                                       .GetAsync();

            if (tasksResponse.Items.Any())
            {
                foreach (var task in tasksResponse.Items)
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
        const string workspaceId_filteredTasks = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID"; // Use configured or placeholder
        // if (workspaceId_filteredTasks != "YOUR_WORKSPACE_ID") // Condition removed to make code reachable
        // {
            Console.WriteLine($"\n  Fetching filtered team tasks from workspace '{workspaceId_filteredTasks}'...");
            var filteredTasksResponse = await client.Tasks.GetFilteredTeamTasks(workspaceId_filteredTasks)
                                                .WithSubtasks(true)
                                                .WithIncludeClosed(false)
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
                Console.WriteLine("    No filtered tasks found in this workspace.");
            }
        }

        // Example: Attachments API - Create a task attachment
        // IMPORTANT: Replace "YOUR_TASK_ID" and "PATH_TO_YOUR_FILE.txt"
        const string taskIdForAttachment = "YOUR_TASK_ID_FOR_ATTACHMENT_EXAMPLE"; // Use a distinct placeholder or settings.TaskIdForAttachment
        const string filePath = "test-attachment.txt"; // Placeholder file name
        // if (taskIdForAttachment != "YOUR_TASK_ID_FOR_ATTACHMENT_EXAMPLE" && File.Exists(filePath)) // Condition removed
        // For this example, we'll create a dummy file if it doesn't exist to make it runnable without manual setup.
        if (!File.Exists(filePath)) { await File.WriteAllTextAsync(filePath, "This is a test file for ClickUp attachment example."); }
        if (taskIdForAttachment != "YOUR_TASK_ID_FOR_ATTACHMENT_EXAMPLE") // Still check if user updated placeholder Task ID
        {
            Console.WriteLine($"\n  Attaching file '{filePath}' to task '{taskIdForAttachment}'...");
            using var fileStream = File.OpenRead(filePath);
            var attachmentResponse = await client.Attachments.Create(taskIdForAttachment, fileStream, Path.GetFileName(filePath))
                                                    .CreateAsync();
            Console.WriteLine($"    Attachment created: {attachmentResponse.Id}");
        }
        else
        {
            Console.WriteLine($"    Skipping attachment example: Task ID placeholder '{taskIdForAttachment}' not replaced or file '{filePath}' issue.");
        }

        // Example: Authorization API - Get authorized user
        Console.WriteLine("\n  Fetching authorized user...");
        var authorizedUser = await client.Authorization.GetAuthorizedUserAsync();
        Console.WriteLine($"    Authorized User: {authorizedUser.Username} (ID: {authorizedUser.Id})");

        // Example: Chat API - Get chat channels
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        const string workspaceIdForChat = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_CHAT";
        // if (workspaceIdForChat != "YOUR_WORKSPACE_ID_FOR_CHAT") // Condition removed
        // {
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

        // Example: Comments API - Get task comments
        // IMPORTANT: Replace "YOUR_TASK_ID"
        const string taskIdForComments = settings.TaskIdForComments ?? "YOUR_TASK_ID_FOR_COMMENTS_EXAMPLE";
        // if (taskIdForComments != "YOUR_TASK_ID_FOR_COMMENTS_EXAMPLE") // Condition removed
        // {
            Console.WriteLine($"\n  Fetching comments for task '{taskIdForComments}' (Note: This requires a valid Task ID to exist)...");
            if (taskIdForComments != "YOUR_TASK_ID_FOR_COMMENTS_EXAMPLE") // Actual check if placeholder was replaced
            {
                await foreach (var comment in client.Comments.GetTaskComments(taskIdForComments).GetStreamAsync())
                {
                    Console.WriteLine($"    - Comment: {comment.CommentText} (ID: {comment.Id})");
                }
            }
            else { Console.WriteLine($"    Skipping Comments API example: Placeholder Task ID '{taskIdForComments}' not replaced.");}
        // }

        // Example: Custom Fields API - Get accessible custom fields
        // IMPORTANT: Replace "YOUR_LIST_ID"
        const string listIdForCustomFields = settings.ListId ?? "YOUR_LIST_ID_FOR_CUSTOM_FIELDS";
        // if (listIdForCustomFields != "YOUR_LIST_ID_FOR_CUSTOM_FIELDS") // Condition removed
        // {
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
        const string workspaceIdForDocs = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_DOCS";
        // if (workspaceIdForDocs != "YOUR_WORKSPACE_ID_FOR_DOCS") // Condition removed
        // {
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

        // Example: Folders API - Get folders in a space
        // IMPORTANT: Replace "YOUR_SPACE_ID"
        const string spaceIdForFolders = settings.SpaceId ?? "YOUR_SPACE_ID_FOR_FOLDERS";
        // if (spaceIdForFolders != "YOUR_SPACE_ID_FOR_FOLDERS") // Condition removed
        // {
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
        const string workspaceIdForGoals = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_GOALS";
        // if (workspaceIdForGoals != "YOUR_WORKSPACE_ID_FOR_GOALS") // Condition removed
        // {
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

        // Example: Guests API - Get guest
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID" and "YOUR_GUEST_ID"
        const string workspaceIdForGuests = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_GUESTS";
        const string guestId = "YOUR_GUEST_ID_EXAMPLE";
        // if (workspaceIdForGuests != "YOUR_WORKSPACE_ID_FOR_GUESTS" && guestId != "YOUR_GUEST_ID_EXAMPLE") // Condition removed
        // {
            Console.WriteLine($"\n  Fetching guest '{guestId}' from workspace '{workspaceIdForGuests}' (Note: Requires valid Guest ID)...");
            if (workspaceIdForGuests != "YOUR_WORKSPACE_ID_FOR_GUESTS" && guestId != "YOUR_GUEST_ID_EXAMPLE")
            {
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

        // Example: Lists API - Get lists in folder (already covered by tasks example, but showing direct call)
        // IMPORTANT: Replace "YOUR_FOLDER_ID"
        const string folderIdForLists = settings.FolderId ?? "YOUR_FOLDER_ID_FOR_LISTS";
        // if (folderIdForLists != "YOUR_FOLDER_ID_FOR_LISTS") // Condition removed
        // {
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
        const string taskIdForMembers = "YOUR_TASK_ID_FOR_MEMBERS_EXAMPLE"; // Use distinct placeholder or settings
        // if (taskIdForMembers != "YOUR_TASK_ID_FOR_MEMBERS_EXAMPLE") // Condition removed
        // {
            Console.WriteLine($"\n  Fetching members for task '{taskIdForMembers}' (Note: Requires valid Task ID)...");
            if (taskIdForMembers != "YOUR_TASK_ID_FOR_MEMBERS_EXAMPLE")
            {
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

        // Example: Roles API - Get custom roles
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        const string workspaceIdForRoles = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_ROLES";
        // if (workspaceIdForRoles != "YOUR_WORKSPACE_ID_FOR_ROLES") // Condition removed
        // {
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

        // Example: Shared Hierarchy API - Get shared hierarchy
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        const string workspaceIdForSharedHierarchy = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_SHAREDH";
        // if (workspaceIdForSharedHierarchy != "YOUR_WORKSPACE_ID_FOR_SHAREDH") // Condition removed
        // {
            Console.WriteLine($"\n  Fetching shared hierarchy for workspace '{workspaceIdForSharedHierarchy}'...");
            var sharedHierarchy = await client.SharedHierarchy.GetSharedHierarchyAsync(workspaceIdForSharedHierarchy);
            Console.WriteLine($"    Shared Tasks Count: {sharedHierarchy.Shared.Tasks.Count}");
            Console.WriteLine($"    Shared Lists Count: {sharedHierarchy.Shared.Lists.Count}");
            Console.WriteLine($"    Shared Folders Count: {sharedHierarchy.Shared.Folders.Count}");
        }

        // Example: Spaces API - Get spaces
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        const string workspaceIdForSpaces = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_SPACES";
        // if (workspaceIdForSpaces != "YOUR_WORKSPACE_ID_FOR_SPACES") // Condition removed
        // {
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

        // Example: Tags API - Get space tags
        // IMPORTANT: Replace "YOUR_SPACE_ID"
        const string spaceIdForTags = settings.SpaceId ?? "YOUR_SPACE_ID_FOR_TAGS";
        // if (spaceIdForTags != "YOUR_SPACE_ID_FOR_TAGS") // Condition removed
        // {
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

        // Example: Task Checklists API - Delete checklist (assuming you have one to delete)
        // IMPORTANT: Replace "YOUR_CHECKLIST_ID"
        const string checklistIdToDelete = "YOUR_CHECKLIST_ID_EXAMPLE";
        // if (checklistIdToDelete != "YOUR_CHECKLIST_ID_EXAMPLE") // Condition removed
        // {
            Console.WriteLine($"\n  Attempting to delete checklist '{checklistIdToDelete}' (Note: Requires valid Checklist ID)...");
            if (checklistIdToDelete != "YOUR_CHECKLIST_ID_EXAMPLE")
            {
                try
                {
                    await client.TaskChecklists.DeleteChecklistAsync(checklistIdToDelete);
                    Console.WriteLine($"    Checklist '{checklistIdToDelete}' deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"    Error deleting checklist: {ex.Message}");
                }
            } else { Console.WriteLine($"    Skipping Delete Checklist example: Placeholder ID '{checklistIdToDelete}' not replaced."); }
        // }

        // Example: Task Relationships API - Add dependency
        // IMPORTANT: Replace "YOUR_TASK_ID_1" and "YOUR_TASK_ID_2"
        const string taskId1 = "YOUR_TASK_ID_1_REL_EXAMPLE";
        const string taskId2 = "YOUR_TASK_ID_2_REL_EXAMPLE";
        // if (taskId1 != "YOUR_TASK_ID_1_REL_EXAMPLE" && taskId2 != "YOUR_TASK_ID_2_REL_EXAMPLE") // Condition removed
        // {
            Console.WriteLine($"\n  Adding dependency: Task '{taskId1}' depends on Task '{taskId2}' (Note: Requires valid Task IDs)...");
            if (taskId1 != "YOUR_TASK_ID_1_REL_EXAMPLE" && taskId2 != "YOUR_TASK_ID_2_REL_EXAMPLE")
            {
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
            } else { Console.WriteLine($"    Skipping Add Dependency example: Placeholder Task IDs not replaced.");}
        // }

        // Example: Templates API - Get task templates
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        const string workspaceIdForTemplates = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_TEMPLATES";
        // if (workspaceIdForTemplates != "YOUR_WORKSPACE_ID_FOR_TEMPLATES") // Condition removed
        // {
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

        // Example: Time Tracking API - Get time entries
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        const string workspaceIdForTimeTracking = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_TIME";
        // if (workspaceIdForTimeTracking != "YOUR_WORKSPACE_ID_FOR_TIME") // Condition removed
        // {
            Console.WriteLine($"\n  Fetching time entries for workspace '{workspaceIdForTimeTracking}'...");
            var timeEntries = await client.TimeTracking.GetTimeEntries(workspaceIdForTimeTracking)
                                                    // .WithStartDate(DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds()) // TODO: Re-enable/update when Fluent API supports GetTimeEntriesRequestParameters (Step 6.7)
                                                    // .WithEndDate(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())   // TODO: Re-enable/update
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
        const string workspaceIdForUserGroups = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_USERGROUPS";
        // if (workspaceIdForUserGroups != "YOUR_WORKSPACE_ID_FOR_USERGROUPS") // Condition removed
        // {
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

        // Example: Users API - Get user from workspace
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID" and "YOUR_USER_ID"
        const string workspaceIdForUsers = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_USERS";
        const string userIdForUsers = "YOUR_USER_ID_EXAMPLE";
        // if (workspaceIdForUsers != "YOUR_WORKSPACE_ID_FOR_USERS" && userIdForUsers != "YOUR_USER_ID_EXAMPLE") // Condition removed
        // {
            Console.WriteLine($"\n  Fetching user '{userIdForUsers}' from workspace '{workspaceIdForUsers}' (Note: Requires valid User ID)...");
            if (workspaceIdForUsers != "YOUR_WORKSPACE_ID_FOR_USERS" && userIdForUsers != "YOUR_USER_ID_EXAMPLE")
            {
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

        // Example: Views API - Get workspace views
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        const string workspaceIdForViews = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_VIEWS";
        // if (workspaceIdForViews != "YOUR_WORKSPACE_ID_FOR_VIEWS") // Condition removed
        // {
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

        // Example: Webhooks API - Get webhooks
        // IMPORTANT: Replace "YOUR_WORKSPACE_ID"
        const string workspaceIdForWebhooks = settings.WorkspaceId ?? "YOUR_WORKSPACE_ID_FOR_WEBHOOKS";
        // if (workspaceIdForWebhooks != "YOUR_WORKSPACE_ID_FOR_WEBHOOKS") // Condition removed
        // {
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
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\nAn error occurred: {ex.Message}");
    Console.WriteLine("Please ensure you have set your API token and other IDs in appsettings.json.");
}