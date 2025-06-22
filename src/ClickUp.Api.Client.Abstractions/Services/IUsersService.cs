using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Users; // Assuming User DTO (or WorkspaceUser) is here
using ClickUp.Api.Client.Models.RequestModels.Users; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents User management operations in the ClickUp API for a specific Workspace.
    /// </summary>
    /// <remarks>
    /// This service allows for retrieving, updating, and removing users from a specific Workspace (Team).
    /// Note that inviting new users is typically handled by <see cref="IGuestsService.InviteGuestToWorkspaceAsync(string, InviteGuestToWorkspaceRequest, CancellationToken)"/>
    /// or a dedicated Invites service if they are invited as full members directly.
    /// Retrieving the currently authenticated user's global details is handled by <see cref="IAuthorizationService.GetAuthorizedUserAsync(CancellationToken)"/>.
    /// This service focuses on managing users *within the context of a specific Workspace*.
    /// Based on ClickUp API endpoints like:
    /// <list type="bullet">
    /// <item><description>GET /v2/team/{team_id}/user/{user_id}</description></item>
    /// <item><description>PUT /v2/team/{team_id}/user/{user_id}</description></item>
    /// <item><description>DELETE /v2/team/{team_id}/user/{user_id}</description></item>
    /// </list>
    /// </remarks>
    public interface IUsersService
    {
        /// <summary>
        /// Retrieves information about a specific user within a given Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team) where the user resides.</param>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <param name="includeShared">Optional. If true, includes details of items shared with the user. Defaults to false. Note: This parameter's behavior might vary or not be supported on all API versions or for all user types.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="User"/> details within the Workspace context.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="userId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as user not found in the workspace or authentication issues.</exception>
        Task<User> GetUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a user's details (e.g., name, role, admin status) within a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team) where the user is being edited.</param>
        /// <param name="userId">The ID of the user to edit.</param>
        /// <param name="editUserRequest">An <see cref="EditUserOnWorkspaceRequest"/> object containing the updated details for the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="User"/> details within the Workspace context.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="userId"/>, or <paramref name="editUserRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid user ID, insufficient permissions, or validation errors.</exception>
        Task<User> EditUserOnWorkspaceAsync(
            string workspaceId,
            string userId,
            EditUserOnWorkspaceRequest editUserRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates or removes a user from a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team) from which the user will be removed.</param>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="userId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as user not found, insufficient permissions, or if trying to remove the Workspace owner.</exception>
        /// <remarks>The ClickUp API might return the updated team/workspace object upon successful removal. This method returns <see cref="System.Threading.Tasks.Task"/> for simplicity, indicating completion.</remarks>
        System.Threading.Tasks.Task RemoveUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            CancellationToken cancellationToken = default);
    }
}
