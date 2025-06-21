# Detailed Plan: Service Implementations

This document details the plan for implementing the service classes that provide the concrete logic for interacting with the ClickUp API. These services will implement the interfaces defined in `docs/plans/updatedPlans/core/01-CoreModelsAndAbstractions.md`.

**Source Documents:**
*   `docs/plans/03-service-implementations-conceptual.md` (Initial conceptual plan)
*   `docs/plans/updatedPlans/core/01-CoreModelsAndAbstractions.md` (Defines the interfaces to be implemented and DTOs to be used)
*   `docs/OpenApiSpec/ClickUp-6-17-25.json` (For endpoint details, request/response schemas)
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 2, Step 4)

**Location in Codebase:** `src/ClickUp.Api.Client/Services/` (e.g., `TaskService.cs`, `ListService.cs`)

## General Implementation Strategy for Each Service (e.g., `TaskService`)

1.  **Class Definition:**
    *   Create a public class named `XxxService` (e.g., `TaskService`) that implements the corresponding `IXxxService` interface.
    *   Example: `public class TaskService : ITasksService`

2.  **Constructor and Dependencies:**
    *   Inject `HttpClient` (or a typed client like `ClickUpHttpClient` if introduced later) for making HTTP calls. This `HttpClient` will be pre-configured with the base API URL and default headers (including authentication via a `DelegatingHandler`).
    *   Inject `ILogger<XxxService>` for logging. Use `Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance` as a default if no logger is provided in tests or simple DI setups.
    *   Store these dependencies in private readonly fields.

    ```csharp
    // Example: TaskService.cs
    private readonly HttpClient _httpClient;
    private readonly ILogger<TaskService> _logger;

    public TaskService(HttpClient httpClient, ILogger<TaskService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? NullLogger<TaskService>.Instance;
    }
    ```

3.  **Implementing Interface Methods:**
    For each method defined in the service interface (e.g., `GetTaskAsync(string taskId, CancellationToken cancellationToken)` in `ITasksService`):

    *   **URL Construction:**
        *   Determine the relative URL based on the OpenAPI specification (`docs/OpenApiSpec/ClickUp-6-17-25.json`).
        *   Use string interpolation for path parameters (e.g., `$"task/{taskId}"`).
        *   For query parameters:
            *   Collect all non-null query parameters from the method arguments.
            *   Use a utility (to be created, see "HTTP Client and Helpers Plan") or `HttpUtility.ParseQueryString` (if appropriate and being careful with encoding) to build the query string. Ensure proper URL encoding of parameter names and values.
            *   Example: `var queryString = QueryHelpers.AddQueryString("", queryParamsDictionary);` (using `Microsoft.AspNetCore.WebUtilities.QueryHelpers`).

    *   **HTTP Method Determination:**
        *   Identify the correct HTTP method (GET, POST, PUT, DELETE) from the OpenAPI spec for the endpoint.

    *   **Request Body Handling (for POST, PUT):**
        *   If the method requires a request body, the corresponding request DTO (e.g., `CreateTaskRequestDto`) will be passed as a parameter.
        *   Serialize this DTO to JSON using `System.Net.Http.Json.JsonContent.Create(requestDto, options: _jsonSerializerOptions)` or `new StringContent(JsonSerializer.Serialize(requestDto, _jsonSerializerOptions), Encoding.UTF8, "application/json")`.
        *   The shared `_jsonSerializerOptions` will be defined in the HTTP Client setup (see "HTTP Client and Helpers Plan") and potentially made accessible to services if not using `HttpClient` extensions that handle it implicitly.

    *   **Making the HTTP Call:**
        *   Use the appropriate `_httpClient` method:
            *   `_httpClient.GetAsync(urlWithQuery, cancellationToken)`
            *   `_httpClient.PostAsync(urlWithQuery, jsonContent, cancellationToken)`
            *   `_httpClient.PutAsync(urlWithQuery, jsonContent, cancellationToken)`
            *   `_httpClient.DeleteAsync(urlWithQuery, cancellationToken)`
        *   Pass the `cancellationToken` to all async HTTP calls.

    *   **Response Processing (Centralized Helper):**
        *   Await the `HttpResponseMessage`.
        *   This part will heavily rely on shared helper methods (planned in "HTTP Client and Helpers" and "Exception Handling" plans).
        *   **Success Handling:**
            *   If `response.IsSuccessStatusCode` is true:
                *   If the method returns data (e.g., `Task<TaskDto>`):
                    *   Read the content: `await response.Content.ReadFromJsonAsync<ResponseDtoType>(_jsonSerializerOptions, cancellationToken);`
                    *   Return the deserialized DTO.
                *   If the method returns `Task` (no data, e.g., HTTP 204):
                    *   Simply `return;` or `return Task.CompletedTask;` (if the interface method is `async Task`).
        *   **Error Handling:**
            *   If `!response.IsSuccessStatusCode`:
                *   Call a shared error handling method (e.g., `async Task HttpErrorHandler.HandleErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)`). This method will:
                    *   Attempt to read and deserialize an error DTO from the response body (e.g., `ClickUpApiErrorResponse`).
                    *   Instantiate and throw the appropriate custom exception from the `ClickUp.Api.Client.Models/Exceptions/` hierarchy (e.g., `ClickUpApiNotFoundException`, `ClickUpApiValidationException`) based on the status code and error DTO content.
                    *   This helper ensures consistent error handling across all service methods.

    *   **Logging:**
        *   _Optional but recommended:_ Log basic request/response information at an appropriate log level (e.g., Debug or Trace).
        *   Log errors with more details at Error level before throwing exceptions (or let the centralized error handler do this).

    *   **XML Documentation:**
        *   Ensure each implemented public method has XML documentation that matches or enhances the interface documentation, detailing any specific implementation notes if necessary.

