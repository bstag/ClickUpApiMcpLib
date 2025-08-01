using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments;

namespace ClickUp.Api.Client.Abstractions.Services.Comments
{
    /// <summary>
    /// Service interface for ClickUp Task Comment operations.
    /// Handles comment operations specifically related to tasks.
    /// </summary>
    /// <remarks>
    /// This interface follows the Interface Segregation Principle by focusing solely on task-related comment operations.
    /// Covered API Endpoints:
    /// - Task Comments: `GET /task/{task_id}/comment`, `POST /task/{task_id}/comment`
    /// </remarks>
    public interface ITaskCommentService
    {
        /// <summary>
        /// Retrieves all comments associated with a specific task.
        /// </summary>
        /// <param name="requestModel">The request model containing parameters like Task ID, custom task ID flags, team ID, and pagination options (start, startId).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Comment"/> objects for the specified task.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="requestModel"/> or its required TaskId is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="requestModel"/> specifies customTaskIds as true but teamId is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access comments for this task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IEnumerable<Comment>> GetTaskCommentsAsync(
            GetTaskCommentsRequest requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all comments associated with a specific task as an asynchronous stream.
        /// This method handles pagination internally, yielding comments as they are fetched.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task for which to retrieve comments.</param>
        /// <param name="requestModel">An object containing options for custom task ID handling and pagination.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields <see cref="Comment"/> objects for the specified task.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="requestModel"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="requestModel"/> specifies CustomTaskIds as true but TeamId is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access comments for this task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        IAsyncEnumerable<Comment> GetTaskCommentsStreamAsync(
            string taskId,
            GetTaskCommentsRequest requestModel,
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
            CreateTaskCommentRequest createCommentRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}