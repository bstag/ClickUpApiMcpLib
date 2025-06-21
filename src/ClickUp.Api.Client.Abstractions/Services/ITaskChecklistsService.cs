using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Checklists;
using ClickUp.Api.Client.Models.RequestModels.Checklists; // Assuming Checklist is here
using ClickUp.Api.Client.Models.ResponseModels.Checklists;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Task Checklists operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// This service allows for managing checklists and their items within tasks.
    /// Based on ClickUp API endpoints like:
    /// <list type="bullet">
    /// <item><description>POST /v2/task/{task_id}/checklist</description></item>
    /// <item><description>PUT /v2/checklist/{checklist_id}</description></item>
    /// <item><description>DELETE /v2/checklist/{checklist_id}</description></item>
    /// <item><description>POST /v2/checklist/{checklist_id}/checklist_item</description></item>
    /// <item><description>PUT /v2/checklist/{checklist_id}/checklist_item/{checklist_item_id}</description></item>
    /// <item><description>DELETE /v2/checklist/{checklist_id}/checklist_item/{checklist_item_id}</description></item>
    /// </list>
    /// </remarks>
    public interface ITaskChecklistsService
    {
        /// <summary>
        /// Adds a new checklist to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task to add the checklist to.</param>
        /// <param name="createChecklistRequest">The request object containing details for the new checklist, such as its name.</param>
        /// <param name="customTaskIds">Optional. If true, Task ID is treated as a custom task ID. Requires <paramref name="teamId"/>.</param>
        /// <param name="teamId">Optional. The Workspace ID, required if <paramref name="customTaskIds"/> is true.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="CreateChecklistResponse"/> with details of the created checklist.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="createChecklistRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid task ID or authentication issues.</exception>
        Task<CreateChecklistResponse> CreateChecklistAsync(
            string taskId,
            CreateChecklistRequest createChecklistRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Renames a task checklist or reorders its items.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist to edit.</param>
        /// <param name="editChecklistRequest">The request object containing the new name or position for the checklist.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The API returns the updated checklist, so this could return <see cref="Checklist"/> or a specific response if available.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="checklistId"/> or <paramref name="editChecklistRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid checklist ID or authentication issues.</exception>
        Task EditChecklistAsync( // API returns the updated checklist object. Consider returning Task<Checklist>
            string checklistId,
            EditChecklistRequest editChecklistRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a checklist from a task.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="checklistId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid checklist ID or authentication issues.</exception>
        System.Threading.Tasks.Task DeleteChecklistAsync(
            string checklistId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new item to a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist to add the item to.</param>
        /// <param name="createChecklistItemRequest">The request object containing details for the new checklist item, such as its name and assignee.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="CreateChecklistItemResponse"/> with details of the created checklist item.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="checklistId"/> or <paramref name="createChecklistItemRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid checklist ID or authentication issues.</exception>
        Task<CreateChecklistItemResponse> CreateChecklistItemAsync(
            string checklistId,
            CreateChecklistItemRequest createChecklistItemRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an individual line item in a task checklist. This can include renaming, reordering, or changing its resolved state.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist containing the item.</param>
        /// <param name="checklistItemId">The ID of the checklist item to edit.</param>
        /// <param name="editChecklistItemRequest">The request object containing the updated details for the checklist item.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="EditChecklistItemResponse"/> with details of the updated checklist item.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="checklistId"/>, <paramref name="checklistItemId"/>, or <paramref name="editChecklistItemRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid IDs or authentication issues.</exception>
        Task<EditChecklistItemResponse> EditChecklistItemAsync(
            string checklistId,
            string checklistItemId,
            EditChecklistItemRequest editChecklistItemRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a line item from a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist containing the item.</param>
        /// <param name="checklistItemId">The ID of the checklist item to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="checklistId"/> or <paramref name="checklistItemId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid IDs or authentication issues.</exception>
        System.Threading.Tasks.Task DeleteChecklistItemAsync(
            string checklistId,
            string checklistItemId,
            CancellationToken cancellationToken = default);
    }
}
