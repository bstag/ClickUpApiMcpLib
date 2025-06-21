using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Comments; // Assuming Comment DTO is here
using ClickUp.Api.Client.Models.RequestModels.Comments; // Assuming Request DTOs are here
using ClickUp.Api.Client.Models.ResponseModels.Comments; // Assuming Response DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Comments operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/task/{task_id}/comment
    /// - POST /v2/task/{task_id}/comment
    /// - GET /v2/view/{view_id}/comment
    /// - POST /v2/view/{view_id}/comment
    /// - GET /v2/list/{list_id}/comment
    /// - POST /v2/list/{list_id}/comment
    /// - PUT /v2/comment/{comment_id}
    /// - DELETE /v2/comment/{comment_id}
    /// - GET /v2/comment/{comment_id}/reply (Get Threaded Comments)
    /// - POST /v2/comment/{comment_id}/reply (Create Threaded Comment)
    /// </remarks>
    public interface ICommentsService // Corrected from ICommentService in my thoughts, but file is ICommentsService
    {
        /// <summary>
        /// Retrieves comments for a specific task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="start">Optional. Unix time in milliseconds to start comments from.</param>
        /// <param name="startId">Optional. Comment ID to start pagination from.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Comment"/> objects for the task.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access comments for this task.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<Comment>> GetTaskCommentsAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            long? start = null, // Changed int? to long? for Unix time ms
            string? startId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new comment to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="createCommentRequest">Details of the comment to create.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="CreateCommentResponse"/> object containing details of the created comment (often includes the comment ID and the comment itself).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="createCommentRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create a comment on this task.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<CreateCommentResponse> CreateTaskCommentAsync(
            string taskId,
            CreateCommentRequest createCommentRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves comments from a Chat view.
        /// </summary>
        /// <param name="viewId">The ID of the Chat view.</param>
        /// <param name="start">Optional. Unix time in milliseconds to start comments from.</param>
        /// <param name="startId">Optional. Comment ID to start pagination from.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Comment"/> objects for the Chat view.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Chat view with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access comments for this view.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<Comment>> GetChatViewCommentsAsync(
            string viewId,
            long? start = null, // Changed int? to long? for Unix time ms
            string? startId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new comment to a Chat view.
        /// </summary>
        /// <param name="viewId">The ID of the Chat view.</param>
        /// <param name="createCommentRequest">Details of the comment to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="CreateCommentResponse"/> object containing details of the created comment.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="viewId"/> or <paramref name="createCommentRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Chat view with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create a comment on this view.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<CreateCommentResponse> CreateChatViewCommentAsync(
            string viewId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves comments added to a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="start">Optional. Unix time in milliseconds to start comments from.</param>
        /// <param name="startId">Optional. Comment ID to start pagination from.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Comment"/> objects for the List.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the list with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access comments for this list.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<Comment>> GetListCommentsAsync(
            string listId,
            long? start = null, // Changed int? to long? for Unix time ms
            string? startId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a comment to a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="createCommentRequest">Details of the comment to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="CreateCommentResponse"/> object containing details of the created comment.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="createCommentRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the list with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create a comment on this list.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<CreateCommentResponse> CreateListCommentAsync(
            string listId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment to update.</param>
        /// <param name="updateCommentRequest">Details for updating the comment.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="commentId"/> or <paramref name="updateCommentRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the comment with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this comment.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task UpdateCommentAsync(
            string commentId,
            UpdateCommentRequest updateCommentRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="commentId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the comment with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this comment.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task DeleteCommentAsync(
            string commentId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves threaded comments for a parent comment. The parent comment is not included.
        /// </summary>
        /// <param name="commentId">The ID of the parent comment.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of threaded <see cref="Comment"/> objects.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="commentId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the parent comment with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access threaded comments.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<Comment>> GetThreadedCommentsAsync(
            string commentId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a threaded comment (reply) to a parent comment.
        /// </summary>
        /// <param name="commentId">The ID of the parent comment.</param>
        /// <param name="createCommentRequest">Details of the threaded comment to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="commentId"/> or <paramref name="createCommentRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the parent comment with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create a threaded comment.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task CreateThreadedCommentAsync(
            string commentId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default);
    }
}
