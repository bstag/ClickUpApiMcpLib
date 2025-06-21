# Detailed Plan: Data Handling (Pagination)

This document details the plan for implementing pagination helpers to simplify the consumption of paginated API endpoints in the ClickUp API SDK.

**Source Documents:**
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 3, Step 9)
*   `docs/OpenApiSpec/ClickUp-6-17-25.json` (To identify paginated endpoints and their parameters)

**Location in Codebase:**
*   Helper methods might be part of individual services or a shared utility/extension class.
*   If using `IAsyncEnumerable<T>`, these would typically be extension methods on the service interfaces or new methods returning `IAsyncEnumerable<T>`.

## 1. Identifying Paginated Endpoints

*   **Action:** Thoroughly review `docs/OpenApiSpec/ClickUp-6-17-25.json` to identify all GET endpoints that support pagination.
*   **Common ClickUp Pagination Parameters:**
    *   `page`: (integer, 0-indexed) The page number to retrieve.
    *   `[some_entity]_last_id` or `start_id`: Some ClickUp endpoints use an ID from the last item of the previous page to fetch the next set (cursor-like).
    *   `date_updated_gt` / `date_created_gt`: Sometimes used for cursor-style pagination based on timestamps.
    *   Response often includes a `last_page: true/false` indicator or relies on an empty item list to signal the end.

*   **Examples of potentially paginated endpoints:**
    *   `GET /list/{list_id}/task` (Get Tasks) - Uses `page`.
    *   `GET /team/{team_id}/task` (Get Filtered Team Tasks) - Uses `page`.
    *   `GET /task/{task_id}/comment` (Get Task Comments) - Uses `start` and `start_id`.
    *   `GET /view/{view_id}/comment` (Get Chat View Comments) - Uses `start` and `start_id`.
    *   `GET /list/{list_id}/comment` (Get List Comments) - Uses `start` and `start_id`.
    *   Many `GET` endpoints returning lists of resources are candidates.

## 2. Pagination Strategy: `IAsyncEnumerable<T>`

The primary strategy for simplifying pagination will be to provide helper methods that return `IAsyncEnumerable<T>`. This allows consumers to iterate over all items across all pages using a simple `await foreach` loop, abstracting the underlying page-by-page fetching.

**Advantages:**
*   **Ease of Use:** Consumers don't need to manage page numbers or cursors.
*   **Efficiency:** Items are fetched lazily as they are enumerated. Only one page of data is typically in memory at a time (unless the consumer materializes the whole sequence with `.ToListAsync()` or similar).
*   **Compatibility:** Works well with LINQ to Objects for further filtering/transformation.

## 3. Implementation Approach for `IAsyncEnumerable<T>` Helpers

For each paginated endpoint (e.g., `GetTasksAsync` in `ITasksService` which originally returns `Task<GetTasksResponse>` where `GetTasksResponse` has `List<TaskDto> Tasks` and `bool LastPage`):

1.  **New Service Method or Extension Method:**
    *   Define a new method, e.g., `GetAllTasksAsyncEnumerable(string listId, GetTasksRequestParameters options, CancellationToken cancellationToken = default)` in the service or as an extension method.
    *   This method will return `IAsyncEnumerable<TaskDto>`.
    *   The `options` parameter would encapsulate all other filter/query parameters for the original `GetTasksAsync` method, *excluding* the pagination-specific parameters like `page`.

2.  **Logic within the Async Iterator Method:**
    *   Initialize `page = 0` (or relevant cursor state).
    *   Loop indefinitely (`while (true)`):
        *   Call the underlying paginated service method (e.g., `_tasksService.GetTasksAsync(listId, page, options..., cancellationToken)`).
        *   If the call fails (exception), the exception will propagate, and the enumeration will stop.
        *   If the response is successful:
            *   If the returned list of items is empty, `break` the loop (no more items).
            *   For each item in the current page's list: `yield return item;`
            *   Check the pagination indicator from the response:
                *   If `response.LastPage == true` (or equivalent), `break` the loop.
                *   Increment `page++`.
                *   For cursor-based pagination: update the `start_id` or timestamp cursor from the last item of the current page.
        *   Handle `cancellationToken.ThrowIfCancellationRequested()` appropriately within the loop.

**Conceptual Example (for `GetTasksAsync` which uses `page` and `last_page`):**

