using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments;

namespace ClickUp.Api.Client.Abstractions.Services.Comments
{
    /// <summary>
    /// Service interface for ClickUp Chat View Comment operations.
    /// Handles comment operations specifically related to chat views.
    /// </summary>
    /// <remarks>
    /// This interface follows the Interface Segregation Principle by focusing solely on chat view-related comment operations.
    /// Covered API Endpoints:
    /// - Chat View Comments: `GET /view/{view_id}/comment`, `POST /view/{view_id}/comment`
    /// </remarks>
    public interface IChatViewCommentService
    {
        /// <summary>
        /// Retrieves all comments from a specific Chat view as an asynchronous stream.
        /// This method handles pagination internally, yielding comments as they are fetched.
        /// </summary>
        /// <param name="viewId">The unique identifier of the Chat view for which to retrieve comments.</param>
        /// <param name="start">Optional. A Unix timestamp (in milliseconds) indicating the starting point from which to retrieve comments for the initial page.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields <see cref="Comment"/> objects for the specified Chat view.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Chat view with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access comments for this view.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        IAsyncEnumerable<Comment> GetChatViewCommentsStreamAsync(
            string viewId,
            long? start = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all comments from a specific Chat view.
        /// </summary>
        /// <param name="viewId">The unique identifier of the Chat view for which to retrieve comments.</param>
        /// <param name="start">Optional. A Unix timestamp (in milliseconds) indicating the starting point from which to retrieve comments.</param>
        /// <param name="startId">Optional. The ID of a comment from which to start pagination.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Comment"/> objects for the specified Chat view.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Chat view with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access comments for this view.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<Comment>> GetChatViewCommentsAsync(
            string viewId,
            long? start = null,
            string? startId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new comment to a specific Chat view.
        /// </summary>
        /// <param name="viewId">The unique identifier of the Chat view to which the comment will be added.</param>
        /// <param name="createCommentRequest">An object containing the details of the comment to be created.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateCommentResponse"/> object with details of the created comment.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="viewId"/> or <paramref name="createCommentRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Chat view with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create a comment on this view.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CreateCommentResponse> CreateChatViewCommentAsync(
            string viewId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default);
    }
}