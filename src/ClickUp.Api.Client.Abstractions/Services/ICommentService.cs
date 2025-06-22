using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Comment operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for managing comments on Tasks, Lists, and Chat Views,
    /// including creating, retrieving, updating, and deleting comments, as well as handling threaded comments.
    /// Covered API Endpoints:
    /// - Task Comments: `GET /task/{task_id}/comment`, `POST /task/{task_id}/comment`
    /// - Chat View Comments: `GET /view/{view_id}/comment`, `POST /view/{view_id}/comment`
    /// - List Comments: `GET /list/{list_id}/comment`, `POST /list/{list_id}/comment`
    /// - General Comment Operations: `PUT /comment/{comment_id}`, `DELETE /comment/{comment_id}`
    /// - Threaded Comments: `GET /comment/{comment_id}/reply`, `POST /comment/{comment_id}/reply`
    /// </remarks>
    public interface ICommentsService
    {
        /// <summary>
        /// Retrieves all comments associated with a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task for which to retrieve comments.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="start">Optional. A Unix timestamp (in milliseconds) indicating the starting point from which to retrieve comments. Only comments created at or after this time will be returned.</param>
        /// <param name="startId">Optional. The ID of a comment from which to start pagination. If provided, comments created after this comment will be returned.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Comment"/> objects for the specified task.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access comments for this task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IEnumerable<Comment>> GetTaskCommentsAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            long? start = null,
            string? startId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new comment to a specified task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task to which the comment will be added.</param>
        /// <param name="createCommentRequest">An object containing the details of the comment to be created, such as its content and any assignees.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateCommentResponse"/> object with details of the created comment.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="createCommentRequest"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create a comment on this task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CreateCommentResponse> CreateTaskCommentAsync(
            string taskId,
            CreateCommentRequest createCommentRequest,
            bool? customTaskIds = null,
            string? teamId = null,
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

        /// <summary>
        /// Retrieves all comments associated with a specific List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List for which to retrieve comments.</param>
        /// <param name="start">Optional. A Unix timestamp (in milliseconds) indicating the starting point from which to retrieve comments.</param>
        /// <param name="startId">Optional. The ID of a comment from which to start pagination.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Comment"/> objects for the specified List.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access comments for this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<Comment>> GetListCommentsAsync(
            string listId,
            long? start = null,
            string? startId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new comment to a specific List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List to which the comment will be added.</param>
        /// <param name="createCommentRequest">An object containing the details of the comment to be created.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateCommentResponse"/> object with details of the created comment.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="createCommentRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create a comment on this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CreateCommentResponse> CreateListCommentAsync(
            string listId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default);

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
