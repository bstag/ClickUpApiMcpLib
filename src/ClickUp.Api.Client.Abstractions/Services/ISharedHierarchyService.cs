using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Shared Hierarchy operations in the ClickUp API.
    // Based on endpoints like:
    // - GET /v2/team/{team_id}/shared

    public interface ISharedHierarchyService
    {
        /// <summary>
        /// Retrieves the tasks, Lists, and Folders that have been shared with the authenticated user for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <returns>An object containing lists of shared tasks, lists, and folders.</returns>
        Task<object> GetSharedHierarchyAsync(double workspaceId);
        // Note: Return type should be SharedHierarchyDto.
    }
}
