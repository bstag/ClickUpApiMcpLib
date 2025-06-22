# Detailed Plan: Service Implementations

This document details the plan for implementing the service classes that provide the concrete logic for interacting with the ClickUp API. These services will implement the interfaces defined in `docs/plans/updatedPlans/core/01-CoreModelsAndAbstractions.md`.

**Source Documents:**
*   [`docs/plans/03-service-implementations-conceptual.md`](../03-service-implementations-conceptual.md) (Initial conceptual plan)
*   [`docs/plans/updatedPlans/core/01-CoreModelsAndAbstractions.md`](./core/01-CoreModelsAndAbstractions.md) (Defines the interfaces to be implemented and DTOs to be used)
*   [`docs/OpenApiSpec/ClickUp-6-17-25.json`](../../OpenApiSpec/ClickUp-6-17-25.json) (For endpoint details, request/response schemas)
*   [`docs/plans/NEW_OVERALL_PLAN.md`](../NEW_OVERALL_PLAN.md) (Phase 2, Step 4)

**Location in Codebase:** `src/ClickUp.Api.Client/Services/` (e.g., `TaskService.cs`, `ListService.cs`)

## General Implementation Strategy for Each Service (e.g., `TaskService`)

- [x] **1. Class Definition:**
    - [x] Create a public class named `XxxService` (e.g., `TaskService`) that implements the corresponding `IXxxService` interface.
    - [x] Example: `public class TaskService : ITasksService` (This pattern is followed for all existing services)

- [x] **2. Constructor and Dependencies:**
    - [x] Inject `IApiConnection` (which internally uses `HttpClient`) for making HTTP calls.
    - [ ] Inject `ILogger<XxxService>` for logging. (Not consistently implemented in constructors)
    - [x] Store these dependencies in private readonly fields. (`IApiConnection` is; `ILogger` would be if injected)
    *   (Note: Current implementations use `IApiConnection` which abstracts `HttpClient` and `JsonSerializerOptions`.)

    ```csharp
    // Example: Current pattern in TaskService.cs
    private readonly IApiConnection _apiConnection;
    // private readonly ILogger<TasksService> _logger; // Logger injection not consistently implemented

    public TasksService(IApiConnection apiConnection /*, ILogger<TasksService> logger */)
    {
        _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        // _logger = logger ?? NullLogger<TasksService>.Instance;
    }
    ```

- [ ] **3. Implementing Interface Methods:**
    For each method defined in the service interface (e.g., `GetTaskAsync(string taskId, CancellationToken cancellationToken)` in `ITasksService`):
    (This is an ongoing process for each service. Most services have methods implemented, but completeness varies.)

    - [x] **URL Construction:**
        - [x] Determine the relative URL based on the OpenAPI specification. (Done per method)
        - [x] Use string interpolation for path parameters. (Done per method)
        - [x] For query parameters:
            - [x] Collect all non-null query parameters. (Done per method in services)
            - [x] Query string is built in services before passing to `IApiConnection`. (Done per method in services)

    - [x] **HTTP Method Determination:**
        - [x] Identify the correct HTTP method (GET, POST, PUT, DELETE) from the OpenAPI spec. (Done per method via `IApiConnection` calls like `GetAsync`, `PostAsync`, etc.)

    - [x] **Request Body Handling (for POST, PUT):**
        - [x] If the method requires a request body, the corresponding request DTO is passed.
        - [x] `IApiConnection` handles serialization. (Done per method)

    - [x] **Making the HTTP Call:**
        - [x] Use the appropriate `_apiConnection` method (e.g., `GetAsync<TResponse>`, `PostAsync<TResponse, TRequest>`).
        - [x] Pass the `cancellationToken`. (Done per method)

    - [x] **Response Processing (Centralized via `IApiConnection`):**
        - [x] `IApiConnection` methods handle `HttpResponseMessage` processing.
        - [x] **Success Handling:**
            - [x] If successful, `IApiConnection` deserializes and returns the `TResponse`.
            - [x] For `Task` (no data) methods, `IApiConnection` has non-generic methods (e.g., `PostAsync<TRequest>`).
        - [x] **Error Handling:**
            - [x] `IApiConnection` is responsible for calling a shared error handler (`ApiConnection.HandleErrorResponseAsync`) and throwing appropriate custom exceptions.

    - [ ] **Logging:**
        - [ ] _Optional but recommended:_ Log basic request/response information. (Partially implemented, needs consistency)
        - [ ] Log errors with more details. (`IApiConnection` or its error handler should manage this).

    - [x] **XML Documentation:**
        - [x] Ensure each implemented public method has XML documentation. (Completed 2024-07-12 - All service implementations in `src/ClickUp.Api.Client/Services/` reviewed. Public methods use `inheritdoc` and corresponding interfaces are comprehensively documented.)

