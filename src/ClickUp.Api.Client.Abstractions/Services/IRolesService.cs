using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Roles;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Roles operations.
    /// </summary>
    /// <remarks>
    /// This service primarily focuses on retrieving information about Custom Roles within a Workspace.
    /// Covered API Endpoints:
    /// - `GET /team/{team_id}/customroles`: Retrieves Custom Roles for a Workspace.
    /// </remarks>
    public interface IRolesService
    {
        /// <summary>
        /// Retrieves a list of Custom Roles available in a specific Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) for which to retrieve Custom Roles.</param>
        /// <param name="includeMembers">Optional. If set to <c>true</c>, includes the list of members associated with each Custom Role in the response. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="CustomRole"/> objects
        /// representing the Custom Roles in the Workspace.
        /// </returns>
        /// <remarks>
        /// The ClickUp API typically wraps the list of roles in a response object (e.g., under a "custom_roles" key).
        /// This method abstracts that and directly returns the enumerable of roles.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Custom Roles for this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IEnumerable<CustomRole>> GetCustomRolesAsync(
            string workspaceId,
            bool? includeMembers = null,
            CancellationToken cancellationToken = default);
    }
}