```csharp
// In TasksService.cs or as an extension method for ITasksService
public async IAsyncEnumerable<TaskDto> GetAllTasksAsyncEnumerable(
    string listId,
    GetTasksFilters filters, // A DTO containing all non-pagination parameters
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    int page = 0;
    bool lastPage = false;

    while (!lastPage)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Assuming original GetTasksAsync takes page number and filter object
        // And returns a response object like GetTasksResponse { Tasks: List<TaskDto>, LastPage: bool }
        var request = new GetTasksRequestDto(filters) { Page = page }; // Construct the full request DTO
        GetTasksResponse currentPageData = await _underlyingService.GetTasksAsync(listId, request, cancellationToken); // Call actual API method

        if (currentPageData?.Tasks == null || !currentPageData.Tasks.Any())
        {
            yield break; // No more items or empty response
        }

        foreach (var task in currentPageData.Tasks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return task;
        }

        lastPage = currentPageData.LastPage; // Or similar check
        if (lastPage)
        {
            yield break;
        }
        page++;
    }
}
```

**Conceptual Example (for cursor-based pagination like `GetTaskCommentsAsync`):**

```csharp
// In CommentsService.cs or as an extension method for ICommentsService
public async IAsyncEnumerable<CommentDto> GetAllTaskCommentsAsyncEnumerable(
    string taskId,
    GetCommentsFilters filters, // DTO for non-pagination params
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    long? startTimestamp = null; // Or initial value if API needs one
    string? startId = null;

    while (true)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new GetTaskCommentsRequestDto(filters) // Construct request
        {
            Start = startTimestamp,
            StartId = startId
        };

        // Assuming GetTaskCommentsAsync returns GetCommentsResponse { Comments: List<CommentDto> }
        // And an empty list means no more comments for that cursor.
        GetCommentsResponse currentPageData = await _underlyingService.GetTaskCommentsAsync(taskId, request, cancellationToken);

        if (currentPageData?.Comments == null || !currentPageData.Comments.Any())
        {
            yield break; // No more comments
        }

        CommentDto lastComment = null;
        foreach (var comment in currentPageData.Comments)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return comment;
            lastComment = comment;
        }

        // Update cursors from the last comment of the page
        // Assuming CommentDto has 'Date' (as long/timestamp) and 'Id' (as string)
        if (lastComment != null)
        {
            // This logic depends heavily on how ClickUp API specifies using the cursor.
            // It might be the timestamp of the *oldest* comment retrieved.
            // For "start" and "start_id" of the *oldest* comment to retrieve the *next* 25,
            // this logic might need adjustment based on API behavior.
            // If the API returns comments oldest first, then lastComment.Date is correct.
            // If newest first, then the logic would be different or the API might provide a 'next_cursor'.
            // For this example, assuming 'start' and 'start_id' refer to the oldest item to start AFTER.
            startTimestamp = lastComment.Date; // Assuming Date is a long timestamp
            startId = lastComment.Id;
        }
        else // Should not happen if !Any() check is above, but defensive.
        {
            yield break;
        }

        // Some APIs might return a specific 'next_page_cursor' in the response, use that if available.
        // If ClickUp's GetTaskComments implies "get comments older than this start/start_id", this is okay.
    }
}
```
**Note on ClickUp's `start` and `start_id` for comments:** The documentation states: *"Use the `start` and `start id` parameters of the oldest comment to retrieve the next 25 comments."* This implies that if comments are returned newest-first, you'd take the `date` and `id` from the *last* (oldest) comment in the current batch to fetch the *next older* batch. The async iterator should handle this direction correctly.

## 4. Helper DTOs for Request Parameters

*   For each paginated endpoint, if not already done, create a request DTO that encapsulates all its filterable parameters (e.g., `GetTasksFilters`, `GetCommentsFilters`).
*   The new `IAsyncEnumerable<T>` helper methods will accept this filter DTO.

## 5. Documentation

*   Clearly document the new `IAsyncEnumerable<T>` helper methods.
*   Explain that these methods handle pagination automatically.
*   Provide examples of using `await foreach` to consume all items.
*   Mention that if the consumer needs to process pages explicitly (e.g., for UI display with page numbers), they should use the original paginated methods.

## 6. Testing

*   Unit test the async iterators by mocking the underlying service calls.
*   Test scenarios:
    *   No items.
    *   Single page of items.
    *   Multiple pages of items.
    *   Cancellation during enumeration.
    *   API error during page fetching.
*   For cursor-based pagination, ensure the cursor logic is correctly extracting and passing the next cursor value.

## Plan Output

*   This document `06-PaginationHelpers.md` will serve as the detailed plan.
*   It will list all identified paginated endpoints and their specific pagination mechanisms (page-based, cursor-based).
*   It will provide a template/pattern for implementing the `IAsyncEnumerable<T>` helpers for both types of pagination.
*   It will specify any new request DTOs needed for filter parameters.
```
