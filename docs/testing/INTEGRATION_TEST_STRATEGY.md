# ClickUp API Client - Integration Test Strategy

This document outlines the strategy for conducting integration tests for the `ClickUp.Api.Client.Net` SDK.

## 1. Objective

The primary objective of integration tests is to verify that the SDK interacts correctly with the live ClickUp API. This includes:
- Ensuring proper request serialization and endpoint construction.
- Verifying successful response deserialization into the defined DTOs.
- Testing the end-to-end flow of operations, including authentication and basic error handling against the live API.
- Providing confidence that the SDK functions as expected in a real-world scenario.

## 2. Test Environment

- **Dedicated ClickUp Workspace:** A dedicated, non-production ClickUp Workspace should be used exclusively for integration testing.
    - This workspace should not contain any critical or production data.
    - It can be a free tier workspace or a paid one specifically allocated for development and testing.
- **SDK Testing Only:** This workspace is solely for the purpose of testing the SDK. Data within it may be created, modified, and deleted frequently by the tests.

## 3. Test Data Management

- **Prerequisite Data:**
    - Some tests may require specific prerequisite data to exist in the ClickUp test workspace (e.g., a specific Space, List, Folder, or Task to act upon).
    - This data can be created manually initially.
    - For more robust and repeatable tests, consider scripting the setup of this prerequisite data (e.g., using the SDK itself in a setup phase, or via ClickUp's UI/another tool if simpler for one-time setup).
- **Data Cleanup:**
    - After tests run, created resources should ideally be cleaned up to maintain a consistent test environment state.
    - Cleanup can be performed manually or, preferably, scripted as part of a test tear-down phase (e.g., deleting created tasks or lists).
- **Idempotency:**
    - Tests should be designed to be as idempotent as possible. This means a test can be run multiple times without changing the outcome or negatively impacting subsequent test runs.
    - For example, instead of relying on a specific count of items, tests should create their own data, operate on it, and then (ideally) clean it up. If cleanup isn't perfect, tests should be resilient to pre-existing data from previous runs (e.g., by using unique names/identifiers for created resources where possible).

## 4. API Token Management

- **Security:** **NEVER** check API tokens (Personal Access Tokens or OAuth refresh tokens) into source control.
- **Local Development:**
    - **User Secrets:** For local development, the .NET User Secrets manager is recommended for storing the Personal Access Token of a dedicated test user in the ClickUp test workspace.
    - The test project would be configured to load these secrets.
- **CI/Other Environments:**
    - **Environment Variables:** For Continuous Integration (CI) servers or other shared environments, API tokens should be configured as environment variables (e.g., `CLICKUP_TEST_PERSONAL_ACCESS_TOKEN`).
- **Loading Tokens:**
    - The test project will use `Microsoft.Extensions.Configuration` (e.g., `ConfigurationBuilder`) to load configuration from user secrets (for local) and environment variables (for CI).
    - This configuration will then be used to populate `ClickUpClientOptions` when setting up services for integration tests.

## 5. Scope of Integration Tests

- **Key CRUD Operations:** Focus on testing the "happy path" for core Create, Read, Update, Delete (CRUD) operations for major resources:
    - Tasks
    - Lists
    - Folders
    - Spaces
    - Comments
    - Goals
    - (Others as deemed critical)
- **Authentication:** Verify that authentication (initially Personal API Token) works correctly, allowing access to protected resources.
- **Basic Error Handling:** Test a few scenarios where the API is expected to return errors, and verify that the SDK throws the appropriate custom exceptions.
    - Example: Requesting a non-existent resource to check for `ClickUpApiNotFoundException`.
    - Example: Attempting an action that should cause a validation error to check for `ClickUpApiValidationException` (if predictable).
- **Pagination:** For one or two key services that support pagination and have `IAsyncEnumerable` helpers (e.g., `TaskService.GetTasksAsyncEnumerableAsync`), test that multiple pages can be retrieved.
- **Not Exhaustive:** Integration tests will not aim for 100% coverage of every API endpoint, parameter combination, or edge case. This level of detail is better suited for unit tests (mocking the API) or would require an excessively complex and slow integration test suite. The goal is to cover representative and critical paths.

## 6. Running Integration Tests

- **Slower Execution:** Integration tests are inherently slower than unit tests due to network latency and dependency on the external ClickUp API.
- **External Dependency:** Test failures can occur due to ClickUp API availability issues, network problems, or changes in the API itself, not just SDK bugs.
- **Categorization:**
    - Use xUnit Traits (e.g., `@Trait("Category", "Integration")`) to distinguish integration tests from unit tests.
    - This allows them to be run separately.
- **Execution Frequency:**
    - Run integration tests less frequently than unit tests. For example:
        - Manually on a developer's machine before committing significant changes.
        - As part of a nightly build or a dedicated integration build in CI, rather than on every commit.

## 7. Tooling

- **Test Framework:** xUnit will be used as the primary test framework.
- **Assertion Library:** FluentAssertions will be used for expressive and readable assertions.
- **Service Setup:** The `ServiceCollectionExtensions.AddClickUpClient` method from the main client library will be used to set up the necessary services (`IApiConnection`, specific service interfaces) for the integration tests. This ensures tests use the same DI setup as a consuming application would.
- **Configuration:** `Microsoft.Extensions.Configuration.Json`, `Microsoft.Extensions.Configuration.UserSecrets`, and `Microsoft.Extensions.Configuration.EnvironmentVariables` will be used to load API tokens and other test settings.
- **HTTP Client:** The same `HttpClient` (configured via `IHttpClientFactory` by `AddClickUpClient`) and `IApiConnection` implementation used by the SDK will be used by the tests, ensuring policies like retry and circuit breaker are also engaged during tests.

This strategy aims to provide a balanced approach to integration testing, ensuring key functionalities are verified against the live API without creating an overly burdensome or brittle test suite.
