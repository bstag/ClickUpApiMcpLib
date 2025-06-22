# Consolidated ClickUp SDK Implementation Plan

This plan consolidates uncompleted items from the 11 detailed plan documents in `docs/plans/updatedPlans/`.

## Phase 1: Core Service Functionality & Enhancements

### 1. Complete Core Service Method Implementations
- [x] **Task:** Implement `GetFilteredTeamTasksAsync` in `TaskService.cs`.
    - *File:* `src/ClickUp.Api.Client/Services/TaskService.cs`
    - *Why:* Core functionality for fetching tasks at a workspace level. Currently throws `NotImplementedException`.
    - *Ref:* `docs/plans/updatedPlans/services/02-ServiceImplementations.md`
- [x] **Task:** Clarify ClickUp API mapping and then correctly implement the `MergeTasksAsync` overloads in `TaskService.cs` and `ITasksService.cs`.
    - *Files:* `src/ClickUp.Api.Client/Services/TaskService.cs`, `src/ClickUp.Api.Client.Abstractions/Services/ITasksService.cs`
    - *Why:* Ensure task merging functionality is correct and usable; current implementation notes ambiguity.
    - *Ref:* `docs/plans/updatedPlans/services/02-ServiceImplementations.md`
- [x] **Task:** Review all service implementations in `src/ClickUp.Api.Client/Services/` for consistent `ILogger<XxxService>` injection in constructors and basic usage (e.g., logging entry/exit or errors).
    - *Files:* All service files in `src/ClickUp.Api.Client/Services/`
    - *Why:* Improves observability and maintainability, aligns with original plan for constructor dependencies.
    - *Ref:* `docs/plans/updatedPlans/services/02-ServiceImplementations.md`

### 2. Enhance Pagination Helpers
- [x] **Task:** Implement `IAsyncEnumerable<T>` helper methods for cursor-based pagination in `CommentService.cs` (e.g., for `GetTaskCommentsAsync`, `GetChatViewCommentsAsync`, `GetListCommentsAsync`). This involves creating new methods that internally loop, manage `start`/`start_id` cursors, and `yield return` items.
    - *File:* `src/ClickUp.Api.Client/Services/CommentService.cs`
    - *Why:* Simplifies consumption of fully paginated comment streams for SDK users.
    - *Ref:* `docs/plans/updatedPlans/helpers/06-PaginationHelpers.md`
- [x] **Task:** Add an `IAsyncEnumerable<T>` wrapper for `GetFilteredTeamTasksAsync` in `TaskService.cs` (once the base method from step 1.1 is implemented).
    - *File:* `src/ClickUp.Api.Client/Services/TaskService.cs`
    - *Why:* Provides a consistent pagination pattern for users.
    - *Ref:* `docs/plans/updatedPlans/helpers/06-PaginationHelpers.md`

### 3. Refine Exception Handling
- [x] **Task:** Implement parsing of detailed validation errors into the `Errors` dictionary of `ClickUpApiValidationException` within `ApiConnection.HandleErrorResponseAsync`. This requires inspecting potential structures of ClickUp API error responses for 400/422 errors.
    - *File:* `src/ClickUp.Api.Client/Http/ApiConnection.cs`
    - *Why:* Provides richer, field-specific error information to the caller for validation failures.
    - *Ref:* `docs/plans/updatedPlans/exceptions/04-ExceptionHandling.md`

## Phase 2: Testing Expansion

### 4. Expand Unit Test Coverage - Models & Helpers
- [x] **Task:** Write unit tests for polymorphic DTOs if any are identified as needing specific testing beyond standard deserialization. (Completed for `CustomFieldValue`)
    - *Files:* New test files in `src/ClickUp.Api.Client.Tests/Models/`
    - *Why:* Ensures correct deserialization of complex model hierarchies.
    - *Ref:* `docs/plans/updatedPlans/testing/07-TestingStrategy.md`
