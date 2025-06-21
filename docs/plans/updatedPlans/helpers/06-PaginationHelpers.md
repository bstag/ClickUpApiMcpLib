# Detailed Plan: Data Handling (Pagination)

This document details the plan for implementing pagination helpers to simplify the consumption of paginated API endpoints in the ClickUp API SDK.

**Source Documents:**
*   [`docs/plans/NEW_OVERALL_PLAN.md`](../NEW_OVERALL_PLAN.md) (Phase 3, Step 9)
*   [`docs/OpenApiSpec/ClickUp-6-17-25.json`](../../OpenApiSpec/ClickUp-6-17-25.json) (To identify paginated endpoints and their parameters)

**Location in Codebase:**
*   Helper methods might be part of individual services or a shared utility/extension class.
*   If using `IAsyncEnumerable<T>`, these would typically be extension methods on the service interfaces or new methods returning `IAsyncEnumerable<T>`.

## 1. Identifying Paginated Endpoints

- [x] **Action:** Thoroughly review `docs/OpenApiSpec/ClickUp-6-17-25.json` to identify all GET endpoints that support pagination. (Assumed to be an ongoing process as services are implemented).
- [x] **Common ClickUp Pagination Parameters:**
    - [x] `page`: (integer, 0-indexed) The page number to retrieve. (e.g., Get Tasks)
    - [x] `start` and `start_id`: Used for cursor-like pagination. (e.g., Get Comments)
    - [x] Response often includes a `last_page: true/false` indicator (for page-based) or relies on an empty item list to signal the end (for cursor-based).

- [x] **Examples of potentially paginated endpoints:**
    - [x] `GET /list/{list_id}/task` (Get Tasks) - Uses `page`. Implemented in `TaskService.GetTasksAsyncEnumerableAsync`.
    - [ ] `GET /team/{team_id}/task` (Get Filtered Team Tasks) - Uses `page`. (No `IAsyncEnumerable` wrapper found in `TaskService.cs` for this yet).
    - [x] `GET /task/{task_id}/comment` (Get Task Comments) - Uses `start` and `start_id`. (No specific `IAsyncEnumerable` wrapper found in `CommentService.cs` for this, but the base method exists).
    - [x] `GET /view/{view_id}/comment` (Get Chat View Comments) - Uses `start` and `start_id`. (No specific `IAsyncEnumerable` wrapper found).
    - [x] `GET /list/{list_id}/comment` (Get List Comments) - Uses `start` and `start_id`. (No specific `IAsyncEnumerable` wrapper found).
    *Many other GET endpoints are candidates.*

## 2. Pagination Strategy: `IAsyncEnumerable<T>`

- [x] The primary strategy for simplifying pagination will be to provide helper methods that return `IAsyncEnumerable<T>`.
    - [x] **Ease of Use:** Achieved.
    - [x] **Efficiency:** Achieved (lazy fetching).
    - [x] **Compatibility:** Works with LINQ.

## 3. Implementation Approach for `IAsyncEnumerable<T>` Helpers

For each paginated endpoint:

- [x] **1. New Service Method or Extension Method:**
    - [x] Example: `TaskService.cs` has `public async IAsyncEnumerable<Models.Entities.Tasks.CuTask> GetTasksAsyncEnumerableAsync(...)`. This is a new method in the service.
    - [x] Returns `IAsyncEnumerable<T>`.
    - [x] Accepts filter parameters excluding pagination-specific ones.

- [x] **2. Logic within the Async Iterator Method:**
    - [x] **Page-based example (`TaskService.GetTasksAsyncEnumerableAsync`):**
        - [x] Initializes `currentPage = 0`.
        - [x] Loops while `!lastPage`.
        - [x] Calls the underlying paged service method (e.g., `GetTasksAsync(..., page: currentPage, ...)`).
        - [x] If call fails, exception propagates.
        - [x] If successful:
            - [x] If items list is empty or null, `lastPage = true`, loop breaks.
            - [x] For each item: `yield return item;`.
            - [x] Checks `response.LastPage == true` to set `lastPage`.
            - [x] Increments `currentPage++`.
        - [x] Handles `cancellationToken.IsCancellationRequested`.

    - [ ] **Cursor-based example (e.g., for comments):**
        - [ ] Initialize `startTimestamp = null`, `startId = null`.
        - [ ] Loop `while (true)`:
            - [ ] Call underlying service method with current cursors.
            - [ ] If items list is empty/null, `yield break;`.
            - [ ] For each item: `yield return item;`, update `lastComment`.
            - [ ] Update `startTimestamp` and `startId` from `lastComment` based on ClickUp's cursor logic (e.g., oldest comment's `date` and `id`).
            - [ ] Handle cancellation.
        *(This specific implementation pattern for cursor-based pagination is not yet observed in `CommentService.cs`. The existing comment methods return `IEnumerable<Comment>` which implies they fetch one "page" based on start/start_id but don't automatically iterate to get all comments across multiple cursor steps.)*

**Conceptual Example (for `GetTasksAsync` which uses `page` and `last_page`):** - Implemented in `TaskService.cs`.

**Conceptual Example (for cursor-based pagination like `GetTaskCommentsAsync`):** - **Not yet implemented as an `IAsyncEnumerable<T>` wrapper.**

## 4. Helper DTOs for Request Parameters

- [x] For each paginated endpoint, if not already done, create a request DTO that encapsulates all its filterable parameters (e.g., `GetTasksFilters`, `GetCommentsFilters`).
    - *Observation:* `TaskService.GetTasksAsyncEnumerableAsync` takes individual filter parameters directly, not a single filter DTO. This is a valid approach, though a filter DTO could also be used.
- [ ] The new `IAsyncEnumerable<T>` helper methods will accept this filter DTO (or individual parameters as currently done in `TaskService`).

## 5. Documentation

- [ ] Clearly document the new `IAsyncEnumerable<T>` helper methods.
- [ ] Explain that these methods handle pagination automatically.
- [ ] Provide examples of using `await foreach` to consume all items.
- [ ] Mention that if the consumer needs to process pages explicitly (e.g., for UI display with page numbers), they should use the original paginated methods.
*(Documentation is pending actual completion of these helpers across services)*.

## 6. Testing

- [ ] Unit test the async iterators by mocking the underlying service calls.
- [ ] Test scenarios:
    - [ ] No items.
    - [ ] Single page of items.
    - [ ] Multiple pages of items.
    - [ ] Cancellation during enumeration.
    - [ ] API error during page fetching.
- [ ] For cursor-based pagination, ensure the cursor logic is correctly extracting and passing the next cursor value.
*(Testing is pending implementation of more helpers)*.

## Plan Output

- [x] This document `06-PaginationHelpers.md` has been updated with checkboxes.
- [x] It lists some identified paginated endpoints.
- [x] `TaskService.cs` provides an example of an `IAsyncEnumerable<T>` helper for page-based pagination (`GetTasksAsyncEnumerableAsync`).
- [ ] `IAsyncEnumerable<T>` helpers for cursor-based pagination (like for comments) are **not yet implemented**.
- [ ] Consistent use of filter DTOs for these helpers is not strictly enforced yet (individual parameters are also used).
```
```
