# Detailed Plan: Exception Handling System

This document details the plan for creating a robust and informative exception handling system for the ClickUp API SDK.

**Source Documents:**
*   `docs/plans/05-exception-system-conceptual.md` (Initial conceptual plan)
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 2, Step 6)
*   `docs/OpenApiSpec/ClickUp-6-17-25.json` (To understand potential API error responses)

**Location in Codebase:**
*   Exception Classes: `src/ClickUp.Api.Client.Models/Exceptions/`
*   Error Handling Logic: Within service implementations, likely calling a shared helper in `src/ClickUp.Api.Client/Http/` or `src/ClickUp.Api.Client/Helpers/`.

## 1. Custom Exception Hierarchy

All custom exceptions will inherit from a base `ClickUpApiException`.

1.  **`ClickUpApiException.cs` (Base Exception)**
    *   Inherits from `System.Exception`.
    *   Properties:
        *   `public HttpStatusCode? HttpStatus { get; }`
        *   `public string? ApiErrorCode { get; }` (e.g., "OAUTH_023", "TEAM_001" - based on `ECODE` from ClickUp error responses if available)
        *   `public string? RawErrorResponse { get; }` (To store the raw JSON error string from the API for debugging)
    *   Constructors:
        *   `public ClickUpApiException(string message)`
        *   `public ClickUpApiException(string message, Exception innerException)`
        *   `public ClickUpApiException(string message, HttpStatusCode? httpStatus, string? apiErrorCode, string? rawErrorResponse, Exception? innerException)`

2.  **Specific Exception Types (inheriting from `ClickUpApiException`)**

    *   **`ClickUpApiAuthenticationException.cs`**
        *   For HTTP 401 (Unauthorized), HTTP 403 (Forbidden).
        *   Indicates issues with API key or permissions.
        *   Constructor: `(string message, HttpStatusCode httpStatus, string? apiErrorCode, string? rawErrorResponse, Exception? innerException)`

    *   **`ClickUpApiNotFoundException.cs`**
        *   For HTTP 404 (Not Found).
        *   Constructor: `(string message, HttpStatusCode httpStatus, string? apiErrorCode, string? rawErrorResponse, Exception? innerException)`

    *   **`ClickUpApiRateLimitException.cs`**
        *   For HTTP 429 (Too Many Requests).
        *   Additional Properties:
            *   `public TimeSpan? RetryAfterDelta { get; }`
            *   `public DateTimeOffset? RetryAfterDate { get; }`
        *   Constructor: `(string message, HttpStatusCode httpStatus, string? apiErrorCode, string? rawErrorResponse, TimeSpan? retryAfterDelta, DateTimeOffset? retryAfterDate, Exception? innerException)`

    *   **`ClickUpApiValidationException.cs`**
        *   For HTTP 400 (Bad Request), HTTP 422 (Unprocessable Entity) when input validation fails.
        *   Additional Properties:
            *   `public IReadOnlyDictionary<string, IReadOnlyList<string>>? ValidationErrors { get; }` (If API provides structured validation errors per field) - *Action: Investigate if ClickUp API provides structured validation errors.* For now, assume it might not and `RawErrorResponse` will be key.
        *   Constructor: `(string message, HttpStatusCode httpStatus, string? apiErrorCode, string? rawErrorResponse, IReadOnlyDictionary<string, IReadOnlyList<string>>? validationErrors, Exception? innerException)`

    *   **`ClickUpApiServerException.cs`**
        *   For HTTP 5xx errors (500, 502, 503, etc.).
        *   Constructor: `(string message, HttpStatusCode httpStatus, string? apiErrorCode, string? rawErrorResponse, Exception? innerException)`

    *   **`ClickUpApiRequestException.cs`** (More general client-side error)
        *   For other 4xx errors not covered above, or if an `HttpRequestException` occurs before a response is received (HttpStatus would be null).
        *   Constructor: `(string message, HttpStatusCode? httpStatus, string? apiErrorCode, string? rawErrorResponse, Exception? innerException)`

## 2. API Error Response Model

