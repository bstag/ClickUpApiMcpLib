using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Sharing;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Shared Hierarchy operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods to retrieve information about items (Tasks, Lists, Folders)
    /// that have been shared with the authenticated user within a specific Workspace.
    /// Covered API Endpoints:
    /// - `GET /team/{team_id}/shared`: Retrieves the shared hierarchy for a Workspace.
    /// </remarks>
    public interface ISharedHierarchyService
    {
        /// <summary>
        /// Retrieves a summary of Tasks, Lists, and Folders that have been shared with the currently authenticated user
        /// within a specific Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) for which to retrieve the shared hierarchy.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="SharedHierarchyResponse"/> object,
        /// which includes lists of shared tasks, lists, and folders.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access the shared hierarchy for this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<SharedHierarchyResponse> GetSharedHierarchyAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);
    }
}
