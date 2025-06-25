# Integration Test Status

This document tracks the progress of creating integration tests for various services in the ClickUp SDK.

## Test Modes

Integration tests support three modes, managed by the `CLICKUP_SDK_TEST_MODE` environment variable:

*   **`Passthrough`**: Live API calls, no recording or playback.
*   **`Record`**: Live API calls, responses are saved as JSON files.
*   **`Playback`**: No live API calls, uses previously recorded JSON responses.

All tests should be implemented to support both `Record` and `Playback` modes.

## Services

| Service                   | Interface                 | Status        | Notes                                                                 |
| ------------------------- | ------------------------- | ------------- | --------------------------------------------------------------------- |
| **Core Hierarchy**        |                           |               |                                                                       |
| Workspaces                | `IWorkspacesService`      | To Do         |                                                                       |
| Spaces                    | `ISpacesService`          | To Do         |                                                                       |
| Folders                   | `IFoldersService`         | To Do         |                                                                       |
| Lists                     | `IListsService`           | To Do         |                                                                       |
| **Task Management**       |                           |               |                                                                       |
| Tasks                     | `ITasksService`           | Done          | Comprehensive tests exist, including CRUD, filters, pagination.       |
| Task Checklists           | `ITaskChecklistsService`  | To Do         |                                                                       |
| Task Relationships        | `ITaskRelationshipsService` | To Do         |                                                                       |
| Templates                 | `ITemplatesService`       | To Do         | Task, List, Folder templates.                                         |
| **Collaboration**         |                           |               |                                                                       |
| Comments                  | `ICommentService`         | To Do         | Task comments, List comments, Chat view comments.                     |
| Chat                      | `IChatService`            | To Do         | View comments (already covered by CommentService potentially?) & Views. |
| **User & Access**         |                           |               |                                                                       |
| Authorization             | `IAuthorizationService`   | To Do         | Get User, Get Authorized Teams (Workspaces).                          |
| Users                     | `IUsersService`           | To Do         | Get User (duplicate of Auth?), Invite, Edit, Delete user from Workspace. |
| Guests                    | `IGuestsService`          | To Do         |                                                                       |
| Roles                     | `IRolesService`           | To Do         | Custom roles.                                                         |
| User Groups (Teams)       | `IUserGroupsService`      | To Do         |                                                                       |
| Members                   | `IMembersService`         | To Do         | Task, List, Folder, Space members.                                    |
| **Organization & Meta**   |                           |               |                                                                       |
| Custom Fields             | `ICustomFieldsService`    | To Do         |                                                                       |
| Tags                      | `ITagsService`            | To Do         |                                                                       |
| Goals                     | `IGoalsService`           | To Do         |                                                                       |
| Views                     | `IViewsService`           | To Do         |                                                                       |
| Shared Hierarchy          | `ISharedHierarchyService` | To Do         |                                                                       |
| Webhooks                  | `IWebhooksService`        | To Do         |                                                                       |
| **Other**                 |                           |               |                                                                       |
| Attachments               | `IAttachmentsService`     | To Do         | Attachments to Tasks, Comments.                                       |
| Time Tracking             | `ITimeTrackingService`    | To Do         | Legacy and v2.                                                        |
| Docs                      | `IDocsService`            | To Do         | ClickUp Docs.                                                         |
