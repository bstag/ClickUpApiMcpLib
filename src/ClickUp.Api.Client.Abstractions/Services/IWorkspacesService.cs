using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces; // Assuming WorkspaceSeats and WorkspacePlan DTOs are here
// Or use ClickUp.Api.Client.Models.Entities if they are defined there.

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents Workspace (also referred to as Team in API v2) level operations in the ClickUp API.
    /// This service provides methods to retrieve information about a Workspace's plan and seat usage.
    /// </summary>
    /// <remarks>
    /// In the ClickUp API v2 documentation, "Team" is often synonymous with "Workspace".
    /// This service focuses on Workspace-wide administrative information.
    /// Operations related to User Groups (which are sometimes called "Teams" *within* a Workspace)
    /// would typically be in a separate service or potentially expanded here if closely related.
    /// Based on ClickUp API endpoints like:
    /// <list type="bullet">
    /// <item><description>GET /v2/team/{team_id}/seats</description></item>
    /// <item><description>GET /v2/team/{team_id}/plan</description></item>
    /// </list>
    /// </remarks>
    public interface IWorkspacesService
    {
        /// <summary>
        /// Retrieves the current seat usage (members and guests) for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team) for which to retrieve seat usage.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="GetWorkspaceSeatsResponse"/> with details about member and guest seats.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid workspace ID or authentication issues.</exception>
        Task<GetWorkspaceSeatsResponse> GetWorkspaceSeatsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current subscription plan details for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team) for which to retrieve plan details.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="GetWorkspacePlanResponse"/> with details about the Workspace's current plan.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as invalid workspace ID or authentication issues.</exception>
        Task<GetWorkspacePlanResponse> GetWorkspacePlanAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        // User Group (Teams within a Workspace) methods could be added here in the future, for example:
        // Task<IEnumerable<UserGroup>> GetUserGroupsAsync(string workspaceId, CancellationToken cancellationToken = default);
        // Task<UserGroup> CreateUserGroupAsync(string workspaceId, CreateUserGroupRequest request, CancellationToken cancellationToken = default);
        // etc.
    }
}