## Example Method Implementation (Conceptual Snippet)

```csharp
// Inside a service, e.g., TaskService.cs
public async Task<TaskDto> GetTaskAsync(string taskId, bool includeSubtasks = false, CancellationToken cancellationToken = default)
{
    _logger.LogInformation("Getting task with ID: {TaskId}", taskId); // Example log

    // 1. URL Construction
    var queryParams = new Dictionary<string, string>();
    if (includeSubtasks)
    {
        queryParams["include_subtasks"] = "true";
    }
    // ... add other query params as needed

    // Assuming _apiBaseUrl is configured on HttpClient or available
    // Assuming QueryHelpers.AddQueryString or a similar utility
    var relativeUrl = $"task/{taskId}";
    var urlWithQuery = QueryHelpers.AddQueryString(relativeUrl, queryParams);

    try
    {
        // 2. Making HTTP Call
        var response = await _httpClient.GetAsync(urlWithQuery, cancellationToken);

        // 3. Response Processing (delegated for error handling)
        if (response.IsSuccessStatusCode)
        {
            // Assuming _jsonSerializerOptions are configured for HttpClient or available
            var taskDto = await response.Content.ReadFromJsonAsync<TaskDto>(cancellationToken: cancellationToken);
            if (taskDto == null)
            {
                // Handle case where deserialization results in null for a success response, if necessary
                _logger.LogWarning("GetTaskAsync for ID {TaskId} returned success but null DTO.", taskId);
                // Depending on API contract, might throw or return null/default
                throw new ClickUpApiException($"API returned success for task {taskId} but the response body was unexpectedly null or empty.");
            }
            return taskDto;
        }
        else
        {
            // Centralized error handling will throw an appropriate ClickUpApiException
            await HttpErrorHandler.HandleErrorResponseAsync(response, cancellationToken);
            return null; // Should be unreachable if HandleErrorResponseAsync always throws
        }
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "HTTP request failed while getting task {TaskId}.", taskId);
        throw new ClickUpApiException($"Network error while getting task {taskId}: {ex.Message}", ex);
    }
    catch (JsonException ex)
    {
        _logger.LogError(ex, "Failed to deserialize response for task {TaskId}.", taskId);
        throw new ClickUpApiException($"Error deserializing response for task {taskId}: {ex.Message}", ex);
    }
    // Catch TaskCanceledException if specific handling is needed, otherwise it will propagate
}
```

## Shared Helper Components (To be detailed in other plans)

*   **`HttpErrorHandler.HandleErrorResponseAsync`**: Central method for processing non-success HTTP responses and throwing specific `ClickUpApiException`s. (Details in `docs/plans/updatedPlans/exceptions/04-ExceptionHandling.md`)
*   **JSON Serialization Options**: A shared `JsonSerializerOptions` instance configured for the ClickUp API specifics (snake_case, enum converters, etc.). (Details in `docs/plans/updatedPlans/http/03-HttpClientAndHelpers.md`)
*   **Query String Building Utility**: Helper to build query strings reliably. (Details in `docs/plans/updatedPlans/http/03-HttpClientAndHelpers.md`)

## List of Services to Implement (Based on `02-abstractions-interfaces-actual.md` and OpenAPI tags)

*   `AttachmentsService`
*   `AuthorizationService`
*   `ChatService`
*   `CommentsService`
*   `CustomFieldsService`
*   `DocsService`
*   `FoldersService`
*   `GoalsService`
*   `GuestsService`
*   `ListsService`
*   `MembersService`
*   `RolesService`
*   `SharedHierarchyService`
*   `SpacesService`
*   `TagsService`
*   `TaskChecklistsService`
*   `TaskRelationshipsService`
*   `TasksService`
*   `TemplatesService`
*   `TimeTrackingService` (and potentially legacy version if distinct)
*   `UserGroupsService` (corresponds to "Teams" in some API contexts, ensure naming consistency)
*   `UsersService`
*   `ViewsService`
*   `WebhooksService`
*   `WorkspacesService`

**Validation:**
*   Ensure each service class correctly implements all methods from its corresponding interface.
*   Method signatures in the implementation must exactly match the refined interface signatures (DTOs, CancellationToken, etc.).
*   Cross-reference with `NEW_OVERALL_PLAN.md` Phase 2, Step 4 to ensure alignment.

This detailed plan will guide the implementation of each service, ensuring consistency and adherence to the defined abstractions and error handling strategies.
```
