using System.Threading;
using System.Threading.Tasks;
// Assuming a placeholder for request/response models for now
// using ClickUp.Net.Models;

namespace ClickUp.Net.Abstractions.Services
{
    /// <summary>
    /// Interface for services interacting with ClickUp Task Checklists.
    /// </summary>
    public interface ITaskChecklistService
    {
        /// <summary>
        /// Adds a new checklist to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="customTaskIds">If referencing task by custom id, set to true.</param>
        /// <param name="teamId">The team ID if using custom task IDs.</param>
        /// <param name="checklistName">The name of the checklist to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the created checklist response.</returns>
        Task<object> CreateChecklistAsync(string taskId, string checklistName, bool? customTaskIds = null, double? teamId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Renames a task checklist or reorders it.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="name">The new name for the checklist.</param>
        /// <param name="position">The new position for the checklist.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task indicating completion.</returns>
        Task EditChecklistAsync(string checklistId, string name = null, int? position = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a checklist from a task.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task indicating completion.</returns>
        Task DeleteChecklistAsync(string checklistId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new line item to a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="itemName">The name of the checklist item.</param>
        /// <param name="assignee">The ID of the user to assign to this item.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the created checklist item response.</returns>
        Task<object> CreateChecklistItemAsync(string checklistId, string itemName, int? assignee = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an individual line item in a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="checklistItemId">The ID of the checklist item.</param>
        /// <param name="name">The new name for the checklist item.</param>
        /// <param name="assignee">The new assignee for the item (can be null).</param>
        /// <param name="resolved">Whether the item is resolved.</param>
        /// <param name="parent">The parent checklist item ID to nest under (can be null).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the updated checklist item response.</returns>
        Task<object> EditChecklistItemAsync(string checklistId, string checklistItemId, string name = null, object assignee = null, bool? resolved = null, string parent = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a line item from a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="checklistItemId">The ID of the checklist item to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task indicating completion.</returns>
        Task DeleteChecklistItemAsync(string checklistId, string checklistItemId, CancellationToken cancellationToken = default);
    }
}