- [x] **Task:** Write unit tests for `AuthenticationDelegatingHandler`. (Tests added for current PAT implementation; OAuth and ClickUpClientOptions support to be added to handler later)
    - *Files:* New test file in `src/ClickUp.Api.Client.Tests/Http/`
    - *Why:* Verifies correct authentication header manipulation.
    - *Ref:* `docs/plans/updatedPlans/testing/07-TestingStrategy.md`
- [ ] **Task:** Write unit tests for `ApiConnection.HandleErrorResponseAsync` by mocking `HttpResponseMessage` to simulate various error scenarios.
    - *Files:* New/existing test file in `src/ClickUp.Api.Client.Tests/Http/`
    - *Why:* Ensures robust and correct error exception generation.
    - *Ref:* `docs/plans/updatedPlans/testing/07-TestingStrategy.md`
- [ ] **Task:** Write unit tests for pagination helper methods (`IAsyncEnumerable<T>`) in services, mocking underlying paged calls and testing different scenarios (no items, single/multiple pages, cancellation, errors).
    - *Files:* `src/ClickUp.Api.Client.Tests/ServiceTests/TaskServiceTests.cs`, new tests in `src/ClickUp.Api.Client.Tests/ServiceTests/CommentServiceTests.cs`, etc.
    - *Why:* Ensures pagination logic is reliable.
    - *Ref:* `docs/plans/updatedPlans/testing/07-TestingStrategy.md`

### 5. Expand Unit Test Coverage - Services
- [ ] **Task:** For all service methods in `src/ClickUp.Api.Client/Services/`, ensure comprehensive unit tests covering:
    - [ ] Verification of correct request construction (URL, query parameters, body serialization).
    - [ ] Simulation of API error cases (testing that appropriate `ClickUpApiException`s are thrown or propagated).
    - [ ] Simulation of network errors and timeouts.
    - [ ] Verification of `CancellationToken` pass-through.
    - *Files:* All files in `src/ClickUp.Api.Client.Tests/ServiceTests/`
    - *Why:* Ensures individual service method logic is correct and robust.
    - *Ref:* `docs/plans/updatedPlans/testing/07-TestingStrategy.md`

### 6. Develop Integration Tests
- [ ] **Task:** Define a strategy for Test Data Setup/Teardown for integration tests.
    - *Why:* Essential for reliable and repeatable integration tests.
    - *Ref:* `docs/plans/updatedPlans/testing/07-TestingStrategy.md` (Conceptual)
- [ ] **Task:** Implement integration tests for:
    - [ ] Authentication (`GetAuthorizedUserAsync` with valid/invalid tokens).
    - [ ] Core CRUD operations for major entities (e.g., Tasks, Lists, Comments).
    - [ ] Endpoints with complex query parameters or filtering.
    - [ ] Paginated methods and `IAsyncEnumerable<T>` helpers against the live API.
    - [ ] Triggering and verifying specific API error responses (404, 401/403, etc.) where safe and possible.
    - *Files:* New test files in `src/ClickUp.Api.Client.Tests/Integration/`
    - *Why:* Verifies the SDK's interaction with the live ClickUp API.
    - *Ref:* `docs/plans/updatedPlans/testing/07-TestingStrategy.md`
- [ ] **Task:** Decide on and implement a strategy for separating integration tests (e.g., test categories or conditional compilation) if they are not to be run with every `dotnet test` execution.
    - *File:* `src/ClickUp.Api.Client.Tests/ClickUp.Api.Client.Tests.csproj` and test files.
    - *Why:* Practical test execution management.
    - *Ref:* `docs/plans/updatedPlans/testing/07-TestingStrategy.md`
- [ ] **Task:** Expand `TestInfrastructure` for reusable test data builders or mock setups if needed.
    - *Files:* New files/folders in `src/ClickUp.Api.Client.Tests/TestInfrastructure/`
    - *Why:* Improves maintainability and readability of tests.
    - *Ref:* `docs/plans/updatedPlans/testing/07-TestingStrategy.md`

