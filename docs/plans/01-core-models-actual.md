
## Phase 1, Step 1: Define Core Models (Actual Implementation)
This document tracks the creation of C# models based on the OpenAPI specification found in /docs/OpenApiSpec/ClickUp-6-17-25.json and the conceptual plan in 01-core-models-conceptual.md.

Models will be implemented in the src/ClickUp.Api.Client.Models/ directory.

Identified Schemas and Prioritization (Based on OpenAPI Spec Review)
This list includes schemas identified from components.schemas (which are generally preferred) and inline schemas from request/response bodies. The priority for implementation is marked (P0 > P1 > P2 > P3).

## Foundational & Core Entities:
* [ ] User (P0) - Represents a ClickUp user. (Seen inline, e.g., /v2/user response, and expected in components.schemas)
* [ ] Workspace / Team (P0) - Represents a ClickUp Workspace. (Seen as Team inline, e.g., /v2/team response, and expected in components.schemas. Consider naming ClickUpWorkspace in C#)
* [ ] Space (P0) - Organizational unit within a Workspace. (Seen inline, e.g., in Task model, and expected in components.schemas)
* [ ] Folder (P0) - Organizational unit within a Space. (Seen inline, e.g., in Task model, and expected in components.schemas)
* [ ] List (P0) - Organizational unit containing tasks. (Seen inline, e.g., in Task model, and expected in components.schemas)
* [ ] Task (P0) - The core unit of work. (Seen inline, e.g., AddTaskLinkresponse, and expected as a detailed model in components.schemas)
* [ ] Status (P0) - Task status (e.g., "Open", "In Progress"). (Seen inline in Task model, and expected in components.schemas)
* [ ] Priority (P0) - Task priority. (Seen inline in Task model, and expected in components.schemas)
* [ ] Comment (P1) - Comments on tasks, etc. (Seen inline, e.g., GetTaskCommentsresponse, and expected in components.schemas)
* [ ] CustomFieldDefinition / Field (P1) - Definition of a Custom Field. (Seen as Field inline, and expected in components.schemas)
* [ ] CustomFieldValue (P1) - Value of a Custom Field on a task. (May be part of Task or separate, structure seen in SetCustomFieldValuerequest)
* [ ] Tag (P1) - Task tags. (Seen inline in Task model, and expected in components.schemas)
* [ ] Checklist (P1) - Checklist on a task. (Seen inline, e.g., CreateChecklistresponse, and expected in components.schemas)
* [ ] ChecklistItem (P1) - Item within a checklist. (Seen inline, e.g., Item in Checklist1, and expected in components.schemas)
* [ ] Attachment (P1) - File attachments. (Seen as CreateTaskAttachmentresponse and likely in components.schemas)
* [ ] Member (P1) - User's membership in various entities. (Seen inline in Team, Goal2, etc., and expected in components.schemas)
* [ ] Goal (P1) - Goal entity. (Seen inline as Goal, Goal2, Goal3, and expected in components.schemas)
* [ ] KeyResult (P1) - Key Result for a Goal. (Seen inline as KeyResult, KeyResult1, and expected in components.schemas)
* [ ] Webhook (P1) - Webhook definition. (Seen in GetWebhooksresponse, and expected in components.schemas)
* [ ] Doc (P1) - Document entity (v3). (Expected based on v3 paths like /v3/workspaces/{workspaceId}/docs)
* [ ] Page (P1) - Page within a Doc (v3). (Expected based on v3 paths like /v3/workspaces/{workspaceId}/docs/{docId}/pages)
* [ ] ChatChannel (P2) - Chat channel (v3). (Expected from #/components/schemas/ChatChannel)
* [ ] ChatMessage (P2) - Message in a chat channel (v3). (Expected from #/components/schemas/ChatMessage)
* [ ] ChatReaction (P2) - Reaction to a chat message (v3). (Expected from #/components/schemas/ChatReaction)
* [ ] Role (P2) - User role. (Seen inline in InviteGuestToWorkspaceresponse within Team1.roles)
* [ ] CustomRole (P2) - Custom user role. (Seen as CustomRole2 inline, and expected in components.schemas)
* [ ] Guest (P2) - Guest user details. (Seen inline, e.g., EditGuestOnWorkspaceresponse, and expected in components.schemas)
* [ ] View (P2) - View definition for tasks, etc. (Seen in GetTeamViewsresponse, and expected in components.schemas)
* [ ] TimeEntry (P2) - Time tracking entry. (Seen as Datum, Datum1, Datum2, Data2, Data3 in various time tracking responses)

## Request/Response Specific Models (Often Wrappers or Inline Definitions):
These might be refactored into the core entities above or generated as specific request/response classes.

* [ ] CreateTaskAttachmentresponse (P2) - Response for creating a task attachment.
* [ ] GetAccessTokenresponse (P3) - Response for getting OAuth access token.
* [ ] CreateChecklistrequest (P2) - Request to create a checklist.
* [ ] CreateChecklistresponse (P2) - Response for creating a checklist.
* [ ] EditChecklistrequest (P2) - Request to edit a checklist.
* [ ] CreateChecklistItemrequest (P2) - Request to create a checklist item.
* [ ] CreateChecklistItemresponse (P2) - Response for creating a checklist item.
* [ ] EditChecklistItemrequest (P2) - Request to edit a checklist item.
* [ ] EditChecklistItemresponse (P2) - Response for editing a checklist item.
* [ ] GetTaskCommentsresponse (P2) - Wrapper for a list of comments.
* [ ] CreateTaskCommentrequest (P2) - Request to create a task comment.
* [ ] CreateTaskCommentresponse (P2) - Response for creating a task comment.
* [ ] GetChatViewCommentsresponse (P2) - Wrapper for chat view comments.
* [ ] CreateChatViewCommentrequest (P2) - Request for creating chat view comment.
* [ ] CreateChatViewCommentresponse (P2) - Response for creating chat view comment.
* [ ] GetListCommentsresponse (P2) - Wrapper for list comments.
* [ ] CreateListCommentrequest (P2) - Request to create a list comment.
* [ ] CreateListCommentresponse (P2) - Response for creating list comment.
* [ ] UpdateCommentrequest (P2) - Request to update a comment.
* [ ] GetAccessibleCustomFieldsresponse (P2) - Wrapper for list of custom fields.
* [ ] TypeConfig (P2) - Configuration for a custom field type (inline in Field).
* [ ] Option (P2) - Option for dropdown/label custom fields (inline in TypeConfig).
* [ ] Tracking (P2) - Progress tracking configuration for custom fields (inline in TypeConfig).
* [ ] SetCustomFieldValuerequest (P2) - Polymorphic request for setting custom field values.
* [ ] AddDependencyrequest (P2) - Request to add a task dependency.
* [ ] AddTaskLinkresponse (P2) - Response for adding a task link.
* [ ] LinkedTask (P2) - Represents a linked task.
* [ ] GetFoldersresponse (P2) - Wrapper for list of folders.
* [ ] CreateFolderrequest (P2) - Request to create a folder.
* [ ] CreateFolderresponse (P2) - Response for creating a folder.
* [ ] GetFolderresponse (P2) - Response for getting a folder.
* [ ] UpdateFolderrequest (P2) - Request to update a folder.
* [ ] UpdateFolderresponse (P2) - Response for updating a folder.
* [ ] GetGoalsresponse (P2) - Wrapper for goals and goal folders.
* [ ] CreateGoalrequest (P1) - Request to create a goal.
* [ ] CreateGoalresponse (P1) - Response for creating a goal.
* [ ] GetGoalresponse (P1) - Response for getting a goal.
* [ ] UpdateGoalrequest (P1) - Request to update a goal.
* [ ] UpdateGoalresponse (P1) - Response for updating a goal.
* [ ] CreateKeyResultrequest (P1) - Request to create a key result.
* [ ] CreateKeyResultresponse (P1) - Response for creating a key result.
* [ ] LastAction (P2) - Details of the last action on a key result.
* [ ] EditKeyResultrequest (P1) - Request to edit a key result.
* [ ] EditKeyResultresponse (P1) - Response for editing a key result.
* [ ] InviteGuestToWorkspacerequest (P2) - Request to invite a guest.
* [ ] InviteGuestToWorkspaceresponse (P2) - Response for inviting a guest.
* [ ] EditGuestOnWorkspacerequest (P2) - Request to edit a guest.
* [ ] EditGuestOnWorkspaceresponse (P2) - Response for editing a guest.
* [ ] RemoveGuestFromWorkspaceresponse (P3)
* [ ] AddGuestToTaskrequest (P2)
* [ ] AddGuestToTaskresponse (P2)
* [ ] Shared1 (P2) - Structure for items shared with a guest.
* [ ] RemoveGuestFromTaskresponse (P3)
* [ ] AddGuestToListrequest (P2)
* [ ] AddGuestToListresponse (P2)
* [ ] RemoveGuestFromListresponse (P3)
* [ ] AddGuestToFolderrequest (P2)
* [ ] AddGuestToFolderresponse (P2)
* [ ] RemoveGuestFromFolderresponse (P3)
* [ ] GetListsresponse (P2) - Wrapper for lists.
* [ ] CreateListrequest (P1) - Request to create a list.
* [ ] CreateListresponse (P1) - Response for creating a list.
* [ ] GetFolderlessListsresponse (P2) - Wrapper for folderless lists.
* [ ] CreateFolderlessListrequest (P1) - Request to create a folderless list.
* [ ] CreateFolderlessListresponse (P1) - Response for creating a folderless list.
* [ ] UpdateListrequest (P1) - Request to update a list.
* [ ] UpdateListresponse (P1) - Response for updating a list.
* [ ] GetTaskMembersresponse (P2) - Wrapper for task members.
* [ ] ProfileInfo (P2) - User profile info within Member.
* [ ] GetListMembersresponse (P2) - Wrapper for list members.
* [ ] GetCustomRolesresponse (P2) - Wrapper for custom roles.
* [ ] SharedHierarchyresponse (P2) - Wrapper for shared hierarchy.
* [ ] GetSpacesresponse (P1) - Wrapper for spaces.
* [ ] Features (P2) - Space features configuration (various versions like Features1, Features4).
* [ ] CreateSpacerequest (P1) - Request to create a space.
* [ ] CreateSpaceresponse (P1) - Response for creating a space.
* [ ] GetSpaceresponse (P1) - Response for getting a space.
* [ ] UpdateSpacerequest (P1) - Request to update a space.
* [ ] UpdateSpaceresponse (P1) - Response for updating a space.
* [ ] GetSpaceTagsresponse (P2) - Wrapper for space tags.
* [ ] CreateSpaceTagrequest (P2) - Request to create a space tag.
* [ ] EditSpaceTagrequest (P2) - Request to edit a space tag.
* [ ] EditSpaceTagresponse (P2) - Response for editing a space tag.
* [ ] DeleteSpaceTagrequest (P3)
* [ ] GetTasksresponse (P1) - Wrapper for tasks (important for full Task structure).
* [ ] CreateTaskrequest (P1) - Request to create a task.
* [ ] CreateTaskresponse.yaml (P1) - Response for creating a task (likely full Task model).
* [ ] GetTaskresponse (P1) - Response for getting a task (full Task model).
* [ ] UpdateTaskrequest (P1) - Request to update a task.
* [ ] UpdateTaskresponse (P1) - Response for updating a task (full Task model).
* [ ] GetFilteredTeamTasksresponse (P2) - Wrapper for filtered tasks.
* [ ] GetTaskTemplatesresponse (P3) - Wrapper for task templates.
* [ ] CreateTaskFromTemplaterequest (P3) - Request to create task from template.
* [ ] Gettrackedtimeresponse (P2) - Wrapper for legacy tracked time.
* [ ] Tracktimerequest (P2) - Request for legacy track time.
* [ ] Edittimetrackedrequest (P2) - Request to edit legacy tracked time.
* [ ] Gettimeentrieswithinadaterangeresponse (P1) - Wrapper for time entries.
* [ ] Createatimeentryrequest (P1) - Request to create a time entry.
* [ ] Createatimeentryresponse (P1) - Response for creating a time entry.
* [ ] Getsingulartimeentryresponse (P1) - Response for getting a single time entry.
* [ ] TaskLocation (P2) - Task location details for time entries.
* [ ] TaskTag (P2) - Tag details for time entries.
* [ ] UpdateatimeEntryrequest (P1) - Request to update a time entry.
* [ ] Getrunningtimeentryresponse (P2) - Response for currently running time entry.
* [ ] StopatimeEntryresponse (P2) - Response for stopping a time entry.
* [ ] Removetagsfromtimeentriesrequest (P3) - Request to remove tags from time entries.
* [ ] Getalltagsfromtimeentriesresponse (P2) - Wrapper for all time entry tags.
* [ ] Addtagsfromtimeentriesrequest (P2) - Request to add tags to time entries.
* [ ] Changetagnamesfromtimeentriesrequest (P2) - Request to change tag names for time entries.
* [ ] StartatimeEntryrequest (P1) - Request to start a time entry.
* [ ] StartatimeEntryresponse (P1) - Response for starting a time entry.
* [ ] InviteUserToWorkspacerequest (P2) - Request to invite a user to workspace.
* [ ] InviteUserToWorkspaceresponse (P2) - Response for inviting user.
* [ ] GetUserresponse (P2) - Response for getting user details.
* [ ] EditUserOnWorkspacerequest (P2) - Request to edit user on workspace.
* [ ] EditUserOnWorkspaceresponse (P2) - Response for editing user.
* [ ] RemoveUserFromWorkspaceresponse (P3)
* [ ] GetTeamViewsresponse (P2) - Wrapper for Workspace/Team views.
* [ ] CreateTeamViewrequest (P2) - Request to create Workspace/Team view.
* [ ] CreateTeamViewresponse (P2) - Response for creating Workspace/Team view.
* [ ] GetSpaceViewsresponse (P2) - Wrapper for Space views.
* [ ] CreateSpaceViewrequest (P2) - Request to create Space view.
* [ ] CreateSpaceViewresponse (P2) - Response for creating Space view.
* [ ] GetFolderViewsresponse (P2) - Wrapper for Folder views.
* [ ] CreateFolderViewrequest (P2) - Request to create Folder view.
* [ ] CreateFolderViewresponse (P2) - Response for creating Folder view.
* [ ] GetListViewsresponse (P2) - Wrapper for List views.
* [ ] CreateListViewrequest (P2) - Request to create List view.
* [ ] CreateListViewresponse (P2) - Response for creating List view.
* [ ] GetViewresponse (P2) - Response for getting a single view.
* [ ] UpdateViewrequest (P2) - Request to update a view.
* [ ] UpdateViewresponse (P2) - Response for updating a view.
* [ ] GetViewTasksresponse (P2) - Wrapper for tasks within a view.
* [ ] GetWebhooksresponse (P1) - Wrapper for webhooks.
* [ ] CreateWebhookrequest (P1) - Request to create a webhook.
* [ ] CreateWebhookresponse (P1) - Response for creating a webhook.
* [ ] UpdateWebhookrequest (P1) - Request to update a webhook.
* [ ] UpdateWebhookresponse (P1) - Response for updating a webhook.
* [ ] GetWorkspaceseatsresponse (P3) - Response for workspace seat info.
* [ ] GetWorkspaceplanresponse (P3) - Response for workspace plan info.
* [ ] CreateTeamrequest (P2) - Request to create a User Group (note: "Team" is used for User Group here).
* [ ] CreateTeamresponse (P2) - Response for creating a User Group.
* [ ] UpdateTeamrequest (P2) - Request to update a User Group.
* [ ] UpdateTeamresponse (P2) - Response for updating a User Group.
* [ ] GetTeamsresponse (P2) - Wrapper for User Groups (note: "Teams" is used for User Group here).
* [ ] searchDocs (P2) (from /v3/workspaces/{workspaceId}/docs GET)
* [ ] createDoc (P1) (from /v3/workspaces/{workspaceId}/docs POST)
* [ ] getDoc (P1) (from /v3/workspaces/{workspaceId}/docs/{docId} GET)
* [ ] getDocPageListing (P2) (from /v3/workspaces/{workspaceId}/docs/{docId}/pageListing GET)
* [ ] getDocPages (P1) (from /v3/workspaces/{workspaceId}/docs/{docId}/pages GET)
* [ ] createPage (P1) (from /v3/workspaces/{workspaceId}/docs/{docId}/pages POST)
* [ ] getPage (P1) (from /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId} GET)
* [ ] editPage (P1) (from /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId} PUT)
* [ ] CreateWorkspaceAuditLog (P3) (from /v3/workspaces/{workspace_id}/auditlogs POST) - request and response structure not fully detailed.
* [ ] UpdatePrivacyAndAccess (P2) (from /v3/workspaces/{workspace_id}/{object_type}/{object_id}/acls PATCH) - request and response structure not fully detailed.

## Schemas from components.schemas (v3 Chat API specific - these are well-defined):
* [ ] ChatPaginatedResponse (P2)
* [ ] ChatChannel (P2)
* [ ] ChatRoomType (P2) - Enum
* [ ] ChatRoomVisibility (P2) - Enum
* [ ] ChatRoomParentDTO (P2)
* [ ] ChatDefaultViewDTO (P2)
* [ ] ChatSubcategoryType (P2) - Enum
* [ ] ChatLastReadAtData (P2)
* [ ] ChatCommentVersionVector (P3)
* [ ] ChatCommentVector (P3)
* [ ] ChatChannelLinks (P2)
* [ ] ChatPublicApiErrorResponse (P3) - General error response.
* [ ] ChatCreateChatChannel (P2) - Request for creating a chat channel.
* [ ] ChatUpdateChatChannel (P2) - Request for updating a chat channel.
* [ ] ChatChannelLocation (P2)
* [ ] ChatSimpleUser (P2) - Simplified user model for chat contexts.
* [ ] ChatCreateDirectMessageChatChannel (P2) - Request for creating DMs.
* [ ] ChatCreateLocationChatChannel (P2) - Request for creating location-based channels.
* [ ] ChatMessage (P2) - Core chat message model.
* [ ] ChatPostData (P2) - Data for a "post" type chat message.
* [ ] ChatPostSubtype (P2)
* [ ] CommentChatMessageLinks2 (P2) - Links within a chat message.
* [ ] CommentCreateChatMessage (P2) - Request for creating a chat message.
* [ ] ChatReaction (P2) - Chat message reaction model.
* [ ] CommentChatPostDataCreate (P2)
* [ ] CommentChatPostSubtypeCreate (P2)
* [ ] CommentCreateChatMessageResponse (P2)
* [ ] CommentPatchChatMessage (P2) - Request for patching a chat message.
* [ ] CommentChatPostDataPatch (P2)
* [ ] CommentChatPostSubtypePatch (P2)
* [ ] CommentPatchChatMessageResponse (P2)
* [ ] ReplyMessage (P2) - Reply to a chat message.
* [ ] CommentReplyMessageLinks2 (P2)
* [ ] CommentCreateReplyMessageResponse (P2)
* [ ] CommentSimpleUser (P2) - Another simple user representation.

This list will be populated with checkboxes as models are implemented in src/ClickUp.Api.Client.Models/.

Note: Many inline schemas are variations or subsets of larger, more complex models (e.g., User, Task, Checklist). The goal will be to define robust core models in components.schemas (or their C# equivalents) and then ensure request/response objects correctly map to/from these, or use them directly if suitable. Some inline schemas are simple (e.g. just an ID or a name) and may not warrant their own C# file but will be properties of their parent model.
