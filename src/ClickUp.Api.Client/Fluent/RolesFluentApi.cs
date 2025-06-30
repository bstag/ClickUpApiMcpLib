using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Roles;
using System.Collections.Generic;
using System.Runtime.CompilerServices; // Added for EnumeratorCancellation
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

// Removed redundant using ClickUp.Api.Client.Models.ResponseModels.Roles; as it's covered by the one above.

public class RolesFluentApi
{
    private readonly IRolesService _rolesService;

    public RolesFluentApi(IRolesService rolesService)
    {
        _rolesService = rolesService;
    }

    public async Task<IEnumerable<CustomRole>> GetCustomRolesAsync(string workspaceId, bool? includeMembers = null, CancellationToken cancellationToken = default)
    {
        return await _rolesService.GetCustomRolesAsync(workspaceId, includeMembers, cancellationToken);
    }

    /// <summary>
    /// Retrieves all custom roles for a specific workspace asynchronously.
    /// While this method returns an IAsyncEnumerable, the underlying ClickUp API for custom roles
    /// does not appear to be paginated, so all roles are typically fetched in a single call by the service.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace (team).</param>
    /// <param name="includeMembers">Optional. Whether to include members in the roles. Defaults to false.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="CustomRole"/>.</returns>
    public async IAsyncEnumerable<CustomRole> GetCustomRolesAsyncEnumerableAsync(
        string workspaceId,
        bool? includeMembers = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var roles = await _rolesService.GetCustomRolesAsync(workspaceId, includeMembers, cancellationToken).ConfigureAwait(false);
        if (roles != null)
        {
            foreach (var role in roles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return role;
            }
        }
    }
}
