# Detailed Plan: Exception Handling System

This document details the plan for creating a robust and informative exception handling system for the ClickUp API SDK.

**Source Documents:**
*   [`docs/plans/05-exception-system-conceptual.md`](../05-exception-system-conceptual.md) (Initial conceptual plan)
*   [`docs/plans/NEW_OVERALL_PLAN.md`](../NEW_OVERALL_PLAN.md) (Phase 2, Step 6)
*   [`docs/OpenApiSpec/ClickUp-6-17-25.json`](../../OpenApiSpec/ClickUp-6-17-25.json) (To understand potential API error responses)

**Location in Codebase:**
*   Exception Classes: `src/ClickUp.Api.Client.Models/Exceptions/`
*   Error Handling Logic: `src/ClickUp.Api.Client/Http/ApiConnection.cs` (within the `HandleErrorResponseAsync` method).

## 1. Custom Exception Hierarchy

All custom exceptions inherit from a base `ClickUpApiException`.

- [x] **1. `ClickUpApiException.cs` (Base Exception)**
    - [x] Exists in `src/ClickUp.Api.Client.Models/Exceptions/ClickUpApiException.cs`.
    - [x] Inherits from `System.Exception`.
    - [x] Properties:
        - [x] `public HttpStatusCode? HttpStatus { get; }`
        - [x] `public string? ApiErrorCode { get; }`
        - [x] `public string? RawErrorContent { get; }` (as per class file)
    - [x] Constructors:
        - [x] `public ClickUpApiException(string message, HttpStatusCode? httpStatus = null, string? apiErrorCode = null, string? rawErrorContent = null, Exception? innerException = null)`
        - [x] `public ClickUpApiException(string message, Exception innerException, HttpStatusCode? httpStatus = null, string? apiErrorCode = null, string? rawErrorContent = null)`

- [x] **2. Specific Exception Types (inheriting from `ClickUpApiException`)**
    *(All listed exceptions exist in the specified location and inherit from `ClickUpApiException`)*

    - [x] **`ClickUpApiAuthenticationException.cs`**
        - [x] For HTTP 401 (Unauthorized), HTTP 403 (Forbidden).
        - [x] Indicates issues with API key or permissions.
        - [x] Constructor matches base, passes relevant info.

    - [x] **`ClickUpApiNotFoundException.cs`**
        - [x] For HTTP 404 (Not Found).
        - [x] Constructor matches base.

    - [x] **`ClickUpApiRateLimitException.cs`**
        - [x] For HTTP 429 (Too Many Requests).
        - [x] Additional Properties:
            - [x] `public TimeSpan? RetryAfterDelta { get; }`
            - [x] `public DateTimeOffset? RetryAfterDate { get; }`
        - [x] Constructor includes `retryAfterDelta` and `retryAfterDate`.

    - [x] **`ClickUpApiValidationException.cs`**
        - [x] For HTTP 400 (Bad Request), HTTP 422 (Unprocessable Entity).
        - [x] Additional Properties:
            - [x] `public IReadOnlyDictionary<string, IReadOnlyList<string>>? ValidationErrors { get; }` (Exists in the class constructor signature, passed as null from `ApiConnection` currently).
        - [x] Constructor includes `validationErrors`.

    - [x] **`ClickUpApiServerException.cs`**
        - [x] For HTTP 5xx errors.
        - [x] Constructor matches base.

    - [x] **`ClickUpApiRequestException.cs`** (More general client-side error)
        - [x] For other 4xx errors not covered above, or if an `HttpRequestException` occurs.
        - [x] Constructor matches base.

## 2. API Error Response Model

A DTO to represent the common error structure returned by the ClickUp API.

- [x] **1. `ClickUpErrorResponse.cs`**
    - [x] Location: `src/ClickUp.Api.Client.Models/Responses/Shared/ClickUpErrorResponse.cs` - Implemented.
    - [x] Properties:
        - [x] `[JsonPropertyName("err")] public string ErrorMessage { get; set; }`
        - [x] `[JsonPropertyName("ECODE")] public string ErrorCode { get; set; }`
    *Action: This DTO has been created and is used in `ApiConnection.HandleErrorResponseAsync`.*

