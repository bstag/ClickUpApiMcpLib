# Integration Test Data Setup and Teardown Strategy

This document outlines the strategy for managing test data for integration tests against the ClickUp API. Reliable and consistent test data is crucial for ensuring the stability and correctness of the SDK's integration with the live ClickUp environment.

## Guiding Principles

1.  **Isolation:** Tests should be isolated and not depend on the state left by other tests.
2.  **Repeatability:** Tests should produce the same results when run multiple times with the same initial conditions.
3.  **Cleanliness:** The ClickUp workspace used for testing should be kept as clean as possible, with test-generated data removed after tests.
4.  **Realism:** Test data should reflect realistic usage scenarios where possible.
5.  **Security:** API keys and other sensitive information must be handled securely.
6.  **Efficiency:** Setup and teardown processes should be reasonably fast to avoid long test execution times.

## Core Strategies

### 1. Dedicated Test Workspace/Account

*   **Strategy:** Utilize a dedicated ClickUp Workspace specifically for running integration tests. This workspace should not be used for any production or critical development work.
*   **Pros:**
    *   Maximizes isolation from real user data.
    *   Simplifies cleanup as the entire workspace's contents can be considered ephemeral or test-related.
    *   Reduces risk of accidental interference with important data.
*   **Cons:**
    *   Requires setting up and maintaining a separate ClickUp account/workspace.
*   **Implementation:**
    *   Obtain a new ClickUp account or use an existing non-critical one.
    *   Create a dedicated Workspace within this account for testing.

### 2. Test Data Lifecycle Management

*   **Strategy:** Each integration test (or test class) should be responsible for creating the specific data it needs and ensuring that data is cleaned up afterwards, even if the test fails.
*   **Methods:**
    *   **Programmatic Setup/Teardown:** Use the SDK itself to create entities (Spaces, Folders, Lists, Tasks, Comments, etc.) before a test run and delete them after.
        *   **Setup:** Implemented in test fixture setup methods (e.g., `IAsyncLifetime.InitializeAsync`, `[CollectionFixture]` setup, or `[ClassInitialize]` methods).
        *   **Teardown:** Implemented in test fixture disposal methods (e.g., `IAsyncLifetime.DisposeAsync`, `IDisposable.Dispose`, or `[ClassCleanup]` methods). Utilize `try...finally` blocks to ensure cleanup happens.
    *   **Unique Naming Conventions:** Use unique prefixes or suffixes (e.g., `test_entity_<timestamp>_<guid>`) for created entities. This can help in:
        *   Identifying test-generated data.
        *   Bulk cleanup scripts if manual intervention is ever needed.
    *   **Hierarchical Cleanup:** When deleting parent entities (like a Space), ensure the API or SDK handles deletion of child entities (Folders, Lists, Tasks within that Space). If not, cleanup must be done from child to parent. ClickUp API generally handles cascading deletes for Spaces, Folders, and Lists.

### 3. API Key and Configuration Management

*   **Strategy:** Securely manage API keys and other configuration details (like Workspace ID, Space ID for testing).
*   **Implementation:**
    *   **Environment Variables:** Store API keys and sensitive configuration (e.g., `CLICKUP_TEST_API_TOKEN`, `CLICKUP_TEST_WORKSPACE_ID`) as environment variables on the CI server and local development machines. This is a common and secure practice.
    *   **User Secrets (for local development):** .NET's User Secrets manager can be used for local development to keep API keys out of source control.
    *   **Configuration Files (for non-sensitive data):** Non-sensitive configuration like default list names or test parameters can be in `appsettings.testing.json` (gitignored if it ever contains template sensitive data, or use a template approach).
    *   **Test Base Class/Fixture:** A base class or collection fixture for integration tests should be responsible for loading this configuration.

### 4. Handling Rate Limiting

*   **Strategy:** Be mindful of ClickUp API rate limits during test setup, execution, and teardown, as these phases can involve numerous API calls.
*   **Implementation:**
    *   **Efficient API Usage:** Create only necessary data. Avoid excessive polling or redundant calls.
    *   **Polly Policies:** The SDK's built-in Polly policies for rate limiting (retry-after, exponential backoff) should help mitigate issues during test execution. Ensure these are active and configured appropriately for the test environment if necessary.
    *   **Staggered Setup/Teardown:** If creating/deleting a very large number of entities, introduce small delays between operations if rate limiting becomes an issue, though this should be a last resort.
    *   **Monitor `Retry-After` Headers:** The SDK (and thus tests using it) should respect `Retry-After` headers.

### 5. Stable vs. Dynamic Data

