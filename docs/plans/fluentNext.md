# Fluent API: Next Steps and Improvements

This document outlines the current state of the Fluent API implementation in the ClickUp.Api.Client, reviews its strengths, and proposes a phased approach for future enhancements and improvements.

## Current State and Strengths

The Fluent API, primarily located under `src/ClickUp.Api.Client/Fluent/`, provides a more intuitive, readable, and chainable interface for interacting with the ClickUp API. Key components include:

-   **`ClickUpClient.cs`**: The main entry point, providing access to various service-specific fluent APIs (e.g., `client.Tasks`, `client.Lists`).
-   **`...FluentApi.cs` files (e.g., `TasksFluentApi.cs`)**: These classes offer methods that typically return request builder objects.
-   **`...FluentRequest.cs` files (e.g., `TaskFluentCreateRequest.cs`, `TasksRequest.cs`)**: These are request builder objects that allow for chained configuration of parameters (e.g., `.WithName("New Task")`, `.WithArchived(false)`) and an execution method (e.g., `.ExecuteAsync()`, `.GetAsync()`).
-   **`...AsyncEnumerableAsync` methods**: Several `...FluentApi.cs` classes directly offer methods (e.g., `TasksFluentApi.GetTasksAsyncEnumerableAsync()`) that return `IAsyncEnumerable<T>`, simplifying the consumption of paginated API responses.

**Strengths:**

-   **Readability:** Chained method calls make the construction of API requests clear and self-documenting.
-   **Discoverability:** Intellisense in IDEs helps users discover available options and parameters.
-   **Reduced Complexity:** Simplifies interaction with API endpoints that have many optional parameters.
-   **`AsyncEnumerable` Support:** Excellent for handling pagination transparently.
-   **Good Coverage:** Most API services defined in `ClickUpClient.cs` have a corresponding fluent API implementation.

## Proposed Enhancements and Improvements

The following phases outline a roadmap for further developing the Fluent API.

---

### Phase 1: Audit and Consistency Pass

**Objective:** Ensure the Fluent API is complete, consistent, and adheres to best practices.

**Steps:**

1.  **Step 1.1: Comprehensive Service Method Coverage Audit**
    *   **Action:** Systematically compare each method in every `I...Service.cs` interface with the public methods offered by its corresponding `...FluentApi.cs` class.
    *   **Details:**
        *   Verify that every operation available in the service interfaces can be initiated through the fluent API.
        *   For methods that return collections of data (e.g., `GetSpacesAsync`, `GetFoldersAsync`, `GetListsAsync`, `GetCommentsAsync`, etc.), ensure that a corresponding `...AsyncEnumerableAsync` method exists directly within the `...FluentApi.cs` class to provide automatic pagination handling.
        *   Document any gaps or discrepancies.
    *   **Value:** Guarantees that users can access 100% of the underlying service functionality through the Fluent API. Provides a superior experience for handling paginated data.

2.  **Step 1.2: Naming Convention Review and Standardization**
    *   **Action:** Review the naming of all Fluent API classes, request builder classes, and their methods.
    *   **Details:**
        *   **Request Builder Classes:** Currently, there's a mix (e.g., `TasksRequest` for getting tasks vs. `TaskFluentCreateRequest` for creating a task). Consider a standardized pattern, such as:
            *   `[ServiceName]FluentGetRequest` (for GET operations returning multiple items, e.g., `TasksFluentGetRequest`)
            *   `[ServiceName]FluentGetSingleRequest` (for GET operations returning a single item, e.g., `TaskFluentGetSingleRequest`)
            *   `[ServiceName]FluentCreateRequest` (e.g., `TaskFluentCreateRequest`)
            *   `[ServiceName]FluentUpdateRequest` (e.g., `TaskFluentUpdateRequest`)
            *   `[ServiceName]FluentDeleteRequest` (e.g., `TaskFluentDeleteRequest`)
            *   Or, more generally, `[EntityName]Fluent[Action]Request`.
        *   **`With...()` Methods:** Ensure consistency in the naming of methods that set parameters on request builder objects (e.g., always `WithParameterName()`).
        *   **Execution Methods:** Standardize the method name used to execute the request on the builder objects (e.g., prefer `ExecuteAsync()` or `GetAsync()` consistently). Currently, `fluent-api-usage.md` shows `.GetAsync()` for a `TasksRequest` and implies `ExecuteAsync()` might be used elsewhere.
    *   **Value:** Enhances API predictability, reduces cognitive load for users, and makes the API more intuitive to learn and use. A consistent naming scheme improves maintainability.

