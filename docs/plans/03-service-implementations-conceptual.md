# Phase 1, Step 3: Plan Service Implementations (Conceptual)

This document outlines the conceptual approach for implementing the C# service classes that will realize the interfaces defined in `docs/plans/06-abstractions-interfaces-conceptual.md`. These classes will reside in the `Client` project and contain the logic for making HTTP requests to the ClickUp API, handling responses, and deserializing them into the defined models.

## General Structure

- Each service interface (e.g., `ITaskService`) will have a corresponding concrete implementation class (e.g., `TaskService`).
- These classes will be responsible for:
    - Constructing HTTP requests (method, URL, headers, body).
    - Sending requests using `HttpClient`.
    - Receiving HTTP responses.
    - Deserializing successful JSON responses into C# models.
    - Handling API errors and translating them into a consistent exception model.

## Constructor Dependencies

Service implementation classes will typically have the following constructor dependencies:

- **`HttpClient`:** An instance of `HttpClient` (or a typed client derived from it) will be injected. This client will be pre-configured with the base API address and potentially default headers (like User-Agent). Authentication headers (e.g., API tokens) will likely be managed by `HttpClient` message handlers or configured per-request if necessary.
- **Configuration Objects (Optional):** If there are service-specific configurations (e.g., specific retry policies beyond what `HttpClient` offers, or flags that alter behavior), these might be injected as options objects (e.g., `IOptions<TaskServiceOptions>`). For most cases, global client configuration should suffice.
- **(Potentially) `ILogger<T>`:** For logging purposes, an `ILogger<T>` can be injected to log request details, errors, or other relevant information during development and production.

Example Constructor:

```csharp
// In Client project (e.g., ClickUp.Net.Client.dll)
public class TaskService : ITaskService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TaskService> _logger; // Optional

    public TaskService(HttpClient httpClient, ILogger<TaskService> logger = null) // logger can be null if using NullLogger by default
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger; // Or Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance if null
    }

    // ... Interface method implementations
}
```

## Request Sending and Deserialization

- **Private Helper Methods:** Generic private helper methods will be created within a base service class or a shared utility class to handle common tasks like:
    - `SendAsync<TRequest, TResponse>(HttpMethod method, string relativeUrl, TRequest requestBody = null, CancellationToken cancellationToken = default)`: Handles request construction, sending, and basic response validation.
    - `DeserializeResponseAsync<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken = default)`: Handles deserialization of the `HttpContent` to `TResponse` using `System.Text.Json.JsonSerializer`.
- **URL Construction:**
    - Base URL is configured on `HttpClient`.
    - Relative URLs for specific endpoints will be constructed within each service method. String interpolation or constants will be used for path segments.
    - Query parameters will be appended dynamically based on method arguments. Helper functions might be used to build query strings correctly (e.g., handling encoding, null/empty values).
- **Request Body Serialization:** Request body objects (C# models) will be serialized to JSON using `System.Text.Json.JsonSerializer` before being sent. `StringContent` with `application/json` media type will typically be used.
- **Response Deserialization:** Successful responses will be deserialized from JSON into the appropriate C# model types using `System.Text.Json.JsonSerializer`.

## Error Handling

- **HTTP Status Code Checking:** After receiving an `HttpResponseMessage`, its `IsSuccessStatusCode` property will be checked.
- **Custom Exceptions:** If the status code indicates an error (4xx or 5xx range):
    - A custom exception hierarchy will be defined (e.g., `ClickUpApiException`, `ClickUpApiRateLimitException`, `ClickUpApiNotFoundException` etc., see "Global Exception System" plan).
    - The response body (which often contains error details from the API) will be read and parsed, if possible, to populate the custom exception with more information (e.g., error codes, messages from the API).
    - The appropriate custom exception will then be thrown.
- **Transient Errors & Retries:** `HttpClient` can be configured (e.g., using Polly policies via `IHttpClientFactory`) to handle transient network errors and retries. Service implementations themselves will generally not implement complex retry logic directly, relying on the `HttpClient` setup.
- **`try-catch` blocks:** Will be used around `_httpClient.SendAsync` calls to catch `HttpRequestException` or other network-related exceptions, potentially wrapping them in a custom API exception.

Conceptual Error Handling Snippet within a service method:

```csharp
public async Task<TaskModel> GetTaskAsync(string taskId, CancellationToken cancellationToken = default)
{
    var relativeUrl = $"task/{taskId}"; // Example
    try
    {
        var response = await _httpClient.GetAsync(relativeUrl, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            // Assumes a helper or direct use of JsonSerializer
            return await response.Content.ReadFromJsonAsync<TaskModel>(/* JsonSerializerOptions */, cancellationToken);
        }
        else
        {
            // Error handling logic:
            // 1. Read error content from response.
            // 2. Create appropriate ClickUpApiException (or derived type).
            // 3. Throw the exception.
            // This logic will be centralized, e.g., in a helper method.
            await HandleApiErrorResponseAsync(response); // This helper would throw
            return null; // Should not be reached if HandleApiErrorResponseAsync throws
        }
    }
    catch (HttpRequestException ex)
    {
        // Log and re-throw as a custom API exception
        _logger?.LogError(ex, "HTTP request failed for GetTaskAsync (taskId: {TaskId})", taskId);
        throw new ClickUpApiException($"Network error while getting task {taskId}: {ex.Message}", ex);
    }
    // Other specific exceptions like TaskCanceledException can be caught if needed.
}
```
*(The `HandleApiErrorResponseAsync` method would be a shared private method.)*

## Managing API Endpoint Paths

- **Constants or `const string`:** For fixed parts of API paths.
- **String Interpolation:** For dynamic parts (e.g., IDs in the path: `$"task/{taskId}"`).
- **Query String Builders:** Utility functions might be used to construct query strings from method parameters, especially when there are multiple optional parameters. This ensures correct encoding and formatting. `System.Web.HttpUtility.ParseQueryString()` (if available/appropriate) or custom logic can be used.

## Use of `System.Text.Json`

- `JsonSerializerOptions` will be configured centrally (e.g., during `HttpClient` setup or as a static holder) to ensure consistency:
    - `PropertyNamingPolicy = JsonNamingPolicy.CamelCase` (or as per API specifics, often snake_case, requiring `JsonPropertyName` attributes on models).
    - `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull` (if appropriate).
    - Converters (e.g., `JsonStringEnumConverter`).
- Extension methods like `HttpContent.ReadFromJsonAsync<T>()` and `HttpClientJsonExtensions.PostAsJsonAsync<T>()` (from `System.Net.Http.Json`) will be used for convenience.

## Next Steps

- Define the global exception system (Phase 2, Step 8).
- Once the OpenAPI specification (`ClickUp-6-17-25.json`) is available and models/interfaces are more concretely defined, start implementing these service classes.
- Develop the shared helper methods for request sending, deserialization, and error handling.
- Implement comprehensive unit tests for these service classes, likely using `Moq` for interfaces and `HttpClientTestingModule` (or similar) for `HttpClient` interactions.
```
