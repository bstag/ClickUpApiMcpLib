using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Users;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp User management operations within a specific Workspace.
    /// </summary>
    /// <remarks>
    /// This service allows for retrieving, updating, and removing users from a specific Workspace (Team).
    /// Note:
    /// - Inviting new users (as guests or members) is typically handled by <see cref="IGuestsService.InviteGuestToWorkspaceAsync(string, Models.RequestModels.Guests.InviteGuestToWorkspaceRequest, CancellationToken)"/> or potentially a dedicated Invites service.
    /// - Retrieving the currently authenticated user's global details (not specific to a Workspace) is handled by <see cref="IAuthorizationService.GetAuthorizedUserAsync(CancellationToken)"/>.
    /// This service focuses on managing users *within the context of a specific Workspace*.
    /// Covered API Endpoints:
    /// - Get User in Workspace: `GET /team/{team_id}/user/{user_id}`
    /// - Edit User in Workspace: `PUT /team/{team_id}/user/{user_id}`
    /// - Remove User from Workspace: `DELETE /team/{team_id}/user/{user_id}`
    /// </remarks>
    public interface IUsersService
    {
        /// <summary>
        /// Retrieves information about a specific user as they exist within a given Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) where the user's details are being requested.</param>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <param name="includeShared">Optional. If set to <c>true</c>, includes details of items (Tasks, Lists, etc.) shared with this user within the Workspace. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="User"/> object with their details relevant to the specified Workspace.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="userId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace or the user within that Workspace is not found.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the authenticated user does not have permission to view this user's details in the Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<User> GetUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a user's details or role within a specific Workspace (Team). This can include changing their name (if permitted by global profile settings), role, or admin status within that Workspace.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) where the user's details are being edited.</param>
        /// <param name="userId">The unique identifier of the user to edit.</param>
        /// <param name="editUserRequest">An <see cref="EditUserOnWorkspaceRequest"/> object containing the updated details for the user, such as their role or admin status in the Workspace.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="User"/> object with their new details in the Workspace context.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="userId"/>, or <paramref name="editUserRequest"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace or user is not found.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the update request is invalid (e.g., invalid role).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the authenticated user does not have permission to edit this user in the Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<User> EditUserOnWorkspaceAsync(
            string workspaceId,
            string userId,
            EditUserOnWorkspaceRequest editUserRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates or removes a user from a specific Workspace (Team). This action typically makes the user inactive in that Workspace.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) from which the user will be removed.</param>
        /// <param name="userId">The unique identifier of the user to remove from the Workspace.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="userId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace or user is not found.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the authenticated user does not have permission to remove this user from the Workspace (e.g., trying to remove the Workspace owner or higher role).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            CancellationToken cancellationToken = default);
    }
}
