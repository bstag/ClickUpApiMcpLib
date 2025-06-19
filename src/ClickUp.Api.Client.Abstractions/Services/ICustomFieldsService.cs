using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models; // Assuming CustomFieldModel and SetCustomFieldValueRequest will be in this namespace

namespace ClickUp.Api.Client.Abstractions
{
    // Represents the Custom Fields operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/list/{list_id}/field
    // - GET /v2/folder/{folder_id}/field
    // - GET /v2/space/{space_id}/field
    // - GET /v2/team/{team_id}/field
    // - POST /v2/task/{task_id}/field/{field_id}
    // - DELETE /v2/task/{task_id}/field/{field_id}

    public interface ICustomFieldsService
    {
        /// <summary>
        /// Retrieves the Custom Fields available in a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <returns>A list of Custom Fields for the List.</returns>
        Task<IEnumerable<object>> GetAccessibleCustomFieldsAsync(double listId);
        // Note: Return type should be IEnumerable<CustomFieldDto>.

        /// <summary>
        /// Retrieves the Custom Fields available in a specific Folder.
        /// Only returns Custom Fields created at the Folder level.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <returns>A list of Custom Fields for the Folder.</returns>
        Task<IEnumerable<object>> GetFolderCustomFieldsAsync(double folderId);
        // Note: Return type should be IEnumerable<CustomFieldDto>.

        /// <summary>
        /// Retrieves the Custom Fields available in a specific Space.
        /// Only returns Custom Fields created at the Space level.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <returns>A list of Custom Fields for the Space.</returns>
        Task<IEnumerable<object>> GetSpaceCustomFieldsAsync(double spaceId);
        // Note: Return type should be IEnumerable<CustomFieldDto>.

        /// <summary>
        /// Retrieves the Custom Fields available in a specific Workspace.
        /// Only returns Custom Fields created at the Workspace level.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <returns>A list of Custom Fields for the Workspace.</returns>
        Task<IEnumerable<object>> GetWorkspaceCustomFieldsAsync(double workspaceId);
        // Note: Return type should be IEnumerable<CustomFieldDto>.

        /// <summary>
        /// Adds or updates data in a Custom Field on a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="fieldId">The UUID of the Custom Field.</param>
        /// <param name="setFieldValueRequest">The value to set for the Custom Field.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task SetCustomFieldValueAsync(string taskId, string fieldId, object setFieldValueRequest, bool? customTaskIds = null, double? teamId = null);
        // Note: setFieldValueRequest should be a specific DTO (e.g. SetCustomFieldValueRequest) that can accommodate various value types.
        // API returns 200 with an empty object.

        /// <summary>
        /// Removes data from a Custom Field on a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="fieldId">The UUID of the Custom Field.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task RemoveCustomFieldValueAsync(string taskId, string fieldId, bool? customTaskIds = null, double? teamId = null);
        // Note: API returns 200 with an empty object.
    }
}
