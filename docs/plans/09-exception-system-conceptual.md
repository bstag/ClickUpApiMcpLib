# Phase 2, Step 9: Design Global Exception System (Conceptual)

This document outlines the conceptual design for a global exception system for the ClickUp .NET client library. A well-defined exception hierarchy makes it easier for consumers of the library to catch and handle API errors programmatically.

## Core Goals

- **Clarity:** Exceptions should clearly indicate the nature of the error.
- **Rich Information:** Exceptions should carry relevant information from the API response (HTTP status code, API error codes/messages, correlation IDs).
- **Catchability:** Consumers should be able to catch specific types of API errors or general API errors.
- **Consistency:** Error handling within the service implementations should consistently lead to these defined exceptions.

## Exception Hierarchy

A custom exception hierarchy will be established, with a common base class for all ClickUp API-related errors.

1.  **`ClickUpApiException` (Base Class)**
    *   Inherits from `System.Exception`.
    *   Serves as the base for all exceptions originating from the ClickUp API client.
    *   Properties:
        *   `HttpStatusCode? HttpStatus { get; }`: The HTTP status code returned by the API. Nullable if the error occurred before an HTTP response was received (e.g., network error).
        *   `ApiErrorCode? ErrorCode { get; }`: A specific error code provided by the ClickUp API in the response body (e.g., "ERR_001"). This would be a string or an enum if the API defines a comprehensive set.
        *   `ApiErrorMessage? MessageFromApi { get; }`: The user-friendly error message string from the API response.
        *   `CorrelationId? CorrelationId { get; }`: If the API returns a correlation ID (e.g., in headers or body), it should be captured here.
    *   Constructors: Will allow setting these properties, in addition to the standard `message` and `innerException`.

2.  **Specific Exception Types (Inheriting from `ClickUpApiException`)**

    These will correspond to common classes of HTTP errors or specific, well-defined API error conditions.

    *   **`ClickUpApiAuthenticationException`:**
        *   Typically thrown for HTTP 401 (Unauthorized) or 403 (Forbidden) errors.
        *   Indicates issues with the API key or permissions.
        *   `ApiErrorCode` might specify "OAUTH_023" or "TOKEN_001" etc.

    *   **`ClickUpApiNotFoundException`:**
        *   Typically thrown for HTTP 404 (Not Found) errors.
        *   Indicates that the requested resource does not exist.

    *   **`ClickUpApiRateLimitException`:**
        *   Typically thrown for HTTP 429 (Too Many Requests) errors.
        *   Properties:
            *   `RetryAfter? RetryAfterDelta { get; }`: If the API provides a `Retry-After` header (as a delta-seconds value), this can be stored as a `TimeSpan`.
            *   `RetryAfter? RetryAfterDate { get; }`: If the API provides a `Retry-After` header (as an HTTP-date), this can be stored as a `DateTimeOffset`.

    *   **`ClickUpApiValidationException`:**
        *   Typically thrown for HTTP 400 (Bad Request) or 422 (Unprocessable Entity) errors where the API indicates issues with the request payload.
        *   Properties:
            *   `ValidationErrors? Errors { get; }`: A dictionary or list of validation errors if the API provides detailed field-specific error messages (e.g., `Dictionary<string, List<string>>`).

    *   **`ClickUpApiServerException`:**
        *   Typically thrown for 5xx HTTP status codes (e.g., 500 Internal Server Error, 502 Bad Gateway, 503 Service Unavailable).
        *   Indicates an issue on the ClickUp server side.

    *   **`ClickUpApiRequestException`:**
        *   A more general client-side error for 4xx status codes that don't fit the more specific categories above.
        *   Could also be used if an `HttpRequestException` occurs (e.g., network issue, DNS failure) before a response is received, in which case `HttpStatus` would be null.

    *   **`ClickUpApiTimeoutException`:**
        *   If a request times out (e.g., `TaskCanceledException` due to `HttpClient.Timeout`). This might wrap the `TaskCanceledException`.

## Throwing Exceptions