## 3. Centralized Error Handling Logic Helper

The logic is centralized within `ApiConnection.HandleErrorResponseAsync`.

- [x] **1. `HandleErrorResponseAsync` method in `ApiConnection.cs`**
    - [x] Location: `src/ClickUp.Api.Client/Http/ApiConnection.cs`.
    - [x] Method: `private async Task HandleErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)`
        - [x] If `response.IsSuccessStatusCode`, it's not called (logic is `if (!response.IsSuccessStatusCode) { await HandleErrorResponseAsync... }`).
        - [x] Otherwise:
            - [x] Reads the raw response body as a string: `string rawErrorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);`
            - [x] Attempts to deserialize `rawErrorContent` into `ClickUpErrorResponse`. (Implemented)
            - [x] Extracts `errorMessage` (from DTO if available, otherwise generic + status code, or `response.ReasonPhrase`).
            - [x] `apiErrorCode` is populated from DTO if available, otherwise null. (Implemented)
            - [x] `httpStatus = response.StatusCode;` is used.
            - [x] Switch on `httpStatus`:
                - [x] `case HttpStatusCode.BadRequest (400):`
                - [x] `case HttpStatusCode.UnprocessableEntity (422):` -> `throw new ClickUpApiValidationException(...)` (Detailed `ValidationErrors` dictionary is not yet parsed from raw content, passed as null).
                - [x] `case HttpStatusCode.Unauthorized (401):`
                - [x] `case HttpStatusCode.Forbidden (403):` -> `throw new ClickUpApiAuthenticationException(...)`.
                - [x] `case HttpStatusCode.NotFound (404):` -> `throw new ClickUpApiNotFoundException(...)`.
                - [x] `case HttpStatusCode.TooManyRequests (429):` -> `throw new ClickUpApiRateLimitException(...)` (Parses `RetryAfter` header).
                - [x] `case HttpStatusCode.InternalServerError, BadGateway, ServiceUnavailable, GatewayTimeout:` (Covers >= 500 range) -> `throw new ClickUpApiServerException(...)`.
                - [x] `default: // Other 4xx` -> `throw new ClickUpApiException(...)` (Note: Plan suggested `ClickUpApiRequestException`, current code uses base `ClickUpApiException` for unmapped 4xx).

- [x] **2. Integration into Service Implementations (via `ApiConnection` methods):**
    - [x] `ApiConnection`'s public methods (GetAsync, PostAsync, etc.) call `HandleErrorResponseAsync` for non-successful responses.
    - [x] Network-level exceptions like `HttpRequestException` or `TaskCanceledException` are caught in `ApiConnection` methods and wrapped in `ClickUpApiRequestException`.

    ```csharp
    // Example from ApiConnection.GetAsync
    try
    {
        // ... send request ...
        if (response.IsSuccessStatusCode) { /* return result */ }
        await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
        return default; // Unreachable
    }
    catch (Exception ex) when (ex is not ClickUpApiException)
    {
        throw new ClickUpApiRequestException($"Request failed for GET {endpoint}: {ex.Message}", null, null, null, ex);
    }
    ```

## 4. Plan Output
- [x] This document `04-ExceptionHandling.md` has been updated with checkboxes.
- [x] The exception class hierarchy is largely implemented as planned.
- [x] The `ClickUpErrorResponse` DTO has been implemented and is used for error detail extraction.
- [x] The logic in `ApiConnection.HandleErrorResponseAsync` has been improved to parse `apiErrorCode` from the error DTO. Parsing detailed `ValidationErrors` and the default exception for unmapped 4xx remain as minor deviations/areas for enhancement.
- [x] Service implementations are correctly integrated via `ApiConnection`.
```
```
