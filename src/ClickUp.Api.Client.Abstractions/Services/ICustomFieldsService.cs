using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.CustomFields; // Assuming CustomField DTO is here
using ClickUp.Api.Client.Models.RequestModels.CustomFields; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services // Corrected namespace
{
    /// <summary>
    /// Represents the Custom Fields operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/list/{list_id}/field
    /// - GET /v2/folder/{folder_id}/field
    /// - GET /v2/space/{space_id}/field
    /// - GET /v2/team/{team_id}/field (Workspace custom fields)
    /// - POST /v2/task/{task_id}/field/{field_id} (Set value)
    /// - DELETE /v2/task/{task_id}/field/{field_id} (Remove value)
    /// </remarks>
    public interface ICustomFieldsService
    {
        /// <summary>
        /// Retrieves the Custom Fields accessible from a specific List. This includes fields from parent Folders, Spaces, and Workspace.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Field"/> objects accessible from the List.</returns>
        Task<IEnumerable<Field>> GetAccessibleCustomFieldsAsync(
            string listId,
            CancellationToken cancellationToken = default);

        // Note: The API also provides specific endpoints for fields created AT folder/space/team level.
        // The original interface had separate methods for these. If GetAccessibleCustomFieldsAsync covers most needs,
        // these might be less critical or could be achieved by filtering client-side if GetAccessibleCustomFieldsAsync
        // provides enough origin information. For now, I'll keep them as they were defined, assuming they fetch fields
        // defined *at* that level only, as per original comments.

        /// <summary>
        /// Retrieves the Custom Fields created at the Folder level.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Field"/> objects for the Folder.</returns>
        Task<IEnumerable<Field>> GetFolderCustomFieldsAsync(
            string folderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the Custom Fields created at the Space level.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Field"/> objects for the Space.</returns>
        Task<IEnumerable<Field>> GetSpaceCustomFieldsAsync(
            string spaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the Custom Fields created at the Workspace (Team) level.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Field"/> objects for the Workspace.</returns>
        Task<IEnumerable<Field>> GetWorkspaceCustomFieldsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds or updates data in a Custom Field on a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="fieldId">The UUID of the Custom Field.</param>
        /// <param name="setFieldValueRequest">The request DTO containing the value to set for the Custom Field. The 'value' property within this DTO might be of type object or a specific structure depending on the field type.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task SetCustomFieldValueAsync(
            string taskId,
            string fieldId,
            SetCustomFieldValueRequest setFieldValueRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes data from a Custom Field on a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="fieldId">The UUID of the Custom Field.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task RemoveCustomFieldValueAsync(
            string taskId,
            string fieldId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}
