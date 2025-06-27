using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Roles;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.ResponseModels.Roles;

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
}
