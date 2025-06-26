# Models

The `ClickUp.Api.Client.Models` project contains the data structures used throughout the SDK. These models are categorized into Entities, Request Models, Response Models, and Exceptions.

## Entities

Entities represent the core objects in the ClickUp API, such as Tasks, Lists, Folders, and Spaces. These are the primary data structures you'll work with when interacting with the SDK.

**Key Entities:**

- `CuTask`: Represents a task in ClickUp, with all its properties like name, description, status, assignees, etc.
- `ClickUpList`: Represents a list, which is a container for tasks.
- `Folder`: Represents a folder, which can contain lists.
- `Space`: Represents a space, a high-level organizational unit.
- `ClickUpWorkspace`: Represents a workspace (team).

## Request Models

Request models are used to send data to the ClickUp API when creating or updating resources. They are designed to be intuitive and provide a clear structure for the data that needs to be sent.

**Example: `CreateTaskRequest`**

When creating a new task, you'll use the `CreateTaskRequest` model to specify the task's properties:

```csharp
var request = new CreateTaskRequest(
    Name: "My new task",
    Description: "This is a detailed description.",
    Assignees: new List<int> { userId },
    Status: "To Do",
    Priority: 1
);
```

## Response Models

Response models are used to deserialize the data returned by the ClickUp API. They are structured to match the JSON responses from the API, making it easy to access the data you need. Many of the response models wrap the core entity models.

**Example: `GetTasksResponse`**

When you request a list of tasks, the API returns a `GetTasksResponse` object, which contains a list of `CuTask` objects:

```csharp
GetTasksResponse response = await tasksService.GetTasksAsync(listId, new GetTasksRequest());
List<CuTask> tasks = response.Tasks;
```

## Exceptions

The SDK uses a set of custom exception classes to handle API errors gracefully. All exceptions derive from `ClickUpApiException`.

- `ClickUpApiException`: The base exception for all API-related errors.
- `ClickUpApiAuthenticationException`: Thrown for authentication errors (e.g., invalid API token).
- `ClickUpApiNotFoundException`: Thrown when a requested resource is not found (HTTP 404).
- `ClickUpApiRateLimitException`: Thrown when the API rate limit is exceeded.
- `ClickUpApiValidationException`: Thrown for validation errors in the request payload.
- `ClickUpApiServerException`: Thrown for server-side errors on the ClickUp API.