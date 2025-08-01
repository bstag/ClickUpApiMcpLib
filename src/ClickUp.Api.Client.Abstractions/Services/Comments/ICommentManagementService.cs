using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments;

namespace ClickUp.Api.Client.Abstractions.Services.Comments
{
    /// <summary>
    /// Service interface for ClickUp Comment Management operations.
    /// Handles general comment CRUD operations and threaded comment management.
    /// </summary>
    /// <remarks>
    /// This interface follows the Interface Segregation Principle by focusing solely on comment management operations.
    /// Covered API Endpoints:
    /// - General Comment Operations: `PUT /comment/{comment_id}`, `DELETE /comment/{comment_id}`
    /// - Threaded Comments: `GET /comment/{comment_id}/reply`, `POST /comment/{comment_id}/reply`
    /// </remarks>
    public interface ICommentManagementService
    {
        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment to update.</param>
        /// <param name="updateCommentRequest">An object containing the properties to update for the comment, such as its text or assignees.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="commentId"/> or <paramref name="updateCommentRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the comment with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this comment.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task UpdateCommentAsync(
            string commentId,
            UpdateCommentRequest updateCommentRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified comment.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="commentId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the comment with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this comment.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteCommentAsync(
            string commentId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all threaded comments (replies) for a specified parent comment. The parent comment itself is not included in the results.
        /// </summary>
        /// <param name="commentId">The unique identifier of the parent comment for which to retrieve threaded replies.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of threaded <see cref="Comment"/> objects.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="commentId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the parent comment with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access threaded comments for this parent comment.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<Comment>> GetThreadedCommentsAsync(
            string commentId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new threaded comment (a reply) to a specified parent comment.
        /// </summary>
        /// <param name="commentId">The unique identifier of the parent comment to which this threaded comment will be a reply.</param>
        /// <param name="createCommentRequest">An object containing the details of the threaded comment to be created.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateCommentResponse"/> object with details of the created threaded comment.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="commentId"/> or <paramref name="createCommentRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the parent comment with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create a threaded comment in reply to the parent comment.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CreateCommentResponse> CreateThreadedCommentAsync(
            string commentId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default);
    }
}