- **Service Implementations:** The concrete service classes (e.g., `TaskService`) will be responsible for catching `HttpRequestException`, examining `HttpResponseMessage`, and throwing the appropriate `ClickUpApiException` derivative.
- **Helper Method:** A common helper method (e.g., `EnsureSuccessOrThrowAsync(HttpResponseMessage response)`) will be used across service implementations. This method will:
    1.  Check `response.IsSuccessStatusCode`.
    2.  If not successful:
        a.  Attempt to read the error content from the response body.
        b.  Attempt to deserialize the error content into a predefined error model (e.g., `ClickUpApiErrorResponse` which mirrors the API's error structure).
        c.  Use the HTTP status code and the deserialized error details to instantiate and throw the correct specific `ClickUpApiException`.
        d.  If error content cannot be parsed, a more generic exception (like `ClickUpApiRequestException` or `ClickUpApiServerException`) will be thrown with the raw response content and status code.

**Conceptual Error Handling in a Service:**

```csharp
// In a service method (e.g., TaskService)
// ... after _httpClient.SendAsync(...)
if (!response.IsSuccessStatusCode)
{
    await HandleErrorResponseAsync(response); // This helper method throws the appropriate ClickUpApiException
}
// ...

// Shared helper method (likely in an internal utility class)
private async Task HandleErrorResponseAsync(HttpResponseMessage response)
{
    string rawErrorContent = string.Empty;
    ClickUpApiErrorResponse apiError = null; // A model representing ClickUp's error JSON structure

    try
    {
        rawErrorContent = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(rawErrorContent))
        {
            apiError = JsonSerializer.Deserialize<ClickUpApiErrorResponse>(rawErrorContent, ClickUpJsonSerializerOptions.Options);
        }
    }
    catch (JsonException ex)
    {
        // Log deserialization error, but proceed with throwing based on status code
        _logger?.LogWarning(ex, "Failed to deserialize API error response. Raw content: {RawErrorContent}", rawErrorContent);
    }

    string message = apiError?.ErrorMessage ?? response.ReasonPhrase ?? "An unexpected error occurred.";
    string apiErrorCode = apiError?.ErrorCode; // Example field from ClickUpApiErrorResponse

    switch ((int)response.StatusCode)
    {
        case 400:
        case 422: // Unprocessable Entity often includes validation errors
            throw new ClickUpApiValidationException(message, (int)response.StatusCode, apiErrorCode, rawErrorContent, null /* validation details */, null);
        case 401:
            throw new ClickUpApiAuthenticationException(message, (int)response.StatusCode, apiErrorCode, rawErrorContent, null);
        case 403:
            // Could be a more specific "PermissionDeniedException" or reuse AuthenticationException
            throw new ClickUpApiAuthenticationException(message, (int)response.StatusCode, apiErrorCode, rawErrorContent, null);
        case 404:
            throw new ClickUpApiNotFoundException(message, (int)response.StatusCode, apiErrorCode, rawErrorContent, null);
        case 429:
            var retryAfter = response.Headers.RetryAfter;
            // Logic to parse retryAfter (delta seconds or date)
            TimeSpan? retryAfterDelta = null; // Parse from retryAfter
            throw new ClickUpApiRateLimitException(message, (int)response.StatusCode, apiErrorCode, rawErrorContent, retryAfterDelta, null);
        case >= 500 and <= 599:
            throw new ClickUpApiServerException(message, (int)response.StatusCode, apiErrorCode, rawErrorContent, null);
        default: // Other 4xx errors
            throw new ClickUpApiRequestException(message, (int)response.StatusCode, apiErrorCode, rawErrorContent, null);
    }
}
```

*(Note: `ClickUpApiErrorResponse` is a hypothetical model that needs to be defined based on the actual error structure of the ClickUp API.)*

## Information Carried by Exceptions

- **Message:** A clear, concise message. For API errors, this should ideally incorporate the message from the API.
- **InnerException:** Preserve the original exception if one was caught (e.g., `HttpRequestException`, `JsonException`).
- **HTTP Status Code:** Essential for programmatic decisions.
- **API Error Code:** Specific code from the API body, if available.
- **API Error Message:** The detailed message from the API body.
- **Raw Response Body (Optional but good for debugging):** The raw error response string, perhaps not as a direct property but available if needed.
- **Correlation ID:** If provided by the API.
- **Specific fields for certain exceptions:** e.g., `RetryAfter` for rate limiting, validation error details for validation exceptions.

## Location of Exception Classes

- These exception classes will be defined in the `Models` project or a dedicated `ClickUp.Net.Core` project if such a project is created for shared domain entities and exceptions. Given the current structure, `Models` is suitable as they are part of the data contract.

## Next Steps

- Analyze the ClickUp API documentation for its specific error response structure (JSON fields like `err`, `ECODE` in the example prompt).
- Define the `ClickUpApiErrorResponse` model to match the API's error payload.
- Implement the base `ClickUpApiException` and the specific derived exception classes.
- Implement the `HandleErrorResponseAsync` (or similar) helper method within the client library's internal utilities.
- Ensure all service implementation methods correctly use this error handling mechanism.
```
