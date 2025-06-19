using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Roles operations in the ClickUp API, primarily for retrieving Custom Roles.
    // Based on endpoints like:
    // - GET /v2/team/{team_id}/customroles

    public interface IRolesService
    {
        /// <summary>
        /// Retrieves the Custom Roles available in a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="includeMembers">Optional. Whether to include members associated with each Custom Role.</param>
        /// <returns>A list of Custom Roles for the Workspace.</returns>
        Task<IEnumerable<object>> GetCustomRolesAsync(double workspaceId, bool? includeMembers = null);
        // Note: Return type should be IEnumerable<CustomRoleDto>.
    }
}
