using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents User management operations in the ClickUp API for a specific Workspace.
    // Based on endpoints like:
    // - GET /v2/team/{team_id}/user/{user_id}
    // - PUT /v2/team/{team_id}/user/{user_id}
    // - DELETE /v2/team/{team_id}/user/{user_id}
    // Note: Inviting users is handled by IGuestsService or a dedicated InvitesService. GetAuthorizedUser is in IAuthorizationService.

    public interface IUsersService
    {
        /// <summary>
        /// Retrieves information about a specific user in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the user by setting to false.</param>
        /// <returns>Details of the user within the Workspace context.</returns>
        Task<object> GetUserFromWorkspaceAsync(double workspaceId, double userId, bool? includeShared = null);
        // Note: Return type should be MemberDetailsDto.

        /// <summary>
        /// Updates a user's name and role within a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="userId">The ID of the user to edit.</param>
        /// <param name="editUserRequest">Details for updating the user.</param>
        /// <returns>Details of the updated user within the Workspace context.</returns>
        Task<object> EditUserOnWorkspaceAsync(double workspaceId, double userId, object editUserRequest);
        // Note: editUserRequest should be EditUserOnWorkspaceRequest. Return type should be MemberDetailsDto.

        /// <summary>
        /// Deactivates/Removes a user from a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task<object> RemoveUserFromWorkspaceAsync(double workspaceId, double userId);
        // Note: API returns a 'team' object. Consider if Task is sufficient or if this DTO is needed.
    }
}
