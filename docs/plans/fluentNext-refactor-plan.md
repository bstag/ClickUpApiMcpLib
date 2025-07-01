# FluentNext & Core SDK Consistency Refactor Plan

> **Status:** Draft ☐
> **Supersedes:** `docs/plans/fluentNext.md`

This document enumerates the concrete refactor steps required to align the Core SDK **services** and the **FluentNext** layer under a single, consistent design philosophy.
Each step contains:

* _Why_ – captures the motivation for the change. It should briefly outline the current pain points (e.g., inconsistent APIs, duplicated logic, hidden runtime errors), the desired end-state we are aiming for, and how the step contributes to that end-state (better developer ergonomics, lower cognitive load, improved maintainability and testability).
* **Tasks** – check-boxes to be ticked off via PRs.
* **Validation Rule** – how we confirm the step is complete (build & test gates, analyzers, etc.).

---

## 1 · Unified Identifier Ordering

**Tasks**
- [X] 1.1 Create `CONTRIBUTING.md` at the repository root if it doesn't exist.
- [X] 1.2 Add a new section named "SDK Method Parameter Conventions" to `CONTRIBUTING.md`.
- [X] 1.3 In this new section, specify the canonical order for common identifiers: `workspaceId` (or `teamId`) → `spaceId` → `folderId` → `listId` → `taskId` → `entityId` (for sub-entities like comments, checklist items, etc.). Also specify that other required parameters come before optional parameters.

---

## 2 · DTO Naming Scheme – *Request / Response*
**Why:** We currently mix `*Dto`, `*Response`, etc. Uniform names improve discoverability.

**Tasks**
- [ ] 2.1 Create a script (e.g., PowerShell or bash) to scan all `*.cs` files under `src/ClickUp.Api.Client.Models/` for class names ending in `Dto`, `Details`, `Info`, or other non-standard suffixes used for data transfer objects. The script should report these names.
    - [ ] 2.1.1 Identify common patterns for request DTOs (e.g., models used in POST/PUT method bodies).
    - [ ] 2.1.2 Identify common patterns for response DTOs (e.g., models returned by GET/POST/PUT methods).
- [ ] 2.2 Rename identified request DTO classes to use the `XxxRequest` suffix.
    - [ ] 2.2.1 Example: If `CreateTaskModel.cs` exists and is used as a request body, rename it to `CreateTaskRequest.cs`.
    - [ ] 2.2.2 Update all usages of these renamed classes across the solution (services, fluent builders, tests, examples).
- [ ] 2.3 Rename identified response DTO classes to use the `XxxResponse` suffix.
    - [ ] 2.3.1 Example: `ClickUp.Api.Client.Models/ResponseModels/Attachments/CreateTaskAttachmentResponse.cs` already follows this. Review others like `ClickUpList.cs` or `ClickUpWorkspace.cs` if they are direct API responses and not general entity models. If they are general entities used in various contexts, this rule might only apply to specific request/response wrappers.
- [ ] 2.4 Update any custom JSON serializer configurations (e.g., `JsonSerializerContext` or attributes) if class names are part of serialization contracts.
- [ ] 2.5 Update all unit tests in `src/ClickUp.Api.Client.Tests/` to use the new DTO names.
- [ ] 2.6 Update all integration tests in `src/ClickUp.Api.Client.IntegrationTests/` to use the new DTO names.
- [ ] 2.7 Update all examples in `examples/` to use the new DTO names.
- [ ] 2.8 Add a section to `CONTRIBUTING.md` specifying the DTO naming convention: `ServiceNameOperationRequest` for request models and `ServiceNameOperationResponse` for response models.

