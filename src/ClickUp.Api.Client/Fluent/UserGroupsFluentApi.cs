using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class UserGroupsFluentApi
{
    private readonly IUserGroupsService _userGroupsService;

    public UserGroupsFluentApi(IUserGroupsService userGroupsService)
    {
        _userGroupsService = userGroupsService;
    }

    public async Task<IEnumerable<UserGroup>> GetUserGroupsAsync(string workspaceId, List<string>? groupIds = null, CancellationToken cancellationToken = default)
    {
        return await _userGroupsService.GetUserGroupsAsync(workspaceId, groupIds, cancellationToken);
    }

    public UserGroupFluentCreateRequest CreateUserGroup(string workspaceId)
    {
        return new UserGroupFluentCreateRequest(workspaceId, _userGroupsService);
    }

    public UserGroupFluentUpdateRequest UpdateUserGroup(string groupId)
    {
        return new UserGroupFluentUpdateRequest(groupId, _userGroupsService);
    }

    public async Task DeleteUserGroupAsync(string groupId, CancellationToken cancellationToken = default)
    {
        await _userGroupsService.DeleteUserGroupAsync(groupId, cancellationToken);
    }
}
