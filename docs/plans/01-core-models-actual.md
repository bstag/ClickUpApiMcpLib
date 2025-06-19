
## Phase 1, Step 1: Define Core Models (Actual Implementation)
This document tracks the creation of C# models based on the OpenAPI specification found in /docs/OpenApiSpec/ClickUp-6-17-25.json and the conceptual plan in 01-core-models-conceptual.md.

Models will be implemented in the src/ClickUp.Api.Client.Models/ directory.

Identified Schemas and Prioritization (Based on OpenAPI Spec Review)
This list includes schemas identified from components.schemas (which are generally preferred) and inline schemas from request/response bodies. The priority for implementation is marked (P0 > P1 > P2 > P3).

## Foundational & Core Entities:
* [x] User (P0) - Represents a ClickUp user. (Seen inline, e.g., /v2/user response, and expected in components.schemas)
* [x] Workspace / Team (P0) - Represents a ClickUp Workspace. (Seen as Team inline, e.g., /v2/team response, and expected in components.schemas. Consider naming ClickUpWorkspace in C#)
* [x] Space (P0) - Organizational unit within a Workspace. (Implemented in `Entities/Spaces/Space.cs`, includes Features and simplified MemberSummary, DefaultListSettings)
* [x] Folder (P0) - Organizational unit within a Space. (Seen inline, e.g., in Task model, and expected in components.schemas)
    * `Archived` property (bool) added.
    * `Statuses` property (`List<Status>?`) added (for when `OverrideStatuses` is true).
* [x] List (P0) - Organizational unit containing tasks. (Seen inline, e.g., in Task model, and expected in components.schemas)
    * `Folder` property is nullable (`Folder? Folder { get; init; }`) to support folderless lists.
    * `Priority` property now uses `ListPriorityInfo?` (new model `ListPriorityInfo.cs` created).
* [x] Task (P0) - The core unit of work. (Seen inline, e.g., AddTaskLinkresponse, and expected as a detailed model in components.schemas. Made more comprehensive; includes simplified List/Folder/Space refs and placeholders for UserGroup, TaskLink, SharingOptions defined within Task.cs)
    * `Checklists` property is now `List<Checklist>?` (model `Checklist.cs` implemented in `Entities/Checklists/` - P1).
    * `Tags` property is now `List<Tag>?` (model `Tag.cs` implemented in `Entities/Tags/` - P1).
    * `CustomFields` property is now `List<CustomFieldValue>?` (new model `CustomFieldValue.cs` created - P1).
    * `Dependencies` property is now `List<Dependency>?` (new model `Dependency.cs` created).
    * `MarkdownDescription` property (string?) added.
* [x] Status (P0) - Task status (e.g., "Open", "In Progress"). (Seen inline in Task model, and expected in components.schemas)
* [x] Priority (P0) - Task priority. (Seen inline in Task model, and expected in components.schemas) - This is for Task specific priority. List priority is handled by `ListPriorityInfo`.
* [x] Comment (P1) - Comments on tasks, etc. (Seen inline, e.g., GetTaskCommentsresponse, and expected in components.schemas)
* [x] CustomFieldDefinition / Field (P1) - Definition of a Custom Field. (Seen as Field inline, and expected in components.schemas. Implemented in `Entities/CustomFields/Field.cs`)
* [x] CustomFieldValue (P1) - Value of a Custom Field on a task. (Implemented in `CustomFieldValue.cs`)
* [x] Tag (P1) - Task tags. (Seen inline in Task model, and expected in components.schemas. Implemented in `Entities/Tags/Tag.cs`)
* [x] Checklist (P1) - Checklist on a task. (Seen inline, e.g., CreateChecklistresponse, and expected in components.schemas. Implemented in `Entities/Checklists/Checklist.cs`)
* [x] ChecklistItem (P1) - Item within a checklist. (Seen inline, e.g., Item in Checklist1, and expected in components.schemas. Implemented in `Entities/Checklists/ChecklistItem.cs`)
* [x] Attachment (P1) - File attachments. (Seen as CreateTaskAttachmentresponse and likely in components.schemas. Implemented in `Entities/Attachments/Attachment.cs`)
* [x] Member (P1) - User's membership in various entities. (Implemented in `Member.cs`)
* [x] Goal (P1) - Goal entity. (Seen inline as Goal, Goal2, Goal3, and expected in components.schemas. Implemented in `Entities/Goals/Goal.cs`)
* [x] KeyResult (P1) - Key Result for a Goal. (Seen inline as KeyResult, KeyResult1, and expected in components.schemas. Implemented in `Entities/Goals/KeyResult.cs`)
* [x] Webhook (P1) - Webhook definition. (Seen in GetWebhooksresponse, and expected in components.schemas. Implemented in `Entities/Webhooks/Webhook.cs`)
* [x] Doc (P1) - Document entity (v3). (Implemented in `Doc.cs`)
* [x] Page (P1) - Page within a Doc (v3). (Implemented in `Page.cs`)
* [x] Dependency (P0) - Represents a task dependency. (Implicitly created as part of Task, implemented in `Dependency.cs`)
* [x] ListPriorityInfo (P0) - Represents a list's priority. (Implicitly created as part of List, implemented in `ListPriorityInfo.cs`)
* [x] WebhookHealth (P2) - Health status of a Webhook. (Implemented in `Entities/Webhooks/WebhookHealth.cs`)
* [x] GoalFolder (P2) - Represents a folder for Goals. (Implemented in `Entities/Goals/GoalFolder.cs`)
* [x] UserGroup (P2) - Represents a user group. (Implemented in `Entities/UserGroups/UserGroup.cs`, includes `UserGroupAvatar.cs`)
* [x] SharingOptions (P2) - Represents common sharing options for various entities. (Implemented in `Common/SharingOptions.cs`)
* [x] MemberSummary (P2) - Simplified member info for Space. (Defined within `Entities/Spaces/Space.cs`)
* [x] DefaultListSettings (P2) - Default settings for lists in a Space. (Defined within `Entities/Spaces/Space.cs`)
* [x] TimeEntry (P2) - Time tracking entry. (Implemented in `Entities/TimeTracking/TimeEntry.cs`)
* [x] TaskLocation (P2) - Task location details for time entries. (Implemented in `Entities/TimeTracking/TaskLocation.cs`)
* [x] TaskTag (P2) - Tag details for time entries. (Implemented in `Entities/TimeTracking/TaskTag.cs`; similar to Common.Tag)
* [x] DocPageListingItem (P2) - Represents an item in a doc's page hierarchy. (Implemented in `Entities/Docs/DocPageListingItem.cs`)
* [x] PageDefaults (P2) - Default settings for pages in a Doc. (Implemented in `Entities/Docs/PageDefaults.cs`)
* [ ] ChatChannel (P2) - Chat channel (v3). (Expected from #/components/schemas/ChatChannel)
* [ ] ChatMessage (P2) - Message in a chat channel (v3). (Expected from #/components/schemas/ChatMessage)
* [ ] ChatReaction (P2) - Reaction to a chat message (v3). (Expected from #/components/schemas/ChatReaction)
* [x] Role (P2) - User role. (Seen inline in InviteGuestToWorkspaceresponse within Team1.roles. Implemented in `Entities/Users/Role.cs`)
* [x] CustomRole (P2) - Custom user role. (Seen as CustomRole2 inline, and expected in components.schemas. Implemented in `Entities/Users/CustomRole.cs`)
* [x] Guest (P2) - Guest user details. (Implemented in `Entities/Users/Guest.cs`, includes `GuestUserInfo.cs`, `InvitedByUserInfo.cs`, and `GuestSharingDetails.cs`)
* [ ] View (P2) - View definition for tasks, etc. (Seen in GetTeamViewsresponse, and expected in components.schemas)

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
* [x] TypeConfig (P2) - Configuration for a custom field type (inline in Field. Implemented in `Entities/CustomFields/TypeConfig.cs`).
* [x] Option (P2) - Option for dropdown/label custom fields (inline in TypeConfig. Implemented in `Entities/CustomFields/Option.cs`).
* [x] Tracking (P2) - Progress tracking configuration for custom fields (inline in TypeConfig. Implemented in `Entities/CustomFields/Tracking.cs`).
* [ ] SetCustomFieldValuerequest (P2) - Polymorphic request for setting custom field values.
* [ ] AddDependencyrequest (P2) - Request to add a task dependency.
* [ ] AddTaskLinkresponse (P2) - Response for adding a task link.
* [~] LinkedTask (P2) - Represents a linked task. (Basic placeholder defined within Entities/Tasks/Task.cs; needs full implementation and separate file.)
* [ ] GetFoldersresponse (P2) - Wrapper for list of folders.
* [ ] CreateFolderrequest (P2) - Request to create a folder.
* [ ] CreateFolderresponse (P2) - Response for creating a folder.
* [ ] GetFolderresponse (P2) - Response for getting a folder.
* [ ] UpdateFolderrequest (P2) - Request to update a folder.
* [ ] UpdateFolderresponse (P2) - Response for updating a folder.
* [x] GetGoalsresponse (P2) - Wrapper for goals and goal folders. (Implemented in `ResponseModels/Goals/GetGoalsResponse.cs`)
* [x] CreateGoalrequest (P1) - Request to create a goal. (Implemented in `RequestModels/Goals/CreateGoalRequest.cs`)
* [x] CreateGoalresponse (P1) - Response for creating a goal. (Implemented in `ResponseModels/Goals/CreateGoalResponse.cs`)
* [x] GetGoalresponse (P1) - Response for getting a goal. (Implemented in `ResponseModels/Goals/GetGoalResponse.cs`)
* [x] UpdateGoalrequest (P1) - Request to update a goal. (Implemented in `RequestModels/Goals/UpdateGoalRequest.cs`)
* [x] UpdateGoalresponse (P1) - Response for updating a goal. (Implemented in `ResponseModels/Goals/UpdateGoalResponse.cs`)
* [x] CreateKeyResultrequest (P1) - Request to create a key result. (Implemented in `RequestModels/Goals/CreateKeyResultRequest.cs`)
* [x] CreateKeyResultresponse (P1) - Response for creating a key result. (Implemented in `ResponseModels/Goals/CreateKeyResultResponse.cs`)
* [x] LastAction (P2) - Details of the last action on a key result. (Implemented in `Entities/Goals/LastAction.cs`)
* [x] EditKeyResultrequest (P1) - Request to edit a key result. (Implemented in `RequestModels/Goals/EditKeyResultRequest.cs`)
* [x] EditKeyResultresponse (P1) - Response for editing a key result. (Implemented in `ResponseModels/Goals/EditKeyResultResponse.cs`)
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
* [x] GetSpacesresponse (P1) - Wrapper for spaces. (Implemented in `ResponseModels/Spaces/GetSpacesResponse.cs`)
* [x] Features (P2) - Space features configuration. (Implemented in `Entities/Spaces/Features.cs`)
* [x] CreateSpacerequest (P1) - Request to create a space. (Implemented in `RequestModels/Spaces/CreateSpaceRequest.cs`)
* [x] CreateSpaceresponse (P1) - Response for creating a space. (Implemented in `ResponseModels/Spaces/CreateSpaceResponse.cs`)
* [x] GetSpaceresponse (P1) - Response for getting a space. (Implemented in `ResponseModels/Spaces/GetSpaceResponse.cs`)
* [x] UpdateSpacerequest (P1) - Request to update a space. (Implemented in `RequestModels/Spaces/UpdateSpaceRequest.cs`)
* [x] UpdateSpaceresponse (P1) - Response for updating a space. (Implemented in `ResponseModels/Spaces/UpdateSpaceResponse.cs`)
* [ ] GetSpaceTagsresponse (P2) - Wrapper for space tags.
* [ ] CreateSpaceTagrequest (P2) - Request to create a space tag.
* [ ] EditSpaceTagrequest (P2) - Request to edit a space tag.
* [ ] EditSpaceTagresponse (P2) - Response for editing a space tag.
* [ ] DeleteSpaceTagrequest (P3)
* [x] GetTasksresponse (P1) - Wrapper for tasks. (Implemented in `ResponseModels/Tasks/GetTasksResponse.cs`)
* [x] CreateTaskrequest (P1) - Request to create a task. (Implemented in `RequestModels/Tasks/CreateTaskRequest.cs`; uses CustomTaskFieldToSet from CustomTaskFieldModels.cs)
* [x] CreateTaskresponse (P1) - Response for creating a task. (Implemented in `ResponseModels/Tasks/CreateTaskResponse.cs`)
* [x] GetTaskresponse (P1) - Response for getting a task. (Implemented in `ResponseModels/Tasks/GetTaskResponse.cs`)
* [x] UpdateTaskrequest (P1) - Request to update a task. (Implemented in `RequestModels/Tasks/UpdateTaskRequest.cs`; uses CustomTaskFieldToSet from CustomTaskFieldModels.cs)
* [x] UpdateTaskresponse (P1) - Response for updating a task. (Implemented in `ResponseModels/Tasks/UpdateTaskResponse.cs`)
* [x] CustomTaskFieldModels (P2) - Contains CustomTaskFieldToSet and CustomTaskFieldValueOptions for task requests. (Implemented in `RequestModels/Tasks/CustomTaskFieldModels.cs`)
* [ ] GetFilteredTeamTasksresponse (P2) - Wrapper for filtered tasks.
* [ ] GetTaskTemplatesresponse (P3) - Wrapper for task templates.
* [ ] CreateTaskFromTemplaterequest (P3) - Request to create task from template.
* [ ] Gettrackedtimeresponse (P2) - Wrapper for legacy tracked time.
* [ ] Tracktimerequest (P2) - Request for legacy track time.
* [ ] Edittimetrackedrequest (P2) - Request to edit legacy tracked time.
* [x] Gettimeentrieswithinadaterangeresponse (P1) - Wrapper for time entries. (Implemented in `ResponseModels/TimeTracking/GetTimeEntriesResponse.cs`)
* [x] Createatimeentryrequest (P1) - Request to create a time entry. (Implemented in `RequestModels/TimeTracking/CreateTimeEntryRequest.cs`)
* [x] Createatimeentryresponse (P1) - Response for creating a time entry. (Implemented in `ResponseModels/TimeTracking/CreateTimeEntryResponse.cs`)
* [x] Getsingulartimeentryresponse (P1) - Response for getting a single time entry. (Implemented in `ResponseModels/TimeTracking/GetSingleTimeEntryResponse.cs`)
* [x] UpdateatimeEntryrequest (P1) - Request to update a time entry. (Implemented in `RequestModels/TimeTracking/UpdateTimeEntryRequest.cs`)
* [ ] Getrunningtimeentryresponse (P2) - Response for currently running time entry.
* [ ] StopatimeEntryresponse (P2) - Response for stopping a time entry.
* [ ] Removetagsfromtimeentriesrequest (P3) - Request to remove tags from time entries.
* [ ] Getalltagsfromtimeentriesresponse (P2) - Wrapper for all time entry tags.
* [ ] Addtagsfromtimeentriesrequest (P2) - Request to add tags to time entries.
* [ ] Changetagnamesfromtimeentriesrequest (P2) - Request to change tag names for time entries.
* [x] StartatimeEntryrequest (P1) - Request to start a time entry. (Implemented in `RequestModels/TimeTracking/StartTimeEntryRequest.cs`)
* [x] StartatimeEntryresponse (P1) - Response for starting a time entry. (Implemented in `ResponseModels/TimeTracking/StartTimeEntryResponse.cs`)
* [x] TimeTrackingTagDefinition (P2) - Defines a tag for time tracking requests. (Implemented in `RequestModels/TimeTracking/TimeTrackingTagDefinition.cs`)
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
* [x] GetWebhooksresponse (P1) - Wrapper for webhooks. (Implemented in `ResponseModels/Webhooks/GetWebhooksResponse.cs`)
* [x] CreateWebhookrequest (P1) - Request to create a webhook. (Implemented in `RequestModels/Webhooks/CreateWebhookRequest.cs`)
* [x] CreateWebhookresponse (P1) - Response for creating a webhook. (Implemented in `ResponseModels/Webhooks/CreateWebhookResponse.cs`)
* [x] UpdateWebhookrequest (P1) - Request to update a webhook. (Implemented in `RequestModels/Webhooks/UpdateWebhookRequest.cs`)
* [x] UpdateWebhookresponse (P1) - Response for updating a webhook. (Implemented in `ResponseModels/Webhooks/UpdateWebhookResponse.cs`)
* [ ] GetWorkspaceseatsresponse (P3) - Response for workspace seat info.
* [ ] GetWorkspaceplanresponse (P3) - Response for workspace plan info.
* [ ] CreateTeamrequest (P2) - Request to create a User Group (note: "Team" is used for User Group here).
* [ ] CreateTeamresponse (P2) - Response for creating a User Group.
* [ ] UpdateTeamrequest (P2) - Request to update a User Group.
* [ ] UpdateTeamresponse (P2) - Response for updating a User Group.
* [ ] GetTeamsresponse (P2) - Wrapper for User Groups (note: "Teams" is used for User Group here).
* [x] searchDocs (P2) - Response for searching docs. (Implemented in `ResponseModels/Docs/SearchDocsResponse.cs`)
* [x] createDoc (P1) - Request & Response for creating a doc. (Request in `RequestModels/Docs/CreateDocRequest.cs`, Response in `ResponseModels/Docs/CreateDocResponse.cs`)
* [x] getDoc (P1) - Response for getting a doc. (Implemented in `ResponseModels/Docs/GetDocResponse.cs`)
* [x] getDocPageListing (P2) - Response for listing doc pages. (Implemented in `ResponseModels/Docs/GetDocPageListingResponse.cs`)
* [x] getDocPages (P1) - Response for getting all pages in a doc. (Implemented in `ResponseModels/Docs/GetDocPagesResponse.cs`)
* [x] createPage (P1) - Request & Response for creating a page in a doc. (Request in `RequestModels/Docs/CreatePageRequest.cs`, Response in `ResponseModels/Docs/CreatePageResponse.cs`)
* [x] getPage (P1) - Response for getting a page in a doc. (Implemented in `ResponseModels/Docs/GetPageResponse.cs`)
* [x] editPage (P1) - Request for editing a page in a doc. (Implemented in `RequestModels/Docs/EditPageRequest.cs`)
* [ ] CreateWorkspaceAuditLog (P3) (from /v3/workspaces/{workspace_id}/auditlogs POST) - request and response structure not fully detailed.
* [ ] UpdatePrivacyAndAccess (P2) (from /v3/workspaces/{workspace_id}/{object_type}/{object_id}/acls PATCH) - request and response structure not fully detailed.

## Schemas from components.schemas (v3 Chat API specific - these are well-defined):
* [x] ChatPaginatedResponse (P2) - Generic paginated response structure for Chat. (Implemented for ChatChannel in `ResponseModels/Chat/ChatChannelPaginatedResponse.cs`)
* [x] ChatChannel (P2) - Chat channel (v3). (Implemented in `Entities/Chat/ChatChannel.cs`)
* [x] ChatRoomType (P2) - Enum for chat room types. (Implemented in `Entities/Chat/Enums/ChatRoomType.cs`)
* [x] ChatRoomVisibility (P2) - Enum for chat room visibility. (Implemented in `Entities/Chat/Enums/ChatRoomVisibility.cs`)
* [x] ChatRoomParentDTO (P2) - DTO for parent of a chat room. (Implemented in `Entities/Chat/ChatRoomParentDTO.cs`)
* [x] ChatDefaultViewDTO (P2) - DTO for default view of a chat room. (Implemented in `Entities/Chat/ChatDefaultViewDTO.cs`)
* [x] ChatSubcategoryType (P2) - Enum for chat subcategory types. (Implemented in `Entities/Chat/Enums/ChatSubcategoryType.cs`)
* [x] ChatLastReadAtData (P2) - Data for last read timestamp and unread counts. (Implemented in `Entities/Chat/ChatLastReadAtData.cs`)
* [ ] ChatCommentVersionVector (P3)
* [ ] ChatCommentVector (P3)
* [x] ChatChannelLinks (P2) - Links related to a chat channel. (Implemented in `Entities/Chat/ChatChannelLinks.cs`)
* [ ] ChatPublicApiErrorResponse (P3) - General error response.
* [x] ChatCreateChatChannel (P2) - Request for creating a chat channel. (Implemented in `RequestModels/Chat/ChatCreateChatChannelRequest.cs`)
* [x] ChatUpdateChatChannel (P2) - Request for updating a chat channel. (Implemented in `RequestModels/Chat/ChatUpdateChatChannelRequest.cs`)
* [x] ChatChannelLocation (P2) - Location of a chat channel (folder, list, space). (Implemented in `Entities/Chat/ChatChannelLocation.cs`)
* [x] ChatSimpleUser (P2) - Simplified user model for chat contexts. (Implemented in `Entities/Chat/ChatSimpleUser.cs`)
* [x] ChatCreateDirectMessageChatChannel (P2) - Request for creating DMs. (Implemented in `RequestModels/Chat/ChatCreateDirectMessageChatChannelRequest.cs`)
* [x] ChatCreateLocationChatChannel (P2) - Request for creating location-based channels. (Implemented in `RequestModels/Chat/ChatCreateLocationChatChannelRequest.cs`)
* [x] ChatMessage (P2) - Core chat message model. (Implemented in `Entities/Chat/ChatMessage.cs`)
* [x] ChatPostData (P2) - Data for a "post" type chat message. (Implemented in `Entities/Chat/ChatPostData.cs`)
* [x] ChatPostSubtype (P2) - Subtype of a chat post. (Implemented in `Entities/Chat/ChatPostSubtype.cs`)
* [x] CommentChatMessageLinks2 (P2) - Links within a chat message. (Implemented in `Entities/Chat/CommentChatMessageLinks2.cs`)
* [x] CommentCreateChatMessage (P2) - Request for creating a chat message. (Implemented in `RequestModels/Chat/CommentCreateChatMessageRequest.cs`)
* [x] ChatReaction (P2) - Chat message reaction model. (Implemented in `Entities/Chat/ChatReaction.cs`)
* [x] CommentChatPostDataCreate (P2) - Data for creating chat post. (Implemented in `RequestModels/Chat/CommentChatPostDataCreate.cs`)
* [x] CommentChatPostSubtypeCreate (P2) - Subtype for creating chat post. (Implemented in `RequestModels/Chat/CommentChatPostSubtypeCreate.cs`)
* [x] CommentCreateChatMessageResponse (P2) - Response for creating chat message. (Implemented in `ResponseModels/Chat/CommentCreateChatMessageResponse.cs`)
* [x] CommentPatchChatMessage (P2) - Request for patching a chat message. (Implemented in `RequestModels/Chat/CommentPatchChatMessageRequest.cs`)
* [x] CommentChatPostDataPatch (P2) - Data for patching chat post. (Implemented in `RequestModels/Chat/CommentChatPostDataPatch.cs`)
* [x] CommentChatPostSubtypePatch (P2) - Subtype for patching chat post. (Implemented in `RequestModels/Chat/CommentChatPostSubtypePatch.cs`)
* [x] CommentPatchChatMessageResponse (P2) - Response for patching chat message. (Implemented in `ResponseModels/Chat/CommentPatchChatMessageResponse.cs`)
* [x] ReplyMessage (P2) - Reply to a chat message. (Implemented in `Entities/Chat/ReplyMessage.cs`)
* [x] CommentReplyMessageLinks2 (P2) - Links for a reply message. (Implemented in `Entities/Chat/CommentReplyMessageLinks2.cs`)
* [x] CommentCreateReplyMessageResponse (P2) - Response for creating a reply. (Implemented in `ResponseModels/Chat/CommentCreateReplyMessageResponse.cs`)
* [x] CommentSimpleUser (P2) - Another simple user representation. (Implemented in `Entities/Chat/CommentSimpleUser.cs`; similar to ChatSimpleUser)

This list will be populated with checkboxes as models are implemented in src/ClickUp.Api.Client.Models/.

Note: Many inline schemas are variations or subsets of larger, more complex models (e.g., User, Task, Checklist). The goal will be to define robust core models in components.schemas (or their C# equivalents) and then ensure request/response objects correctly map to/from these, or use them directly if suitable. Some inline schemas are simple (e.g. just an ID or a name) and may not warrant their own C# file but will be properties of their parent model.