## Phase 3: SDK Usability & Advanced Features

### 7. Enhance SDK Configuration & Resilience
- [ ] **Task:** Implement `ClickUpPollyOptions` class and integrate with `IOptions` in `ServiceCollectionExtensions.cs` to make Polly policy parameters (retry count, delays, break duration) configurable.
    - *Files:* New `ClickUpPollyOptions.cs` in `src/ClickUp.Api.Client.Abstractions/Options/`, update `src/ClickUp.Api.Client/Extensions/ServiceCollectionExtensions.cs`.
    - *Why:* Increases flexibility for SDK consumers.
    - *Ref:* `docs/plans/updatedPlans/resilience/05-ResilienceWithPolly.md`
- [ ] **Task:** Enhance Polly retry/circuit breaker policies in `ServiceCollectionExtensions.cs` for specific HTTP 429 handling, respecting `Retry-After` headers if present.
    - *File:* `src/ClickUp.Api.Client/Extensions/ServiceCollectionExtensions.cs`
    - *Why:* More robust and compliant rate limit handling.
    - *Ref:* `docs/plans/updatedPlans/resilience/05-ResilienceWithPolly.md`
- [ ] **Task:** Replace `Console.WriteLine` in Polly `onRetry`/`onBreak`/`onReset` delegates with `ILogger` obtained from the service provider.
    - *File:* `src/ClickUp.Api.Client/Extensions/ServiceCollectionExtensions.cs`
    - *Why:* Adherence to proper logging practices.
    - *Ref:* `docs/plans/updatedPlans/resilience/05-ResilienceWithPolly.md`
- [ ] **Task:** Implement OAuth 2.0 support:
    - [ ] Extend `ClickUpClientOptions` with OAuth token properties.
    - [ ] Update `AuthenticationDelegatingHandler` to prioritize OAuth token and use "Bearer" scheme if `OAuthToken` is present.
    - [ ] Provide necessary helper methods or guidance for the OAuth flow (token acquisition is typically outside SDK scope, but handling an acquired token is key).
    - *Files:* `src/ClickUp.Api.Client.Abstractions/Options/ClickUpClientOptions.cs`, `src/ClickUp.Api.Client/Http/Handlers/AuthenticationDelegatingHandler.cs`
    - *Why:* Supports an alternative, often preferred, authentication method.
    - *Ref:* `docs/plans/updatedPlans/http/03-HttpClientAndHelpers.md`

### 8. Develop Example Projects
- [ ] **Task:** Add configuration options to examples for Workspace IDs, List IDs, etc., as needed for demonstrating more scenarios.
    - *Files:* `appsettings.template.json` and `Program.cs`/`App.cs` in example projects.
    - *Ref:* `docs/plans/updatedPlans/examples/09-ExampleProjects.md`
- [ ] **Task:** Implement remaining scenarios in `examples/ClickUp.Api.Client.Console`: Basic CRUD, listing resources, pagination demo, error handling, specific service methods.
    - *Files:* `examples/ClickUp.Api.Client.Console/`
    - *Why:* Provide clear, runnable demonstrations of SDK features.
    - *Ref:* `docs/plans/updatedPlans/examples/09-ExampleProjects.md`
- [ ] **Task:** Implement `AddClickUpClient` DI setup in `examples/ClickUp.Api.Client.Worker/Program.cs`.
    - *File:* `examples/ClickUp.Api.Client.Worker/Program.cs`
    - *Why:* Essential for worker to use the SDK.
    - *Ref:* `docs/plans/updatedPlans/examples/09-ExampleProjects.md`
- [ ] **Task:** Implement scenarios in `examples/ClickUp.Api.Client.Worker`: Periodic polling, full `CancellationToken` usage.
    - *Files:* `examples/ClickUp.Api.Client.Worker/Worker.cs` and potentially new services.
    - *Why:* Showcases SDK in background service scenarios.
    - *Ref:* `docs/plans/updatedPlans/examples/09-ExampleProjects.md`
