using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.ResponseModels.Roles;
// Removed: using ClickUp.Api.Client.Models.Entities.Users;
// CustomRole will be fully qualified or using ClickUp.Api.Client.Models.ResponseModels.Roles; will be added

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Roles operations in the ClickUp API, primarily for retrieving Custom Roles.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/team/{team_id}/customroles
    /// </remarks>
    public interface IRolesService
    {
        /// <summary>
        /// Retrieves the Custom Roles available in a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="includeMembers">Optional. Whether to include members associated with each Custom Role.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An enumerable of <see cref="CustomRole"/> objects for the Workspace.</returns>
        /// <remarks>The ClickUp API returns roles in a {"custom_roles": []} structure. This method should ideally return a wrapper response object if additional details like total count are needed.</remarks>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the workspace with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access custom roles for this workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<CustomRole>> GetCustomRolesAsync(
            string workspaceId,
            bool? includeMembers = null,
            CancellationToken cancellationToken = default);
    }
}
