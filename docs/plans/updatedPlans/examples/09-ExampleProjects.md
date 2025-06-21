# Detailed Plan: Showcase Example Projects

This document outlines the plan for developing showcase example projects that demonstrate the usage of the ClickUp API SDK.

**Source Documents:**
*   [`docs/plans/NEW_OVERALL_PLAN.md`](../NEW_OVERALL_PLAN.md) (Phase 3, Step 12)
*   Other detailed plan documents for features to be showcased (Auth, Pagination, Error Handling, etc.).

**Location in Codebase:**
*   `examples/ClickUp.Api.Client.Console/` - Exists.
*   `examples/ClickUp.Api.Client.Worker/` - Exists.

## 1. General Goals for Example Projects

- [x] **Practical Demonstrations:** Show how to perform common operations. (Goal)
- [x] **Best Practices:** Illustrate recommended ways to initialize, configure, and use. (Goal)
- [x] **Feature Highlights:** Showcase key SDK features. (Goal)
- [x] **Clarity and Simplicity:** Examples should be easy to understand. (Goal)
- [x] **Runnable:** Examples should be complete and runnable. (Goal, current examples are skeletons).

## 2. Configuration Management for Examples

- [ ] **API Token:**
    - [ ] Both examples will require a ClickUp Personal API Token.
    - [x] Token **MUST NOT** be hardcoded.
    - [ ] Use `Microsoft.Extensions.Configuration`. (Partially set up in Worker, Console is basic).
    - [ ] Provide `appsettings.template.json` or `secrets.template.json`. - **Missing for both.**
    - [x] Actual `appsettings.json` or `secrets.json` should be in `.gitignore`. (Assumed standard practice, .gitignore not checked here).
    - [ ] Instructions in README for configuration. - **READMEs for examples missing.**
    - [ ] Example `appsettings.template.json` provided in plan.
- [ ] **Other Configuration:** Workspace IDs, List IDs, etc. via configuration.

## 3. `examples/ClickUp.Api.Client.Console`

**Purpose:** A simple console application to demonstrate basic SDK usage.
*(Current status: `Program.cs` is a basic "Hello, World!".)*

- [x] **Project Setup:**
    - [x] .NET Console Application - Exists.
    - [ ] PackageReferences:
        - [ ] `ClickUp.Api.Client` (project reference) - **Likely needs to be added.**
        - [ ] `Microsoft.Extensions.Hosting` (for DI and configuration) - **Missing.**
        - [ ] `Microsoft.Extensions.Http.Polly` - **Missing.**
        - [ ] `Serilog.Extensions.Hosting` and `Serilog.Sinks.Console` - **Missing.**

- [ ] **Scenarios to Demonstrate:** (All scenarios are pending implementation)
    - [ ] **1. Initialization and Authentication:**
        - [ ] Configure `ClickUpApiClientOptions`.
        - [ ] Show `IServiceCollection.AddClickUpApiClient(...)` setup.
        - [ ] Inject and use a service (e.g., `IUsersService`).
        - [ ] Example: Fetch authorized user.
    - [ ] **2. Basic CRUD Operations (e.g., Tasks):**
        - [ ] Requires configured `ListId`.
        - [ ] Create, Read, Update, Delete Task examples.
    - [ ] **3. Listing Resources:**
        - [ ] Example: Get Lists in a Folder.
    - [ ] **4. Pagination Demonstration:**
        - [ ] Use `IAsyncEnumerable<T>` helper with `await foreach`.
    - [ ] **5. Error Handling:**
        - [ ] Catch specific `ClickUpApiException`s (e.g., `ClickUpApiNotFoundException`).
    - [ ] **6. Using Specific Service Methods:**
        - [ ] Showcase 1-2 other distinct service methods.

- [ ] **Console Output:** (Pending implementation)
    - [ ] Clear, informative messages.
    - [ ] Display relevant data.
    - [ ] Format exception info.

- [ ] **Structure:** (Pending implementation)
    - [ ] `Program.cs` for host setup & orchestration.
    - [ ] Separate classes/methods for scenarios.

## 4. `examples/ClickUp.Api.Client.Worker`

**Purpose:** A .NET Worker Service for long-running/background tasks.
*(Current status: Basic Worker Service template. `Program.cs` sets up `Worker`. `Worker.cs` logs a message every second. `appsettings.json` is basic.)*

- [x] **Project Setup:**
    - [x] .NET Worker Service project template - Exists.
    - [ ] PackageReferences:
        - [ ] `ClickUp.Api.Client` (project reference) - **Likely needs to be added.**
        - [ ] `Microsoft.Extensions.Hosting` - Exists (part of worker template).
        - [ ] `Microsoft.Extensions.Http.Polly` - **Missing.**
        - [ ] `Serilog.Extensions.Hosting` / `Serilog.Sinks.Console` (or other logger) - Basic MS logging is present.

- [ ] **Scenarios to Demonstrate:** (All scenarios are pending implementation within the worker logic)
    - [ ] **1. Initialization and Configuration:**
        - [ ] DI setup for `AddClickUpClient` in `Program.cs`.
        - [ ] API token configured via `appsettings.json` (needs `ClickUpApiOptions` section).
    - [ ] **2. Periodic Polling Example (e.g., Check for New Tasks):**
        - [ ] Implement in `Worker.cs` or a new `BackgroundService`.
        - [ ] Inject `ITasksService`, `ILogger`.
        - [ ] Loop with delay, fetch tasks using filters (e.g., `date_created_gt`).
        - [ ] Log new tasks, handle errors.
    - [ ] **3. (Optional) Processing Items from a Paginated Endpoint:**
        - [ ] Another `BackgroundService` using `IAsyncEnumerable<T>`.
    - [ ] **4. Demonstrating `CancellationToken` Usage:**
        - [ ] Ensure `ExecuteAsync` responds to `stoppingToken`.
        - [ ] Pass `stoppingToken` to SDK calls. (Partially done in skeleton `Worker.cs` for `Task.Delay`).

- [ ] **Worker Output:** (Pending implementation of actual SDK usage)
    - [ ] Structured logging showing activity.

## 5. README Files for Examples

- [ ] Each example project should have its own `README.md`. - **Missing for both.**
    - [ ] Description, Prerequisites.
    - [ ] Configuration steps (copy template, fill API key/IDs).
    - [ ] How to run.
    - [ ] Expected output.

**Overall Status:** The example project skeletons (`ClickUp.Api.Client.Console` and `ClickUp.Api.Client.Worker`) exist. However, they are very basic and do not yet implement any of the planned scenarios demonstrating SDK usage, configuration, or features. Key tasks like setting up proper configuration, adding SDK references, implementing DI, and writing the actual demonstration logic are all pending. README files with setup and run instructions are also missing.
```
```
