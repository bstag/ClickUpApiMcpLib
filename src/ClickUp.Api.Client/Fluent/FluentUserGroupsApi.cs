using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentUserGroupsApi
{
    private readonly IUserGroupsService _userGroupsService;

    public FluentUserGroupsApi(IUserGroupsService userGroupsService)
    {
        _userGroupsService = userGroupsService;
    }

    public async Task<IEnumerable<UserGroup>> GetUserGroupsAsync(string workspaceId, List<string>? groupIds = null, CancellationToken cancellationToken = default)
    {
        return await _userGroupsService.GetUserGroupsAsync(workspaceId, groupIds, cancellationToken);
    }

    public FluentCreateUserGroupRequest CreateUserGroup(string workspaceId)
    {
        return new FluentCreateUserGroupRequest(workspaceId, _userGroupsService);
    }

    public FluentUpdateUserGroupRequest UpdateUserGroup(string groupId)
    {
        return new FluentUpdateUserGroupRequest(groupId, _userGroupsService);
    }

    public async Task DeleteUserGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        await _userGroupsService.DeleteUserGroupAsync(groupId, cancellationToken);
    }
}