A DTO to represent the common error structure returned by the ClickUp API. Based on typical API error responses (and inspecting `ClickUp-6-17-25.json` for common error schemas, though it's not explicitly defined as a single global error model):

1.  **`ClickUpErrorResponse.cs`**
    *   Location: `src/ClickUp.Api.Client.Models/Responses/Shared/` (or similar)
    *   Properties:
        *   `[JsonPropertyName("err")] public string? ErrorMessage { get; set; }`
        *   `[JsonPropertyName("ECODE")] public string? ErrorCode { get; set; }`
        *   Other potential fields if the API consistently provides them (e.g., `error_id`, `details`). This needs to be confirmed by checking actual API error responses or more detailed error schema documentation if available. For now, these two are the most common.

## 3. Centralized Error Handling Logic Helper

A helper method, likely internal to the `ClickUp.Api.Client` project, will be responsible for processing `HttpResponseMessage` and throwing the appropriate exception.

1.  **`HttpErrorHandler.cs` (Static class or injectable service)**
    *   Location: `src/ClickUp.Api.Client/Http/` or `src/ClickUp.Api.Client/Helpers/`
    *   Method: `public static async Task ThrowIfErrorAsync(HttpResponseMessage response, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)`
        *   If `response.IsSuccessStatusCode`, return.
        *   Otherwise:
            *   Attempt to read the raw response body as a string: `string rawErrorContent = await response.Content.ReadAsStringAsync(cancellationToken);`
            *   Attempt to deserialize `rawErrorContent` into `ClickUpErrorResponse` using the provided `jsonSerializerOptions`.
                ```csharp
                ClickUpErrorResponse? apiError = null;
                if (!string.IsNullOrWhiteSpace(rawErrorContent))
                {
                    try
                    {
                        apiError = JsonSerializer.Deserialize<ClickUpErrorResponse>(rawErrorContent, jsonSerializerOptions);
                    }
                    catch (JsonException /* log this */) { /* ignore, proceed with raw content */ }
                }
                ```
            *   Extract `errorMessage = apiError?.ErrorMessage ?? response.ReasonPhrase ?? "An unexpected API error occurred."`
            *   Extract `apiErrorCode = apiError?.ErrorCode;`
            *   `httpStatus = response.StatusCode;`
            *   Switch on `httpStatus`:
                *   `case HttpStatusCode.BadRequest (400):`
                *   `case HttpStatusCode.UnprocessableEntity (422):`
                    *   `throw new ClickUpApiValidationException(errorMessage, httpStatus, apiErrorCode, rawErrorContent, null, null);`
                *   `case HttpStatusCode.Unauthorized (401):`
                *   `case HttpStatusCode.Forbidden (403):`
                    *   `throw new ClickUpApiAuthenticationException(errorMessage, httpStatus, apiErrorCode, rawErrorContent, null);`
                *   `case HttpStatusCode.NotFound (404):`
                    *   `throw new ClickUpApiNotFoundException(errorMessage, httpStatus, apiErrorCode, rawErrorContent, null);`
                *   `case HttpStatusCode.TooManyRequests (429):`
                    *   Parse `response.Headers.RetryAfter` to get `TimeSpan? retryAfterDelta` or `DateTimeOffset? retryAfterDate`.
                    *   `throw new ClickUpApiRateLimitException(errorMessage, httpStatus, apiErrorCode, rawErrorContent, retryAfterDelta, retryAfterDate, null);`
                *   `case >= 500:`
                    *   `throw new ClickUpApiServerException(errorMessage, httpStatus, apiErrorCode, rawErrorContent, null);`
                *   `default: // Other 4xx`
                    *   `throw new ClickUpApiRequestException(errorMessage, httpStatus, apiErrorCode, rawErrorContent, null);`

2.  **Integration into Service Implementations:**
    *   Service methods will call this helper after making an HTTP request.
    *   `await HttpErrorHandler.ThrowIfErrorAsync(response, _jsonSerializerOptions, cancellationToken);`
    *   Network-level exceptions like `HttpRequestException` or `TaskCanceledException` (due to timeout) will be caught separately in service methods and wrapped in `ClickUpApiRequestException` or a new `ClickUpApiTimeoutException`.

    ```csharp
    // Example usage in a service
    try
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);
        await HttpErrorHandler.ThrowIfErrorAsync(response, _settings.JsonSerializerOptions, cancellationToken); // _settings would provide options
        return await response.Content.ReadFromJsonAsync<DtoType>(_settings.JsonSerializerOptions, cancellationToken);
    }
    catch (HttpRequestException ex)
    {
        throw new ClickUpApiRequestException($"Network error: {ex.Message}", null, null, null, ex);
    }
    catch (TaskCanceledException ex) // Could be due to HttpClient timeout
    {
        throw new ClickUpApiRequestException("Request timed out.", null, null, null, ex); // Or a specific timeout exception
    }
    // ClickUpApiExceptions thrown by ThrowIfErrorAsync will propagate
    ```

## 4. Plan Output
*   This document `04-ExceptionHandling.md` will contain the finalized plan.
*   It will define the full hierarchy of exception classes with their properties and constructors.
*   It will specify the `ClickUpErrorResponse` DTO.
*   It will outline the logic for the `HttpErrorHandler.ThrowIfErrorAsync` method.
*   It will provide clear instructions on how service implementations should integrate this error handling.
```
