using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class CommentFluentApi
{
    private readonly ICommentsService _commentService; // Corrected service name

    public CommentFluentApi(ICommentsService commentService) // Corrected service name
    {
        _commentService = commentService;
    }

    public TaskCommentsFluentGetRequest GetTaskComments(string taskId)
    {
        // This request builder will eventually use the GetTaskCommentsAsyncEnumerableAsync or a similar paginated method.
        // For now, its internal implementation might use the older GetTaskCommentsAsync.
        // This change is about adding the AsyncEnumerable directly to the fluent API.
        return new TaskCommentsFluentGetRequest(taskId, _commentService);
    }

    public async Task<CreateCommentResponse> CreateTaskCommentAsync(string taskId, CreateTaskCommentRequest createCommentRequest, bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        return await _commentService.CreateTaskCommentAsync(taskId, createCommentRequest, customTaskIds, teamId, cancellationToken);
    }

    /// <summary>
    /// Retrieves all comments for a specific task asynchronously, handling pagination.
    /// </summary>
    /// <param name="taskId">The ID of the task.</param>
    /// <param name="customTaskIds">Optional. If true, the taskId is treated as a custom task ID.</param>
    /// <param name="teamId">Optional. The Workspace ID, required if customTaskIds is true.</param>
    /// <param name="start">Optional. A Unix timestamp (in milliseconds) to start fetching comments from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Comment"/>.</returns>
    public IAsyncEnumerable<Comment> GetTaskCommentsAsyncEnumerableAsync(
        string taskId,
        bool? customTaskIds = null,
        string? teamId = null,
        long? start = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        return _commentService.GetTaskCommentsStreamAsync(taskId, customTaskIds, teamId, start, cancellationToken);
    }

    /// <summary>
    /// Retrieves all comments for a specific list asynchronously, handling pagination.
    /// </summary>
    /// <param name="listId">The ID of the list.</param>
    /// <param name="start">Optional. A Unix timestamp (in milliseconds) to start fetching comments from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Comment"/>.</returns>
    public IAsyncEnumerable<Comment> GetListCommentsAsyncEnumerableAsync(
        string listId,
        long? start = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        return _commentService.GetListCommentsStreamAsync(listId, start, cancellationToken);
    }

    /// <summary>
    /// Retrieves all comments for a specific chat view asynchronously, handling pagination.
    /// </summary>
    /// <param name="viewId">The ID of the chat view.</param>
    /// <param name="start">Optional. A Unix timestamp (in milliseconds) to start fetching comments from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Comment"/>.</returns>
    public IAsyncEnumerable<Comment> GetChatViewCommentsAsyncEnumerableAsync(
        string viewId,
        long? start = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        return _commentService.GetChatViewCommentsStreamAsync(viewId, start, cancellationToken);
    }
}