---

### Phase 2: Enhancements and Usability Improvements

**Objective:** Act on the findings from Phase 1 to improve the usability and completeness of the Fluent API.

**Steps:**

1.  **Step 2.1: Implement Missing `AsyncEnumerable` Methods**
    *   **Action:** Based on the audit in Step 1.1, implement `...AsyncEnumerableAsync` methods in their respective `...FluentApi.cs` classes for all service operations that return paginated lists of items and currently lack this feature.
    *   **Details:** These methods should encapsulate the pagination logic, making it trivial for users to iterate over all items in a collection without manual page handling. Follow the pattern established in `TasksFluentApi.cs`.
    *   **Value:** Significantly simplifies common data retrieval tasks, reduces boilerplate code for users, and makes the library more user-friendly.
    *   **Tasks:**
        *   - [ ] **AttachmentsFluentApi:** Review methods like `GetTaskAttachments` and implement `...AsyncEnumerableAsync` if applicable. (Update: API endpoint for getting task attachments does not exist as of 2024-07-12. Cannot implement.)
        *   - [x] **ChatFluentApi:** Implement for methods like `GetViewMessagesAsyncEnumerableAsync`, `GetChannelMessagesAsyncEnumerableAsync`. https://developer.clickup.com/reference/getchatmessages (Implemented `GetChannelMessagesAsyncEnumerableAsync`)
        *   - [x] **CommentsFluentApi:** Implement for `GetTaskCommentsAsyncEnumerableAsync`, `GetListCommentsAsyncEnumerableAsync`, `GetChatViewCommentsAsyncEnumerableAsync` (renamed from `GetChatMessagesCommentsAsyncEnumerableAsync` to align with service method). https://developer.clickup.com/reference/getchatviewcomments
        *   - [x] **CustomFieldsFluentApi:** Implement for `GetAccessibleCustomFieldsAsyncEnumerableAsync`. (Note: API is not paginated, so this is a wrapper.)
        *   - [x] **DocsFluentApi:** Implement for `GetDocsAsyncEnumerableAsync`, `SearchDocsAsyncEnumerableAsync`. (Leveraged existing `SearchAllDocsAsync` from service layer which handles pagination.)
        *   - [x] **FoldersFluentApi:** Implement for `GetFoldersAsyncEnumerableAsync`. (Note: API is not paginated, so this is a wrapper.)
        *   - [x] **GoalsFluentApi:** Implement for `GetGoalsAsyncEnumerableAsync`. it does not have pagination.
        *   - [x] **GuestsFluentApi:** Review all list-returning methods (e.g., `GetGuests`, `GetGuestTasks`) and implement `...AsyncEnumerableAsync` equivalents. it does not have pagination. (Update: `IGuestsService` does not currently have methods that return lists of guests/guest-tasks suitable for `AsyncEnumerable` wrappers. API endpoint might exist but not implemented in service.)
        *   - [x] **ListsFluentApi:** Implement for `GetListsAsyncEnumerableAsync`, `GetFolderlessListsAsyncEnumerableAsync`.
        *   - [x] **MembersFluentApi:** Review all list-returning methods (e.g., `GetWorkspaceMembers`, `GetListMembers`, `GetTaskMembers`) and implement `...AsyncEnumerableAsync` equivalents. it does not have pagination. (Implemented for `GetListMembers` and `GetTaskMembers`. `GetWorkspaceMembers` not available in service.)
        *   - [x] **RolesFluentApi:** Implement for `GetRolesAsyncEnumerableAsync` it does not have pagination.
        *   - [x] **SharedHierarchyFluentApi:** Implement for `GetSharedHierarchyAsyncEnumerableAsync`. it does not have pagination. (Implemented as `GetSharedTaskIdsAsyncEnumerableAsync`, `GetSharedListsAsyncEnumerableAsync`, and `GetSharedFoldersAsyncEnumerableAsync` due to response structure.)
        *   - [x] **SpacesFluentApi:** Implement for `GetSpacesAsyncEnumerableAsync`.
        *   - [x] **TagsFluentApi:** Implement for `GetSpaceTagsAsyncEnumerableAsync`.
        *   - [x] **TasksFluentApi:** Already implements `GetTasksAsyncEnumerableAsync` and `GetFilteredTeamTasksAsyncEnumerableAsync`. Serve as a model.
        *   - [x] **TemplatesFluentApi:** Implement for `GetTemplatesAsyncEnumerableAsync`.
        *   - [x] **TimeTrackingFluentApi:** Implement for `GetTimeEntriesAsyncEnumerableAsync`, `GetTimeEntryHistoryAsyncEnumerableAsync`. it does not have pagination. (`GetTimeEntriesAsyncEnumerableAsync` is available via `TimeEntriesFluentGetRequest.GetStreamAsync()`. Implemented wrapper for `GetTimeEntryHistoryAsyncEnumerableAsync`.)
        *   - [x] **UserGroupsFluentApi:** Implement for `GetUserGroupsAsyncEnumerableAsync`, `GetGroupMembersAsyncEnumerableAsync`. it does not have pagination. (Implemented `GetUserGroupsAsyncEnumerableAsync`. `GetGroupMembersAsyncEnumerableAsync` is not directly applicable as members are part of `UserGroup` object; service lacks a direct separate endpoint for only members of a group.)
        *   - [ ] **UsersFluentApi:** Implement for `GetUsersAsyncEnumerableAsync` if applicable for listing multiple users. you can not list multiple users.
        *   - [x] **ViewsFluentApi:** Implement for `GetSpaceViewsAsyncEnumerableAsync`, `GetFolderViewsAsyncEnumerableAsync`, `GetListViewsAsyncEnumerableAsync`. (Note: `GetTaskViews` might be singular). it does not have pagination. (Also implemented `GetWorkspaceViewsAsyncEnumerableAsync`.)
        *   - [ ] **WebhooksFluentApi:** Implement for `GetWebhooksAsyncEnumerableAsync`.it does not have pagination. 
        *   - [ ] **WorkspacesFluentApi:** Implement for `GetWorkspacesAsyncEnumerableAsync` (though likely small, for consistency if API supports pagination). it does not have pagination. 

