# ClickUp API Abstractions and Interfaces

This document outlines the C# interfaces for abstracting the ClickUp API.

## Core Entities

- [x] ITaskRelationshipsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  namespace ClickUp.Abstract;

  // Represents the Task Relationships operations in the ClickUp API
  // Based on endpoints like:
  // - POST /v2/task/{task_id}/dependency
  // - DELETE /v2/task/{task_id}/dependency
  // - POST /v2/task/{task_id}/link/{links_to}
  // - DELETE /v2/task/{task_id}/link/{links_to}

  public interface ITaskRelationshipsService
  {
      /// <summary>
      /// Sets a task as waiting on or blocking another task.
      /// </summary>
      /// <param name="taskId">The ID of the task which is waiting on or blocking another task.</param>
      /// <param name="dependsOnTaskId">The ID of the task that must be completed before the task identified by taskId.</param>
      /// <param name="dependencyOfTaskId">The ID of the task that's waiting for the task identified by taskId to be completed.</param>
      /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task AddDependencyAsync(string taskId, string? dependsOnTaskId = null, string? dependencyOfTaskId = null, bool? customTaskIds = null, double? teamId = null);

      /// <summary>
      /// Removes a dependency relationship between two tasks.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="dependsOnTaskId">The ID of the task that the primary task depends on.</param>
      /// <param name="dependencyOfTaskId">The ID of the task that is a dependency of the primary task.</param>
      /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteDependencyAsync(string taskId, string dependsOnTaskId, string dependencyOfTaskId, bool? customTaskIds = null, double? teamId = null);

      /// <summary>
      /// Links two tasks together.
      /// </summary>
      /// <param name="taskId">The ID of the task to initiate the link from.</param>
      /// <param name="linksToTaskId">The ID of the task to link to.</param>
      /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains the linked task details.</returns>
      Task<object> AddTaskLinkAsync(string taskId, string linksToTaskId, bool? customTaskIds = null, double? teamId = null);
      // Note: The return type 'object' should be replaced with a specific DTO representing the linked task structure from the API response.

      /// <summary>
      /// Removes the link between two tasks.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="linksToTaskId">The ID of the task linked to.</param>
      /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains the updated task details.</returns>
      Task<object> DeleteTaskLinkAsync(string taskId, string linksToTaskId, bool? customTaskIds = null, double? teamId = null);
      // Note: The return type 'object' should be replaced with a specific DTO representing the task structure from the API response.
  }
  ```

- [x] IAttachmentsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;
  // Assuming a DTO for the attachment response and a way to represent file content for upload.
  // For example, a simple DTO could be:
  // public record TaskAttachmentDto(string Id, string Version, int Date, string Title, string Extension, string ThumbnailSmall, string ThumbnailLarge, string Url);
  // And for upload, perhaps:
  // public record FileUploadDto(byte[] Content, string FileName);

  namespace ClickUp.Abstract;

  // Represents the Attachments operations in the ClickUp API
  // Based on endpoints like:
  // - POST /v2/task/{task_id}/attachment
  // (Assuming there might be GET, DELETE operations for attachments as well, though not explicitly listed in the immediate previous context,
  // a full-fledged service would typically include them if they exist in the full API spec)

  public interface IAttachmentsService
  {
      /// <summary>
      /// Uploads a file to a task as an attachment.
      /// </summary>
      /// <param name="taskId">The ID of the task to attach the file to.</param>
      /// <param name="attachmentContent">The content of the file to upload. This would typically be a stream or byte array and filename.</param>
      /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains details of the created attachment.</returns>
      Task<object> CreateTaskAttachmentAsync(string taskId, object attachmentContent, bool? customTaskIds = null, double? teamId = null);
      // Note: 'object attachmentContent' should be a more specific type representing the file to be uploaded (e.g., Stream, byte[], or a custom DTO).
      // Note: The return type 'object' should be replaced with a specific DTO (e.g., TaskAttachmentDto) representing the attachment details from the API response.

      // Placeholder for GetTaskAttachmentsAsync if the API supports it
      // /// <summary>
      // /// Gets all attachments for a specific task.
      // /// </summary>
      // /// <param name="taskId">The ID of the task.</param>
      // /// <returns>A list of attachments for the task.</returns>
      // Task<IEnumerable<object>> GetTaskAttachmentsAsync(string taskId);

      // Placeholder for DeleteTaskAttachmentAsync if the API supports it
      // /// <summary>
      // /// Deletes an attachment from a task.
      // /// </summary>
      // /// <param name="taskId">The ID of the task.</param>
      // /// <param name="attachmentId">The ID of the attachment to delete.</param>
      // /// <returns>An awaitable task representing the asynchronous operation.</returns>
      // Task DeleteTaskAttachmentAsync(string taskId, string attachmentId);
  }
  ```
- [x] IAuthorizationService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for AccessToken, AuthorizedUser, and AuthorizedWorkspace
  // public record AccessTokenDto(string AccessToken);
  // public record UserDto(int Id, string Username, string Email, string Color, string ProfilePicture, string Initials, int WeekStartDay, bool GlobalFontSupport, string Timezone);
  // public record MemberDto(UserDto User);
  // public record WorkspaceDto(string Id, string Name, string Color, string Avatar, IEnumerable<MemberDto> Members);


  namespace ClickUp.Abstract;

  // Represents the Authorization operations in the ClickUp API
  // Based on endpoints like:
  // - POST /v2/oauth/token
  // - GET /v2/user
  // - GET /v2/team (related to authorized user's workspaces)

  public interface IAuthorizationService
  {
      /// <summary>
      /// Exchanges an authorization code for an access token.
      /// </summary>
      /// <param name="clientId">OAuth app client ID.</param>
      /// <param name="clientSecret">OAuth app client secret.</param>
      /// <param name="code">Authorization code received from redirect.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains the access token.</returns>
      Task<object> GetAccessTokenAsync(string clientId, string clientSecret, string code);
      // Note: Return type 'object' should be AccessTokenDto.

      /// <summary>
      /// Retrieves the details of the authenticated user.
      /// </summary>
      /// <returns>A task that represents the asynchronous operation. The task result contains the authorized user's details.</returns>
      Task<object> GetAuthorizedUserAsync();
      // Note: Return type 'object' should be a DTO representing the User object (e.g., UserDto from GetAuthorizedUserresponse).

      /// <summary>
      /// Retrieves the Workspaces available to the authenticated user.
      /// </summary>
      /// <returns>A task that represents the asynchronous operation. The task result contains a list of authorized Workspaces.</returns>
      Task<IEnumerable<object>> GetAuthorizedWorkspacesAsync();
      // Note: Return type 'IEnumerable<object>' should be IEnumerable<WorkspaceDto>.
  }
  ```
- [x] IWorkspacesService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for WorkspaceSeats and WorkspacePlan
  // public record MemberSeatsDto(int FilledMembersSeats, int TotalMemberSeats, int EmptyMemberSeats);
  // public record GuestSeatsDto(int FilledGuestSeats, int TotalGuestSeats, int EmptyGuestSeats);
  // public record WorkspaceSeatsDto(MemberSeatsDto Members, GuestSeatsDto Guests);
  // public record WorkspacePlanDto(string PlanName, int PlanId);

  namespace ClickUp.Abstract;

  // Represents Workspace (Team) specific operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/team/{team_id}/seats
  // - GET /v2/team/{team_id}/plan
  // Note: GET /v2/team is covered by IAuthorizationService.GetAuthorizedWorkspacesAsync()

  public interface IWorkspacesService
  {
      /// <summary>
      /// Retrieves the seat usage for a specific Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains the workspace seat details.</returns>
      Task<object> GetWorkspaceSeatsAsync(string workspaceId);
      // Note: Return type 'object' should be WorkspaceSeatsDto.

      /// <summary>
      /// Retrieves the current plan for a specific Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains the workspace plan details.</returns>
      Task<object> GetWorkspacePlanAsync(string workspaceId);
      // Note: Return type 'object' should be WorkspacePlanDto.
  }
  ```
