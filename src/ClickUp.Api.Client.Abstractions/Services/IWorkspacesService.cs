using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    public interface IWorkspacesService
    {
        /// <summary>
        /// Retrieves the seat usage for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the workspace seat details.</returns>
        Task<object> GetWorkspaceSeatsAsync(string workspaceId);
        // Note: Return type 'object' should be WorkspaceSeatsDto.

        /// <summary>
        /// Retrieves the current plan for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the workspace plan details.</returns>
        Task<object> GetWorkspacePlanAsync(string workspaceId);
        // Note: Return type 'object' should be WorkspacePlanDto.
    }
}