*   **Strategy:** Differentiate between data that can be relatively stable and data that must be dynamic for each test run.
    *   **Stable Base Data:** A pre-existing Test Space or even a Test Folder/List could be manually created and its ID configured for tests that don't modify it heavily or can clean up within it. This can save setup time.
        *   *Caution:* If tests modify this stable data, they MUST reliably revert it to its original state.
    *   **Dynamic Data:** For most CRUD operations, entities should be created and deleted per test or per test class to ensure isolation. For example, a test for "creating a task" should create a new list (or use a freshly created one for the test class) and then delete it.

### 6. Test Categories / Conditional Execution

*   **Strategy:** Separate integration tests from unit tests so they can be run on demand, especially in CI environments where they might be slower or require specific configurations.
*   **Implementation:**
    *   **xUnit Test Traits:** Use `[Trait("Category", "Integration")]` to mark integration tests.
    *   **Separate Test Project (Optional):** Could have a `ClickUp.Api.Client.IntegrationTests.csproj` if separation needs to be stricter.
    *   **CI Configuration:** Configure the CI pipeline to run integration tests based on specific triggers (e.g., nightly, on release branches, or manually triggered) using the test category filter.

## 7. Recorded Response Testing (Mocking Strategy)

To improve test stability, reduce reliance on the live API (avoiding rate limits and plan restrictions), and enable offline testing, a recorded response strategy is implemented. This allows tests to run against pre-recorded JSON responses from the API.

### 7.1 Test Modes

Integration tests can be run in one of three modes, controlled by the `CLICKUP_SDK_TEST_MODE` environment variable:

*   **`Passthrough` (Default):** Tests make live API calls. No recording or playback occurs. This is useful for verifying compatibility with the actual API.
*   **`Record`:** Tests make live API calls. The responses from GET, POST, PUT, DELETE requests are automatically saved as JSON files. This mode is used to capture the data needed for playback.
*   **`Playback`:** Tests do *not* make live API calls. Instead, they use the `RichardSzalay.MockHttp` library to serve responses from previously recorded JSON files. This is the primary mode for CI and frequent local testing.

The `IntegrationTestBase` class handles the configuration of these modes.

### 7.2 Recording Process and File Structure

*   **Handler:** `RecordingDelegatingHandler.cs` is an `HttpMessageHandler` that intercepts outgoing requests in `Record` mode.
*   **File Location:** Recorded JSON files are saved to:
    `src/ClickUp.Api.Client.IntegrationTests/test-data/recorded-responses/{ServiceName}/{MethodName}/{ScenarioName}_{queryhash_for_get}.json`
    *   `{ServiceName}`: e.g., `AuthorizationService`, `SpaceService`. Derived from the request URI.
    *   `{MethodName}`: e.g., `GetAuthorizedUser`, `CreateSpace`, `GetSpaces`. Derived from HTTP method and request URI.
    *   `{ScenarioName}`: Initially `RecordedResponse`. This should be manually renamed by the developer to something descriptive like `Success`, `NotFound`, `SpecificCondition`, etc. For GET requests with query parameters, a short hash of the query string is appended to `RecordedResponse` before the `.json` extension to differentiate between calls to the same endpoint with different queries (e.g., `RecordedResponse_a1b2c3d4.json`).
*   **Content:** JSON files store only the raw API response body. Status codes and headers are defined in the test setup code when configuring the mock response.

### 7.3 Converting Tests to Use Playback Mode

1.  **Set Record Mode:** Set the environment variable `CLICKUP_SDK_TEST_MODE=Record`.
2.  **Run Tests:** Execute the integration test(s) you want to convert. This will generate JSON files in the `test-data/recorded-responses` directory according to the structure above.
3.  **Verify and Rename JSON Files:**
    *   Locate the generated `RecordedResponse...json` files.
    *   Rename them to meaningful scenario names (e.g., `GetSpace_Success.json`, `CreateTask_ValidInput_Success.json`). This makes tests easier to understand and maintain.
4.  **Edit JSON (If Necessary):**
    *   **Placeholder IDs:** For complex tests involving creation of multiple dependent entities (e.g., create a space, then a folder in it, then a list), you might need to edit the recorded JSONs to use consistent placeholder IDs (e.g., `"space_id": "mockSpace123"`) across related files. The test code would then also use these placeholder IDs when setting up mocks and making assertions. This ensures data consistency in `Playback` mode.
    *   **Model Corrections:** If a model mismatch was identified and corrected *after* recording, the JSON might need to be updated to reflect the corrected model structure for successful deserialization during playback.