2.  **Step 2.2: Refactor Service Methods to Use Request Objects (If Necessary)**
    *   **Action:** If the audit in Step 1.1 reveals any `I...Service.cs` methods that still accept a large number of optional parameters directly (instead of a single request object), refactor them.
    *   **Details:**
        *   Introduce a specific `...Request.cs` model for such methods in `ClickUp.Api.Client.Models.RequestModels`.
        *   Update the service interface (`I...Service.cs`) and implementation (`...Service.cs`) to use this request model.
        *   Ensure the corresponding fluent API builder (`...Fluent[Action]Request.cs`) correctly populates this new request model.
        *   This applies to both standard async methods and their `...AsyncEnumerableAsync` counterparts if the parameter list is extensive.
    *   **Value:** Improves the clarity and maintainability of the core service layer. Ensures that the fluent API builders have a consistent and well-structured object to populate, leading to cleaner fluent implementations.
    *   **Tasks:**
        *   - [ ] **General Audit:** Review all `I...Service.cs` methods that return lists or have numerous optional parameters.
        *   - [ ] **ITasksService Example:**
            *   - [ ] Refactor `ITasksService.GetFilteredTeamTasksAsync(...)` to accept a `GetFilteredTeamTasksRequest` object.
            *   - [ ] Update `TasksService.GetFilteredTeamTasksAsync` implementation.
            *   - [ ] Update `TasksFluentGetFilteredTeamRequest` to build and pass the new request object.
            *   - [ ] Refactor `ITasksService.GetFilteredTeamTasksAsyncEnumerableAsync(...)` to also accept or internally use the `GetFilteredTeamTasksRequest` object for consistency.
        *   - [ ] **Other Services:** Identify and list other service methods requiring similar refactoring based on the audit. For each identified method:
            *   - [ ] Define the new `...Request` DTO.
            *   - [ ] Update the service interface and implementation.
            *   - [ ] Update or create the corresponding Fluent API request builder.