- [x] ITaskChecklistsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Checklist, ChecklistItem, and request bodies.
  // public record ChecklistDto(string Id, string TaskId, string Name, int OrderIndex, int Resolved, int Unresolved, IEnumerable<ChecklistItemDto> Items);
  // public record ChecklistItemDto(string Id, string Name, int OrderIndex, object Assignee, bool Resolved, string ParentId, string DateCreated, IEnumerable<string> Children);
  // public record CreateChecklistRequest(string Name);
  // public record EditChecklistRequest(string? Name, int? Position);
  // public record CreateChecklistItemRequest(string Name, int? Assignee);
  // public record EditChecklistItemRequest(string? Name, object? Assignee, bool? Resolved, string? Parent); // Assignee could be int or null

  namespace ClickUp.Abstract;

  // Represents the Task Checklists operations in the ClickUp API
  // Based on endpoints like:
  // - POST /v2/task/{task_id}/checklist
  // - PUT /v2/checklist/{checklist_id}
  // - DELETE /v2/checklist/{checklist_id}
  // - POST /v2/checklist/{checklist_id}/checklist_item
  // - PUT /v2/checklist/{checklist_id}/checklist_item/{checklist_item_id}
  // - DELETE /v2/checklist/{checklist_id}/checklist_item/{checklist_item_id}

  public interface ITaskChecklistsService
  {
      /// <summary>
      /// Adds a new checklist to a task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="createChecklistRequest">Details of the checklist to create.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains the created checklist.</returns>
      Task<object> CreateChecklistAsync(string taskId, object createChecklistRequest, bool? customTaskIds = null, double? teamId = null);
      // Note: createChecklistRequest should be CreateChecklistRequest, return type should be ChecklistDto.

      /// <summary>
      /// Renames a task checklist or reorders it.
      /// </summary>
      /// <param name="checklistId">The ID of the checklist.</param>
      /// <param name="editChecklistRequest">Details for editing the checklist.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task EditChecklistAsync(string checklistId, object editChecklistRequest);
      // Note: editChecklistRequest should be EditChecklistRequest. API returns 200 with an empty object.

      /// <summary>
      /// Deletes a checklist from a task.
      /// </summary>
      /// <param name="checklistId">The ID of the checklist to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteChecklistAsync(string checklistId);
      // Note: API returns 200 with an empty object.

      /// <summary>
      /// Adds a line item to a task checklist.
      /// </summary>
      /// <param name="checklistId">The ID of the checklist.</param>
      /// <param name="createChecklistItemRequest">Details of the checklist item to create.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains the updated checklist.</returns>
      Task<object> CreateChecklistItemAsync(string checklistId, object createChecklistItemRequest);
      // Note: createChecklistItemRequest should be CreateChecklistItemRequest, return type should be ChecklistDto (representing the parent checklist).

      /// <summary>
      /// Updates an individual line item in a task checklist.
      /// </summary>
      /// <param name="checklistId">The ID of the checklist.</param>
      /// <param name="checklistItemId">The ID of the checklist item.</param>
      /// <param name="editChecklistItemRequest">Details for editing the checklist item.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains the updated checklist.</returns>
      Task<object> EditChecklistItemAsync(string checklistId, string checklistItemId, object editChecklistItemRequest);
      // Note: editChecklistItemRequest should be EditChecklistItemRequest, return type should be ChecklistDto (representing the parent checklist).

      /// <summary>
      /// Deletes a line item from a task checklist.
      /// </summary>
      /// <param name="checklistId">The ID of the checklist.</param>
      /// <param name="checklistItemId">The ID of the checklist item to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteChecklistItemAsync(string checklistId, string checklistItemId);
      // Note: API returns 200 with an empty object.
  }
  ```
- [x] ICommentsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Comment, Comment_Text_Block, User_Brief, Reaction, etc.
  // public record CommentDto(string Id, IEnumerable<CommentTextBlockDto> Comment, string CommentText, UserBriefDto User, bool Resolved, UserBriefDto Assignee, UserBriefDto AssignedBy, IEnumerable<ReactionDto> Reactions, string Date, string? ReplyCount);
  // public record CommentTextBlockDto(string Text); // Simplified
  // public record UserBriefDto(int Id, string Username, string Initials, string Email, string Color, string ProfilePicture);
  // public record CreateCommentResponseDto(string Id, string HistId, long Date);
  // public record CreateTaskCommentRequest(string CommentText, int? Assignee, string? GroupAssignee, bool NotifyAll);
  // public record CreateChatViewCommentRequest(string CommentText, bool NotifyAll);
  // public record CreateListCommentRequest(string CommentText, int Assignee, bool NotifyAll);
  // public record UpdateCommentRequest(string CommentText, int Assignee, string? GroupAssignee, bool Resolved);


  namespace ClickUp.Abstract;

  // Represents the Comments operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/task/{task_id}/comment
  // - POST /v2/task/{task_id}/comment
  // - GET /v2/view/{view_id}/comment
  // - POST /v2/view/{view_id}/comment
  // - GET /v2/list/{list_id}/comment
  // - POST /v2/list/{list_id}/comment
  // - PUT /v2/comment/{comment_id}
  // - DELETE /v2/comment/{comment_id}
  // - GET /v2/comment/{comment_id}/reply (Get Threaded Comments)
  // - POST /v2/comment/{comment_id}/reply (Create Threaded Comment)

  public interface ICommentsService
  {
      /// <summary>
      /// Retrieves comments for a specific task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <param name="start">Optional. Unix time in milliseconds to start comments from.</param>
      /// <param name="startId">Optional. Comment ID to start pagination from.</param>
      /// <returns>A list of comments for the task.</returns>
      Task<IEnumerable<object>> GetTaskCommentsAsync(string taskId, bool? customTaskIds = null, double? teamId = null, int? start = null, string? startId = null);
      // Note: Return type should be IEnumerable<CommentDto>.

      /// <summary>
      /// Adds a new comment to a task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="createCommentRequest">Details of the comment to create.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>Details of the created comment.</returns>
      Task<object> CreateTaskCommentAsync(string taskId, object createCommentRequest, bool? customTaskIds = null, double? teamId = null);
      // Note: createCommentRequest should be CreateTaskCommentRequest, return type should be CreateCommentResponseDto.

      /// <summary>
      /// Retrieves comments from a Chat view.
      /// </summary>
      /// <param name="viewId">The ID of the Chat view.</param>
      /// <param name="start">Optional. Unix time in milliseconds to start comments from.</param>
      /// <param name="startId">Optional. Comment ID to start pagination from.</param>
      /// <returns>A list of comments for the Chat view.</returns>
      Task<IEnumerable<object>> GetChatViewCommentsAsync(string viewId, int? start = null, string? startId = null);
      // Note: Return type should be IEnumerable<CommentDto>.

      /// <summary>
      /// Adds a new comment to a Chat view.
      /// </summary>
      /// <param name="viewId">The ID of the Chat view.</param>
      /// <param name="createCommentRequest">Details of the comment to create.</param>
      /// <returns>Details of the created comment.</returns>
      Task<object> CreateChatViewCommentAsync(string viewId, object createCommentRequest);
      // Note: createCommentRequest should be CreateChatViewCommentRequest, return type should be CreateCommentResponseDto.

      /// <summary>
      /// Retrieves comments added to a List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <param name="start">Optional. Unix time in milliseconds to start comments from.</param>
      /// <param name="startId">Optional. Comment ID to start pagination from.</param>
      /// <returns>A list of comments for the List.</returns>
      Task<IEnumerable<object>> GetListCommentsAsync(double listId, int? start = null, string? startId = null);
      // Note: Return type should be IEnumerable<CommentDto>.

      /// <summary>
      /// Adds a comment to a List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <param name="createCommentRequest">Details of the comment to create.</param>
      /// <returns>Details of the created comment.</returns>
      Task<object> CreateListCommentAsync(double listId, object createCommentRequest);
      // Note: createCommentRequest should be CreateListCommentRequest, return type should be CreateCommentResponseDto.

      /// <summary>
      /// Updates a comment.
      /// </summary>
      /// <param name="commentId">The ID of the comment to update.</param>
      /// <param name="updateCommentRequest">Details for updating the comment.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task UpdateCommentAsync(double commentId, object updateCommentRequest);
      // Note: updateCommentRequest should be UpdateCommentRequest. API returns 200 with an empty object.

      /// <summary>
      /// Deletes a comment.
      /// </summary>
      /// <param name="commentId">The ID of the comment to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteCommentAsync(double commentId);
      // Note: API returns 200 with an empty object.

      /// <summary>
      /// Retrieves threaded comments for a parent comment. The parent comment is not included.
      /// </summary>
      /// <param name="commentId">The ID of the parent comment.</param>
      /// <returns>A list of threaded comments.</returns>
      Task<IEnumerable<object>> GetThreadedCommentsAsync(double commentId);
      // Note: Return type should be IEnumerable<CommentDto>.

      /// <summary>
      /// Creates a threaded comment.
      /// </summary>
      /// <param name="commentId">The ID of the parent comment.</param>
      /// <param name="createCommentRequest">Details of the threaded comment to create. This is likely similar to CreateTaskCommentRequest.</param>
      /// <returns>An awaitable task representing the asynchronous operation. The API returns 200 with an empty object, consider if a different return is more useful or if specific response DTO exists.</returns>
      Task CreateThreadedCommentAsync(double commentId, object createCommentRequest);
      // Note: createCommentRequest should be a DTO similar to CreateTaskCommentRequest.
  }
  ```
- [x] ICustomFieldsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for CustomField, FieldValueRequest, etc.
  // public record CustomFieldDto(string Id, string Name, string Type, object TypeConfig, string DateCreated, bool HideFromGuests);
  // public record SetCustomFieldValueRequest(object Value, object? ValueOptions = null); // Value can be string, int, object, array etc. based on field type

  namespace ClickUp.Abstract;

  // Represents the Custom Fields operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/list/{list_id}/field
  // - GET /v2/folder/{folder_id}/field
  // - GET /v2/space/{space_id}/field
  // - GET /v2/team/{team_id}/field
  // - POST /v2/task/{task_id}/field/{field_id}
  // - DELETE /v2/task/{task_id}/field/{field_id}

  public interface ICustomFieldsService
  {
      /// <summary>
      /// Retrieves the Custom Fields available in a specific List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <returns>A list of Custom Fields for the List.</returns>
      Task<IEnumerable<object>> GetAccessibleCustomFieldsAsync(double listId);
      // Note: Return type should be IEnumerable<CustomFieldDto>.

      /// <summary>
      /// Retrieves the Custom Fields available in a specific Folder.
      /// Only returns Custom Fields created at the Folder level.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <returns>A list of Custom Fields for the Folder.</returns>
      Task<IEnumerable<object>> GetFolderCustomFieldsAsync(double folderId);
      // Note: Return type should be IEnumerable<CustomFieldDto>.

      /// <summary>
      /// Retrieves the Custom Fields available in a specific Space.
      /// Only returns Custom Fields created at the Space level.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <returns>A list of Custom Fields for the Space.</returns>
      Task<IEnumerable<object>> GetSpaceCustomFieldsAsync(double spaceId);
      // Note: Return type should be IEnumerable<CustomFieldDto>.

      /// <summary>
      /// Retrieves the Custom Fields available in a specific Workspace.
      /// Only returns Custom Fields created at the Workspace level.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <returns>A list of Custom Fields for the Workspace.</returns>
      Task<IEnumerable<object>> GetWorkspaceCustomFieldsAsync(double workspaceId);
      // Note: Return type should be IEnumerable<CustomFieldDto>.

      /// <summary>
      /// Adds or updates data in a Custom Field on a task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="fieldId">The UUID of the Custom Field.</param>
      /// <param name="setFieldValueRequest">The value to set for the Custom Field.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task SetCustomFieldValueAsync(string taskId, string fieldId, object setFieldValueRequest, bool? customTaskIds = null, double? teamId = null);
      // Note: setFieldValueRequest should be a specific DTO (e.g. SetCustomFieldValueRequest) that can accommodate various value types.
      // API returns 200 with an empty object.

      /// <summary>
      /// Removes data from a Custom Field on a task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="fieldId">The UUID of the Custom Field.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task RemoveCustomFieldValueAsync(string taskId, string fieldId, bool? customTaskIds = null, double? teamId = null);
      // Note: API returns 200 with an empty object.
  }
  ```
- [x] IFoldersService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Folder, CreateFolderRequest, UpdateFolderRequest, FolderFromTemplateOptions etc.
  // public record FolderDto(string Id, string Name, int OrderIndex, bool OverrideStatuses, bool Hidden, SpaceReferenceDto Space, string TaskCount, IEnumerable<ListReferenceDto> Lists, bool? Archived, IEnumerable<StatusDto>? Statuses, string? PermissionLevel );
  // public record SpaceReferenceDto(string Id, string Name, bool Access);
  // public record ListReferenceDto(string Id, string Name, bool Access); // Simplified
  // public record StatusDto(string Id, string Status, int OrderIndex, string Color, string Type);
  // public record CreateFolderRequest(string Name);
  // public record UpdateFolderRequest(string Name);
  // public record CreateFolderFromTemplateRequest(string Name, object? Options = null); // Options is a complex object

  namespace ClickUp.Abstract;

  // Represents the Folders operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/space/{space_id}/folder
  // - POST /v2/space/{space_id}/folder
  // - GET /v2/folder/{folder_id}
  // - PUT /v2/folder/{folder_id}
  // - DELETE /v2/folder/{folder_id}
  // - POST /v2/space/{space_id}/folder_template/{template_id}

  public interface IFoldersService
  {
      /// <summary>
      /// Retrieves Folders in a specific Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="archived">Optional. Whether to include archived Folders.</param>
      /// <returns>A list of Folders in the Space.</returns>
      Task<IEnumerable<object>> GetFoldersAsync(double spaceId, bool? archived = null);
      // Note: Return type should be IEnumerable<FolderDto>.

      /// <summary>
      /// Creates a new Folder in a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="createFolderRequest">Details of the Folder to create.</param>
      /// <returns>The created Folder.</returns>
      Task<object> CreateFolderAsync(double spaceId, object createFolderRequest);
      // Note: createFolderRequest should be CreateFolderRequest, return type should be FolderDto.

      /// <summary>
      /// Retrieves details of a specific Folder.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <returns>Details of the Folder.</returns>
      Task<object> GetFolderAsync(double folderId);
      // Note: Return type should be FolderDto.

      /// <summary>
      /// Renames a Folder.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <param name="updateFolderRequest">Details for updating the Folder.</param>
      /// <returns>The updated Folder.</returns>
      Task<object> UpdateFolderAsync(double folderId, object updateFolderRequest);
      // Note: updateFolderRequest should be UpdateFolderRequest, return type should be FolderDto.

      /// <summary>
      /// Deletes a Folder.
      /// </summary>
      /// <param name="folderId">The ID of the Folder to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteFolderAsync(double folderId);
      // Note: API returns 200 with an empty object.

      /// <summary>
      /// Creates a new Folder from a template within a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="templateId">The ID of the Folder template.</param>
      /// <param name="createFolderFromTemplateRequest">Details for creating the Folder from a template.</param>
      /// <returns>The created Folder, or an object containing an ID if return_immediately is true in options.</returns>
      Task<object> CreateFolderFromTemplateAsync(string spaceId, string templateId, object createFolderFromTemplateRequest);
      // Note: createFolderFromTemplateRequest should be CreateFolderFromTemplateRequest.
      // Return type depends on options, could be FolderDto or an object with just an ID.
  }
  ```
