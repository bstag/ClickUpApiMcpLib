**Phase 1: Finalize Core Contracts and Update Documentation (Current Task)**

1.  - [x] **Update `docs/prompts.md` and `jules.md`:**
    *   Append the latest user prompt to `docs/prompts.md` with a timestamp.
    *   Update `jules.md` to reflect the new consolidated plan and current understanding of the project state. Note the completion of model creation and the need to refine interfaces.

2.  - [x] **Refine Service Interfaces in `ClickUp.Api.Client.Abstractions`:**
    *   Iterate through all service interfaces (e.g., `ITasksService`, `IListsService`, etc.) in the `src/ClickUp.Api.Client.Abstractions/Services/` directory.
    *   Replace generic `object` placeholders in method signatures with specific DTOs from the `ClickUp.Api.Client.Models` project. For example, in `ITasksService.GetTaskAsync`, change `Task<object>` to `Task<TaskDto>` (or the appropriate response DTO if the actual task is nested).
    *   Ensure all request parameters also use specific DTOs (e.g., `CreateTaskRequestDto` instead of `object createTaskRequest`).
    *   Verify and correct parameter types (e.g., change `double listId` to `string listId` if API identifiers are strings).
    *   Review and clean up namespaces (e.g., the nested `ClickUp.Abstract` in `ITasksService.cs`).
    *   Ensure all methods include an optional `CancellationToken cancellationToken = default` parameter.
    *   Update XML documentation comments for all interface methods to accurately reflect parameters and return types.

3.  - [x] **Update Conceptual Plan Documents:**
    *   Review and update `docs/plans/02-abstractions-interfaces-conceptual.md` and `docs/plans/02-abstractions-interfaces-actual.md` to reflect the refined state of the interfaces. Mark `02-abstractions-interfaces-actual.md` as complete after refinement.
    *   Create a new document `docs/plans/NEW_OVERALL_PLAN.md` and copy this full plan into it for future reference.

**Phase 2: Implement Core Client Functionality (Following `geminiPlan.md` Sections II & III)**

4.  - [ ] **Implement Service Layer in `ClickUp.Api.Client`:**
    - [x] Create initial stub implementations for all service classes.
    - [ ] Implement actual HTTP call logic using HttpClient (to be done in conjunction with Step 5).
    - [ ] Integrate error handling (Step 6) and authentication (Step 7).
    *   Create concrete service classes (e.g., `TaskService`, `ListService`) in `src/ClickUp.Api.Client/Services/` that implement the refined interfaces from Phase 1.
    *   These services will orchestrate request construction, execution, and response processing.
    *   Initially, focus on direct `HttpClient` usage without Polly or advanced auth handlers, to be added in subsequent steps.

5.  - [ ] **Setup `IHttpClientFactory` and Basic `HttpClient` Configuration:**
    *   Integrate `IHttpClientFactory` for `HttpClient` management.
    *   Configure a typed client (e.g., `ClickUpHttpClient`) or a named client.
    *   Set the base API address (`https://api.clickup.com/api/v2/`) and default headers (e.g., `User-Agent`).
    *   Implement basic JSON serialization/deserialization helpers using `System.Text.Json` with centrally configured `JsonSerializerOptions` (as per `docs/plans/04-httpclient-helpers-conceptual.md`).

6.  - [ ] **Implement Robust Exception Handling:**
    *   Define the custom exception hierarchy (e.g., `ClickUpApiException`, `ClickUpApiValidationException`, etc.) in the `ClickUp.Api.Client.Models` project, as outlined in `docs/plans/05-exception-system-conceptual.md`.
    *   Implement centralized error handling logic within the service implementations (or a shared helper) to translate HTTP error responses into these custom exceptions.

7.  - [ ] **Implement Authentication Handling:**
    *   **Personal API Token:** Implement a `DelegatingHandler` (e.g., `AuthenticationDelegatingHandler`) to add the Personal API Token to requests.
    *   **OAuth 2.0 Support (Conceptual):** Design and document how OAuth 2.0 will be supported, focusing on helper methods for the consuming application to manage the flow (as per `geminiPlan.md Section 3.4`). Actual implementation of OAuth token management can be a later step.

8.  - [ ] **Integrate Polly for Resilience:**
    *   Add Polly to the `IHttpClientFactory` configuration.
    *   Implement policies for retry with exponential backoff and jitter for transient HTTP errors (5xx, network issues, 408).
    *   Implement a circuit breaker policy.

**Phase 3: Enhance Developer Experience (Following `geminiPlan.md` Section IV)**

9.  - [ ] **Implement Efficient Data Handling (Pagination):**
    *   For API endpoints that return paginated lists, implement helper methods (e.g., using `IAsyncEnumerable<T>`) to abstract away manual page management for consumers, as described in `geminiPlan.md Section 3.5`.

10. - [ ] **Develop Unit and Integration Tests:**
    *   Create a test project `src/ClickUp.Api.Client.Tests/`.
    *   Write unit tests for service layer logic, mocking dependencies (`IApiConnection` or `HttpClient`).
    *   Plan and implement integration tests that make real API calls to a ClickUp test environment (requires separate configuration for API tokens).

11. - [ ] **Create API Documentation using DocFX:**
    *   Set up DocFX in the `docs/` directory.
    *   Ensure all public types and members in `ClickUp.Api.Client.Abstractions` and `ClickUp.Api.Client.Models` have comprehensive XML documentation comments.
    *   Write conceptual documentation (guides, tutorials like "Getting Started," "Authentication") as Markdown files.
    *   Configure DocFX to generate a documentation website.
    *   Set up a GitHub Actions workflow to build and deploy the documentation (e.g., to GitHub Pages).

12. - [ ] **Develop Showcase Example Projects:**
    *   Implement the example projects in `examples/ClickUp.Api.Client.Console` and `examples/ClickUp.Api.Client.Worker`.
    *   Demonstrate client initialization (Personal Token), core operations (CRUD for tasks), advanced filtering, pagination, and error handling.

**Phase 4: Advanced Features and Future-Proofing (Following `geminiPlan.md` Section V)**

13. - [ ] **Support for Webhooks (Helpers):**
    *   Provide guidance and utility methods for validating webhook signatures and processing webhook payloads, as outlined in `geminiPlan.md Section 3.6`.

14. - [ ] **Semantic Layer for AI Consumption (Semantic Kernel):**
    *   Design and implement Semantic Kernel plugins that wrap key service methods, providing natural language descriptions for AI agents (as per `geminiPlan.md Section 5.3`).
    *   This involves adding the `Microsoft.SemanticKernel` NuGet package.

15. - [ ] **(Optional) Model Context Protocol (MCP) Integration:**
    *   Conceptually plan for exposing the Semantic Kernel plugins via an MCP Server for broader AI agent interoperability, as described in `geminiPlan.md Section 5.4`. Implementation can be deferred.

**Phase 5: Submission**

16. - [ ] **Final Review and Submission:**
    *   Review all code, tests, and documentation.
    *   Submit the completed SDK with a comprehensive commit message.
