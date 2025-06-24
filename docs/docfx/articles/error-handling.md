# Error Handling

The `ClickUp.Api.Client` SDK provides a custom exception hierarchy to help you manage errors returned by the ClickUp API and other issues that may occur during API interactions.

## Base Exception: `ClickUpApiException`

All exceptions specific to ClickUp API interactions inherit from `ClickUp.Api.Client.Models.Exceptions.ClickUpApiException`. This base exception includes:

-   `Message` (string): A general error message.
-   `StatusCode` (System.Net.HttpStatusCode?): The HTTP status code returned by the API, if applicable.
-   `ApiErrorCode` (string?): The ClickUp-specific error code (e.g., "TEAM_001"), if provided in the API response.
-   `ApiErrorText` (string?): The ClickUp-specific error text (e.g., "Team not found"), if provided.
-   `CorrelationId` (string?): The `X-Correlation-ID` header value from the API response, useful for debugging and support.

Catching `ClickUpApiException` allows you to handle any error originating from the SDK's interaction with the API.

```csharp
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Exceptions; // Namespace for ClickUpApiException

// ... inject ITasksService ...

try
{
    var task = await _tasksService.GetTaskAsync("some_task_id");
}
catch (ClickUpApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"ClickUp Error Code: {ex.ApiErrorCode}");
    Console.WriteLine($"ClickUp Error Text: {ex.ApiErrorText}");
    Console.WriteLine($"Correlation ID: {ex.CorrelationId}");
    // Handle the error (e.g., log, display to user)
}
catch (HttpRequestException httpEx)
{
    // Handle network-level errors not directly processed by ApiConnection
    Console.WriteLine($"Network Error: {httpEx.Message}");
}
catch (Exception ex)
{
    // Handle other unexpected errors
    Console.WriteLine($"Unexpected Error: {ex.Message}");
}
```

## Specific Exception Types

The SDK defines several more specific exception types that inherit from `ClickUpApiException` for common error scenarios:

-   **`ClickUpApiAuthenticationException`**: Thrown for authentication failures (typically HTTP 401 or 403).
    -   Indicates an issue with your Personal Access Token or OAuth token.

-   **`ClickUpApiNotFoundException`**: Thrown when a requested resource is not found (typically HTTP 404).
    -   Example: Requesting a task with an ID that doesn't exist.

-   **`ClickUpApiRateLimitException`**: Thrown when API rate limits are exceeded (typically HTTP 429).
    -   The SDK's Polly policies attempt to handle rate limiting with retries (respecting `Retry-After` headers). This exception might be thrown if retries are exhausted or if a circuit breaker is open.
    -   Contains an additional `RetryAfter` (TimeSpan?) property indicating how long to wait before retrying, if provided by the API.

-   **`ClickUpApiValidationException`**: Thrown for request validation errors (typically HTTP 400 or 422).
    -   Indicates that the data sent in the request was invalid or missing.
    -   Contains an `Errors` (IReadOnlyDictionary<string, string[]>?) property that *may* be populated with field-specific validation messages if the API provides them in a structured format. *(Support for parsing detailed errors is an ongoing enhancement).*

-   **`ClickUpApiServerException`**: Thrown for server-side errors on ClickUp's end (typically HTTP 5xx).
    -   These errors usually indicate a temporary problem with the ClickUp API. Retrying later might resolve the issue.

-   **`ClickUpApiRequestException`**: A more general exception for other client-side errors (HTTP 4xx) not covered by the more specific types above.

## Best Practices for Error Handling

1.  **Catch Specific Exceptions First:** Start by catching the most specific exceptions you want to handle differently (e.g., `ClickUpApiAuthenticationException`, `ClickUpApiValidationException`).
2.  **Fall Back to `ClickUpApiException`:** Catch the base `ClickUpApiException` to handle any other API-related errors.
3.  **Handle Network Errors:** Be prepared to catch `HttpRequestException` for network-level issues (e.g., DNS resolution failure, connection refused) that might occur before an HTTP response is received from the API. The SDK's Polly policies also help mitigate some of these.
4.  **Log Detailed Information:** When logging errors, include all available information from the exception (message, status code, API error code, correlation ID, stack trace) to aid in debugging.
5.  **User-Friendly Messages:** For user-facing applications, translate exceptions into user-friendly messages rather than exposing raw error details directly.
6.  **Retry Strategies:** While the SDK has built-in retry mechanisms for transient errors and rate limiting, consider your own application-level retry logic for certain operations if appropriate, especially for `ClickUpApiServerException`.

**Example of layered catching:**

```csharp
try
{
    // ... API call ...
}
catch (ClickUpApiAuthenticationException authEx)
{
    _logger.LogError(authEx, "Authentication failed. Check API token and permissions.");
    // Potentially trigger re-authentication or notify the user.
}
catch (ClickUpApiValidationException valEx)
{
    _logger.LogWarning(valEx, "Validation error processing request.");
    // Display validation messages to the user if available in valEx.Errors
    if (valEx.Errors != null)
    {
        foreach (var error in valEx.Errors)
        {
            _logger.LogWarning("Field: {Field}, Issues: {Issues}", error.Key, string.Join(", ", error.Value));
        }
    }
}
catch (ClickUpApiNotFoundException nfEx)
{
    _logger.LogInformation(nfEx, "Requested resource not found.");
    // Handle as appropriate for your application (e.g., return null, show 404 page).
}
catch (ClickUpApiRateLimitException rlEx)
{
    _logger.LogWarning(rlEx, "Rate limit hit. Retry after: {RetryAfter}", rlEx.RetryAfter);
    // The SDK might have already retried. This catch is if it ultimately fails.
}
catch (ClickUpApiException apiEx) // General API error
{
    _logger.LogError(apiEx, "An unexpected ClickUp API error occurred. Status: {StatusCode}, Code: {ApiErrorCode}, CorrID: {CorrelationId}",
        apiEx.StatusCode, apiEx.ApiErrorCode, apiEx.CorrelationId);
    // Generic error message to user.
}
catch (HttpRequestException httpEx) // Network or fundamental HTTP issue
{
    _logger.Critical(httpEx, "A network error occurred while communicating with ClickUp API.");
    // Inform user about potential connectivity issues.
}
catch (Exception ex) // Catch-all for anything else
{
    _logger.LogError(ex, "An unexpected application error occurred.");
    // Generic error message.
}
```
