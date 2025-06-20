using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Checklists;
using ClickUp.Api.Client.Models.RequestModels.Checklists; // Assuming Checklist is here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the CuTask Checklists operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - POST /v2/task/{task_id}/checklist
    /// - PUT /v2/checklist/{checklist_id}
    /// - DELETE /v2/checklist/{checklist_id}
    /// - POST /v2/checklist/{checklist_id}/checklist_item
    /// - PUT /v2/checklist/{checklist_id}/checklist_item/{checklist_item_id}
    /// - DELETE /v2/checklist/{checklist_id}/checklist_item/{checklist_item_id}
    /// </remarks>
    public interface ITaskChecklistsService
    {
        /// <summary>
        /// Adds a new checklist to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="createChecklistRequest">Details of the checklist to create.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Checklist"/>.</returns>
        Task<Checklist> CreateChecklistAsync(
            string taskId,
            CreateChecklistRequest createChecklistRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Renames a task checklist or reorders its items.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="editChecklistRequest">Details for editing the checklist (e.g., name, position).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Checklist"/>.</returns>
        Task<Checklist> EditChecklistAsync(
            string checklistId,
            EditChecklistRequest editChecklistRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a checklist from a task.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task DeleteChecklistAsync(
            string checklistId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a line item to a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="createChecklistItemRequest">Details of the checklist item to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated parent <see cref="Checklist"/>.</returns>
        Task<Checklist> CreateChecklistItemAsync(
            string checklistId,
            CreateChecklistItemRequest createChecklistItemRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an individual line item in a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="checklistItemId">The ID of the checklist item.</param>
        /// <param name="editChecklistItemRequest">Details for editing the checklist item.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated parent <see cref="Checklist"/>.</returns>
        Task<Checklist> EditChecklistItemAsync(
            string checklistId,
            string checklistItemId,
            EditChecklistItemRequest editChecklistItemRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a line item from a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="checklistItemId">The ID of the checklist item to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task DeleteChecklistItemAsync(
            string checklistId,
            string checklistItemId,
            CancellationToken cancellationToken = default);
    }
}
