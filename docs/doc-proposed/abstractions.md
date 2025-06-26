# Abstractions

The `ClickUp.Api.Client.Abstractions` project defines the core contracts and interfaces for the ClickUp API client library. It establishes a clear separation between the public-facing API of the SDK and its internal implementation details. This design allows for greater flexibility, testability, and maintainability.

## Key Abstractions

### `IApiConnection`

The `IApiConnection` interface is the central point for all HTTP communication with the ClickUp API. It abstracts away the complexities of making HTTP requests, handling authentication, and deserializing responses.

**Responsibilities:**

- Sending GET, POST, PUT, and DELETE requests.
- Handling JSON serialization and deserialization.
- Managing authentication headers (Personal Access Token or OAuth).

### Service Interfaces

The SDK is organized into a set of service interfaces, each corresponding to a specific area of the ClickUp API. This modular approach makes the SDK easier to understand and use.

**Available Services:**

- `IAttachmentsService`: Manage task attachments.
- `IAuthorizationService`: Handle OAuth and retrieve authorized user information.
- `IChatService`: Interact with the ClickUp Chat API.
- `ICommentsService`: Manage comments on tasks, lists, and views.
- `ICustomFieldsService`: Work with custom fields.
- `IDocsService`: Manage ClickUp Docs and pages.
- `IFoldersService`: Manage folders.
- `IGoalsService`: Manage goals and targets.
- `IGuestsService`: Manage guest users.
- `IListsService`: Manage lists.
- `IMembersService`: Retrieve members of tasks and lists.
- `IRolesService`: Get information about custom roles.
- `ISharedHierarchyService`: Access shared items.
- `ISpacesService`: Manage spaces.
- `ITagsService`: Manage tags.
- `ITaskChecklistsService`: Manage task checklists.
- `ITaskRelationshipsService`: Manage task dependencies and links.
- `ITasksService`: A comprehensive service for managing tasks.
- `ITemplatesService`: Retrieve task templates.
- `ITimeTrackingService`: Manage time entries.
- `IUserGroupsService`: Manage user groups.
- `IUsersService`: Manage users within a workspace.
- `IViewsService`: Manage views.
- `IWebhooksService`: Manage webhooks.
- `IWorkspacesService`: Get workspace-level information like plan and seat usage.

### Options

- `ClickUpClientOptions`: Provides a way to configure the `ClickUpApiClient`, including authentication tokens and the base API address.
- `ClickUpPollyOptions`: Configures the Polly resilience policies for handling transient HTTP errors.

## Usage

The abstractions are designed to be used with a dependency injection container. By registering the services and `IApiConnection`, you can inject the required service interfaces into your application components, promoting loose coupling and making your code easier to test.