- [x] IGoalsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Goal, KeyResult, request bodies etc.
  // public record GoalDto(...); // Define based on GetGoal response
  // public record KeyResultDto(...); // Define based on CreateKeyResult response
  // public record CreateGoalRequest(string Name, long DueDate, string Description, bool MultipleOwners, IEnumerable<int> Owners, string Color);
  // public record UpdateGoalRequest(string Name, long DueDate, string Description, IEnumerable<int> RemoveOwners, IEnumerable<int> AddOwners, string Color);
  // public record CreateKeyResultRequest(string Name, IEnumerable<int> Owners, string Type, int StepsStart, int StepsEnd, string Unit, IEnumerable<string> TaskIds, IEnumerable<string> ListIds);
  // public record EditKeyResultRequest(int StepsCurrent, string Note); // Plus other fields from CreateKeyResultRequest potentially

  namespace ClickUp.Abstract;

  // Represents the Goals and Key Results operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/team/{team_id}/goal
  // - POST /v2/team/{team_id}/goal
  // - GET /v2/goal/{goal_id}
  // - PUT /v2/goal/{goal_id}
  // - DELETE /v2/goal/{goal_id}
  // - POST /v2/goal/{goal_id}/key_result
  // - PUT /v2/key_result/{key_result_id}
  // - DELETE /v2/key_result/{key_result_id}

  public interface IGoalsService
  {
      /// <summary>
      /// Retrieves Goals for a specific Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="includeCompleted">Optional. Whether to include completed Goals.</param>
      /// <returns>A list of Goals for the Workspace.</returns>
      Task<IEnumerable<object>> GetGoalsAsync(double workspaceId, bool? includeCompleted = null);
      // Note: Return type should be a DTO that includes both 'goals' and 'folders' arrays from the response (e.g., GetGoalsResponseDto).

      /// <summary>
      /// Creates a new Goal in a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="createGoalRequest">Details of the Goal to create.</param>
      /// <returns>The created Goal.</returns>
      Task<object> CreateGoalAsync(double workspaceId, object createGoalRequest);
      // Note: createGoalRequest should be CreateGoalRequest, return type should be GoalDto (wrapped in a 'goal' property in response).

      /// <summary>
      /// Retrieves details of a specific Goal.
      /// </summary>
      /// <param name="goalId">The UUID of the Goal.</param>
      /// <returns>Details of the Goal.</returns>
      Task<object> GetGoalAsync(string goalId);
      // Note: Return type should be GoalDto (wrapped in a 'goal' property in response).

      /// <summary>
      /// Updates a Goal.
      /// </summary>
      /// <param name="goalId">The UUID of the Goal.</param>
      /// <param name="updateGoalRequest">Details for updating the Goal.</param>
      /// <returns>The updated Goal.</returns>
      Task<object> UpdateGoalAsync(string goalId, object updateGoalRequest);
      // Note: updateGoalRequest should be UpdateGoalRequest, return type should be GoalDto (wrapped in a 'goal' property in response).

      /// <summary>
      /// Deletes a Goal.
      /// </summary>
      /// <param name="goalId">The UUID of the Goal to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteGoalAsync(string goalId);
      // Note: API returns 200 with an empty object.

      /// <summary>
      /// Adds a Target (Key Result) to a Goal.
      /// </summary>
      /// <param name="goalId">The UUID of the Goal.</param>
      /// <param name="createKeyResultRequest">Details of the Key Result to create.</param>
      /// <returns>The created Key Result.</returns>
      Task<object> CreateKeyResultAsync(string goalId, object createKeyResultRequest);
      // Note: createKeyResultRequest should be CreateKeyResultRequest, return type should be KeyResultDto (wrapped in a 'key_result' property in response).

      /// <summary>
      /// Updates a Target (Key Result).
      /// </summary>
      /// <param name="keyResultId">The UUID of the Key Result.</param>
      /// <param name="editKeyResultRequest">Details for editing the Key Result.</param>
      /// <returns>The updated Key Result.</returns>
      Task<object> EditKeyResultAsync(string keyResultId, object editKeyResultRequest);
      // Note: editKeyResultRequest should be EditKeyResultRequest, return type should be KeyResultDto (wrapped in a 'key_result' property in response).

      /// <summary>
      /// Deletes a Target (Key Result) from a Goal.
      /// </summary>
      /// <param name="keyResultId">The UUID of the Key Result to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteKeyResultAsync(string keyResultId);
      // Note: API returns 200 with an empty object.
  }
  ```
- [x] IGuestsService
  ```csharp
  using System.Threading.Tasks;

  // Assuming DTOs for Guest, InviteGuestRequest, EditGuestRequest, AddGuestToItemRequest etc.
  // public record GuestDetailsDto(...); // From GET /v2/team/{team_id}/guest/{guest_id}
  // public record InviteGuestRequest(string Email, bool? CanEditTags, bool? CanSeeTimeSpent, bool? CanSeeTimeEstimated, bool? CanCreateViews, bool? CanSeePointsEstimated, int? CustomRoleId);
  // public record EditGuestOnWorkspaceRequest(bool? CanEditTags, bool? CanSeeTimeSpent, bool? CanSeeTimeEstimated, bool? CanCreateViews, bool? CanSeePointsEstimated, int? CustomRoleId);
  // public record AddGuestToItemRequest(string PermissionLevel); // "read", "comment", "edit", "create"

  namespace ClickUp.Abstract;

  // Represents the Guests operations in the ClickUp API
  // Based on endpoints like:
  // - POST /v2/team/{team_id}/guest
  // - GET /v2/team/{team_id}/guest/{guest_id}
  // - PUT /v2/team/{team_id}/guest/{guest_id}
  // - DELETE /v2/team/{team_id}/guest/{guest_id}
  // - POST /v2/task/{task_id}/guest/{guest_id}
  // - DELETE /v2/task/{task_id}/guest/{guest_id}
  // - POST /v2/list/{list_id}/guest/{guest_id}
  // - DELETE /v2/list/{list_id}/guest/{guest_id}
  // - POST /v2/folder/{folder_id}/guest/{guest_id}
  // - DELETE /v2/folder/{folder_id}/guest/{guest_id}

  public interface IGuestsService
  {
      /// <summary>
      /// Invites a guest to join a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="inviteGuestRequest">Details for inviting the guest.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains details of the invited guest within the team structure.</returns>
      Task<object> InviteGuestToWorkspaceAsync(double workspaceId, object inviteGuestRequest);
      // Note: inviteGuestRequest should be InviteGuestRequest. Return type should be a DTO representing the 'team' structure with member details from the response.

      /// <summary>
      /// Retrieves information about a specific guest in a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="guestId">The ID of the guest.</param>
      /// <returns>Details of the guest.</returns>
      Task<object> GetGuestAsync(double workspaceId, double guestId);
      // Note: Return type should be GuestDetailsDto. The API returns an empty object for this, which seems unusual. Verify actual response structure. If empty, Task might be more appropriate or a custom success/failure result.

      /// <summary>
      /// Configures options for a guest in a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="guestId">The ID of the guest.</param>
      /// <param name="editGuestRequest">Details for editing the guest's permissions.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains the updated guest details.</returns>
      Task<object> EditGuestOnWorkspaceAsync(double workspaceId, double guestId, object editGuestRequest);
      // Note: editGuestRequest should be EditGuestOnWorkspaceRequest. Return type should be a DTO representing the 'guest' structure from the response.

      /// <summary>
      /// Revokes a guest's access to a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="guestId">The ID of the guest to remove.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task<object> RemoveGuestFromWorkspaceAsync(double workspaceId, double guestId);
      // Note: API returns a 'team' object. Consider if Task is sufficient or if this DTO is needed.

      /// <summary>
      /// Shares a task with a guest.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="guestId">The ID of the guest.</param>
      /// <param name="addGuestToItemRequest">Permission level for the guest.</param>
      /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their shared items.</returns>
      Task<object> AddGuestToTaskAsync(string taskId, double guestId, object addGuestToItemRequest, bool? includeShared = null, bool? customTaskIds = null, double? teamId = null);
      // Note: addGuestToItemRequest should be AddGuestToItemRequest. Return type should be a DTO representing the 'guest' with 'shared' items structure.

      /// <summary>
      /// Revokes a guest's access to a task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="guestId">The ID of the guest.</param>
      /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their remaining shared items.</returns>
      Task<object> RemoveGuestFromTaskAsync(string taskId, double guestId, bool? includeShared = null, bool? customTaskIds = null, double? teamId = null);
      // Note: Return type should be a DTO representing the 'guest' with 'shared' items structure.

      /// <summary>
      /// Shares a List with a guest.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <param name="guestId">The ID of the guest.</param>
      /// <param name="addGuestToItemRequest">Permission level for the guest.</param>
      /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their shared items.</returns>
      Task<object> AddGuestToListAsync(double listId, double guestId, object addGuestToItemRequest, bool? includeShared = null);
      // Note: addGuestToItemRequest should be AddGuestToItemRequest. Return type should be a DTO representing the 'guest' with 'shared' items structure.

      /// <summary>
      /// Revokes a guest's access to a List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <param name="guestId">The ID of the guest.</param>
      /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their remaining shared items.</returns>
      Task<object> RemoveGuestFromListAsync(double listId, double guestId, bool? includeShared = null);
      // Note: Return type should be a DTO representing the 'guest' with 'shared' items structure.

      /// <summary>
      /// Shares a Folder with a guest.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <param name="guestId">The ID of the guest.</param>
      /// <param name="addGuestToItemRequest">Permission level for the guest.</param>
      /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their shared items.</returns>
      Task<object> AddGuestToFolderAsync(double folderId, double guestId, object addGuestToItemRequest, bool? includeShared = null);
      // Note: addGuestToItemRequest should be AddGuestToItemRequest. Return type should be a DTO representing the 'guest' with 'shared' items structure.

      /// <summary>
      /// Revokes a guest's access to a Folder.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <param name="guestId">The ID of the guest.</param>
      /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their remaining shared items.</returns>
      Task<object> RemoveGuestFromFolderAsync(double folderId, double guestId, bool? includeShared = null);
      // Note: Return type should be a DTO representing the 'guest' with 'shared' items structure.
  }
  ```
- [x] IListsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for List, CreateListRequest, UpdateListRequest, etc.
  // public record ListDto(...); // Define based on GetList response
  // public record CreateListInFolderRequest(string Name, string? Content, string? MarkdownContent, long? DueDate, bool? DueDateTime, int? Priority, int? Assignee, string? Status);
  // public record CreateFolderlessListRequest(string Name, string? Content, string? MarkdownContent, long? DueDate, bool? DueDateTime, int? Priority, int? Assignee, string? Status);
  // public record UpdateListRequest(string Name, string? Content, string? MarkdownContent, long? DueDate, bool? DueDateTime, int? Priority, string? Assignee, string? Status, bool? UnsetStatus);
  // public record CreateListFromTemplateRequest(string Name, object? Options = null); // Options is complex

  namespace ClickUp.Abstract;

  // Represents the Lists operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/folder/{folder_id}/list
  // - POST /v2/folder/{folder_id}/list
  // - GET /v2/space/{space_id}/list (Folderless)
  // - POST /v2/space/{space_id}/list (Folderless)
  // - GET /v2/list/{list_id}
  // - PUT /v2/list/{list_id}
  // - DELETE /v2/list/{list_id}
  // - POST /v2/list/{list_id}/task/{task_id}
  // - DELETE /v2/list/{list_id}/task/{task_id}
  // - POST /v2/folder/{folder_id}/list_template/{template_id}
  // - POST /v2/space/{space_id}/list_template/{template_id}

  public interface IListsService
  {
      /// <summary>
      /// Retrieves Lists within a specific Folder.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <param name="archived">Optional. Whether to include archived Lists.</param>
      /// <returns>A list of Lists in the Folder.</returns>
      Task<IEnumerable<object>> GetListsInFolderAsync(double folderId, bool? archived = null);
      // Note: Return type should be IEnumerable<ListDto>.

      /// <summary>
      /// Creates a new List in a Folder.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <param name="createListRequest">Details of the List to create.</param>
      /// <returns>The created List.</returns>
      Task<object> CreateListInFolderAsync(double folderId, object createListRequest);
      // Note: createListRequest should be CreateListInFolderRequest, return type should be ListDto.

      /// <summary>
      /// Retrieves Folderless Lists in a specific Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="archived">Optional. Whether to include archived Lists.</param>
      /// <returns>A list of Folderless Lists in the Space.</returns>
      Task<IEnumerable<object>> GetFolderlessListsAsync(double spaceId, bool? archived = null);
      // Note: Return type should be IEnumerable<ListDto>.

      /// <summary>
      /// Creates a new Folderless List in a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="createListRequest">Details of the List to create.</param>
      /// <returns>The created List.</returns>
      Task<object> CreateFolderlessListAsync(double spaceId, object createListRequest);
      // Note: createListRequest should be CreateFolderlessListRequest, return type should be ListDto.

      /// <summary>
      /// Retrieves details of a specific List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <returns>Details of the List.</returns>
      Task<object> GetListAsync(double listId);
      // Note: Return type should be ListDto.

      /// <summary>
      /// Updates a List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <param name="updateListRequest">Details for updating the List.</param>
      /// <returns>The updated List.</returns>
      Task<object> UpdateListAsync(string listId, object updateListRequest);
      // Note: updateListRequest should be UpdateListRequest, return type should be ListDto.

      /// <summary>
      /// Deletes a List.
      /// </summary>
      /// <param name="listId">The ID of the List to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteListAsync(double listId);
      // Note: API returns 200 with an empty object.

      /// <summary>
      /// Adds a task to an additional List. Requires the Tasks in Multiple Lists ClickApp.
      /// </summary>
      /// <param name="listId">The ID of the List to add the task to.</param>
      /// <param name="taskId">The ID of the task to add.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task AddTaskToListAsync(double listId, string taskId);
      // Note: API returns 200 with an empty object.

      /// <summary>
      /// Removes a task from an additional List. Requires the Tasks in Multiple Lists ClickApp.
      /// </summary>
      /// <param name="listId">The ID of the List to remove the task from.</param>
      /// <param name="taskId">The ID of the task to remove.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task RemoveTaskFromListAsync(double listId, string taskId);
      // Note: API returns 200 with an empty object.

      /// <summary>
      /// Creates a new List from a template in a Folder.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <param name="templateId">The ID of the List template.</param>
      /// <param name="createListFromTemplateRequest">Details for creating the List from a template.</param>
      /// <returns>The created List, or an object with an ID if return_immediately is true.</returns>
      Task<object> CreateListFromTemplateInFolderAsync(string folderId, string templateId, object createListFromTemplateRequest);
      // Note: createListFromTemplateRequest should be CreateListFromTemplateRequest.

      /// <summary>
      /// Creates a new List from a template in a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="templateId">The ID of the List template.</param>
      /// <param name="createListFromTemplateRequest">Details for creating the List from a template.</param>
      /// <returns>The created List, or an object with an ID if return_immediately is true.</returns>
      Task<object> CreateListFromTemplateInSpaceAsync(string spaceId, string templateId, object createListFromTemplateRequest);
      // Note: createListFromTemplateRequest should be CreateListFromTemplateRequest.
  }
  ```
