using ClickUp.Api.Client.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions
{
    /// <summary>
    /// Defines the contract for a service that interacts with ClickUp Comments.
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Get comments from a task.
        /// </summary>
        /// <remarks>
        /// Operation ID: GetTaskComments
        /// Summary: Get Task Comments
        /// </remarks>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="customTaskIds">If you want to reference a task by it's custom task ID, this value must be `true`.</param>
        /// <param name="teamId">Only used when the `custom_task_ids` parameter is set to `true`.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of <see cref="Comment"/> objects.</returns>
        Task<GetTaskCommentsResponse> GetTaskCommentsAsync(string taskId, bool? customTaskIds, double? teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get comments from a List.
        /// </summary>
        /// <remarks>
        /// Operation ID: GetListComments
        /// Summary: Get List Comments
        /// </remarks>
        /// <param name="listId">The ID of the list.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of <see cref="Comment"/> objects.</returns>
        Task<GetListCommentsResponse> GetListCommentsAsync(string listId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get comments from a Chat view.
        /// </summary>
        /// <remarks>
        /// Operation ID: GetChatViewComments
        /// Summary: Get Chat View Comments
        /// </remarks>
        /// <param name="viewId">The ID of the view.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of <see cref="Comment"/> objects.</returns>
        Task<GetChatViewCommentsResponse> GetChatViewCommentsAsync(string viewId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add a comment to a task.
        /// </summary>
        /// <remarks>
        /// Operation ID: CreateTaskComment
        /// Summary: Create Task Comment
        /// </remarks>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="request">The <see cref="CreateTaskCommentRequest"/> object.</param>
        /// <param name="customTaskIds">If you want to reference a task by it's custom task ID, this value must be `true`.</param>
        /// <param name="teamId">Only used when the `custom_task_ids` parameter is set to `true`.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The created <see cref="CreateTaskCommentResponse"/> object.</returns>
        Task<CreateTaskCommentResponse> CreateTaskCommentAsync(string taskId, CreateTaskCommentRequest request, bool? customTaskIds, double? teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add a comment to a List.
        /// </summary>
        /// <remarks>
        /// Operation ID: CreateListComment
        /// Summary: Create List Comment
        /// </remarks>
        /// <param name="listId">The ID of the list.</param>
        /// <param name="request">The <see cref="CreateListCommentRequest"/> object.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The created <see cref="CreateCommentresponse"/> object.</returns>
        Task<CreateCommentresponse> CreateListCommentAsync(string listId, CreateListCommentRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add a comment to a Chat view.
        /// </summary>
        /// <remarks>
        /// Operation ID: CreateChatViewComment
        /// Summary: Create Chat View Comment
        /// </remarks>
        /// <param name="viewId">The ID of the view.</param>
        /// <param name="request">The <see cref="CreateChatViewCommentRequest"/> object.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The created <see cref="CreateCommentresponse"/> object.</returns>
        Task<CreateCommentresponse> CreateChatViewCommentAsync(string viewId, CreateChatViewCommentRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a comment.
        /// </summary>
        /// <remarks>
        /// Operation ID: UpdateComment
        /// Summary: Update Comment
        /// </remarks>
        /// <param name="commentId">The ID of the comment.</param>
        /// <param name="request">The <see cref="UpdateCommentRequest"/> object.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateCommentAsync(double commentId, UpdateCommentRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a comment.
        /// </summary>
        /// <remarks>
        /// Operation ID: DeleteComment
        /// Summary: Delete Comment
        /// </remarks>
        /// <param name="commentId">The ID of the comment.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteCommentAsync(double commentId, CancellationToken cancellationToken = default);
    }
}
