using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Task Checklists operations in the ClickUp API
    // Based on endpoints like:
    // - POST /v2/task/{task_id}/checklist
    // - PUT /v2/checklist/{checklist_id}
    // - DELETE /v2/checklist/{checklist_id}
    // - POST /v2/checklist/{checklist_id}/checklist_item
    // - PUT /v2/checklist/{checklist_id}/checklist_item/{checklist_item_id}
    // - DELETE /v2/checklist/{checklist_id}/checklist_item/{checklist_item_id}

    public interface ITaskChecklistsService
    {
        /// <summary>
        /// Adds a new checklist to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="createChecklistRequest">Details of the checklist to create.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created checklist.</returns>
        Task<object> CreateChecklistAsync(string taskId, object createChecklistRequest, bool? customTaskIds = null, double? teamId = null);
        // Note: createChecklistRequest should be CreateChecklistRequest, return type should be ChecklistDto.

        /// <summary>
        /// Renames a task checklist or reorders it.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="editChecklistRequest">Details for editing the checklist.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task EditChecklistAsync(string checklistId, object editChecklistRequest);
        // Note: editChecklistRequest should be EditChecklistRequest. API returns 200 with an empty object.

        /// <summary>
        /// Deletes a checklist from a task.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteChecklistAsync(string checklistId);
        // Note: API returns 200 with an empty object.

        /// <summary>
        /// Adds a line item to a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="createChecklistItemRequest">Details of the checklist item to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated checklist.</returns>
        Task<object> CreateChecklistItemAsync(string checklistId, object createChecklistItemRequest);
        // Note: createChecklistItemRequest should be CreateChecklistItemRequest, return type should be ChecklistDto (representing the parent checklist).

        /// <summary>
        /// Updates an individual line item in a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="checklistItemId">The ID of the checklist item.</param>
        /// <param name="editChecklistItemRequest">Details for editing the checklist item.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated checklist.</returns>
        Task<object> EditChecklistItemAsync(string checklistId, string checklistItemId, object editChecklistItemRequest);
        // Note: editChecklistItemRequest should be EditChecklistItemRequest, return type should be ChecklistDto (representing the parent checklist).

        /// <summary>
        /// Deletes a line item from a task checklist.
        /// </summary>
        /// <param name="checklistId">The ID of the checklist.</param>
        /// <param name="checklistItemId">The ID of the checklist item to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteChecklistItemAsync(string checklistId, string checklistItemId);
        // Note: API returns 200 with an empty object.
    }
}