- [x] IMembersService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Member and ProfileInfo.
  // public record MemberProfileInfoDto(string? DisplayProfile, string? VerifiedAmbassador, string? VerifiedConsultant, string? TopTierUser, string? ViewedVerifiedAmbassador, string? ViewedVerifiedConsultant, string? ViewedTopTierUser);
  // public record TaskMemberDto(int Id, string Username, string Email, string? Color, string Initials, string ProfilePicture, MemberProfileInfoDto ProfileInfo);

  namespace ClickUp.Abstract;

  // Represents the Members operations in the ClickUp API, focusing on retrieving members of tasks and lists.
  // Based on endpoints like:
  // - GET /v2/task/{task_id}/member
  // - GET /v2/list/{list_id}/member

  public interface IMembersService
  {
      /// <summary>
      /// Retrieves members who have access to a specific task.
      /// Does not include users with inherited Hierarchy permission.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <returns>A list of members associated with the task.</returns>
      Task<IEnumerable<object>> GetTaskMembersAsync(string taskId);
      // Note: Return type should be IEnumerable<TaskMemberDto>.

      /// <summary>
      /// Retrieves Workspace members who have access to a specific List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <returns>A list of members associated with the List.</returns>
      Task<IEnumerable<object>> GetListMembersAsync(double listId);
      // Note: Return type should be IEnumerable<TaskMemberDto> (or a similar Member DTO if structure varies).
  }
  ```
- [x] IRolesService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for CustomRole.
  // public record CustomRoleDto(int Id, string TeamId, string Name, int InheritedRole, string DateCreated, IEnumerable<int> Members);

  namespace ClickUp.Abstract;

  // Represents the Roles operations in the ClickUp API, primarily for retrieving Custom Roles.
  // Based on endpoints like:
  // - GET /v2/team/{team_id}/customroles

  public interface IRolesService
  {
      /// <summary>
      /// Retrieves the Custom Roles available in a specific Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="includeMembers">Optional. Whether to include members associated with each Custom Role.</param>
      /// <returns>A list of Custom Roles for the Workspace.</returns>
      Task<IEnumerable<object>> GetCustomRolesAsync(double workspaceId, bool? includeMembers = null);
      // Note: Return type should be IEnumerable<CustomRoleDto>.
  }
  ```
- [x] ISharedHierarchyService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for SharedItems, and the specific item types (TaskReference, ListReference, FolderReference).
  // public record TaskReferenceDto(string Id /*, ... other relevant fields if returned */);
  // public record ListReferenceDto(string Id, string Name, int OrderIndex, string? Content, object? Status, object? Priority, object? Assignee, string TaskCount, string? DueDate, string? StartDate, bool Archived);
  // public record FolderReferenceDto(string Id, string Name, int OrderIndex, string? Content, string TaskCount, string? DueDate, bool Archived);
  // public record SharedHierarchyDto(IEnumerable<TaskReferenceDto> Tasks, IEnumerable<ListReferenceDto> Lists, IEnumerable<FolderReferenceDto> Folders);

  namespace ClickUp.Abstract;

  // Represents the Shared Hierarchy operations in the ClickUp API.
  // Based on endpoints like:
  // - GET /v2/team/{team_id}/shared

  public interface ISharedHierarchyService
  {
      /// <summary>
      /// Retrieves the tasks, Lists, and Folders that have been shared with the authenticated user for a specific Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <returns>An object containing lists of shared tasks, lists, and folders.</returns>
      Task<object> GetSharedHierarchyAsync(double workspaceId);
      // Note: Return type should be SharedHierarchyDto.
  }
  ```
- [x] ISpacesService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Space, SpaceFeatures, CreateSpaceRequest, UpdateSpaceRequest etc.
  // public record SpaceDto(...); // Define based on GetSpace response
  // public record SpaceFeaturesDto(...); // Define based on 'features' object in SpaceDto
  // public record CreateSpaceRequest(string Name, bool MultipleAssignees, SpaceFeaturesDto Features);
  // public record UpdateSpaceRequest(string Name, string Color, bool Private, bool AdminCanManage, bool MultipleAssignees, SpaceFeaturesDto Features);

  namespace ClickUp.Abstract;

  // Represents the Spaces operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/team/{team_id}/space
  // - POST /v2/team/{team_id}/space
  // - GET /v2/space/{space_id}
  // - PUT /v2/space/{space_id}
  // - DELETE /v2/space/{space_id}

  public interface ISpacesService
  {
      /// <summary>
      /// Retrieves Spaces available in a specific Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="archived">Optional. Whether to include archived Spaces.</param>
      /// <returns>A list of Spaces in the Workspace.</returns>
      Task<IEnumerable<object>> GetSpacesAsync(double workspaceId, bool? archived = null);
      // Note: Return type should be IEnumerable<SpaceDto>.

      /// <summary>
      /// Creates a new Space in a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="createSpaceRequest">Details of the Space to create.</param>
      /// <returns>The created Space.</returns>
      Task<object> CreateSpaceAsync(double workspaceId, object createSpaceRequest);
      // Note: createSpaceRequest should be CreateSpaceRequest, return type should be SpaceDto.

      /// <summary>
      /// Retrieves details of a specific Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <returns>Details of the Space.</returns>
      Task<object> GetSpaceAsync(double spaceId);
      // Note: Return type should be SpaceDto.

      /// <summary>
      /// Updates a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="updateSpaceRequest">Details for updating the Space.</param>
      /// <returns>The updated Space.</returns>
      Task<object> UpdateSpaceAsync(double spaceId, object updateSpaceRequest);
      // Note: updateSpaceRequest should be UpdateSpaceRequest, return type should be SpaceDto.

      /// <summary>
      /// Deletes a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteSpaceAsync(double spaceId);
      // Note: API returns 200 with an empty object.
  }
  ```
