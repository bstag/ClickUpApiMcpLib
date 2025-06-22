using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ClickUp.Api.Client.Models.Entities.UserGroups;
using ClickUp.Api.Client.Models.RequestModels.UserGroups;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp User Group (Team) operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for creating, retrieving, updating, and deleting User Groups within a Workspace.
    /// API Endpoints:
    /// - Get User Groups: `GET /team/{team_id}/group`
    /// - Create User Group: `POST /team/{team_id}/group`
    /// - Update User Group: `PUT /group/{group_id}`
    /// - Delete User Group: `DELETE /group/{group_id}`
    /// </remarks>
    public interface IUserGroupsService
    {
        /// <summary>
        /// Retrieves all User Groups for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="groupIds">Optional. A list of specific User Group IDs to retrieve. If null or empty, all groups for the workspace are fetched.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="UserGroup"/> objects.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<UserGroup>> GetUserGroupsAsync(
            string workspaceId,
            List<string>? groupIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new User Group within a specified Workspace.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) where the User Group will be created.</param>
        /// <param name="request">An object containing the details for the new User Group.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="UserGroup"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="request"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the request is invalid (e.g., name missing, invalid member IDs).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<UserGroup> CreateUserGroupAsync(
            string workspaceId,
            CreateUserGroupRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing User Group.
        /// </summary>
        /// <param name="groupId">The unique identifier of the User Group to update.</param>
        /// <param name="request">An object containing the properties to update for the User Group.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="UserGroup"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="groupId"/> or <paramref name="request"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the User Group with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the update request is invalid.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<UserGroup> UpdateUserGroupAsync(
            string groupId,
            UpdateUserGroupRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified User Group.
        /// </summary>
        /// <param name="groupId">The unique identifier of the User Group to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="groupId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the User Group with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteUserGroupAsync(
            string groupId,
            CancellationToken cancellationToken = default);
    }
}
