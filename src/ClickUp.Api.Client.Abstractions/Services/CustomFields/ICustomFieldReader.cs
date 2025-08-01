using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.CustomFields;

namespace ClickUp.Api.Client.Abstractions.Services.CustomFields
{
    /// <summary>
    /// Interface for reading ClickUp Custom Field definitions from various hierarchy levels.
    /// This interface focuses on retrieving Custom Field definitions and metadata.
    /// </summary>
    public interface ICustomFieldReader
    {
        /// <summary>
        /// Retrieves all Custom Fields that are accessible from a specific List.
        /// This includes fields defined on the List itself, as well as those inherited from parent Folders, Spaces, and the Workspace.
        /// </summary>
        /// <param name="listId">The unique identifier of the List.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="CustomFieldDefinition"/> objects accessible from the specified List.</returns>
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
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="CustomFieldDefinition"/> objects defined on the specified Folder.</returns>
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
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="CustomFieldDefinition"/> objects defined on the specified Space.</returns>
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
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="CustomFieldDefinition"/> objects defined on the specified Workspace.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Custom Fields for this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<CustomFieldDefinition>> GetWorkspaceCustomFieldsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);
    }
}