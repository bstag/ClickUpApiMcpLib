using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.ResponseModels; // Assuming SharedHierarchy DTO is in a general ResponseModels namespace or similar

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Shared Hierarchy operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/team/{team_id}/shared
    /// </remarks>
    public interface ISharedHierarchyService
    {
        /// <summary>
        /// Retrieves the tasks, Lists, and Folders that have been shared with the authenticated user for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="SharedHierarchy"/> object containing lists of shared tasks, lists, and folders.</returns>
        Task<SharedHierarchy> GetSharedHierarchyAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);
    }
}