3.  **Step 2.3: Apply Standardized Naming Conventions**
    *   **Action:** Based on the decisions made in Step 1.2 and further review, refactor the names of fluent request builder classes, `With...()` methods, and execution methods across the entire Fluent API surface.
    *   **Details:**
        *   **Proposed Request Builder Naming Strategy:**
            *   For operations on a single entity (Create, Get Single, Update, Delete): `[EntityName]Fluent[Action]Request` (e.g., `TaskFluentCreateRequest`, `TaskFluentGetSingleRequest`, `TaskFluentUpdateRequest`, `TaskFluentDeleteRequest`).
            *   For operations fetching multiple entities (Get List/Collection): `[EntityName]FluentQueryRequest` (e.g., `TasksFluentQueryRequest`, `FoldersFluentQueryRequest`). This replaces previous suggestions like `[ServiceName]FluentGetRequest` for more clarity on the "query" nature.
            *   For other specific actions: `[EntityName]Fluent[SpecificAction]Request` (e.g., `TaskFluentMergeRequest`).
        *   **Execution Methods:** Standardize to `ExecuteAsync()` for commands (Create, Update, Delete, other actions) and `GetAsync()` for queries (Get Single, Get Multiple).
        *   **`With...()` Methods:** Ensure consistency (e.g., always `WithParameterName()`). This is largely good but needs a final check.
        *   GetAsync(): Returning a Task<TResponse> with a paginated object (e.g., GetTasksResponse).
        *   GetAsyncEnumerableAsync(): Returning an IAsyncEnumerable<T> for easy iteration over all pages.
    *   **Value:** Long-term benefits of improved learnability, usability, and maintainability of the Fluent API.
    *   **Tasks:**
        *   - [ ] **Finalize Naming Strategy:** Confirm the proposed naming strategy for request builders and execution methods.
        *   - [ ] **Request Builder Class Renaming:** Audit and rename existing request builder classes according to the chosen strategy. Create tasks per service/entity:
            *   - [ ] Attachments: Review and rename request builders.
            *   - [ ] Authorization: Review and rename request builders (e.g., `GetAccessTokenFluentRequest`).
            *   - [ ] Chat: Review and rename request builders (e.g., `ChatChannelsFluentGetRequest`).
            *   - [ ] Checklists: Review and rename request builders (e.g., `ChecklistFluentCreateRequest`, `ChecklistItemFluentEditRequest`).
            *   - [ ] Comments: Review and rename request builders.
            *   - [ ] CustomFields: Review and rename request builders.
            *   - [ ] Docs: Review and rename request builders.
            *   - [ ] Folders: Review and rename request builders.
            *   - [ ] Goals: Review and rename request builders (e.g., `GoalsFluentGetRequest` to `GoalsFluentQueryRequest`).
            *   - [ ] Guests: Review and rename request builders.
            *   - [ ] Lists: Review and rename request builders.
            *   - [ ] Members: Review and rename request builders.
            *   - [ ] Roles: Review and rename request builders.
            *   - [ ] SharedHierarchy: Review and rename request builders.
            *   - [ ] Spaces: Review and rename request builders.
            *   - [ ] Tags: Review and rename request builders.
            *   - [ ] Tasks: Review and rename (e.g., `TasksRequest` to `TasksFluentQueryRequest`, `TaskFluentGetRequest` to `TaskFluentGetSingleRequest`).
            *   - [ ] Templates: Review and rename request builders.
            *   - [ ] TimeTracking: Review and rename request builders.
            *   - [ ] UserGroups: Review and rename request builders.
            *   - [ ] Users: Review and rename request builders.
            *   - [ ] Views: Review and rename request builders.
            *   - [ ] Webhooks: Review and rename request builders.
            *   - [ ] Workspaces: Review and rename request builders.
        *   - [ ] **Execution Method Standardization:**
            *   - [ ] Audit all request builder classes and standardize their execution methods (e.g., `CreateAsync`, `UpdateAsync`, `DeleteAsync` to `ExecuteAsync()`; `GetAsync` for single/list to remain `GetAsync()` ).
                - [ ] 
        *   - [ ] **`With...()` Method Consistency Check:**
            *   - [ ] Perform a quick audit across all request builders to ensure `With...()` methods are consistently named.