- [x] ITagsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Tag, CreateTagRequest, EditTagRequest.
  // public record TagDto(string Name, string TagFg, string TagBg);
  // public record CreateTagRequest(TagDto Tag);
  // public record EditTagBody(string Name, string FgColor, string BgColor); // Renamed from Tag1 in spec for clarity
  // public record EditTagRequest(EditTagBody Tag);


  namespace ClickUp.Abstract;

  // Represents the Tags operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/space/{space_id}/tag
  // - POST /v2/space/{space_id}/tag
  // - PUT /v2/space/{space_id}/tag/{tag_name}
  // - DELETE /v2/space/{space_id}/tag/{tag_name}
  // - POST /v2/task/{task_id}/tag/{tag_name}
  // - DELETE /v2/task/{task_id}/tag/{tag_name}

  public interface ITagsService
  {
      /// <summary>
      /// Retrieves task Tags available in a specific Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <returns>A list of Tags in the Space.</returns>
      Task<IEnumerable<object>> GetSpaceTagsAsync(double spaceId);
      // Note: Return type should be IEnumerable<TagDto>. Response is { "tags": [...] }.

      /// <summary>
      /// Creates a new task Tag in a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="createTagRequest">Details of the Tag to create.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task CreateSpaceTagAsync(double spaceId, object createTagRequest);
      // Note: createTagRequest should be CreateTagRequest. API returns 200 with an empty object.

      /// <summary>
      /// Updates a task Tag in a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="tagName">The original name of the Tag to edit.</param>
      /// <param name="editTagRequest">Details for updating the Tag.</param>
      /// <returns>The updated Tag details.</returns>
      Task<object> EditSpaceTagAsync(double spaceId, string tagName, object editTagRequest);
      // Note: editTagRequest should be EditTagRequest. Response is { "tag": {...} }. Return type should be TagDto.

      /// <summary>
      /// Deletes a task Tag from a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="tagName">The name of the Tag to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteSpaceTagAsync(double spaceId, string tagName);
      // Note: API returns 200 with an empty object. The request body in spec seems incorrect for a DELETE; it should likely be parameter-based or empty. Assuming empty for now.

      /// <summary>
      /// Adds a Tag to a task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="tagName">The name of the Tag to add.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task AddTagToTaskAsync(string taskId, string tagName, bool? customTaskIds = null, double? teamId = null);
      // Note: API returns 200 with an empty object.

      /// <summary>
      /// Removes a Tag from a task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="tagName">The name of the Tag to remove.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task RemoveTagFromTaskAsync(string taskId, string tagName, bool? customTaskIds = null, double? teamId = null);
      // Note: API returns 200 with an empty object.
  }
  ```
- [x] ITasksService
  ```csharp
  using System;
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Task, CreateTaskRequest, UpdateTaskRequest, TaskTimeInStatus, etc.
  // public record TaskDto(...); // Define based on GetTask response
  // public record CreateTaskInListRequest(...); // Define based on POST /v2/list/{list_id}/task
  // public record UpdateTaskRequestDto(...); // Define based on PUT /v2/task/{task_id}
  // public record FilteredTasksRequestParams(...); // To encapsulate query params for GetFilteredTeamTasks
  // public record TaskTimeInStatusDto(...);
  // public record BulkTaskTimeInStatusDto(Dictionary<string, TaskTimeInStatusDto> Tasks); // Assuming task IDs are keys
  // public record MergeTasksRequest(IEnumerable<string> SourceTaskIds);
  // public record CreateTaskFromTemplateRequestDto(string Name);


  namespace ClickUp.Abstract;

  // Represents the Tasks operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/list/{list_id}/task
  // - POST /v2/list/{list_id}/task
  // - GET /v2/task/{task_id}
  // - PUT /v2/task/{task_id}
  // - DELETE /v2/task/{task_id}
  // - GET /v2/team/{team_Id}/task
  // - POST /v2/task/{task_id}/merge
  // - GET /v2/task/{task_id}/time_in_status
  // - GET /v2/task/bulk_time_in_status/task_ids
  // - POST /v2/list/{list_id}/taskTemplate/{template_id}

  public interface ITasksService
  {
      /// <summary>
      /// Retrieves tasks in a specific List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <param name="archived">Optional. Whether to include archived tasks.</param>
      /// <param name="includeMarkdownDescription">Optional. Whether to return task descriptions in Markdown format.</param>
      /// <param name="page">Optional. Page to fetch (starts at 0).</param>
      /// <param name="orderBy">Optional. Field to order by (e.g., "created", "updated", "due_date").</param>
      /// <param name="reverse">Optional. Whether to reverse the order.</param>
      /// <param name="subtasks">Optional. Whether to include subtasks.</param>
      /// <param name="statuses">Optional. Filter by statuses.</param>
      /// <param name="includeClosed">Optional. Whether to include closed tasks.</param>
      /// <param name="assignees">Optional. Filter by assignees (user IDs).</param>
      /// <param name="watchers">Optional. Filter by watchers (user IDs).</param>
      /// <param name="tags">Optional. Filter by tags.</param>
      /// <param name="dueDateGreaterThan">Optional. Filter by due date greater than (Unix time ms).</param>
      /// <param name="dueDateLessThan">Optional. Filter by due date less than (Unix time ms).</param>
      /// <param name="dateCreatedGreaterThan">Optional. Filter by date created greater than (Unix time ms).</param>
      /// <param name="dateCreatedLessThan">Optional. Filter by date created less than (Unix time ms).</param>
      /// <param name="dateUpdatedGreaterThan">Optional. Filter by date updated greater than (Unix time ms).</param>
      /// <param name="dateUpdatedLessThan">Optional. Filter by date updated less than (Unix time ms).</param>
      /// <param name="dateDoneGreaterThan">Optional. Filter by date done greater than (Unix time ms).</param>
      /// <param name="dateDoneLessThan">Optional. Filter by date done less than (Unix time ms).</param>
      /// <param name="customFields">Optional. Filter by custom fields (complex JSON string).</param>
      /// <param name="customItems">Optional. Filter by custom task types.</param>
      /// <returns>A list of tasks.</returns>
      Task<IEnumerable<object>> GetTasksAsync(double listId, bool? archived = null, bool? includeMarkdownDescription = null, int? page = null, string? orderBy = null, bool? reverse = null, bool? subtasks = null, IEnumerable<string>? statuses = null, bool? includeClosed = null, IEnumerable<string>? assignees = null, IEnumerable<string>? watchers = null, IEnumerable<string>? tags = null, long? dueDateGreaterThan = null, long? dueDateLessThan = null, long? dateCreatedGreaterThan = null, long? dateCreatedLessThan = null, long? dateUpdatedGreaterThan = null, long? dateUpdatedLessThan = null, long? dateDoneGreaterThan = null, long? dateDoneLessThan = null, string? customFields = null, IEnumerable<double>? customItems = null);
      // Note: Return type should be a DTO that includes 'tasks' and 'last_page' (e.g. GetTasksResponseDto). Individual task objects should be TaskDto.

      /// <summary>
      /// Creates a new task in a List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <param name="createTaskRequest">Details of the task to create.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>The created task.</returns>
      Task<object> CreateTaskAsync(double listId, object createTaskRequest, bool? customTaskIds = null, double? teamId = null);
      // Note: createTaskRequest should be CreateTaskInListRequest, return type should be TaskDto.

      /// <summary>
      /// Retrieves details of a specific task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <param name="includeSubtasks">Optional. Whether to include subtasks.</param>
      /// <param name="includeMarkdownDescription">Optional. Whether to return task description in Markdown format.</param>
      /// <returns>Details of the task.</returns>
      Task<object> GetTaskAsync(string taskId, bool? customTaskIds = null, double? teamId = null, bool? includeSubtasks = null, bool? includeMarkdownDescription = null);
      // Note: Return type should be TaskDto.

      /// <summary>
      /// Updates a task.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="updateTaskRequest">Details for updating the task.</param>
      /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>The updated task.</returns>
      Task<object> UpdateTaskAsync(string taskId, object updateTaskRequest, bool? customTaskIds = null, double? teamId = null);
      // Note: updateTaskRequest should be UpdateTaskRequestDto, return type should be TaskDto.

      /// <summary>
      /// Deletes a task.
      /// </summary>
      /// <param name="taskId">The ID of the task to delete.</param>
      /// <param name="customTaskIds">Optional. If true, references task by custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteTaskAsync(string taskId, bool? customTaskIds = null, double? teamId = null);
      // Note: API returns 204 No Content.

      /// <summary>
      /// Retrieves tasks from a Workspace based on specified filters.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_Id).</param>
      /// <param name="filterParams">Parameters for filtering tasks.</param>
      /// <returns>A list of filtered tasks.</returns>
      Task<IEnumerable<object>> GetFilteredTeamTasksAsync(double workspaceId, object filterParams);
      // Note: filterParams should be a DTO (e.g. FilteredTasksRequestParams) encapsulating all query parameters. Return type should be a DTO like GetTasksResponseDto.

      /// <summary>
      /// Merges multiple source tasks into a target task.
      /// </summary>
      /// <param name="targetTaskId">ID of the target task that other tasks will be merged into.</param>
      /// <param name="mergeTasksRequest">Contains the list of source task IDs to merge.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task MergeTasksAsync(string targetTaskId, object mergeTasksRequest);
      // Note: mergeTasksRequest should be MergeTasksRequest. API returns 200 OK.

      /// <summary>
      /// Retrieves the time a task has spent in each status.
      /// </summary>
      /// <param name="taskId">The ID of the task.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>Time in status information for the task.</returns>
      Task<object> GetTaskTimeInStatusAsync(string taskId, bool? customTaskIds = null, double? teamId = null);
      // Note: Return type should be TaskTimeInStatusDto.

      /// <summary>
      /// Retrieves the time in status for multiple tasks.
      /// </summary>
      /// <param name="taskIds">A list of task IDs.</param>
      /// <param name="customTaskIds">Optional. If true, references tasks by their custom task ids.</param>
      /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
      /// <returns>A dictionary mapping task IDs to their time in status information.</returns>
      Task<object> GetBulkTasksTimeInStatusAsync(IEnumerable<string> taskIds, bool? customTaskIds = null, double? teamId = null);
      // Note: Return type should be BulkTaskTimeInStatusDto or Dictionary<string, TaskTimeInStatusDto>.

      /// <summary>
      /// Creates a new task from a task template.
      /// </summary>
      /// <param name="listId">The ID of the List where the task will be created.</param>
      /// <param name="templateId">The ID of the task template.</param>
      /// <param name="createTaskFromTemplateRequest">Details for creating the task from a template (e.g., new task name).</param>
      /// <returns>The created task.</returns>
      Task<object> CreateTaskFromTemplateAsync(double listId, string templateId, object createTaskFromTemplateRequest);
      // Note: createTaskFromTemplateRequest should be CreateTaskFromTemplateRequestDto. Return type is not clearly defined in spec for this, might be TaskDto or an empty object.
  }
  ```
- [x] ITemplatesService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTO for TaskTemplate (the actual structure of which is not detailed in the GetTaskTemplates response example, it only shows an empty array).
  // For now, we can assume it might return a list of objects, each representing a template with at least an ID and Name.
  // public record TaskTemplateDto(string Id, string Name, ... other properties ...);

  namespace ClickUp.Abstract;

  // Represents the Templates operations in the ClickUp API, focusing on retrieving Task Templates.
  // Based on endpoints like:
  // - GET /v2/team/{team_id}/taskTemplate

  public interface ITemplatesService
  {
      /// <summary>
      /// Retrieves the task templates available in a specific Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="page">The page number to retrieve. API is 0-indexed. This is a required parameter in the ClickUp API for this endpoint.</param>
      /// <returns>A list of task templates available in the Workspace.</returns>
      Task<IEnumerable<object>> GetTaskTemplatesAsync(double workspaceId, int page);
      // Note: Return type should be IEnumerable<TaskTemplateDto>. The response example { "templates": [] } suggests it's a list.
      // The 'page' parameter is marked as required in the OpenAPI spec for this endpoint.
  }
  ```
