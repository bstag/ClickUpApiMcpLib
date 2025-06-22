using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Task Checklists operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for managing Checklists and their individual items within Tasks.
    /// It allows for creating, editing, and deleting Checklists and Checklist Items.
    /// Covered API Endpoints:
    /// - Create Checklist: `POST /task/{task_id}/checklist`
    /// - Edit Checklist: `PUT /checklist/{checklist_id}`
    /// - Delete Checklist: `DELETE /checklist/{checklist_id}`
    /// - Create Checklist Item: `POST /checklist/{checklist_id}/checklist_item`
    /// - Edit Checklist Item: `PUT /checklist/{checklist_id}/checklist_item/{checklist_item_id}`
    /// - Delete Checklist Item: `DELETE /checklist/{checklist_id}/checklist_item/{checklist_item_id}`
    /// </remarks>
    public interface ITaskChecklistsService
    {
        /// <summary>
        /// Adds a new Checklist to a specified Task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the Task to which the Checklist will be added.</param>
        /// <param name="createChecklistRequest">An object containing details for the new Checklist, such as its name.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateChecklistResponse"/> object with details of the created Checklist.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="createChecklistRequest"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Task with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add Checklists to this Task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<CreateChecklistResponse> CreateChecklistAsync(
            string taskId,
            CreateChecklistRequest createChecklistRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Edits an existing Checklist, allowing for renaming or reordering of its items.
        /// </summary>
        /// <param name="checklistId">The unique identifier of the Checklist to edit.</param>
        /// <param name="editChecklistRequest">An object containing the properties to update for the Checklist, such as its name or the order of its items.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The API typically returns the updated Checklist object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="checklistId"/> or <paramref name="editChecklistRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Checklist with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit this Checklist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task EditChecklistAsync(
            string checklistId,
            EditChecklistRequest editChecklistRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Checklist from a Task.
        /// </summary>
        /// <param name="checklistId">The unique identifier of the Checklist to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="checklistId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Checklist with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this Checklist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteChecklistAsync(
            string checklistId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new item to an existing Checklist.
        /// </summary>
        /// <param name="checklistId">The unique identifier of the Checklist to which the item will be added.</param>
        /// <param name="createChecklistItemRequest">An object containing details for the new Checklist item, such as its name and assignee.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateChecklistItemResponse"/> object with details of the created Checklist item.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="checklistId"/> or <paramref name="createChecklistItemRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Checklist with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add items to this Checklist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<CreateChecklistItemResponse> CreateChecklistItemAsync(
            string checklistId,
            CreateChecklistItemRequest createChecklistItemRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an individual item within a Checklist. This can include renaming the item, reordering it, assigning it, or changing its resolved state.
        /// </summary>
        /// <param name="checklistId">The unique identifier of the Checklist containing the item to be edited.</param>
        /// <param name="checklistItemId">The unique identifier of the Checklist item to edit.</param>
        /// <param name="editChecklistItemRequest">An object containing the updated details for the Checklist item.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="EditChecklistItemResponse"/> object with details of the updated Checklist item.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="checklistId"/>, <paramref name="checklistItemId"/>, or <paramref name="editChecklistItemRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Checklist or Checklist item with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit this Checklist item.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<EditChecklistItemResponse> EditChecklistItemAsync(
            string checklistId,
            string checklistItemId,
            EditChecklistItemRequest editChecklistItemRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an individual item from a Checklist.
        /// </summary>
        /// <param name="checklistId">The unique identifier of the Checklist containing the item to be deleted.</param>
        /// <param name="checklistItemId">The unique identifier of the Checklist item to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="checklistId"/> or <paramref name="checklistItemId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Checklist or Checklist item with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this Checklist item.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteChecklistItemAsync(
            string checklistId,
            string checklistItemId,
            CancellationToken cancellationToken = default);
    }
}
