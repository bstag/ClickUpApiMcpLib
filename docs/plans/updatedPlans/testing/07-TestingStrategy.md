# Detailed Plan: Testing Strategy

This document outlines the testing strategy for the ClickUp API SDK, covering unit tests and integration tests.

**Source Documents:**
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 3, Step 10)

**Location in Codebase:**
*   Test Project: `src/ClickUp.Api.Client.Tests/` (A new xUnit or MSTest project will be created).

## 1. General Principles

*   **Test Pyramid:** Focus on a larger base of unit tests, a moderate number of integration tests, and minimal end-to-end tests (if any, for this SDK, integration tests might be the highest level).
*   **Coverage:** Aim for high test coverage for critical components, especially model serialization/deserialization, service logic, and error handling.
*   **Readability and Maintainability:** Tests should be easy to understand and maintain.
*   **Isolation:** Unit tests must be isolated and not depend on external services or states.
*   **Automation:** All tests should be runnable via `dotnet test` and integrated into any CI/CD pipeline.

## 2. Unit Tests

**Purpose:** Verify the correctness of individual components (classes, methods) in isolation.

**Testing Framework:** xUnit (preferred) or MSTest.
**Mocking Framework:** Moq or NSubstitute.

**Key Areas for Unit Testing:**

1.  **Model (DTO) Serialization/Deserialization:**
    *   For each DTO in `ClickUp.Api.Client.Models`:
        *   Verify correct serialization to JSON, especially property names (e.g., `JsonPropertyName` attributes) and enum string values.
        *   Verify correct deserialization from sample JSON payloads (representative of API responses) into C# objects, including handling of nullable properties, collections, and nested objects.
        *   Test edge cases: empty JSON, JSON with missing optional fields, JSON with extra fields (if `JsonUnmappedMemberHandling` is relevant).
        *   Test polymorphic DTOs using `JsonDerivedType` to ensure correct type resolution during deserialization.

2.  **Service Implementations (`src/ClickUp.Api.Client/Services/`):**
    *   For each service method (e.g., `TasksService.GetTaskAsync`):
        *   **Mock Dependencies:** Mock `HttpClient` (typically by mocking `HttpMessageHandler` passed to `HttpClient`) and `ILogger`.
        *   **Verify Request Construction:**
            *   Assert that the correct HTTP method, relative URL, and query parameters are used.
            *   For POST/PUT, verify that the request body is correctly serialized from the request DTO.
            *   Verify that `Authorization` headers (if handled directly, though usually by `DelegatingHandler`) and other necessary headers are added.
            *   Verify `CancellationToken` is passed through.
        *   **Simulate API Responses (using mocked `HttpMessageHandler`):**
            *   **Success Case:** Simulate a successful `HttpResponseMessage` with a sample JSON payload. Verify that the service method correctly deserializes the payload into the expected response DTO and returns it.
            *   **API Error Cases:** Simulate `HttpResponseMessage` with various error status codes (400, 401, 403, 404, 429, 500, etc.) and sample error JSON payloads. Verify that the service method throws the correct custom `ClickUpApiException` (e.g., `ClickUpApiNotFoundException`, `ClickUpApiRateLimitException`) with appropriate properties populated (HttpStatus, ApiErrorCode, RawErrorResponse).
            *   **Network Error Case:** Simulate an `HttpRequestException` being thrown by the `HttpMessageHandler`. Verify that it's wrapped in an appropriate `ClickUpApiException` (e.g., `ClickUpApiRequestException`).
            *   **Timeout Case:** Simulate `TaskCanceledException` (as if from `HttpClient.Timeout`). Verify appropriate handling (e.g., wrapping in `ClickUpApiRequestException` or a specific timeout exception).
        *   **Verify Logging:** If logging is implemented, verify that appropriate log messages (especially errors) are written.

3.  **Helper Classes (`src/ClickUp.Api.Client/Helpers/`, `src/ClickUp.Api.Client/Http/`):**
    *   **`AuthenticationDelegatingHandler`:**
        *   Verify that it correctly adds the `Authorization` header when an API key is provided.
        *   Verify it doesn't add the header if no key is provided.
        *   Verify it correctly uses "Bearer" scheme if OAuth token support is added and a token is provided.
    *   **`ClickUpJsonSerializerSettings`:** While not directly testable as a static class, its effects are tested via model serialization/deserialization tests.
    *   **`QueryStringBuilder` (if created):**
        *   Test with various inputs: empty dictionary, dictionary with one param, multiple params, params needing URL encoding, null/empty values.
    *   **`HttpErrorHandler`:**
        *   Test its logic by providing mock `HttpResponseMessage` objects with different status codes and error payloads. Verify it throws the correct derived `ClickUpApiException`.

4.  **Pagination Helpers (`IAsyncEnumerable<T>` methods):**
    *   Mock the underlying service calls that fetch individual pages.
    *   Test scenarios:
        *   No items returned from the first call.
        *   One page of items.
        *   Multiple pages of items (ensure all items are yielded).
        *   API returns `last_page = true` correctly.
        *   Cursor logic (if applicable) correctly updates for the next call.
        *   Cancellation during enumeration.
        *   API error during a page fetch (ensure exception propagates).

