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
    - [x] Create initial stub implementations for all service classes in `src/ClickUp.Api.Client/Services/`.
    - [ ] **For each service (e.g., `TaskService`, `ListService`, etc.):**
        - [ ] **Inject `HttpClient` (or typed `ClickUpHttpClient`) and `ILogger`.** (Conceptual plan: `docs/plans/03-service-implementations-conceptual.md`)
        - [ ] **Implement actual HTTP call logic for each method:**
            - [ ] Construct the correct relative URL for the endpoint (refer to OpenAPI spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`).
            - [ ] Determine the HTTP method (GET, POST, PUT, DELETE).
            - [ ] If the method requires a request body:
                - [ ] Serialize the request DTO (from `ClickUp.Api.Client.Models`) to JSON.
                - [ ] Create `StringContent` with `application/json` media type.
            - [ ] Make the HTTP call using the injected `HttpClient` (passing `CancellationToken`).
            - [ ] Receive the `HttpResponseMessage`.
            - [ ] **Process the response:**
                - [ ] Check `response.IsSuccessStatusCode`.
                - [ ] If successful:
                    - [ ] If body returned, deserialize JSON content into the response DTO (using shared `JsonSerializerOptions` from Step 5).
                    - [ ] Return deserialized DTO or `Task.CompletedTask`.
                - [ ] If not successful:
                    - [ ] Implement error handling logic (see Step 6).
        - [ ] Ensure all public methods align with refined interfaces (correct DTOs, `CancellationToken`).
        - [ ] Add XML documentation comments for implemented methods.

5.  - [ ] **Setup `IHttpClientFactory` and Basic `HttpClient` Configuration:**
    - [x] Conceptual plan: `docs/plans/04-httpclient-helpers-conceptual.md`.
    - [ ] **In client library DI setup (e.g., `services.AddClickUpApiClient(...)` extension method):**
        - [ ] Register and configure a typed client (e.g., `ClickUpHttpClient`) or named client.
            - [ ] Set Base API Address: `https://api.clickup.com/api/v2/`.
            - [ ] Set Default Headers: `User-Agent`, `Accept: application/json`.
        - [ ] Integrate `AuthenticationDelegatingHandler` (Step 7) via `.AddHttpMessageHandler()`.
    - [ ] **Implement JSON Serialization/Deserialization Helpers:**
        - [ ] Create/Use a central place for `JsonSerializerOptions` (e.g., static class `ClickUpJsonSerializerSettings`).
            - [ ] Configure `PropertyNamingPolicy` (Action: Verify ClickUp API's casing).
            - [ ] Configure `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull`.
            - [ ] Add necessary converters (e.g., `JsonStringEnumConverter`).
        - [ ] Ensure services use these shared options.
    - [ ] **(Optional) Create basic helper methods if `System.Net.Http.Json` isn't sufficient.**
    - [ ] **(Optional) Query String Builder Utility for complex queries.** (Refer to `docs/plans/04-httpclient-helpers-conceptual.md`).

6.  - [ ] **Implement Robust Exception Handling:**
    - [x] Conceptual plan: `docs/plans/05-exception-system-conceptual.md`.
    - [x] Basic custom exception types defined in `ClickUp.Api.Client.Models/Exceptions/`.
    - [ ] **Refine and Expand Custom Exception Hierarchy (as needed):**
        - [ ] `ClickUpApiValidationException`
        - [ ] `ClickUpApiAuthenticationException`
        - [ ] `ClickUpApiNotFoundException`
        - [ ] `ClickUpApiRateLimitException`
        - [ ] `ClickUpApiServerException`
        - [ ] Ensure all inherit from `ClickUpApiException` and include properties for API error details.
    - [ ] **Implement Centralized Error Handling Logic:**
        - [ ] Create a shared helper method (e.g., `async Task ThrowClickUpApiExceptionAsync(HttpResponseMessage response)`).
        - [ ] This helper should:
            - [ ] Read and deserialize error content into a structured error DTO (Action: Define `ClickUpApiErrorResponse` model).
            - [ ] Instantiate and throw the appropriate custom exception based on status code and error details.
    - [ ] **Integrate into Service Implementations:** Call the error helper when `!response.IsSuccessStatusCode`.
    - [ ] **Handle `HttpRequestException`:** Wrap in `ClickUpApiException`.

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