**Validation Rule:**
- `grep -E "class .*(Dto|Details|Info|Model|Body|Payload)(?!Request|Response)" src/ClickUp.Api.Client.Models/**/*.cs | wc -l` → 0 (Adjust regex as needed to catch undesirable suffixes, ensuring it doesn't catch legitimate internal models not intended as DTOs).
- All unit and integration tests green.
- `CONTRIBUTING.md` must contain the DTO naming convention.

---

## 3 · Pagination Abstraction (`IPagedResult<T>`)
**Why:** Paging parameters are re-invented in multiple services.

**Tasks**
- [ ] 3.1 Define `IPagedResult<T>` interface in `src/ClickUp.Api.Client.Models/Common/` (or a new `Pagination` namespace).
    ```csharp
    // Possible IPagedResult<T> structure
    namespace ClickUp.Api.Client.Models.Common.Pagination;
    public interface IPagedResult<T>
    {
        IReadOnlyList<T> Items { get; }
        int Page { get; }
        int PageSize { get; } // Or PerPage
        int TotalPages { get; }
        long TotalCount { get; } // If available from API
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
    }
    ```
- [ ] 3.2 Implement a concrete `PagedResult<T>` class implementing `IPagedResult<T>`.
- [ ] 3.3 Create helper extension method `AsPagedResult()` for `IApiConnection` or a dedicated pagination helper service that constructs `PagedResult<T>` from API responses that include pagination info (e.g. headers or a specific JSON structure).
    - [ ] 3.3.1 Identify how ClickUp API returns pagination details (e.g., `last_page` field, `Link` headers, or if it's cursor-based). The current OpenAPI spec (e.g. for `GetTasks`) shows `page` and `last_page` parameters. The responses often include the items directly in an array, sometimes with a root object like `tasks`.
- [ ] 3.4 Identify all service interface methods in `src/ClickUp.Api.Client.Abstractions/Services/*.cs` that currently return collections and support pagination (e.g., `GetTasks`, `GetComments`).
    - [ ] 3.4.1 Refactor these methods to return `Task<IPagedResult<TModel>>` instead of `Task<IEnumerable<TModel>>` or `Task<List<TModel>>`.
    - [ ] 3.4.2 Remove individual `page`, `pageSize` (or similar) parameters from these methods. The pagination parameters will now be handled by the fluent layer or a dedicated request object.
- [ ] 3.5 Update corresponding service implementations in `src/ClickUp.Api.Client/Services/*.cs` to implement the new interface signatures and correctly populate `IPagedResult<T>`.
- [ ] 3.6 Add fluent helper methods like `.Page(int pageNumber, int pageSize)` to relevant fluent builders in `src/ClickUp.Api.Client/Fluent/`.
    - [ ] 3.6.1 These methods should configure the pagination parameters on the underlying request object used by the service.
    - [ ] 3.6.2 Example: A fluent call like `client.Tasks.Get().ForList("listId").Page(2, 20).ExecuteAsync()`
- [ ] 3.7 Update unit tests in `src/ClickUp.Api.Client.Tests/` for affected services and fluent builders.
    - [ ] 3.7.1 Test that pagination parameters are correctly passed to the API.
    - [ ] 3.7.2 Test that API responses are correctly mapped to `IPagedResult<T>`.
- [ ] 3.8 Update integration tests in `src/ClickUp.Api.Client.IntegrationTests/` for paginated endpoints.
- [ ] 3.9 Update examples in `examples/` to demonstrate usage of the new pagination abstraction.

**Validation Rule:**
- No public service methods in `src/ClickUp.Api.Client.Abstractions/Services/*.cs` that are known to be paginated by the ClickUp API should expose raw `page`/`pageSize` (or similar) parameters.
- Contract tests (or dedicated unit/integration tests) verify that services returning `IPagedResult<T>` correctly handle pagination parameters and map responses.
- Fluent APIs for paginated resources should offer `.Page()` style helpers.

---

## 4 · Mandatory `CancellationToken`
**Why:** Missing tokens make graceful shutdown impossible.

**Tasks**
- [ ] 4.1 Enable nullable reference types (`<Nullable>enable</Nullable>`) in all relevant `.csproj` files (`src/ClickUp.Api.Client.Abstractions/ClickUp.Api.Client.Abstractions.csproj`, `src/ClickUp.Api.Client/ClickUp.Api.Client.csproj`, `src/ClickUp.Api.Client.Models/ClickUp.Api.Client.Models.csproj`, etc.) if not already enabled globally.
- [ ] 4.2 Add or ensure the Roslyn analyzer rule `CA2016` (Forward CancellationToken parameters to methods that call them) is enabled and treated as an error in the project's `.editorconfig` or build settings.
    ```editorconfig
    dotnet_diagnostic.CA2016.severity = error
    ```
- [ ] 4.3 Review all public `async` methods in service interfaces (`src/ClickUp.Api.Client.Abstractions/Services/*.cs`).
    - [ ] 4.3.1 Ensure each `async` method accepts a `CancellationToken cancellationToken = default` as the last parameter.
    - [ ] 4.3.2 Example: `IAttachmentsService.CreateTaskAttachmentAsync` already has this. Verify all others.
- [ ] 4.4 Review all public `async` methods in service implementations (`src/ClickUp.Api.Client/Services/*.cs`).
    - [ ] 4.4.1 Ensure each `async` method accepts and correctly passes the `CancellationToken` to any underlying `async` calls (e.g., `_apiConnection` methods).
    - [ ] 4.4.2 Example: `AttachmentsService.CreateTaskAttachmentAsync` passes `cancellationToken` to `_apiConnection.PostMultipartAsync`.
- [ ] 4.5 Review all public `async` methods in fluent API classes (`src/ClickUp.Api.Client/Fluent/**/*.cs`), especially `ExecuteAsync` or similar terminal methods.
    - [ ] 4.5.1 Ensure these methods accept a `CancellationToken cancellationToken = default`.
    - [ ] 4.5.2 Ensure the token is passed down to the service calls.
- [ ] 4.6 Fix any violations reported by `CA2016` and nullable reference type analysis across the codebase.
- [ ] 4.7 Update unit tests to verify `OperationCanceledException` is thrown when a token is cancelled, where applicable.
    - [ ] 4.7.1 This typically involves mocking `IApiConnection` methods to throw `OperationCanceledException` when their passed token is cancelled.

**Validation Rule:**
- `dotnet build -p:TreatWarningsAsErrors=true` passes with `CA2016` enabled and nullable reference types fully addressed for all projects under `src/`.
- Manual review confirms all public async methods (services, fluent APIs) accept and propagate `CancellationToken`.

---

## 5 · Centralised Exception Handling
**Why:** Today, services throw mixed exception types. The goal is for services to consistently throw exceptions derived from `ClickUpApiException`.

**Tasks**
- [ ] 5.1 Define `ClickUpApiExceptionFactory` in `src/ClickUp.Api.Client/Http/` (or a new `Exceptions` utility folder).
    ```csharp
    // src/ClickUp.Api.Client/Http/ClickUpApiExceptionFactory.cs (or similar location)
    namespace ClickUp.Api.Client.Http; // Or ClickUp.Api.Client.Exceptions

    using ClickUp.Api.Client.Models.Exceptions;
    using System.Net;
    using System.Net.Http; // For HttpResponseMessage

    public static class ClickUpApiExceptionFactory
    {
        public static ClickUpApiException Create(
            HttpResponseMessage response,
            string? responseContent,
            string? apiErrorCode = null, // Often parsed from responseContent
            string? customMessage = null)
        {
            var baseMessage = customMessage ?? $"API request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).";
            // Attempt to parse ClickUp specific error code and message from responseContent
            // Example: "ECODE", "err" fields in JSON.
            // This part needs to be adapted based on actual ClickUp error response structure.
            // For now, a generic approach:
            // string detailedError = ParseErrorFromBody(responseContent); // Implement this
            // apiErrorCode = apiErrorCode ?? ParseErrorCodeFromBody(responseContent);

            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => new ClickUpApiAuthenticationException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
                HttpStatusCode.Forbidden => new ClickUpApiAuthenticationException(baseMessage, response.StatusCode, apiErrorCode, responseContent), // Or a more specific "Forbidden" exception
                HttpStatusCode.NotFound => new ClickUpApiNotFoundException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
                HttpStatusCode.TooManyRequests => new ClickUpApiRateLimitException(baseMessage, response.StatusCode, apiErrorCode, responseContent, GetRetryAfter(response)),
                >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError => new ClickUpApiRequestException(baseMessage, response.StatusCode, apiErrorCode, responseContent), // Catch-all for 4xx
                >= HttpStatusCode.InternalServerError => new ClickUpApiServerException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
                _ => new ClickUpApiException(baseMessage, response.StatusCode, apiErrorCode, responseContent) // Default for unexpected status codes
            };
        }

        public static ClickUpApiException Create(Exception innerException, string message)
        {
            // For non-HTTP related issues that should still be wrapped
            return new ClickUpApiException(message, innerException);
        }

        // private static string ParseErrorFromBody(string? content) { /* ... */ }
        // private static string ParseErrorCodeFromBody(string? content) { /* ... */ }
        private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
        {
            if (response.Headers.RetryAfter?.Delta.HasValue)
                return response.Headers.RetryAfter.Delta.Value;
            if (response.Headers.RetryAfter?.Date.HasValue)
                return response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
            return null;
        }
    }
    ```
- [ ] 5.2 Refactor `ApiConnection.SendAsync<T>` (and similar methods like `PostMultipartAsync`) in `src/ClickUp.Api.Client/Http/ApiConnection.cs` to use `ClickUpApiExceptionFactory`.
    - [ ] 5.2.1 Instead of throwing generic `HttpRequestException` or returning null/default, catch exceptions or check response status codes.
    - [ ] 5.2.2 If response indicates failure (e.g., non-success status code), call `ClickUpApiExceptionFactory.Create(httpResponseMessage, responseBody)` to generate and throw the appropriate `ClickUpApiException` derivative.
- [ ] 5.3 Ensure all existing custom exceptions in `src/ClickUp.Api.Client.Models/Exceptions/` (like `ClickUpApiNotFoundException`, `ClickUpApiRateLimitException`, etc.) are comprehensive and used by the factory.
    - [ ] 5.3.1 Review `ClickUpApiException.cs` and its derivatives. They already seem well-defined.
- [ ] 5.4 Review all service implementations in `src/ClickUp.Api.Client/Services/*.cs`.
    - [ ] 5.4.1 Remove any direct throwing of `HttpRequestException` or other generic exceptions if the `ApiConnection` now handles this.
    - [ ] 5.4.2 Ensure services propagate exceptions from `ApiConnection` correctly.
- [ ] 5.5 Update unit tests for `ApiConnection` to verify that it throws the correct specific `ClickUpApiException` for different HTTP error statuses using the factory.
- [ ] 5.6 Update unit tests for services to ensure they correctly propagate exceptions thrown by `ApiConnection`.
    - [ ] 5.6.1 Example: `AttachmentsServiceTests` should have tests mocking `IApiConnection` to throw various `ClickUpApiException` types, and assert the service method re-throws them.

**Validation Rule:**
- Contract tests (or dedicated unit tests for `ApiConnection` and services) simulate 4xx/5xx HTTP responses from `IApiConnection` mock.
- Assert that the thrown exception type derives from `ClickUpApiException` and is the specific type corresponding to the HTTP status code (e.g., `ClickUpApiNotFoundException` for 404).
- No direct `HttpRequestException` (or other generic HTTP exceptions) should be thrown from service layer; all should be wrapped in `ClickUpApiException` or its derivatives by `ApiConnection`.

---

## 6 · Shared Value Objects (Filters, TimeRange, etc.)
**Why:** Reduce duplicated query-string logic.

**Tasks**
- [ ] 6.1 Identify common query parameters used for filtering, sorting, and date ranges across the ClickUp API (refer to `docs/OpenApiSpec/ClickUp-6-17-25.json` and `https://developer.clickup.com/reference/`).
    - [ ] 6.1.1 Examples: `start_date`, `end_date`, `order_by`, `reverse` (for sort direction), custom field filters.
- [ ] 6.2 Define `TimeRange` value object in `src/ClickUp.Api.Client.Models/Common/` (or a new `ValueObjects` sub-namespace).
    ```csharp
    // src/ClickUp.Api.Client.Models/Common/ValueObjects/TimeRange.cs
    public record TimeRange(DateTimeOffset StartDate, DateTimeOffset EndDate)
    {
        // Consider adding validation, e.g., StartDate <= EndDate
        // Method to convert to query parameters
        public Dictionary<string, string> ToQueryParameters(string startDateParamName = "start_date", string endDateParamName = "end_date")
        {
            return new Dictionary<string, string>
            {
                { startDateParamName, new DateTimeOffset(StartDate.UtcDateTime).ToUnixTimeMilliseconds().ToString() }, // ClickUp API uses Unix timestamp in milliseconds
                { endDateParamName, new DateTimeOffset(EndDate.UtcDateTime).ToUnixTimeMilliseconds().ToString() }
            };
        }
    }
    ```
- [ ] 6.3 Define `SortDirection` enum in `src/ClickUp.Api.Client.Models/Common/`.
    ```csharp
    public enum SortDirection { Ascending, Descending }
    ```
    And a helper to convert to API string (e.g., "asc", "desc", or true/false for "reverse").
- [ ] 6.4 Define `DateFilter` (or more specific filter objects like `TaskDateFilter`) value object. This might encapsulate a field name and a `TimeRange` or specific date.
- [ ] 6.5 Refactor service interface methods (`src/ClickUp.Api.Client.Abstractions/Services/*.cs`) that use such parameters to accept these value objects instead of raw `DateTimeOffset`, `string`, `bool reverse`, etc.
    - [ ] 6.5.1 Example: A method `GetTasksAsync(string listId, TimeRange? dateRangeFilter, string? sortBy, SortDirection? sortDirection)`
- [ ] 6.6 Update service implementations (`src/ClickUp.Api.Client/Services/*.cs`) to use these value objects.
    - [ ] 6.6.1 Use methods on the value objects (e.g., `TimeRange.ToQueryParameters()`) to build query strings, replacing manual `StringBuilder` logic for these parts.
    - [ ] 6.6.2 Example: `AttachmentsService.BuildQueryString` might be simplified if common query parameters become value objects.
- [ ] 6.7 Update fluent API builders (`src/ClickUp.Api.Client/Fluent/**/*.cs`) to provide methods for setting these value objects.
    - [ ] 6.7.1 Example: `.DueBetween(DateTimeOffset start, DateTimeOffset end)` or `.SortedBy("fieldName", SortDirection.Ascending)`.
- [ ] 6.8 Update unit tests and integration tests for affected methods.

**Validation Rule:**
- `grep -E "start_date=|end_date=|order_by=|reverse=" src/ClickUp.Api.Client/Services/**/*.cs` should show minimal to no direct string manipulation for these parameters outside of the value object files themselves or a centralized query building mechanism that uses them.
- Unit tests verify that value objects correctly translate to API query parameters.
- All tests green.

---

## 7 · Builder Validation & `Validate()` API
**Why:** Eager validation prevents late runtime failures.

**Tasks**
- [ ] 7.1 Define a custom `ValidationException` (e.g., `ClickUpRequestValidationException`) in `src/ClickUp.Api.Client.Models/Exceptions/`.
    ```csharp
    // src/ClickUp.Api.Client.Models/Exceptions/ClickUpRequestValidationException.cs
    public class ClickUpRequestValidationException : ClickUpApiException
    {
        public IEnumerable<string> ValidationErrors { get; }
        public ClickUpRequestValidationException(string message, IEnumerable<string> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors ?? new List<string>();
        }
        // Add other constructors as needed
    }
    ```
- [ ] 7.2 For each fluent request builder class in `src/ClickUp.Api.Client/Fluent/` (e.g., `TaskFluentCreateRequest.cs`):
    - [ ] 7.2.1 Add a public method `Validate()`:
        ```csharp
        // Example in a hypothetical TaskFluentCreateRequest
        public class TaskFluentCreateRequest
        {
            // ... properties and constructor ...
            private List<string> _validationErrors = new List<string>();

            public void Validate()
            {
                _validationErrors.Clear();
                if (string.IsNullOrWhiteSpace(ListId)) _validationErrors.Add("ListId is required.");
                if (string.IsNullOrWhiteSpace(Name)) _validationErrors.Add("Task name is required.");
                // ... other validation rules ...

                if (_validationErrors.Any())
                {
                    throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
                }
            }

            public async Task<CreateTaskResponse> ExecuteAsync(CancellationToken cancellationToken = default)
            {
                Validate(); // Auto-validate if not called explicitly
                // ... proceed with API call ...
            }
        }
        ```
    - [ ] 7.2.2 The `Validate()` method should check all required parameters and any complex validation rules (e.g., mutually exclusive parameters).
    - [ ] 7.2.3 If validation fails, `Validate()` should throw the `ClickUpRequestValidationException` with a list of validation errors.
- [ ] 7.3 Modify the `ExecuteAsync()` (or equivalent terminal method) in each fluent request builder.
    - [ ] 7.3.1 Call `Validate()` at the beginning of `ExecuteAsync()`. This ensures validation occurs even if the user doesn't call it explicitly.
- [ ] 7.4 Add unit tests for each fluent builder's `Validate()` method in `src/ClickUp.Api.Client.Tests/ServiceTests/Fluent/`.
    - [ ] 7.4.1 Test scenarios where required parameters are missing, verify `ClickUpRequestValidationException` is thrown.
    - [ ] 7.4.2 Test scenarios with valid parameters, verify no exception is thrown.
    - [ ] 7.4.3 Test that `ExecuteAsync()` calls `Validate()` and throws if validation fails.

**Validation Rule:**
- New unit tests for fluent builders pass, specifically testing:
    - `Validate()` throws `ClickUpRequestValidationException` when required parameters are missing or invalid.
    - `Validate()` does not throw when parameters are valid.
    - `ExecuteAsync()` throws `ClickUpRequestValidationException` (due to internal `Validate()` call) if pre-validation is not done and parameters are invalid.
- All existing tests remain green.

---

## 8 · Service ↔ Fluent Contract Tests
**Why:** Guard against divergence (method missing in one layer).

**Tasks**
- [ ] 8.1 Create a new test project: `src/ClickUp.Api.ContractTests/ClickUp.Api.ContractTests.csproj`.
- [ ] 8.2 In this project, create a test class (e.g., `ServiceFluentParityTests.cs`).
- [ ] 8.3 Implement reflection-based tests:
    - [ ] 8.3.1 The test should get all public methods from each service interface in `src/ClickUp.Api.Client.Abstractions/Services/I*.cs`.
    - [ ] 8.3.2 For each service method, it should try to find a corresponding entry point (factory method) in the main `ClickUpClient.cs` (or individual `XxxFluentApi.cs` classes) that ultimately allows invoking an equivalent operation.
    - [ ] 8.3.3 This might involve checking that for `IFooService.BarAsync(string id, BazRequest request)`, there's something like `client.Foo.Bar(string id, Action<BazRequestBuilder> configure)` or `client.Foo.Bar(string id, BazRequest request)`.
    - [ ] 8.3.4 This test is complex to make perfectly generic. It might start by listing service methods and checking if a corresponding fluent API class exists (e.g., `ITasksService` -> `TasksFluentApi`). Then, for key methods, manually ensure parity or use a convention (e.g., fluent factory method has same name as service method, or a clear mapping).
    - [ ] 8.3.5 Consider focusing on ensuring that each *service interface* has a corresponding *fluent API class* exposed by `ClickUpClient`.
    - [ ] 8.3.6 For method parity: for each public method in `I[ServiceName]Service`, check if `[ServiceName]FluentApi` has a public method that eventually calls it. Parameter matching can be tricky due to fluent builders. A simpler check might be that the fluent builder for an operation eventually calls the correct service method.
- [ ] 8.4 The test should report any service methods that do not have a clear corresponding fluent API entry point or execution path.

**Validation Rule:**
- Test project `ClickUp.Api.ContractTests` builds and all its tests pass.
- The contract tests should successfully identify and map most, if not all, service methods to their fluent counterparts based on agreed conventions. Discrepancies should be flagged.

---

## 9 · Helper Consolidation
**Why:** Remove duplicated `QueryStringBuilder`, etc.

**Tasks**
- [ ] 9.1 Identify all helper classes/methods currently dispersed across the solution (e.g., query string builders, JSON helpers, pagination helpers before step 3 is fully done).
    - [ ] 9.1.1 `src/ClickUp.Api.Client/Services/AttachmentsService.cs` contains `BuildQueryString`.
    - [ ] 9.1.2 Search for other instances of query building, URI manipulation, or common utilities.
- [ ] 9.2 Create a dedicated `Helpers` folder: `src/ClickUp.Api.Client/Helpers/`.
- [ ] 9.3 Consolidate identified helpers into this new folder.
    - [ ] 9.3.1 Create `src/ClickUp.Api.Client/Helpers/QueryStringBuilderHelper.cs` (or a more generic `UrlHelpers.cs`). Move and refactor `BuildQueryString` from `AttachmentsService` into this shared helper. Make it static and generic if possible.
    - [ ] 9.3.2 Ensure `JsonSerializerOptionsHelper.cs` is appropriately placed (already in `src/ClickUp.Api.Client/Helpers/`).
    - [ ] 9.3.3 If `PaginationHelpers.cs` was created in step 3, ensure it's in this `Helpers` folder.
- [ ] 9.4 Update all parts of the codebase that were using the old/duplicated helpers to use the new centralized ones.
- [ ] 9.5 Delete the redundant implementations from their old locations.

**Validation Rule:**
- Only one implementation for each distinct helper functionality should exist within the `src/ClickUp.Api.Client/Helpers/` namespace/folder.
- `grep` for old helper names or patterns outside the new `Helpers` folder should yield no results (or only legitimate distinct usages).
- `dotnet build` and `dotnet test` pass.

---

## 10 · Nullable Reference Types & Code Clean-up
**Why:** Ensure null-safety and consistent `!` suppression. This step is partially covered by Step 4 but focuses on a full sweep.

**Tasks**
- [ ] 10.1 Ensure `<Nullable>enable</Nullable>` is present in all `.csproj` files under `src/` (e.g., `ClickUp.Api.Client.csproj`, `ClickUp.Api.Client.Abstractions.csproj`, `ClickUp.Api.Client.Models.csproj`).
- [ ] 10.2 Systematically review and fix all nullable reference type warnings (`CS8600` to `CS8614`, `CS8618` to `CS8622`, etc.) across the entire solution (`src/*`).
    - [ ] 10.2.1 Prioritize public APIs: ensure method parameters and return types correctly express nullability.
    - [ ] 10.2.2 Properly initialize all non-nullable properties in constructors or with initializers. Use `required` keyword where appropriate for .NET 7+.
    - [ ] 10.2.3 Use null-forgiving operator (`!`) only when absolutely certain a value cannot be null at that point, and add a comment explaining why if not obvious.
    - [ ] 10.2.4 For DTOs, ensure properties are nullable if the API can omit them.
- [ ] 10.3 Review code for any other static analysis warnings (FxCop, StyleCop if enabled) and address them.
- [ ] 10.4 Remove any unused `using` statements.
- [ ] 10.5 Ensure consistent code formatting according to `.editorconfig` if one exists, or general .NET conventions.

**Validation Rule:**
- `dotnet build -warnaserror` (or with specific warning-as-error configurations for nullability) produces 0 nullable warnings for all projects in `src/`.
- Code review confirms judicious use of `!` and overall code cleanliness.

---

## Continuous Integration Updates
- [ ] 11.1 Review the existing GitHub Actions workflow file (e.g., `.github/workflows/dotnet.yml`).
- [ ] 11.2 Ensure the workflow executes on every Pull Request targeting the main branch.
- [ ] 11.3 Add/verify steps in the CI pipeline for:
    - [ ] 11.3.1 Restore dependencies: `dotnet restore src/ClickUp.Api.sln`
    - [ ] 11.3.2 Build the solution with warnings as errors: `dotnet build src/ClickUp.Api.sln --configuration Release --nologo -warnaserror`
    - [ ] 11.3.3 Run all unit tests: `dotnet test src/ClickUp.Api.sln --configuration Release --no-build --nologo`
    - [ ] 11.3.4 (Future, if applicable) Run Roslyn analyzers if they are not part of the build, or ensure build step includes them. The `ClickUp.IdOrderAnalyzer` from Step 1 should be active here.
    - [ ] 11.3.5 (Optional, nice-to-have) Add a markdown link checker for documentation files (e.g., using `markdown-link-check`).
- [ ] 11.4 Ensure the CI pipeline fails if any of these steps fail.

**Validation Rule:**
- A new Pull Request with a trivial change, after these CI updates are merged, successfully runs all the specified checks (build, test, analyzers) and passes.
- The CI pipeline is documented or easily discoverable in `.github/workflows/`.

---

## Glossary
| Term | Description |
|------|-------------|
| **Core SDK** | `Services/*` layer interacting with ClickUp HTTP API (`src/ClickUp.Api.Client/Services/`) |
| **FluentNext** | Fluent builders (`Fluent/*`) providing ergonomic façade (`src/ClickUp.Api.Client/Fluent/`) |
| **Contract Test** | Test ensuring parity or expected interaction between layers (e.g., `src/ClickUp.Api.ContractTests/`) |
| **Analyzer** | Roslyn rule enforcing conventions (e.g., `ClickUp.IdOrderAnalyzer`) |
| **DTO** | Data Transfer Object, typically classes in `src/ClickUp.Api.Client.Models/RequestModels/` and `src/ClickUp.Api.Client.Models/ResponseModels/` |

---

*End of plan.*
