using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces; // Assuming WorkspaceSeats and WorkspacePlan DTOs are here
// Or use ClickUp.Api.Client.Models.Entities if they are defined there.

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Workspace (Team) level operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// In ClickUp API v2, "Team" is often used to refer to a Workspace.
    /// This service might also include User Group management in the future (often called "Teams" within a Workspace).
    /// </remarks>
    public interface IWorkspacesService
    {
        /// <summary>
        /// Retrieves the seat usage for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="GetWorkspaceSeatsResponse"/> details.</returns>
        Task<GetWorkspaceSeatsResponse> GetWorkspaceSeatsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current plan for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="GetWorkspacePlanResponse"/> details.</returns>
        Task<GetWorkspacePlanResponse> GetWorkspacePlanAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        // User Group (Teams within a Workspace) methods would go here if they were part of this service, e.g.:
        // CuTask<IEnumerable<UserGroup>> GetUserGroupsAsync(string workspaceId, CancellationToken cancellationToken = default);
        // CuTask<UserGroup> CreateUserGroupAsync(string workspaceId, CreateUserGroupRequest request, CancellationToken cancellationToken = default);
        // etc.
    }
}
