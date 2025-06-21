# Detailed Plan: Showcase Example Projects

This document outlines the plan for developing showcase example projects that demonstrate the usage of the ClickUp API SDK.

**Source Documents:**
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 3, Step 12)
*   Other detailed plan documents for features to be showcased (Auth, Pagination, Error Handling, etc.).

**Location in Codebase:**
*   `examples/ClickUp.Api.Client.Console/`
*   `examples/ClickUp.Api.Client.Worker/`

## 1. General Goals for Example Projects

*   **Practical Demonstrations:** Show how to perform common operations using the SDK.
*   **Best Practices:** Illustrate recommended ways to initialize, configure, and use the SDK.
*   **Feature Highlights:** Showcase key SDK features like authentication, pagination, error handling, and specific service interactions.
*   **Clarity and Simplicity:** Examples should be easy to understand and adapt.
*   **Runnable:** Examples should be complete and runnable with minimal setup (primarily API token configuration).

## 2. Configuration Management for Examples

*   **API Token:**
    *   Both examples will require a ClickUp Personal API Token to run.
    *   This token **MUST NOT** be hardcoded.
    *   Use `Microsoft.Extensions.Configuration` to load settings.
    *   Provide a `appsettings.template.json` or `secrets.template.json` file that users can copy and fill in with their token. This template file will be checked into source control.
    *   The actual `appsettings.json` or `secrets.json` (containing the token) should be in `.gitignore`.
    *   Instructions in the README of each example project will guide users on how to set up their configuration.
    *   Example `appsettings.template.json`:
        ```json
        {
          "ClickUpApiOptions": {
            "PersonalApiKey": "YOUR_CLICKUP_PERSONAL_API_KEY_HERE",
            "WorkspaceIdForSomeExamples": "YOUR_TEST_WORKSPACE_ID_HERE" // Optional, for specific examples
          }
        }
        ```
*   **Other Configuration:** Workspace IDs, List IDs, etc., needed for specific examples can also be managed via configuration.

## 3. `examples/ClickUp.Api.Client.Console`

**Purpose:** A simple console application to demonstrate basic SDK usage and one-off operations.

**Project Setup:**
*   .NET Console Application.
*   PackageReferences:
    *   `ClickUp.Api.Client` (project reference)
    *   `Microsoft.Extensions.Hosting` (for DI and configuration)
    *   `Microsoft.Extensions.Http.Polly` (if not already pulled by client for DI setup)
    *   `Serilog.Extensions.Hosting` and `Serilog.Sinks.Console` (for simple console logging).

**Scenarios to Demonstrate:**

1.  **Initialization and Authentication:**
    *   Show how to configure `ClickUpApiClientOptions` (API key).
    *   Show `IServiceCollection.AddClickUpApiClient(...)` setup.
    *   Inject a service (e.g., `IAuthorizationService` or `IUsersService`).
    *   Example: Fetch and display authorized user details.

2.  **Basic CRUD Operations (e.g., Tasks):**
    *   Requires a configured `ListId` in `appsettings.json`.
    *   **Create Task:** Create a new task in the specified list. Log the created task's ID and name.
    *   **Read Task:** Fetch the newly created task by its ID. Display its details.
    *   **Update Task:** Update the task's name or description. Fetch and display again to show changes.
    *   **Delete Task:** Delete the task. Attempt to fetch it again to confirm it's gone (expect `ClickUpApiNotFoundException`).

3.  **Listing Resources (e.g., Lists in a Folder, Tasks in a List):**
    *   Requires configured `FolderId` or `SpaceId`.
    *   Example: Get all Lists within a specific Folder (or Folderless Lists in a Space).
    *   Iterate and display basic info (ID, Name).

4.  **Pagination Demonstration:**
    *   Target an endpoint known to have multiple pages of data (e.g., `GetTasksAsync` in a list with many tasks, or `GetCommentsAsync`).
    *   Show usage of the `IAsyncEnumerable<T>` helper method with `await foreach` to iterate through all items seamlessly.
    *   Log a count or a subset of the retrieved items.

5.  **Error Handling:**
    *   Demonstrate catching specific `ClickUpApiException`s.
    *   Example 1: Try to fetch a task with a clearly invalid/non-existent ID to trigger `ClickUpApiNotFoundException`.
    *   Example 2 (if feasible without complex setup): Attempt an action that might be forbidden (though this is harder to set up reliably without specific test user roles).
    *   Log exception details (`HttpStatus`, `ApiErrorCode`, `Message`).

6.  **Using Specific Service Methods:**
    *   Pick 1-2 other distinct service methods to showcase variety, e.g.:
        *   Adding a comment to a task.
        *   Getting details of a Space.

**Console Output:**
*   Clear, informative messages about what the example is doing.
*   Display relevant data fetched or IDs of created resources.
*   Properly format exception information when demonstrating error handling.

**Structure:**
*   `Program.cs` will set up the host, DI, and orchestrate calls to different example methods.
*   Separate classes or static methods for each scenario (e.g., `TaskExamples.cs`, `ErrorHandlingExamples.cs`).

## 4. `examples/ClickUp.Api.Client.Worker`

**Purpose:** A .NET Worker Service to demonstrate long-running or background tasks using the SDK, such as polling for changes or periodic processing.

**Project Setup:**
*   .NET Worker Service project template.
*   PackageReferences: Same as Console example.

**Scenarios to Demonstrate:**

1.  **Initialization and Configuration:**
    *   Similar DI setup as the console app in `Program.cs`.
    *   API token configured via `appsettings.json`.

2.  **Periodic Polling Example (e.g., Check for New Tasks):**
    *   Implement a `BackgroundService` (e.g., `NewTaskMonitorWorker`).
    *   Inject an `ITasksService` and `ILogger`.
    *   In `ExecuteAsync`:
        *   Loop with a configurable delay (e.g., every 1 minute).
        *   Fetch tasks from a specific list (configured `ListId`).
        *   Use a filter like `date_created_gt` to find tasks created since the last check. Store the timestamp of the newest task from the last successful poll.
        *   Log any new tasks found.
        *   Demonstrate robust error handling within the loop (e.g., log errors but continue polling).

3.  **(Optional) Processing Items from a Paginated Endpoint:**
    *   Another `BackgroundService` that uses an `IAsyncEnumerable<T>` helper to process all items from a paginated endpoint (e.g., all tasks in a list).
    *   For each item, perform a simple simulated action and log it.
    *   This demonstrates processing large datasets without manual page management.

4.  **Demonstrating `CancellationToken` Usage:**
    *   Ensure the `ExecuteAsync` methods of worker services correctly respond to cancellation requests passed via the `stoppingToken`.
    *   Pass the `stoppingToken` to SDK service method calls.

**Worker Output:**
*   Structured logging to the console (or other configured sinks) showing worker activity, tasks processed, and any errors encountered.

## 5. README Files for Examples

Each example project (`ClickUp.Api.Client.Console` and `ClickUp.Api.Client.Worker`) will have its own `README.md` file with:
*   A brief description of the example project's purpose.
*   Prerequisites (e.g., .NET SDK version).
*   Configuration steps:
    *   How to copy `appsettings.template.json` to `appsettings.json`.
    *   Instructions to fill in `PersonalApiKey` and any other required IDs (like `WorkspaceIdForSomeExamples`, `ListIdForTaskExamples`).
*   How to run the example.
*   Expected output or behavior for key scenarios.

This plan ensures that the example projects are practical, informative, and help developers get started quickly with the ClickUp API SDK.
```
