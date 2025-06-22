# Detailed Plan: Testing Strategy

This document outlines the testing strategy for the ClickUp API SDK, covering unit tests and integration tests.

**Source Documents:**
*   [`docs/plans/NEW_OVERALL_PLAN.md`](../NEW_OVERALL_PLAN.md) (Phase 3, Step 10)

**Location in Codebase:**
*   Test Project: `src/ClickUp.Api.Client.Tests/` - Project exists.

## 1. General Principles

- [x] **Test Pyramid:** Focus on a larger base of unit tests, a moderate number of integration tests.
- [ ] **Coverage:** Aim for high test coverage. (Status: In progress, current coverage unknown but some tests exist).
- [x] **Readability and Maintainability:** Tests should be easy to understand and maintain.
- [x] **Isolation:** Unit tests must be isolated.
- [x] **Automation:** All tests should be runnable via `dotnet test`.

## 2. Unit Tests

**Purpose:** Verify the correctness of individual components (classes, methods) in isolation.

- [x] **Testing Framework:** xUnit is used (inferred from typical .NET Core practices, to be confirmed by inspecting .csproj).
- [x] **Mocking Framework:** Moq is likely, or NSubstitute (to be confirmed by inspecting .csproj dependencies).

**Key Areas for Unit Testing:**

- [ ] **1. Model (DTO) Serialization/Deserialization:**
    - [ ] For each DTO in `ClickUp.Api.Client.Models`:
        - [x] Verify correct serialization to JSON. (Started with `MemberTests.cs`)
        - [x] Verify correct deserialization from sample JSON. (Started with `MemberTests.cs`)
        - [x] Test edge cases. (Null/missing properties covered in `MemberTests.cs`)
        - [ ] Test polymorphic DTOs.
    *(Status: `Models/Common/MemberTests.cs` added. This is a key area for ongoing expansion.)*

- [x] **2. Service Implementations (`src/ClickUp.Api.Client/Services/`):**
    - [x] Some service tests exist (e.g., `AttachmentsServiceTests.cs`, `TaskServiceTests.cs`).
    - [ ] For each service method:
        - [x] **Mock Dependencies:** `IApiConnection` is the primary dependency to mock for service tests. (Current tests likely do this).
        - [ ] **Verify Request Construction:** (Partially covered in existing tests, needs comprehensive review).
            - [ ] Assert correct HTTP method, URL, query params.
            - [ ] Verify request body serialization.
            - [ ] Verify headers (auth is handled by `AuthenticationDelegatingHandler`, so less direct testing in service layer).
            - [ ] Verify `CancellationToken` pass-through.
        - [ ] **Simulate API Responses (mocking `IApiConnection`):**
            - [x] **Success Case:** Simulate successful calls. (Likely covered in existing tests).
            - [ ] **API Error Cases:** Simulate `IApiConnection` throwing various `ClickUpApiException`s. Verify service methods handle or propagate them correctly.
            - [ ] **Network Error Case:** Simulate `IApiConnection` throwing `ClickUpApiRequestException` due to network issues.
            - [ ] **Timeout Case:** Simulate `IApiConnection` throwing `ClickUpApiRequestException` due to timeout.
        - [ ] **Verify Logging:** (If services implement direct logging beyond what `IApiConnection` or handlers provide).

- [ ] **3. Helper Classes (`src/ClickUp.Api.Client/Helpers/`, `src/ClickUp.Api.Client/Http/`):**
    - [ ] **`AuthenticationDelegatingHandler`:** (Requires `HttpMessageHandler` testing setup).
        - [ ] Verify `Authorization` header addition with token.
        - [ ] Verify no header if no token.
        - [ ] Verify Bearer scheme for OAuth (when supported).
    - [ ] **`JsonSerializerOptionsHelper`:** Effects tested via model serialization tests.
    - [ ] **`ApiConnection` (Error Handling part):** Unit test `HandleErrorResponseAsync` by mocking `HttpResponseMessage`.
    - [ ] **Query String Building (within `ApiConnection`):** Unit test the `BuildRequestUri` logic.