- [ ] **Task:** Update READMEs for example projects with expected outputs once scenarios are implemented.
    - *Files:* `examples/ClickUp.Api.Client.Console/README.md`, `examples/ClickUp.Api.Client.Worker/README.md`
    - *Ref:* `docs/plans/updatedPlans/examples/09-ExampleProjects.md`

## Phase 4: Documentation & Advanced SDK Features (Continued)

### 9. Set Up and Generate API Documentation (DocFX)
- [ ] **Task:** Install DocFX, initialize the DocFX project in `docs/docfx/`.
- [ ] **Task:** Configure `docfx.json` (metadata, build sections).
- [ ] **Task:** Create root `toc.yml` and `index.md` for the documentation site.
- [ ] **Task:** Write conceptual articles for `docs/docfx/articles/`: `intro.md`, `getting-started.md`, `authentication.md` (including OAuth), `error-handling.md`, `pagination.md`, `rate-limiting.md` (Polly policies).
- [ ] **Task:** Establish a process/workflow for building and serving/deploying documentation (e.g., GitHub Actions).
- *Files:* New files/folders under `docs/docfx/`, potentially `.github/workflows/`.
- *Why:* Provides comprehensive and browsable documentation for SDK users.
- *Ref:* `docs/plans/updatedPlans/documentation/08-ApiDocumentation.md`

### 10. Implement Webhook Helpers
- [ ] **Task:** Thoroughly research and document ClickUp's webhook payload structures for common events and the exact signature validation mechanism.
    - *Why:* Foundational knowledge for building reliable helpers.
    - *Ref:* `docs/plans/updatedPlans/webhooks/10-WebhookHelpers.md` (Conceptual)
- [ ] **Task:** Define C# DTOs in `src/ClickUp.Api.Client.Models/Webhooks/` for common incoming webhook payloads.
- [ ] **Task:** Implement `WebhookSignatureValidator.cs` in `src/ClickUp.Api.Client/Webhooks/` to validate incoming webhook signatures.
- [ ] **Task:** Write unit tests for payload DTO deserialization and the `WebhookSignatureValidator`.
- [ ] **Task:** Add conceptual documentation (`articles/webhooks.md`) explaining setup and usage.
- *Files:* New files in `src/ClickUp.Api.Client.Models/Webhooks/`, `src/ClickUp.Api.Client/Webhooks/`, `src/ClickUp.Api.Client.Tests/Webhooks/`, `docs/docfx/articles/webhooks.md`.
- *Why:* Assists users in securely consuming ClickUp webhooks.
- *Ref:* `docs/plans/updatedPlans/webhooks/10-WebhookHelpers.md`

### 11. Implement AI Integration (Semantic Kernel)
- [ ] **Task:** Create the new project `src/ClickUp.Api.Client.SemanticKernel/`.
- [ ] **Task:** Design and implement Semantic Kernel plugins wrapping key SDK services (e.g., `TaskPlugin`, `ListPlugin`). Focus on methods identified as high-value for AI.
- [ ] **Task:** Implement DI registration for these plugins.
- [ ] **Task:** Write unit tests for the Semantic Kernel plugins, mocking SDK services.
- [ ] **Task:** Add conceptual documentation (`articles/semantic-kernel-integration.md`) on using these plugins.
- *Files:* New project and files under `src/ClickUp.Api.Client.SemanticKernel/`, new tests, `docs/docfx/articles/semantic-kernel-integration.md`.
- *Why:* Enables AI agent interaction with the ClickUp API via the SDK.
- *Ref:* `docs/plans/updatedPlans/ai_integration/11-SemanticKernelIntegration.md`

## Ongoing Tasks (Implicit across phases)
*   Continuously review models and interfaces against OpenAPI spec for deviations or missing elements (`01-CoreModelsAndAbstractions.md`).
*   Maintain high test coverage as new features are added (`07-TestingStrategy.md`).
