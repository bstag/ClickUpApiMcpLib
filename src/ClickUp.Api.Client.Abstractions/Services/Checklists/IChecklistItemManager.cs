using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;

namespace ClickUp.Api.Client.Abstractions.Services.Checklists
{
    /// <summary>
    /// Service interface for ClickUp Checklist Item operations.
    /// Handles individual checklist item operations such as creating, editing, and deleting checklist items.
    /// </summary>
    public interface IChecklistItemManager
    {
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