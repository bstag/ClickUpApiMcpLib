using ClickUp.Api.Client.Abstractions.Services.Comments;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Comprehensive service interface for ClickUp Comment operations.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing comments on Tasks, Lists, and Chat Views,
    /// including creating, retrieving, updating, and deleting comments, as well as handling threaded comments.
    /// The interface is composed of:
    /// - ITaskCommentService: For task-related comment operations
    /// - IListCommentService: For list-related comment operations
    /// - IChatViewCommentService: For chat view-related comment operations
    /// - ICommentManagementService: For general comment CRUD and threaded comment operations
    /// Covered API Endpoints:
    /// - Task Comments: `GET /task/{task_id}/comment`, `POST /task/{task_id}/comment`
    /// - Chat View Comments: `GET /view/{view_id}/comment`, `POST /view/{view_id}/comment`
    /// - List Comments: `GET /list/{list_id}/comment`, `POST /list/{list_id}/comment`
    /// - General Comment Operations: `PUT /comment/{comment_id}`, `DELETE /comment/{comment_id}`
    /// - Threaded Comments: `GET /comment/{comment_id}/reply`, `POST /comment/{comment_id}/reply`
    /// </remarks>
    public interface ICommentsService : ITaskCommentService, IListCommentService, IChatViewCommentService, ICommentManagementService
    {
        // All methods are now inherited from the composed interfaces:
        // - ITaskCommentService: Task-related comment operations
        // - IListCommentService: List-related comment operations
        // - IChatViewCommentService: Chat view-related comment operations
        // - ICommentManagementService: General comment CRUD and threaded comment operations


    }
}
