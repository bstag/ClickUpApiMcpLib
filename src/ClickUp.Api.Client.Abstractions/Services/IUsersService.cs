using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities; // Assuming User DTO (or WorkspaceUser) is here
using ClickUp.Api.Client.Models.RequestModels.Users; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents User management operations in the ClickUp API for a specific Workspace.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/team/{team_id}/user/{user_id}
    /// - PUT /v2/team/{team_id}/user/{user_id}
    /// - DELETE /v2/team/{team_id}/user/{user_id}
    /// Note: Inviting new users to a Workspace might be handled by a dedicated Invites service or via IGuestsService if they are invited as guests initially.
    /// Retrieving the currently authenticated user's details is typically handled by IAuthorizationService.GetAuthorizedUserAsync().
    /// This service focuses on managing existing users within a specific Workspace.
    /// </remarks>
    public interface IUsersService
    {
        /// <summary>
        /// Retrieves information about a specific user in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the user by setting to false. Note: This parameter might not be supported on all API versions or for all user types.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="User"/> within the Workspace context.</returns>
        Task<User> GetUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a user's details (e.g., name, role) within a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="userId">The ID of the user to edit.</param>
        /// <param name="editUserRequest">Details for updating the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the updated <see cref="User"/> within the Workspace context.</returns>
        Task<User> EditUserOnWorkspaceAsync(
            string workspaceId,
            string userId,
            EditUserOnWorkspaceRequest editUserRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates or Removes a user from a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <remarks>Original note mentioned API returns a 'team' object. Returning Task for simplicity unless team DTO is essential for client.</remarks>
        System.Threading.Tasks.Task RemoveUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            CancellationToken cancellationToken = default);
    }
}