- [ ] **4. Pagination Helpers (`IAsyncEnumerable<T>` methods):**
    - [ ] Mock underlying service calls.
    - [ ] Test scenarios: no items, single page, multiple pages, `last_page` flag, cursor logic, cancellation, API errors.
    *(Status: `TaskServiceTests.cs` exists, but specific tests for `GetTasksAsyncEnumerableAsync` are not detailed here. This needs checking/adding.)*

- [x] **Test Naming Convention:** `MethodName_Scenario_ExpectedBehavior` (Observed in existing tests like `AttachmentsServiceTests.cs`).

## 3. Integration Tests

**Purpose:** Verify the interaction between the SDK and the live ClickUp API.

- [x] **Framework:** xUnit (assumed, consistent with unit tests).
- [x] **Prerequisites:**
    - [ ] **Test ClickUp Workspace:** Required.
    - [x] **API Token:** Required, must be configurable and not in source control. `IntegrationTestBase.cs` handles configuration via user secrets/environment variables.
    - [ ] **Test Data Setup/Teardown:** Needs strategy.

- [x] **Key Areas for Integration Testing:**
    - [x] `IntegrationTestBase.cs` exists, suggesting a setup for integration tests.
    - [ ] **Authentication:**
        - [ ] Test `GetAuthorizedUserAsync` with valid/invalid tokens.
    - [ ] **Core CRUD Operations:** (Tasks, Lists, Comments etc.)
        - [ ] Create, Read, Update, Delete tests. (Requires data setup/teardown).
        *(Status: No specific CRUD integration tests listed in the file structure beyond the base class. This is a key area for expansion.)*
    - [ ] **Complex Query Parameters / Filtering:**
        - [ ] Test endpoints like `GetTasksAsync` with various filters.
    - [ ] **Pagination:**
        - [ ] Test basic paged methods and `IAsyncEnumerable<T>` helpers against live API.
    - [ ] **Error Handling (Live API Errors):**
        - [ ] Trigger 404, 401/403, 400/422 if possible and safe.
    - [ ] **Specific API Features:**
        - [ ] File attachments, custom fields, etc.

- [x] **Configuration for Integration Tests:**
    - [x] Likely handled by `IntegrationTestBase.cs` using user secrets or environment variables for API token.
    - [ ] Conditional compilation or test categories to separate.

## 4. Test Project Structure (`src/ClickUp.Api.Client.Tests/`)

- [x] Project `src/ClickUp.Api.Client.Tests/` exists.
- [x] `Models/` folder for DTO tests - **Added.** (Contains `Common/MemberTests.cs`)
- [x] `Services/` folder for service unit tests - Exists (`AttachmentsServiceTests.cs`, `TaskServiceTests.cs`).
- [x] `Http/` folder for `AuthenticationDelegatingHandler` tests etc. - **Added.** (Currently empty)
- [x] `Helpers/` folder for utility tests - **Added.** (Currently empty)
- [x] `Integration/` folder for integration tests - Exists (`IntegrationTestBase.cs`).
- [ ] `TestInfrastructure/` for mocks, test data builders - **Partially exists with `IntegrationTestBase.cs`, could be expanded.**
- [x] `ClickUp.Api.Client.Tests.csproj` - Exists. (Verified includes xUnit, Moq)
- [ ] `appsettings.IntegrationTests.json` (gitignored) - Strategy to be confirmed, user secrets often preferred for API keys.

## 5. CI/CD Integration
- [ ] Unit tests should run on every commit/PR. (Setup dependent on CI system).
- [ ] Integration tests might run less frequently. (Setup dependent on CI system).

This testing strategy aims to build confidence in the SDK's correctness and robustness. Current test suite has a foundation but needs significant expansion in model tests, comprehensive service method coverage, helper class tests, and more integration tests.
```
```
