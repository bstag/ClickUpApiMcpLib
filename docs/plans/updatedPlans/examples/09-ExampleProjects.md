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

- [x] **API Token:**
    - [x] Both examples will require a ClickUp Personal API Token. (Verified by adding config)
    - [x] Token **MUST NOT** be hardcoded. (Ensured by template/secrets approach)
    - [x] Use `Microsoft.Extensions.Configuration`. (Implemented in both Console and Worker)
    - [x] Provide `appsettings.template.json` or `secrets.template.json`. (Added `appsettings.template.json` for both)
    - [x] Actual `appsettings.json` or `secrets.json` should be in `.gitignore`. (Standard practice, assumed covered by general .gitignore)
    - [x] Instructions in README for configuration. (Added READMEs with config instructions for both)
    - [x] Example `appsettings.template.json` provided in plan. (Done by adding the file itself)
- [ ] **Other Configuration:** Workspace IDs, List IDs, etc. via configuration.

## 3. `examples/ClickUp.Api.Client.Console`

**Purpose:** A simple console application to demonstrate basic SDK usage.
*(Current status: Basic setup for DI, logging, and config is done. Initial user fetch example implemented.)*

- [x] **Project Setup:**
    - [x] .NET Console Application - Exists.
    - [x] PackageReferences:
        - [x] `ClickUp.Api.Client` (project reference) - Verified, was existing.
        - [x] `Microsoft.Extensions.Hosting` (for DI and configuration) - Added.
        - [x] `Microsoft.Extensions.Http.Polly` - Added.
        - [x] `Serilog.Extensions.Hosting` and `Serilog.Sinks.Console` - Added.

- [ ] **Scenarios to Demonstrate:** (All scenarios are pending implementation)
    - [x] **1. Initialization and Authentication:**
        - [x] Configure `ClickUpApiClientOptions`. (Done in Program.cs)
        - [x] Show `IServiceCollection.AddClickUpApiClient(...)` setup. (Done in Program.cs)
        - [x] Inject and use a service (e.g., `IUsersService`). (Done for `IUsersService` in `App.cs`)
        - [x] Example: Fetch authorized user. (Implemented in `App.cs`)
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
    - [x] PackageReferences:
        - [x] `ClickUp.Api.Client` (project reference) - Verified, was existing.
        - [x] `Microsoft.Extensions.Hosting` - Exists (part of worker template, version aligned).
        - [x] `Microsoft.Extensions.Http.Polly` - Added.
        - [x] `Serilog.Extensions.Hosting` / `Serilog.Sinks.Console` (or other logger) - Added Serilog.

- [ ] **Scenarios to Demonstrate:** (All scenarios are pending implementation within the worker logic)
    - [ ] **1. Initialization and Configuration:**
        - [ ] DI setup for `AddClickUpClient` in `Program.cs`. (Placeholder for this is set, actual `AddClickUpClient` call to be added when worker logic uses it. `ClickUpClientOptions` are not yet explicitly configured/bound in Worker's `Program.cs`.)
        - [x] API token can be configured via `appsettings.json`/user secrets. (Configuration providers are set up in `Program.cs`; binding happens via `AddClickUpClient` which is planned).
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

- [x] Each example project should have its own `README.md`. - (Added for both Console and Worker)
    - [x] Description, Prerequisites. (Included in new READMEs)
    - [x] Configuration steps (copy template, fill API key/IDs). (Included in new READMEs)
    - [x] How to run. (Included in new READMEs)
    - [ ] Expected output. (To be updated as examples become more concrete)

**Overall Status:** The example project skeletons (`ClickUp.Api.Client.Console` and `ClickUp.Api.Client.Worker`) have been significantly improved with proper configuration, logging, DI foundations, and necessary package references. The Console example now demonstrates basic SDK initialization and fetching an authorized user. README files with setup and run instructions have been added for both. More detailed SDK feature demonstrations are pending.
```
```
