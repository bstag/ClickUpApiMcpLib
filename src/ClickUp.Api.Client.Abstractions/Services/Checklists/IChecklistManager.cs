using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists;

namespace ClickUp.Api.Client.Abstractions.Services.Checklists
{
    /// <summary>
    /// Service interface for ClickUp Checklist operations.
    /// Handles checklist-level operations such as creating, editing, and deleting checklists.
    /// </summary>
    public interface IChecklistManager
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
    }
}