## Example Method Implementation (Conceptual Snippet based on current `IApiConnection` usage)

```csharp
// Inside a service, e.g., TasksService.cs
public async Task<GetTaskResponse> GetTaskAsync(string taskId, bool includeSubtasks = false, CancellationToken cancellationToken = default)
{
    _logger.LogInformation("Getting task with ID: {TaskId}", taskId);

    var queryParams = new Dictionary<string, string>();
    if (includeSubtasks)
    {
        queryParams["include_subtasks"] = "true";
    }
    // ... add other query params as needed

    var endpoint = $"task/{taskId}"; // Relative endpoint

    // IApiConnection handles base URL, query string building, serialization, and error handling
    return await _apiConnection.GetAsync<GetTaskResponse>(endpoint, queryParams, cancellationToken);
}
```

## Shared Helper Components (To be detailed in other plans)

- [x] **`IApiConnection`**: Abstracts `HttpClient`, JSON options, error handling. (Implemented)
    - [ ] `HttpErrorHandler.HandleErrorResponseAsync` (or similar logic within `ApiConnection`): Central method for processing non-success HTTP responses and throwing specific `ClickUpApiException`s. (Details in `docs/plans/updatedPlans/exceptions/04-ExceptionHandling.md`) (Logic exists in `ApiConnection.cs` as `ApiConnection.HandleErrorResponseAsync`)
- [x] **JSON Serialization Options**: A shared `JsonSerializerOptions` instance configured. (Managed by `JsonSerializerOptionsHelper.cs` and used by `ApiConnection.cs`) (Details in `docs/plans/updatedPlans/http/03-HttpClientAndHelpers.md`)
- [ ] **Query String Building Utility**: Handled by helper methods within each service, not by `IApiConnection` directly. (Details in `docs/plans/updatedPlans/http/03-HttpClientAndHelpers.md`)

## List of Services to Implement (Based on `02-abstractions-interfaces-actual.md` and OpenAPI tags)
(Status based on existence of the service file in `src/ClickUp.Api.Client/Services/`. Individual method completion varies.)

- [x] `AttachmentsService.cs`
- [x] `AuthorizationService.cs`
- [x] `ChatService.cs`
- [x] `CommentService.cs` (Note: filename is `CommentService.cs`, interface `ICommentService`)
- [x] `CustomFieldsService.cs`
- [x] `DocsService.cs`
- [x] `FoldersService.cs`
- [x] `GoalsService.cs`
- [x] `GuestsService.cs`
- [x] `ListsService.cs`
- [x] `MembersService.cs`
- [x] `RolesService.cs`
- [x] `SharedHierarchyService.cs`
- [x] `SpacesService.cs`
- [x] `TagsService.cs`
- [x] `TaskChecklistsService.cs`
- [x] `TaskRelationshipsService.cs`
- [x] `TaskService.cs` (Note: filename is `TaskService.cs`, interface `ITasksService`)
- [x] `TemplatesService.cs`
- [x] `TimeTrackingService.cs`
- [x] `UserGroupsService.cs` (Implemented 2024-07-12. Handles User Groups, also known as Teams in ClickUp's UI/some API docs.)
- [x] `UsersService.cs`
- [x] `ViewsService.cs`
- [x] `WebhooksService.cs`
- [x] `WorkspacesService.cs`

**Validation:**
- [ ] Ensure each service class correctly implements all methods from its corresponding interface. (Ongoing task for each service)
- [x] Method signatures in the implementation must exactly match the refined interface signatures (DTOs, CancellationToken, etc.). (Largely true for existing methods, verified during implementation)
- [x] Cross-reference with `NEW_OVERALL_PLAN.md` Phase 2, Step 4 to ensure alignment. (This plan aligns with that step)

This detailed plan will guide the implementation of each service, ensuring consistency and adherence to the defined abstractions and error handling strategies.
```
```
