using System.Threading;
using System.Threading.Tasks;
// Assuming a placeholder for request/response models for now
// using ClickUp.Net.Models;

namespace ClickUp.Net.Abstractions.Services
{
    /// <summary>
    /// Interface for services interacting with ClickUp Comments.
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Gets comments for a specific task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="customTaskIds">If referencing task by custom id, set to true.</param>
        /// <param name="teamId">The team ID if using custom task IDs.</param>
        /// <param name="start">Unix time in milliseconds for pagination.</param>
        /// <param name="startId">Comment ID for pagination.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the list of comments.</returns>
        Task<object> GetTaskCommentsAsync(string taskId, bool? customTaskIds = null, double? teamId = null, int? start = null, string startId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new comment to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="commentText">The text of the comment.</param>
        /// <param name="assignee">User ID of the assignee for the comment.</param>
        /// <param name="groupAssignee">Group ID of the assignee for the comment.</param>
        /// <param name="notifyAll">Whether to notify all users.</param>
        /// <param name="customTaskIds">If referencing task by custom id, set to true.</param>
        /// <param name="teamId">The team ID if using custom task IDs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the created comment response.</returns>
        Task<object> CreateTaskCommentAsync(string taskId, string commentText, bool notifyAll, int? assignee = null, string groupAssignee = null, bool? customTaskIds = null, double? teamId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets comments from a Chat view.
        /// </summary>
        /// <param name="viewId">The ID of the Chat view.</param>
        /// <param name="start">Unix time in milliseconds for pagination.</param>
        /// <param name="startId">Comment ID for pagination.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the list of comments.</returns>
        Task<object> GetChatViewCommentsAsync(string viewId, int? start = null, string startId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new comment to a Chat view.
        /// </summary>
        /// <param name="viewId">The ID of the Chat view.</param>
        /// <param name="commentText">The text of the comment.</param>
        /// <param name="notifyAll">Whether to notify all users.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the created comment response.</returns>
        Task<object> CreateChatViewCommentAsync(string viewId, string commentText, bool notifyAll, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets comments for a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="start">Unix time in milliseconds for pagination.</param>
        /// <param name="startId">Comment ID for pagination.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the list of comments.</returns>
        Task<object> GetListCommentsAsync(double listId, int? start = null, string startId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a comment to a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="commentText">The text of the comment.</param>
        /// <param name="assignee">User ID of the assignee for the comment.</param>
        /// <param name="notifyAll">Whether to notify all users.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the created comment response.</returns>
        Task<object> CreateListCommentAsync(double listId, string commentText, int assignee, bool notifyAll, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment.</param>
        /// <param name="commentText">The new text for the comment.</param>
        /// <param name="assignee">The new user ID of the assignee.</param>
        /// <param name="groupAssignee">The new group ID of the assignee.</param>
        /// <param name="resolved">Whether the comment is resolved.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task indicating completion.</returns>
        Task UpdateCommentAsync(double commentId, string commentText, int assignee, bool resolved, int? groupAssignee = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task indicating completion.</returns>
        Task DeleteCommentAsync(double commentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets threaded comments for a parent comment.
        /// </summary>
        /// <param name="commentId">The ID of the parent comment.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the list of threaded comments.</returns>
        Task<object> GetThreadedCommentsAsync(double commentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a threaded comment.
        /// </summary>
        /// <param name="commentId">The ID of the parent comment.</param>
        /// <param name="commentText">The text of the comment.</param>
        /// <param name="assignee">User ID of the assignee for the comment.</param>
        /// <param name="groupAssignee">Group ID of the assignee for the comment.</param>
        /// <param name="notifyAll">Whether to notify all users.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task indicating completion.</returns>
        Task CreateThreadedCommentAsync(double commentId, string commentText, bool notifyAll, int? assignee = null, string groupAssignee = null, CancellationToken cancellationToken = default);
    }
}