**Test Naming Convention:** `MethodName_Scenario_ExpectedBehavior`
    *   Example: `GetTaskAsync_ValidId_ReturnsTaskDto`
    *   Example: `GetTaskAsync_ApiReturns404_ThrowsClickUpApiNotFoundException`

## 3. Integration Tests

**Purpose:** Verify the interaction between the SDK and the live ClickUp API. These tests ensure that the SDK works correctly with the actual API, including request/response formats, authentication, and API behavior.

**Framework:** Same as unit tests (xUnit or MSTest).

**Prerequisites:**
*   **Test ClickUp Workspace:** A dedicated ClickUp Workspace (or a safe, isolated area within a development Workspace) is required.
*   **API Token:** A valid Personal API Token for the test Workspace. This token should be configurable and **NOT** checked into source control (e.g., use environment variables, user secrets, or a configuration file ignored by git).
*   **Test Data Setup/Teardown:** Some tests might require specific data to be present in the ClickUp Workspace (e.g., a specific task, list, folder). Consider strategies for:
    *   Creating necessary data at the beginning of a test run or test class.
    *   Cleaning up created data after tests to ensure idempotency and avoid polluting the test Workspace. This can be complex.
    *   Alternatively, target read-only operations or endpoints that are less sensitive to state.

**Key Areas for Integration Testing (Prioritize based on risk and complexity):**

1.  **Authentication:**
    *   Test a simple read operation (e.g., `GetAuthorizedUserAsync`) with a valid API token to ensure authentication works.
    *   Test with an invalid/revoked token to ensure a `ClickUpApiAuthenticationException` is thrown.

2.  **Core CRUD Operations for Key Entities:**
    *   Select a few key entities (e.g., Tasks, Lists, Comments).
    *   **Create:** Create an entity, verify the response.
    *   **Read:** Read the created entity by its ID, verify its properties.
    *   **Update:** Update some properties of the entity, verify the update.
    *   **Delete:** Delete the entity, verify it's no longer accessible (or an appropriate status is returned).
    *   *Caution:* These tests modify state and require careful setup/teardown.

3.  **Complex Query Parameters / Filtering:**
    *   For endpoints with complex filtering (e.g., `GetTasksAsync` with various filters like assignees, statuses, due dates):
        *   Set up tasks that match specific criteria.
        *   Call the SDK method with the corresponding filters.
        *   Verify that only the expected tasks are returned.

4.  **Pagination:**
    *   Test an endpoint that returns multiple pages of data.
    *   Use the basic pagination method (e.g., `GetTasksAsync` with `page` parameter) to fetch a few pages and verify data consistency.
    *   Test the `IAsyncEnumerable<T>` helper method to ensure it correctly iterates through all items across pages.

5.  **Error Handling (Live API Errors):**
    *   Intentionally trigger specific API errors if possible and safe:
        *   Request a non-existent resource to verify `ClickUpApiNotFoundException`.
        *   Attempt an operation without sufficient permissions (if test users with different roles can be set up) to verify `ClickUpApiAuthenticationException` (403).
        *   Send an invalid request payload to an endpoint to verify `ClickUpApiValidationException`.
    *   *Note:* Triggering rate limits (429) or server errors (5xx) reliably in tests is difficult and usually not a primary focus for SDK integration tests, but handling their structure if they occur is tested at the unit level.

6.  **Specific API Features:**
    *   Test any particularly complex or critical API features the SDK supports, e.g., file attachments, custom field manipulation.

**Configuration for Integration Tests:**
*   Use a configuration mechanism (e.g., `appsettings.IntegrationTests.json`, environment variables) to store the API token and test Workspace details. This file should be in `.gitignore`.
*   Consider using conditional compilation (`#if INTEGRATION_TESTS`) or test categories/traits to separate integration tests from unit tests, allowing them to be run selectively.

## 4. Test Project Structure (`src/ClickUp.Api.Client.Tests/`)

```
ClickUp.Api.Client.Tests/
|-- Models/  // Tests for DTOs
|   |-- TaskDtoTests.cs
|   |-- ...
|-- Services/ // Tests for service implementations
|   |-- TasksServiceTests.cs
|   |-- ...
|-- Http/ // Tests for DelegatingHandlers, etc.
|   |-- AuthenticationDelegatingHandlerTests.cs
|-- Helpers/ // Tests for other utilities
|   |-- QueryStringBuilderTests.cs // (if created)
|-- Integration/ // Integration tests
|   |-- TasksIntegrationTests.cs
|   |-- AuthenticationIntegrationTests.cs
|   |-- ...
|-- TestInfrastructure/ // Mocks, test data builders, config helpers
|   |-- MockHttpMessageHandler.cs
|   |-- TestConfiguration.cs
|-- ClickUp.Api.Client.Tests.csproj
|-- appsettings.IntegrationTests.json (gitignored) // For integration test config
```

## 5. CI/CD Integration
*   Unit tests should run automatically on every commit/PR.
*   Integration tests might run on a less frequent schedule (e.g., nightly) or manually triggered, due to their dependency on the live API and potentially longer execution time.

This testing strategy aims to build confidence in the SDK's correctness and robustness.
```