- [x] ITimeTrackingService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for TimeEntry, TimeEntryTag, TimeEntryHistory, RunningTimeEntry, etc.
  // public record TimeEntryDto(...); // Define based on GET /v2/team/{team_Id}/time_entries
  // public record CreateTimeEntryRequest(...); // Define based on POST /v2/team/{team_Id}/time_entries
  // public record UpdateTimeEntryRequest(...); // Define based on PUT /v2/team/{team_id}/time_entries/{timer_id}
  // public record TimeEntryTagDto(string Name, string TagFg, string TagBg, int Creator);
  // public record TimeEntryTagsRequest(IEnumerable<string> TimeEntryIds, IEnumerable<TagDto> Tags); // TagDto from ITagsService might be reusable or need specific structure here
  // public record ChangeTagNameRequest(string Name, string NewName, string TagBg, string TagFg);
  // public record StartTimeEntryRequest(string? Description, IEnumerable<TagDto>? Tags, string? Tid, bool? Billable);


  namespace ClickUp.Abstract;

  // Represents the Time Tracking operations in the ClickUp API
  // Based on endpoints under the "Time Tracking" tag

  public interface ITimeTrackingService
  {
      /// <summary>
      /// Retrieves time entries within a specified date range for a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_Id).</param>
      /// <param name="startDate">Optional. Unix time in milliseconds for the start of the range.</param>
      /// <param name="endDate">Optional. Unix time in milliseconds for the end of the range.</param>
      /// <param name="assignee">Optional. Filter by user ID(s).</param>
      /// <param name="includeTaskTags">Optional. Whether to include task tags.</param>
      /// <param name="includeLocationNames">Optional. Whether to include List, Folder, and Space names.</param>
      /// <param name="spaceId">Optional. Filter by Space ID.</param>
      /// <param name="folderId">Optional. Filter by Folder ID.</param>
      /// <param name="listId">Optional. Filter by List ID.</param>
      /// <param name="taskId">Optional. Filter by Task ID.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
      /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for task_id.</param>
      /// <param name="isBillable">Optional. Filter by billable status.</param>
      /// <returns>A list of time entries.</returns>
      Task<IEnumerable<object>> GetTimeEntriesAsync(double workspaceId, double? startDate = null, double? endDate = null, string? assignee = null, bool? includeTaskTags = null, bool? includeLocationNames = null, bool? includeApprovalHistory = null, bool? includeApprovalDetails = null, double? spaceId = null, double? folderId = null, double? listId = null, string? taskId = null, bool? customTaskIds = null, double? teamIdForCustomTaskIds = null, bool? isBillable = null);
      // Note: Return type should be IEnumerable<TimeEntryDto>. Consider a request DTO for all optional parameters.

      /// <summary>
      /// Creates a new time entry.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_Id).</param>
      /// <param name="createTimeEntryRequest">Details of the time entry to create.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id (if tid is provided).</param>
      /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for tid.</param>
      /// <returns>The created time entry.</returns>
      Task<object> CreateTimeEntryAsync(double workspaceId, object createTimeEntryRequest, bool? customTaskIds = null, double? teamIdForCustomTaskIds = null);
      // Note: createTimeEntryRequest should be CreateTimeEntryRequest, return type should be TimeEntryDto (or a specific create response DTO).

      /// <summary>
      /// Retrieves a specific time entry.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="timerId">The ID of the time entry (timer_id).</param>
      /// <param name="includeTaskTags">Optional. Whether to include task tags.</param>
      /// <param name="includeLocationNames">Optional. Whether to include List, Folder, and Space names.</param>
      /// <returns>Details of the time entry.</returns>
      Task<object> GetTimeEntryAsync(double workspaceId, string timerId, bool? includeTaskTags = null, bool? includeLocationNames = null, bool? includeApprovalHistory = null, bool? includeApprovalDetails = null);
      // Note: Return type should be TimeEntryDto.

      /// <summary>
      /// Updates a time entry.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="timerId">The ID of the time entry (timer_id).</param>
      /// <param name="updateTimeEntryRequest">Details for updating the time entry.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id (if tid is provided).</param>
      /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for tid.</param>
      /// <returns>An awaitable task representing the asynchronous operation. The API returns 200 with an empty object or updated time entry details.</returns>
      Task<object> UpdateTimeEntryAsync(double workspaceId, string timerId, object updateTimeEntryRequest, bool? customTaskIds = null, double? teamIdForCustomTaskIds = null);
      // Note: updateTimeEntryRequest should be UpdateTimeEntryRequest. The response seems to be the updated time entry.

      /// <summary>
      /// Deletes a time entry.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="timerId">The ID of the time entry (timer_id) to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteTimeEntryAsync(double workspaceId, string timerId);
      // Note: API returns 200 with an empty object in some cases, or the deleted object. Check spec for specifics. For simplicity, Task if empty.

      /// <summary>
      /// Retrieves the history of changes for a specific time entry.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="timerId">The ID of the time entry (timer_id).</param>
      /// <returns>A list of history records for the time entry.</returns>
      Task<IEnumerable<object>> GetTimeEntryHistoryAsync(double workspaceId, string timerId);
      // Note: Return type should be IEnumerable<TimeEntryHistoryDto>.

      /// <summary>
      /// Retrieves the currently running time entry for the authenticated user (or specified assignee).
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="assignee">Optional. User ID to get running timer for. Only for Workspace Owners/Admins.</param>
      /// <returns>The currently running time entry, or null if none.</returns>
      Task<object?> GetRunningTimeEntryAsync(double workspaceId, double? assignee = null);
      // Note: Return type should be RunningTimeEntryDto or a similar DTO. Nullable if no timer is running.

      /// <summary>
      /// Starts a new time entry (timer).
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_Id).</param>
      /// <param name="startTimeEntryRequest">Details for starting the timer, such as task ID (tid), description, tags.</param>
      /// <param name="customTaskIds">Optional. If true, references task by its custom task id (if tid is provided).</param>
      /// <param name="teamIdForCustomTaskIds">Optional. Workspace ID, required if customTaskIds is true for tid.</param>
      /// <returns>Details of the started time entry.</returns>
      Task<object> StartTimeEntryAsync(double workspaceId, object startTimeEntryRequest, bool? customTaskIds = null, double? teamIdForCustomTaskIds = null);
      // Note: startTimeEntryRequest should be StartTimeEntryRequest. Return type should be a DTO for the running timer.

      /// <summary>
      /// Stops the currently running time entry for the authenticated user.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <returns>Details of the stopped time entry.</returns>
      Task<object> StopTimeEntryAsync(double workspaceId);
      // Note: Return type should be a DTO representing the stopped time entry.

      /// <summary>
      /// Retrieves all tags used in time entries for a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <returns>A list of time entry tags.</returns>
      Task<IEnumerable<object>> GetAllTimeEntryTagsAsync(double workspaceId);
      // Note: Return type should be IEnumerable<TimeEntryTagDto>.

      /// <summary>
      /// Adds tags to specified time entries.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="tagsRequest">Request containing time entry IDs and tags to add.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task AddTagsToTimeEntriesAsync(double workspaceId, object tagsRequest);
      // Note: tagsRequest should be TimeEntryTagsRequest. API returns 200 with an empty object.

      /// <summary>
      /// Removes tags from specified time entries.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="tagsRequest">Request containing time entry IDs and tags to remove.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task RemoveTagsFromTimeEntriesAsync(double workspaceId, object tagsRequest);
      // Note: tagsRequest should be TimeEntryTagsRequest. API returns 200 with an empty object.

      /// <summary>
      /// Changes the name and colors of a time entry tag across a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="changeTagNameRequest">Details for changing the tag name and colors.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task ChangeTimeEntryTagNameAsync(double workspaceId, object changeTagNameRequest);
      // Note: changeTagNameRequest should be ChangeTagNameRequest. API returns 200 with an empty object.
  }
  ```
- [x] IUsersService
  ```csharp
  using System.Threading.Tasks;

  // Assuming DTOs for User, MemberDetails, EditUserRequest etc.
  // public record UserDto(...); // Already referenced in IAuthorizationService, ensure consistency or specific variant if needed
  // public record MemberDetailsDto(UserDto User, object InvitedBy, object Shared); // Simplified, based on GetUser response
  // public record EditUserOnWorkspaceRequest(string Username, bool Admin, int? CustomRoleId);

  namespace ClickUp.Abstract;

  // Represents User management operations in the ClickUp API for a specific Workspace.
  // Based on endpoints like:
  // - GET /v2/team/{team_id}/user/{user_id}
  // - PUT /v2/team/{team_id}/user/{user_id}
  // - DELETE /v2/team/{team_id}/user/{user_id}
  // Note: Inviting users is handled by IGuestsService or a dedicated InvitesService. GetAuthorizedUser is in IAuthorizationService.

  public interface IUsersService
  {
      /// <summary>
      /// Retrieves information about a specific user in a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="userId">The ID of the user.</param>
      /// <param name="includeShared">Optional. Exclude details of items shared with the user by setting to false.</param>
      /// <returns>Details of the user within the Workspace context.</returns>
      Task<object> GetUserFromWorkspaceAsync(double workspaceId, double userId, bool? includeShared = null);
      // Note: Return type should be MemberDetailsDto.

      /// <summary>
      /// Updates a user's name and role within a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="userId">The ID of the user to edit.</param>
      /// <param name="editUserRequest">Details for updating the user.</param>
      /// <returns>Details of the updated user within the Workspace context.</returns>
      Task<object> EditUserOnWorkspaceAsync(double workspaceId, double userId, object editUserRequest);
      // Note: editUserRequest should be EditUserOnWorkspaceRequest. Return type should be MemberDetailsDto.

      /// <summary>
      /// Deactivates/Removes a user from a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="userId">The ID of the user to remove.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task<object> RemoveUserFromWorkspaceAsync(double workspaceId, double userId);
      // Note: API returns a 'team' object. Consider if Task is sufficient or if this DTO is needed.
  }
  ```
- [x] IViewsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for View, ViewTask, CreateViewRequest, UpdateViewRequest etc.
  // public record ViewDto(...); // Define based on GetView response
  // public record ViewTaskDto(...); // Define based on GetViewTasks response, likely similar to TaskDto
  // public record CreateViewRequest(...); // Define based on POST requests for views
  // public record UpdateViewRequestDto(...); // Define based on PUT /v2/view/{view_id}

  namespace ClickUp.Abstract;

  // Represents the Views operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/team/{team_id}/view
  // - POST /v2/team/{team_id}/view
  // - GET /v2/space/{space_id}/view
  // - POST /v2/space/{space_id}/view
  // - GET /v2/folder/{folder_id}/view
  // - POST /v2/folder/{folder_id}/view
  // - GET /v2/list/{list_id}/view
  // - POST /v2/list/{list_id}/view
  // - GET /v2/view/{view_id}
  // - PUT /v2/view/{view_id}
  // - DELETE /v2/view/{view_id}
  // - GET /v2/view/{view_id}/task

  public interface IViewsService
  {
      /// <summary>
      /// Retrieves Views at the Workspace (Everything) level.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <returns>A list of Views at the Workspace level.</returns>
      Task<IEnumerable<object>> GetWorkspaceViewsAsync(double workspaceId);
      // Note: Return type should be IEnumerable<ViewDto>.

      /// <summary>
      /// Creates a new View at the Workspace (Everything) level.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <param name="createViewRequest">Details of the View to create.</param>
      /// <returns>The created View.</returns>
      Task<object> CreateWorkspaceViewAsync(double workspaceId, object createViewRequest);
      // Note: createViewRequest should be CreateViewRequest, return type should be ViewDto.

      /// <summary>
      /// Retrieves Views for a specific Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <returns>A list of Views in the Space.</returns>
      Task<IEnumerable<object>> GetSpaceViewsAsync(double spaceId);
      // Note: Return type should be IEnumerable<ViewDto>.

      /// <summary>
      /// Creates a new View in a Space.
      /// </summary>
      /// <param name="spaceId">The ID of the Space.</param>
      /// <param name="createViewRequest">Details of the View to create.</param>
      /// <returns>The created View.</returns>
      Task<object> CreateSpaceViewAsync(double spaceId, object createViewRequest);
      // Note: createViewRequest should be CreateViewRequest, return type should be ViewDto.

      /// <summary>
      /// Retrieves Views for a specific Folder.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <returns>A list of Views in the Folder.</returns>
      Task<IEnumerable<object>> GetFolderViewsAsync(double folderId);
      // Note: Return type should be IEnumerable<ViewDto>.

      /// <summary>
      /// Creates a new View in a Folder.
      /// </summary>
      /// <param name="folderId">The ID of the Folder.</param>
      /// <param name="createViewRequest">Details of the View to create.</param>
      /// <returns>The created View.</returns>
      Task<object> CreateFolderViewAsync(double folderId, object createViewRequest);
      // Note: createViewRequest should be CreateViewRequest, return type should be ViewDto.

      /// <summary>
      /// Retrieves Views for a specific List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <returns>A list of Views in the List.</returns>
      Task<IEnumerable<object>> GetListViewsAsync(double listId);
      // Note: Return type should be IEnumerable<ViewDto>.

      /// <summary>
      /// Creates a new View in a List.
      /// </summary>
      /// <param name="listId">The ID of the List.</param>
      /// <param name="createViewRequest">Details of the View to create.</param>
      /// <returns>The created View.</returns>
      Task<object> CreateListViewAsync(double listId, object createViewRequest);
      // Note: createViewRequest should be CreateViewRequest, return type should be ViewDto.

      /// <summary>
      /// Retrieves details of a specific View.
      /// </summary>
      /// <param name="viewId">The ID of the View.</param>
      /// <returns>Details of the View.</returns>
      Task<object> GetViewAsync(string viewId);
      // Note: Return type should be ViewDto.

      /// <summary>
      /// Updates a View.
      /// </summary>
      /// <param name="viewId">The ID of the View.</param>
      /// <param name="updateViewRequest">Details for updating the View.</param>
      /// <returns>The updated View.</returns>
      Task<object> UpdateViewAsync(string viewId, object updateViewRequest);
      // Note: updateViewRequest should be UpdateViewRequestDto, return type should be ViewDto.

      /// <summary>
      /// Deletes a View.
      /// </summary>
      /// <param name="viewId">The ID of the View to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteViewAsync(string viewId);
      // Note: API returns 200 with an empty object.

      /// <summary>
      /// Retrieves tasks visible in a specific View.
      /// </summary>
      /// <param name="viewId">The ID of the View.</param>
      /// <param name="page">The page number to retrieve (0-indexed).</param>
      /// <returns>A list of tasks in the View.</returns>
      Task<IEnumerable<object>> GetViewTasksAsync(string viewId, int page);
      // Note: Return type should be a DTO that includes 'tasks' and 'last_page' (e.g. GetViewTasksResponseDto). Individual task objects should be ViewTaskDto/TaskDto.
  }
  ```
