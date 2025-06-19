using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models; // Assuming CustomFieldModel and SetCustomFieldValueRequest will be in this namespace

namespace ClickUp.Api.Client.Abstractions
{
    /// <summary>
    /// Defines the contract for a service that interacts with ClickUp Custom Fields.
    /// </summary>
    public interface ICustomFieldsService
    {
        /// <summary>
        /// Get all accessible Custom Fields for a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="CustomFieldModel"/> objects.</returns>
        /// <remarks>
        /// Corresponds to the ClickUp API operation <c>GetAccessibleCustomFields</c> (<c>GET /v2/list/{list_id}/field</c>).
        /// </remarks>
        Task<IEnumerable<CustomFieldModel>> GetAccessibleCustomFieldsAsync(double listId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all available Custom Fields for a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="CustomFieldModel"/> objects.</returns>
        /// <remarks>
        /// Corresponds to the ClickUp API operation <c>getFolderAvailableFields</c> (<c>GET /v2/folder/{folder_id}/field</c>).
        /// </remarks>
        Task<IEnumerable<CustomFieldModel>> GetFolderAvailableFieldsAsync(double folderId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all available Custom Fields for a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="CustomFieldModel"/> objects.</returns>
        /// <remarks>
        /// Corresponds to the ClickUp API operation <c>getSpaceAvailableFields</c> (<c>GET /v2/space/{space_id}/field</c>).
        /// </remarks>
        Task<IEnumerable<CustomFieldModel>> GetSpaceAvailableFieldsAsync(double spaceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all available Custom Fields for a Workspace (Team).
        /// </summary>
        /// <param name="teamId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="CustomFieldModel"/> objects.</returns>
        /// <remarks>
        /// Corresponds to the ClickUp API operation <c>getTeamAvailableFields</c> (<c>GET /v2/team/{team_id}/field</c>).
        /// </remarks>
        Task<IEnumerable<CustomFieldModel>> GetTeamAvailableFieldsAsync(double teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set a Custom Field value for a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="fieldId">The ID of the Custom Field.</param>
        /// <param name="request">The request object containing the value to set for the Custom Field.</param>
        /// <param name="customTaskIds">Optional. If true, <paramref name="taskId"/> refers to a custom task ID. <paramref name="teamId"/> must be provided if this is true.</param>
        /// <param name="teamId">Optional. The ID of the team (Workspace) if using custom task IDs.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Corresponds to the ClickUp API operation <c>SetCustomFieldValue</c> (<c>POST /v2/task/{task_id}/field/{field_id}</c>).
        /// The request body for SetCustomFieldValue is complex (anyOf). For the interface, a general placeholder like <see cref="SetCustomFieldValueRequest"/> is used.
        /// The actual implementation and models will need to handle the variations.
        /// </remarks>
        Task SetCustomFieldValueAsync(string taskId, string fieldId, SetCustomFieldValueRequest request, bool? customTaskIds = null, double? teamId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove a Custom Field value from a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="fieldId">The ID of the Custom Field.</param>
        /// <param name="customTaskIds">Optional. If true, <paramref name="taskId"/> refers to a custom task ID. <paramref name="teamId"/> must be provided if this is true.</param>
        /// <param name="teamId">Optional. The ID of the team (Workspace) if using custom task IDs.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Corresponds to the ClickUp API operation <c>RemoveCustomFieldValue</c> (<c>DELETE /v2/task/{task_id}/field/{field_id}</c>).
        /// </remarks>
        Task RemoveCustomFieldValueAsync(string taskId, string fieldId, bool? customTaskIds = null, double? teamId = null, CancellationToken cancellationToken = default);
    }
}