---

### Phase 3: Documentation and Examples

**Objective:** Provide comprehensive documentation and a rich set of examples to facilitate user adoption and understanding of the Fluent API.

**Steps:**

1.  **Step 3.1: Expand and Diversify Usage Examples**
    *   **Action:** Significantly expand the existing `docs/plans/fluent/fluent-api-usage.md` or create a new set of example documents.
    *   **Details:**
        *   Provide clear, concise, and runnable examples for every Fluent API service (e.g., `client.Workspaces`, `client.Spaces`, `client.Folders`, `client.Lists`, `client.Tasks`, `client.Comments`, etc.).
        *   Cover common use cases:
            *   Simple GET requests for single entities and collections.
            *   Creating new entities (POST requests).
            *   Updating existing entities (PUT requests).
            *   Deleting entities (DELETE requests).
            *   Requests involving complex filtering and parameterization using the `With...()` methods.
        *   Prominently feature examples of the `...AsyncEnumerableAsync` methods for easy consumption of paginated data.
        *   Show examples of error handling (e.g., `try-catch` blocks for `ClickUpApiException`).
    *   **Value:** Lowers the barrier to entry for new users. Provides practical templates that users can adapt for their own needs. Showcases the power and simplicity of the Fluent API.

2.  **Step 3.2: Document Fluent API Design Patterns and Conventions**
    *   **Action:** Create a dedicated document (or a section within developer documentation) that outlines the design philosophy, patterns, and naming conventions used in the Fluent API.
    *   **Details:**
        *   Explain the role of `ClickUpClient`, `...FluentApi.cs` classes, and `...Fluent[Action]Request.cs` (or the standardized name) classes.
        *   Document the chosen naming conventions for classes and methods (from Phase 1 & 2).
        *   Provide guidelines for contributors on how to extend the Fluent API (e.g., adding new methods or new services) while maintaining consistency.
    *   **Value:** Essential for long-term maintainability and scalability of the Fluent API. Ensures that new contributions align with the established design, keeping the API coherent.

---

### Phase 4: Advanced Features (Future Considerations)

**Objective:** Explore and potentially implement advanced features that can further enhance the power and flexibility of the Fluent API.

**Steps:**

1.  **Step 4.1: Investigate and Implement Batching Capabilities**
    *   **Action:** Research if the ClickUp API offers batch endpoints for operations like creating, updating, or deleting multiple entities in a single request (e.g., batch creating tasks, batch updating custom fields).
    *   **Details:** If such endpoints exist:
        *   Design and implement fluent API methods to support these batch operations. This might involve new types of request builder objects that can accumulate multiple entities or operations.
        *   Ensure the fluent interface for batching is intuitive and handles potential partial failures gracefully (if the API provides such details in responses).
    *   **Value:** Can significantly improve performance and reduce API call overhead for users performing bulk operations, leading to more efficient applications and better adherence to rate limits.

2.  **Step 4.2: Explore Support for More Complex Conditional Query Logic**
    *   **Action:** Evaluate the need and feasibility of adding support for more complex conditional logic within the fluent query builders (e.g., OR conditions between different filter groups, if supported by the API but not easily expressible currently).
    *   **Details:**
        *   This would likely involve more advanced expression building capabilities within the `With...()` methods or introducing new methods for grouping conditions.
        *   Careful consideration is needed to avoid overcomplicating the API. The primary goal of fluency and readability should be maintained.
    *   **Value:** Could provide greater flexibility for very specific and complex query scenarios that go beyond simple ANDing of filters. This should be driven by clear user demand or API capabilities that are currently hard to leverage.

---