- [x] IWebhooksService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Webhook, WebhookHealth, CreateWebhookRequest, UpdateWebhookRequest.
  // public record WebhookHealthDto(string Status, int FailCount);
  // public record WebhookDto(string Id, int UserId, int TeamId, string Endpoint, string ClientId, IEnumerable<string> Events, string? TaskId, string? ListId, string? FolderId, string? SpaceId, WebhookHealthDto Health, string Secret);
  // public record CreateWebhookRequest(string Endpoint, IEnumerable<string> Events, int? SpaceId = null, int? FolderId = null, int? ListId = null, string? TaskId = null);
  // public record UpdateWebhookRequest(string Endpoint, string Events, string Status); // "Events" is string in spec for update, likely "*" or specific events.

  namespace ClickUp.Abstract;

  // Represents the Webhooks operations in the ClickUp API
  // Based on endpoints like:
  // - GET /v2/team/{team_id}/webhook
  // - POST /v2/team/{team_id}/webhook
  // - PUT /v2/webhook/{webhook_id}
  // - DELETE /v2/webhook/{webhook_id}

  public interface IWebhooksService
  {
      /// <summary>
      /// Retrieves Webhooks created by the authenticated user for a specific Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
      /// <returns>A list of Webhooks for the Workspace.</returns>
      Task<IEnumerable<object>> GetWebhooksAsync(double workspaceId);
      // Note: Return type should be IEnumerable<WebhookDto>. Response is { "webhooks": [...] }.

      /// <summary>
      /// Creates a new Webhook.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace (team_id) where the webhook will be created.</param>
      /// <param name="createWebhookRequest">Details of the Webhook to create.</param>
      /// <returns>The created Webhook details, including its ID and secret.</returns>
      Task<object> CreateWebhookAsync(double workspaceId, object createWebhookRequest);
      // Note: createWebhookRequest should be CreateWebhookRequest. Return type should be a DTO containing "id" and "webhook" (WebhookDto).

      /// <summary>
      /// Updates a Webhook.
      /// </summary>
      /// <param name="webhookId">The UUID of the Webhook to update.</param>
      /// <param name="updateWebhookRequest">Details for updating the Webhook (endpoint, events, status).</param>
      /// <returns>The updated Webhook details.</returns>
      Task<object> UpdateWebhookAsync(string webhookId, object updateWebhookRequest);
      // Note: updateWebhookRequest should be UpdateWebhookRequest. Return type should be a DTO containing "id" and "webhook" (WebhookDto).

      /// <summary>
      /// Deletes a Webhook.
      /// </summary>
      /// <param name="webhookId">The UUID of the Webhook to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteWebhookAsync(string webhookId);
      // Note: API returns 200 with an empty object.
  }
  ```
- [x] IDocsService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Doc, Page, PageListing, request bodies, etc.
  // public record DocDto(...); // Define based on /v3/workspaces/{workspaceId}/docs/{docId} GET response
  // public record PageDto(...); // Define based on /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId} GET response
  // public record PageListingItemDto(...); // Define based on /v3/workspaces/{workspaceId}/docs/{docId}/pageListing response
  // public record CreateDocRequest(string Name, object Parent, string? Visibility, bool? CreatePage);
  // public record CreatePageRequest(string? ParentPageId, string Name, string? SubTitle, string Content, string? ContentFormat);
  // public record EditPageRequest(string? Name, string? SubTitle, string? Content, string? ContentEditMode, string? ContentFormat);
  // public record SearchDocsParams(string? Id, double? Creator, bool? Deleted, bool? Archived, string? ParentId, string? ParentType, int? Limit, string? NextCursor);

  namespace ClickUp.Abstract;

  // Represents the Docs operations in the ClickUp API (v3)
  // Based on endpoints like:
  // - GET /v3/workspaces/{workspaceId}/docs
  // - POST /v3/workspaces/{workspaceId}/docs
  // - GET /v3/workspaces/{workspaceId}/docs/{docId}
  // - GET /v3/workspaces/{workspaceId}/docs/{docId}/pageListing
  // - GET /v3/workspaces/{workspaceId}/docs/{docId}/pages
  // - POST /v3/workspaces/{workspaceId}/docs/{docId}/pages
  // - GET /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}
  // - PUT /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId} (Edit Page - was PATCH in my thought, but spec says PUT for editPage operationId)

  public interface IDocsService
  {
      /// <summary>
      /// Searches for Docs within a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="searchParams">Parameters for searching/filtering docs.</param>
      /// <returns>A list of Docs matching the search criteria.</returns>
      Task<object> SearchDocsAsync(double workspaceId, object searchParams);
      // Note: searchParams should be SearchDocsParams. Return should be a DTO with 'docs' and 'next_cursor'.

      /// <summary>
      /// Creates a new Doc in a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="createDocRequest">Details for creating the Doc.</param>
      /// <returns>The created Doc.</returns>
      Task<object> CreateDocAsync(double workspaceId, object createDocRequest);
      // Note: createDocRequest should be CreateDocRequest, return type should be DocDto.

      /// <summary>
      /// Retrieves details of a specific Doc.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="docId">The ID of the Doc.</param>
      /// <returns>Details of the Doc.</returns>
      Task<object> GetDocAsync(double workspaceId, string docId);
      // Note: Return type should be DocDto.

      /// <summary>
      /// Retrieves the PageListing for a Doc.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="docId">The ID of the Doc.</param>
      /// <param name="maxPageDepth">Optional. Maximum depth to retrieve pages and subpages.</param>
      /// <returns>A list of page listing items.</returns>
      Task<IEnumerable<object>> GetDocPageListingAsync(double workspaceId, string docId, double? maxPageDepth = null);
      // Note: Return type should be IEnumerable<PageListingItemDto>.

      /// <summary>
      /// Retrieves all pages within a Doc.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="docId">The ID of the Doc.</param>
      /// <param name="maxPageDepth">Optional. Maximum depth to retrieve pages and subpages.</param>
      /// <param name="contentFormat">Optional. Format for page content (e.g., "text/md").</param>
      /// <returns>A list of pages within the Doc.</returns>
      Task<IEnumerable<object>> GetDocPagesAsync(double workspaceId, string docId, double? maxPageDepth = null, string? contentFormat = null);
      // Note: Return type should be IEnumerable<PageDto>.

      /// <summary>
      /// Creates a new page in a Doc.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="docId">The ID of the Doc.</param>
      /// <param name="createPageRequest">Details for creating the page.</param>
      /// <returns>The created page.</returns>
      Task<object> CreatePageAsync(double workspaceId, string docId, object createPageRequest);
      // Note: createPageRequest should be CreatePageRequest, return type should be PageDto.

      /// <summary>
      /// Retrieves details of a specific page within a Doc.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="docId">The ID of the Doc.</param>
      /// <param name="pageId">The ID of the page.</param>
      /// <param name="contentFormat">Optional. Format for page content.</param>
      /// <returns>Details of the page.</returns>
      Task<object> GetPageAsync(double workspaceId, string docId, string pageId, string? contentFormat = null);
      // Note: Return type should be PageDto.

      /// <summary>
      /// Edits a page within a Doc.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="docId">The ID of the Doc.</param>
      /// <param name="pageId">The ID of the page to edit.</param>
      /// <param name="editPageRequest">Details for editing the page.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task EditPageAsync(double workspaceId, string docId, string pageId, object editPageRequest);
      // Note: editPageRequest should be EditPageRequest. API returns 200 with an empty object.
  }
  ```
