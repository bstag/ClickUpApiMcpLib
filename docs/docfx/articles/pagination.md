# Pagination

Many ClickUp API endpoints that return lists of items (e.g., tasks, comments, lists) use pagination to manage large datasets. The `ClickUp.Api.Client` SDK provides convenient ways to work with these paginated responses, primarily through `IAsyncEnumerable<T>`.

## Using `IAsyncEnumerable<T>`

For services and methods that support it, the SDK offers helper methods that return `IAsyncEnumerable<T>`. This allows you to easily iterate over all items across all pages using a simple `await foreach` loop, abstracting away the underlying page-by-page fetching logic.

**Example: Iterating through all comments on a task**

```csharp
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels; // For Comment
using System.Threading.Tasks;

public class CommentProcessor
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentProcessor> _logger;

    public CommentProcessor(ICommentService commentService, ILogger<CommentProcessor> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    public async Task ProcessAllTaskCommentsAsync(string taskId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all comments for task {TaskId}...", taskId);
        int commentCount = 0;

        // GetTaskCommentsStreamAsync returns IAsyncEnumerable<Comment>
        await foreach (var comment in _commentService.GetTaskCommentsStreamAsync(taskId, cancellationToken: cancellationToken))
        {
            // Process each comment
            _logger.LogInformation("Comment ID: {CommentId}, Text: {CommentText}", comment.Id, comment.CommentText);
            commentCount++;
            // Add your comment processing logic here
        }

        _logger.LogInformation("Finished processing {CommentCount} comments for task {TaskId}.", commentCount, taskId);
    }
}
```

**How it works:**
Under the hood, methods like `GetTaskCommentsStreamAsync` handle:
1.  Making the initial API request for the first page of results.
2.  Yielding each item from the current page.
3.  Checking if there's a next page (based on ClickUp's pagination mechanism, often using `page` query parameters or cursor-based approaches like `start_id`).
4.  Automatically fetching the next page if available and continuing the iteration.
5.  This continues until all items from all pages have been retrieved.

**Benefits of `IAsyncEnumerable<T>`:**
-   **Simplicity:** Greatly simplifies the code required to consume paginated data.
-   **Efficiency:** Fetches data lazily, page by page, as you iterate. This can be more memory-efficient than fetching all pages into a single large list upfront.
-   **Async Support:** Fully asynchronous, integrating well with modern .NET async patterns.
-   **Cancellation:** Respects `CancellationToken` to stop fetching further pages if the operation is cancelled.

**Available `IAsyncEnumerable<T>` Methods:**
Check the service interfaces in `ClickUp.Api.Client.Abstractions` and their implementations for methods suffixed with `StreamAsync` or similar that return `IAsyncEnumerable<T>`. Examples include:
-   `ICommentService.GetTaskCommentsStreamAsync(...)`
-   `ICommentService.GetChatViewCommentsStreamAsync(...)`
-   `ICommentService.GetListCommentsStreamAsync(...)`
-   `ITasksService.GetTasksStreamAsync(...)` (for team/workspace tasks, if available)
-   `ITasksService.GetFilteredTeamTasksStreamAsync(...)`

## Manual Pagination (When `IAsyncEnumerable<T>` is not available or needed)

Some API endpoints might not yet have `IAsyncEnumerable<T>` helpers, or you might have specific scenarios where you need finer control over page fetching (e.g., displaying page numbers to a user, fetching a specific page).

In such cases, you'll typically interact with methods that accept pagination parameters (like `page`, `limit`, `offset`, `start_id`, etc.) and return a single page of results, often within a response object that includes information about the total number of items or if more pages are available.

**Conceptual Example (Manual Paging):**
This is a generic example; actual parameter names and response structures will vary by ClickUp API endpoint.

```csharp
public async Task ManuallyPageResultsAsync(string listId, IListsService listsService)
{
    int page = 0;
    bool hasMore = true;
    var allItems = new List<SomeListItem>();

    while (hasMore)
    {
        // Assuming a method like GetListItemsAsync exists that takes a page parameter
        // and returns a response object with items and pagination info.
        var pagedResponse = await listsService.GetItemsFromListAsync(listId, page: page, limit: 100); // Fictional method

        allItems.AddRange(pagedResponse.Items);

        // Determine if there are more pages
        // This logic depends on how the ClickUp API indicates more pages
        // e.g., if (pagedResponse.Items.Count < 100) hasMore = false;
        // or if pagedResponse has a 'last_page' or 'next_page_cursor' field.
        hasMore = pagedResponse.Items.Any() && pagedResponse.Items.Count == 100; // Common simple check
        page++;

        if (!hasMore)
        {
            _logger.LogInformation("No more items to fetch for list {ListId}.", listId);
            break;
        }
         _logger.LogInformation("Fetched page {PageNum} for list {ListId}. Total items so far: {TotalCount}", page, listId, allItems.Count);
    }
    // Process allItems
}
```

**Key Considerations for Manual Pagination:**
-   **API Documentation:** Always refer to the official ClickUp API documentation for the specific endpoint to understand its pagination parameters (`page`, `limit`, `offset`, `start`, `start_id`, `next_page_id`, etc.) and how it indicates whether more data is available.
-   **Rate Limiting:** Be mindful of API rate limits when fetching many pages. The SDK's Polly policies will help, but aggressive manual paging could still hit limits.
-   **Error Handling:** Implement robust error handling for each page request.

Whenever possible, prefer using the `IAsyncEnumerable<T>` helpers provided by the SDK for a simpler and more efficient pagination experience. If you find a commonly used paginated endpoint that lacks an `IAsyncEnumerable<T>` helper, consider contributing one to the SDK!
