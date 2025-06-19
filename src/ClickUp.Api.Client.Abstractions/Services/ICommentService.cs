using ClickUp.Api.Client.Models;

using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Comments operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/task/{task_id}/comment
    // - POST /v2/task/{task_id}/comment
    // - GET /v2/view/{view_id}/comment
    // - POST /v2/view/{view_id}/comment
    // - GET /v2/list/{list_id}/comment
    // - POST /v2/list/{list_id}/comment
    // - PUT /v2/comment/{comment_id}
    // - DELETE /v2/comment/{comment_id}
    // - GET /v2/comment/{comment_id}/reply (Get Threaded Comments)
    // - POST /v2/comment/{comment_id}/reply (Create Threaded Comment)

    public interface ICommentsService
    {
        /// <summary>
        /// Retrieves comments for a specific task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <param name="start">Optional. Unix time in milliseconds to start comments from.</param>
        /// <param name="startId">Optional. Comment ID to start pagination from.</param>
        /// <returns>A list of comments for the task.</returns>
        Task<IEnumerable<object>> GetTaskCommentsAsync(string taskId, bool? customTaskIds = null, double? teamId = null, int? start = null, string? startId = null);
        // Note: Return type should be IEnumerable<CommentDto>.

        /// <summary>
        /// Adds a new comment to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="createCommentRequest">Details of the comment to create.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>Details of the created comment.</returns>
        Task<object> CreateTaskCommentAsync(string taskId, object createCommentRequest, bool? customTaskIds = null, double? teamId = null);
        // Note: createCommentRequest should be CreateTaskCommentRequest, return type should be CreateCommentResponseDto.

        /// <summary>
        /// Retrieves comments from a Chat view.
        /// </summary>
        /// <param name="viewId">The ID of the Chat view.</param>
        /// <param name="start">Optional. Unix time in milliseconds to start comments from.</param>
        /// <param name="startId">Optional. Comment ID to start pagination from.</param>
        /// <returns>A list of comments for the Chat view.</returns>
        Task<IEnumerable<object>> GetChatViewCommentsAsync(string viewId, int? start = null, string? startId = null);
        // Note: Return type should be IEnumerable<CommentDto>.

        /// <summary>
        /// Adds a new comment to a Chat view.
        /// </summary>
        /// <param name="viewId">The ID of the Chat view.</param>
        /// <param name="createCommentRequest">Details of the comment to create.</param>
        /// <returns>Details of the created comment.</returns>
        Task<object> CreateChatViewCommentAsync(string viewId, object createCommentRequest);
        // Note: createCommentRequest should be CreateChatViewCommentRequest, return type should be CreateCommentResponseDto.

        /// <summary>
        /// Retrieves comments added to a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="start">Optional. Unix time in milliseconds to start comments from.</param>
        /// <param name="startId">Optional. Comment ID to start pagination from.</param>
        /// <returns>A list of comments for the List.</returns>
        Task<IEnumerable<object>> GetListCommentsAsync(double listId, int? start = null, string? startId = null);
        // Note: Return type should be IEnumerable<CommentDto>.

        /// <summary>
        /// Adds a comment to a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="createCommentRequest">Details of the comment to create.</param>
        /// <returns>Details of the created comment.</returns>
        Task<object> CreateListCommentAsync(double listId, object createCommentRequest);
        // Note: createCommentRequest should be CreateListCommentRequest, return type should be CreateCommentResponseDto.

        /// <summary>
        /// Updates a comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment to update.</param>
        /// <param name="updateCommentRequest">Details for updating the comment.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task UpdateCommentAsync(double commentId, object updateCommentRequest);
        // Note: updateCommentRequest should be UpdateCommentRequest. API returns 200 with an empty object.

        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteCommentAsync(double commentId);
        // Note: API returns 200 with an empty object.

        /// <summary>
        /// Retrieves threaded comments for a parent comment. The parent comment is not included.
        /// </summary>
        /// <param name="commentId">The ID of the parent comment.</param>
        /// <returns>A list of threaded comments.</returns>
        Task<IEnumerable<object>> GetThreadedCommentsAsync(double commentId);
        // Note: Return type should be IEnumerable<CommentDto>.

        /// <summary>
        /// Creates a threaded comment.
        /// </summary>
        /// <param name="commentId">The ID of the parent comment.</param>
        /// <param name="createCommentRequest">Details of the threaded comment to create. This is likely similar to CreateTaskCommentRequest.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The API returns 200 with an empty object, consider if a different return is more useful or if specific response DTO exists.</returns>
        Task CreateThreadedCommentAsync(double commentId, object createCommentRequest);
        // Note: createCommentRequest should be a DTO similar to CreateTaskCommentRequest.
    }
}