- [x] IChatService
  ```csharp
  using System.Threading.Tasks;
  using System.Collections.Generic;

  // Assuming DTOs for Channel, Message, Reaction, User, request bodies, etc.
  // public record ChatChannelDto(...);
  // public record ChatMessageDto(...);
  // public record ChatReactionDto(...);
  // public record ChatUserDto(...); // Simplified user representation for chat contexts
  // public record CreateChannelRequest(string Name, string? Description, string? Topic, IEnumerable<string>? UserIds, string? Visibility);
  // public record CreateLocationChannelRequest(object Location, string? Description, string? Topic, IEnumerable<string>? UserIds, string? Visibility); // Location DTO
  // public record CreateDirectMessageChannelRequest(IEnumerable<string>? UserIds);
  // public record UpdateChannelRequest(string? Name, string? Description, string? Topic, string? Visibility, object? Location);
  // public record CreateChatMessageRequest(string Content, string? Assignee = null, string? GroupAssignee = null, ...); // And other fields from spec
  // public record PatchChatMessageRequest(string? Content, string? Assignee = null, string? GroupAssignee = null, bool? Resolved = null, ...);
  // public record CreateChatReactionRequest(string Reaction);


  namespace ClickUp.Abstract;

  // Represents the Chat (Experimental) operations in the ClickUp API (v3)
  // Based on endpoints under the "Chat (Experimental)" tag

  public interface IChatService
  {
      #region Channels
      /// <summary>
      /// Retrieves Channels in a Workspace.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="descriptionFormat">Optional. Format of the Channel Description.</param>
      /// <param name="cursor">Optional. Cursor for pagination.</param>
      /// <param name="limit">Optional. Maximum number of results per page.</param>
      /// <param name="isFollower">Optional. Filter to Channels the user is following.</param>
      /// <param name="includeHidden">Optional. Include DMs/Group DMs that have been explicitly closed.</param>
      /// <param name="withCommentSince">Optional. Only return Channels with comments since the given timestamp.</param>
      /// <param name="roomTypes">Optional. Types of Channels to return (CHANNEL, DM, GROUP_DM).</param>
      /// <returns>A paginated list of Channels.</returns>
      Task<object> GetChatChannelsAsync(long workspaceId, string? descriptionFormat = null, string? cursor = null, int? limit = null, bool? isFollower = null, bool? includeHidden = null, long? withCommentSince = null, IEnumerable<string>? roomTypes = null);
      // Note: Return type should be a DTO containing 'data' (IEnumerable<ChatChannelDto>) and 'next_cursor'.

      /// <summary>
      /// Creates a new Channel not tied to a specific location.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="createChannelRequest">Details for creating the Channel.</param>
      /// <returns>The created or existing Channel.</returns>
      Task<object> CreateChatChannelAsync(long workspaceId, object createChannelRequest);
      // Note: createChannelRequest should be CreateChannelRequest. Return type should be ChatChannelDto (wrapped in 'data').

      /// <summary>
      /// Creates a Channel on a Space, Folder, or List.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="createLocationChannelRequest">Details for creating the location-based Channel.</param>
      /// <returns>The created or existing Channel.</returns>
      Task<object> CreateLocationChatChannelAsync(long workspaceId, object createLocationChannelRequest);
      // Note: createLocationChannelRequest should be CreateLocationChannelRequest. Return type should be ChatChannelDto (wrapped in 'data').

      /// <summary>
      /// Creates a new Direct Message or Group Direct Message.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="createDirectMessageChannelRequest">User IDs for the DM/Group DM.</param>
      /// <returns>The created or existing Direct Message Channel.</returns>
      Task<object> CreateDirectMessageChatChannelAsync(long workspaceId, object createDirectMessageChannelRequest);
      // Note: createDirectMessageChannelRequest should be CreateDirectMessageChannelRequest. Return type should be ChatChannelDto (wrapped in 'data').

      /// <summary>
      /// Retrieves a specific Channel by its ID.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="channelId">The ID of the Channel.</param>
      /// <param name="descriptionFormat">Optional. Format of the Channel Description.</param>
      /// <returns>Details of the Channel.</returns>
      Task<object> GetChatChannelAsync(long workspaceId, string channelId, string? descriptionFormat = null);
      // Note: Return type should be ChatChannelDto (wrapped in 'data').

      /// <summary>
      /// Updates a Channel.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="channelId">The ID of the Channel to update.</param>
      /// <param name="updateChannelRequest">Details for updating the Channel.</param>
      /// <returns>The updated Channel.</returns>
      Task<object> UpdateChatChannelAsync(long workspaceId, string channelId, object updateChannelRequest);
      // Note: updateChannelRequest should be UpdateChannelRequest. Return type should be ChatChannelDto (wrapped in 'data').

      /// <summary>
      /// Deletes a Channel.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="channelId">The ID of the Channel to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteChatChannelAsync(long workspaceId, string channelId);
      // Note: API returns 204 No Content.

      /// <summary>
      /// Retrieves followers of a specific Channel.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="channelId">The ID of the Channel.</param>
      /// <param name="cursor">Optional. Cursor for pagination.</param>
      /// <param name="limit">Optional. Maximum number of results per page.</param>
      /// <returns>A paginated list of Channel followers.</returns>
      Task<object> GetChatChannelFollowersAsync(long workspaceId, string channelId, string? cursor = null, int? limit = null);
      // Note: Return type should be a DTO with 'data' (IEnumerable<ChatUserDto>) and 'next_cursor'.

      /// <summary>
      /// Retrieves members of a specific Channel.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="channelId">The ID of the Channel.</param>
      /// <param name="cursor">Optional. Cursor for pagination.</param>
      /// <param name="limit">Optional. Maximum number of results per page.</param>
      /// <returns>A paginated list of Channel members.</returns>
      Task<object> GetChatChannelMembersAsync(long workspaceId, string channelId, string? cursor = null, int? limit = null);
      // Note: Return type should be a DTO with 'data' (IEnumerable<ChatUserDto>) and 'next_cursor'.
      #endregion

      #region Messages
      /// <summary>
      /// Retrieves messages for a specified Channel.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="channelId">The ID of the Channel.</param>
      /// <param name="cursor">Optional. Cursor for pagination.</param>
      /// <param name="limit">Optional. Maximum number of results per page.</param>
      /// <param name="contentFormat">Optional. Format of the message content.</param>
      /// <returns>A paginated list of messages.</returns>
      Task<object> GetChatMessagesAsync(long workspaceId, string channelId, string? cursor = null, int? limit = null, string? contentFormat = null);
      // Note: Return type should be a DTO with 'data' (IEnumerable<ChatMessageDto>) and 'next_cursor'.

      /// <summary>
      /// Sends a message to a Channel.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="channelId">The ID of the Channel.</param>
      /// <param name="createMessageRequest">Details of the message to send.</param>
      /// <returns>The created message.</returns>
      Task<object> CreateChatMessageAsync(long workspaceId, string channelId, object createMessageRequest);
      // Note: createMessageRequest should be CreateChatMessageRequest. Return type should be ChatMessageDto.

      /// <summary>
      /// Updates a message.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="messageId">The ID of the message to update.</param>
      /// <param name="patchMessageRequest">Details for updating the message.</param>
      /// <returns>The updated message.</returns>
      Task<object> UpdateChatMessageAsync(long workspaceId, string messageId, object patchMessageRequest);
      // Note: patchMessageRequest should be PatchChatMessageRequest. Return type should be ChatMessageDto.

      /// <summary>
      /// Deletes a message.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="messageId">The ID of the message to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteChatMessageAsync(long workspaceId, string messageId);
      // Note: API returns 204 No Content.

      /// <summary>
      /// Creates a reply to a message.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="messageId">The ID of the parent message.</param>
      /// <param name="createReplyRequest">Details of the reply message to send.</param>
      /// <returns>The created reply message.</returns>
      Task<object> CreateReplyMessageAsync(long workspaceId, string messageId, object createReplyRequest);
      // Note: createReplyRequest should be CreateChatMessageRequest (as it's similar). Return type should be ChatMessageDto.

      /// <summary>
      /// Retrieves replies to a message.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="messageId">The ID of the parent message.</param>
      /// <param name="cursor">Optional. Cursor for pagination.</param>
      /// <param name="limit">Optional. Maximum number of results per page.</param>
      /// <param name="contentFormat">Optional. Format of the message content.</param>
      /// <returns>A paginated list of reply messages.</returns>
      Task<object> GetChatMessageRepliesAsync(long workspaceId, string messageId, string? cursor = null, int? limit = null, string? contentFormat = null);
      // Note: Return type should be a DTO with 'data' (IEnumerable<ChatMessageDto>) and 'next_cursor'.
      #endregion

      #region Reactions & Tagged Users
      /// <summary>
      /// Retrieves reactions for a message.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="messageId">The ID of the message.</param>
      /// <param name="cursor">Optional. Cursor for pagination.</param>
      /// <param name="limit">Optional. Maximum number of results per page.</param>
      /// <returns>A paginated list of reactions.</returns>
      Task<object> GetChatMessageReactionsAsync(long workspaceId, string messageId, string? cursor = null, int? limit = null);
      // Note: Return type should be a DTO with 'data' (IEnumerable<ChatReactionDto>) and 'next_cursor'.

      /// <summary>
      /// Creates a reaction to a message.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="messageId">The ID of the message.</param>
      /// <param name="createReactionRequest">The reaction emoji name.</param>
      /// <returns>The created reaction.</returns>
      Task<object> CreateChatReactionAsync(long workspaceId, string messageId, object createReactionRequest);
      // Note: createReactionRequest should be CreateChatReactionRequest. Return type should be ChatReactionDto.

      /// <summary>
      /// Deletes a message reaction.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="messageId">The ID of the message.</param>
      /// <param name="reaction">The name of the reaction to delete.</param>
      /// <returns>An awaitable task representing the asynchronous operation.</returns>
      Task DeleteChatReactionAsync(long workspaceId, string messageId, string reaction);
      // Note: API returns 204 No Content.

      /// <summary>
      /// Retrieves users tagged in a message.
      /// </summary>
      /// <param name="workspaceId">The ID of the Workspace.</param>
      /// <param name="messageId">The ID of the message.</param>
      /// <param name="cursor">Optional. Cursor for pagination.</param>
      /// <param name="limit">Optional. Maximum number of results per page.</param>
      /// <returns>A paginated list of tagged users.</returns>
      Task<object> GetChatMessageTaggedUsersAsync(long workspaceId, string messageId, string? cursor = null, int? limit = null);
      // Note: Return type should be a DTO with 'data' (IEnumerable<ChatUserDto>) and 'next_cursor'.
      #endregion
  }
  ```