5.  **Modify Test Method for Playback:**
    *   Ensure your test class inherits from `IntegrationTestBase`.
    *   In the test method, check `if (CurrentTestMode == TestMode.Playback)`.
    *   Inside this block, use `MockHttpHandler` (available from `IntegrationTestBase`) to set up expected requests and their corresponding responses from the renamed JSON files.
        ```csharp
        if (CurrentTestMode == TestMode.Playback)
        {
            Assert.NotNull(MockHttpHandler); // From IntegrationTestBase
            var responsePath = Path.Combine(RecordedResponsesBasePath, "ServiceName", "MethodName", "ScenarioName.json");
            var responseContent = await File.ReadAllTextAsync(responsePath);

            MockHttpHandler.When("https://api.clickup.com/api/v2/expected/path")
                           .Respond(HttpStatusCode.OK, "application/json", responseContent);

            // Mock other calls if the test makes multiple requests
        }
        // ... rest of your test logic ...
        ```
    *   The `RecordedResponsesBasePath` property from `IntegrationTestBase` provides the root path to `test-data/recorded-responses/`.
6.  **Test in Playback Mode:** Set `CLICKUP_SDK_TEST_MODE=Playback` (or leave it unset if your local/CI default is Playback) and run the test. It should now pass using the mocked data.
7.  **Full Coverage:** For a test to be truly runnable offline and independent of the live API, *all* HTTP interactions it triggers (including setup in `InitializeAsync` or constructors, and teardown in `DisposeAsync`) must be mocked in `Playback` mode.

### 7.4 Handling Dynamic Data and IDs

*   **During Recording:** Live IDs are captured.
*   **During Playback:**
    *   **Option 1 (Use Recorded IDs):** If the test logic can work with the exact IDs captured during recording, no changes to JSON IDs are needed. This is simpler for GET requests.
    *   **Option 2 (Placeholder IDs):** For tests involving creation (POST/PUT) and subsequent actions on the created entity, it's more robust to:
        1.  Record the creation response.
        2.  Edit the JSON to use a known placeholder ID (e.g., "mock-task-id-123").
        3.  Modify the test to expect this placeholder ID from the mocked creation step.
        4.  Ensure any subsequent recorded JSON files for operations on this entity also use this placeholder ID.
    *   This ensures that if the live API generates different IDs on a new recording session, the playback tests using placeholder IDs remain consistent.

## Example Workflow for a Test Class (e.g., `TaskServiceIntegrationTests`) using Recorded Responses

1.  **Fixture Setup (`IAsyncLifetime.InitializeAsync` or similar):**
    *   Load API token and Workspace ID (still needed if some setup calls are live during `Record` mode, or if `Passthrough` mode is used).
    *   If in `Playback` mode, this section might also set up mocks for common setup operations (e.g., creating a prerequisite space/list if these are not part of the per-test recording).
    *   The `TestHierarchyHelper` can be used, and its interactions would also need to be mocked in `Playback` mode.

2.  **Test Method (e.g., `CreateTask_ValidData_ReturnsTask`):**
    *   **(Record Mode):** Run test, `RecordingDelegatingHandler` saves `POST .../task` response. Rename it (e.g., `CreateTask_Success.json`).
    *   **(Playback Mode Setup):**
        ```csharp
        if (CurrentTestMode == TestMode.Playback)
        {
            var createTaskResponsePath = Path.Combine(RecordedResponsesBasePath, "TaskService", "CreateTask", "CreateTask_Success.json");
            var createTaskContent = await File.ReadAllTextAsync(createTaskResponsePath);
            // Assume _testListId is known or also mocked if hierarchy setup is mocked
            MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/list/{_testListId}/task")
                           .Respond(HttpStatusCode.OK, "application/json", createTaskContent);
        }
        ```
    *   **Action:** Call the SDK method to create a task (`var createdTask = await _taskService.CreateTaskAsync(...)`).
    *   **Assertions:** Perform assertions on `createdTask`. The properties of `createdTask` will come from the `CreateTask_Success.json` in `Playback` mode.

3.  **Fixture Teardown (`IAsyncLifetime.DisposeAsync` or similar):**
    *   If entities were created, they need to be deleted.
    *   **(Record Mode):** `RecordingDelegatingHandler` saves `DELETE .../task/{id}` response. Rename (e.g., `DeleteTask_Success.json`).
    *   **(Playback Mode Setup):** Mocks for these DELETE operations need to be added, typically responding with `HttpStatusCode.NoContent` or an empty body. These mocks might be general or specific to IDs used in the test.

## Future Considerations

*   **Test Data Builders/Factories:** For complex objects, implement builder patterns to create test data DTOs easily, both for request creation and for constructing expected objects in assertions.
*   **Snapshot Testing:** For complex response objects, consider snapshot testing approaches (less applicable with this explicit JSON file strategy but could complement it for verifying deserialized objects).
*   **Automated Renaming/Organization:** Future enhancements could involve passing scenario context from tests to the `RecordingDelegatingHandler` to automate more of the scenario naming.

This strategy will be refined as integration tests are developed and more is learned about the practicalities of interacting with the ClickUp API in a test context.
