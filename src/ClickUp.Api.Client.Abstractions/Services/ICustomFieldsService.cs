using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.CustomFields;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Custom Fields operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for retrieving Custom Field definitions from various levels (List, Folder, Space, Workspace)
    /// and for setting or removing Custom Field values on tasks.
    /// Covered API Endpoints:
    /// - `GET /list/{list_id}/field`: Retrieves accessible Custom Fields for a List.
    /// - `GET /folder/{folder_id}/field`: Retrieves Custom Fields defined at the Folder level.
    /// - `GET /space/{space_id}/field`: Retrieves Custom Fields defined at the Space level.
    /// - `GET /team/{team_id}/field`: Retrieves Custom Fields defined at the Workspace (Team) level.
    /// - `POST /task/{task_id}/field/{field_id}`: Sets or updates the value of a Custom Field on a task.
    /// - `DELETE /task/{task_id}/field/{field_id}`: Removes the value of a Custom Field from a task.
    /// </remarks>
    public interface ICustomFieldsService
    {
        /// <summary>
        /// Retrieves all Custom Fields that are accessible from a specific List.
        /// This includes fields defined on the List itself, as well as those inherited from parent Folders, Spaces, and the Workspace.
        /// </summary>
        /// <param name="listId">The unique identifier of the List.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Field"/> objects accessible from the specified List.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Custom Fields for this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IEnumerable<CustomFieldDefinition>> GetAccessibleCustomFieldsAsync(
            string listId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the Custom Fields that were specifically created at the Folder level.
        /// </summary>
        /// <param name="folderId">The unique identifier of the Folder.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Field"/> objects defined on the specified Folder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Folder with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Custom Fields for this Folder.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<CustomFieldDefinition>> GetFolderCustomFieldsAsync(
            string folderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the Custom Fields that were specifically created at the Space level.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Field"/> objects defined on the specified Space.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Custom Fields for this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<CustomFieldDefinition>> GetSpaceCustomFieldsAsync(
            string spaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the Custom Fields that were specifically created at the Workspace (Team) level.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Field"/> objects defined on the specified Workspace.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Custom Fields for this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<CustomFieldDefinition>> GetWorkspaceCustomFieldsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets or updates the value of a Custom Field on a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="fieldId">The unique identifier (UUID) of the Custom Field whose value is to be set.</param>
        /// <param name="setFieldValueRequest">A <see cref="SetCustomFieldValueRequest"/> object containing the value to set for the Custom Field. The structure of the 'value' property within this request depends on the type of the Custom Field (e.g., string, number, array of user IDs).</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/>, <paramref name="fieldId"/>, or <paramref name="setFieldValueRequest"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or Custom Field with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the provided value in <paramref name="setFieldValueRequest"/> is invalid for the type of the Custom Field.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify this Custom Field on the task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task SetCustomFieldValueAsync(
            string taskId,
            string fieldId,
            SetCustomFieldValueRequest setFieldValueRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the value from a Custom Field on a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="fieldId">The unique identifier (UUID) of the Custom Field from which the value will be removed.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="fieldId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or Custom Field with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify this Custom Field on the task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveCustomFieldValueAsync(
            string taskId,
            string fieldId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}
