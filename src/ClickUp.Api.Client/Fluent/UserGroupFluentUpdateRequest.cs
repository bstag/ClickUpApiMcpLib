using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using ClickUp.Api.Client.Models.RequestModels.UserGroups;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class UserGroupFluentUpdateRequest
{
    private string? _name;
    private string? _handle;
    private UpdateUserGroupMembersRequest? _members;

    private readonly string _groupId;
    private readonly IUserGroupsService _userGroupsService;

    public UserGroupFluentUpdateRequest(string groupId, IUserGroupsService userGroupsService)
    {
        _groupId = groupId;
        _userGroupsService = userGroupsService;
    }

    public UserGroupFluentUpdateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserGroupFluentUpdateRequest WithHandle(string handle)
    {
        _handle = handle;
        return this;
    }

    public UserGroupFluentUpdateRequest WithMembers(UpdateUserGroupMembersRequest members)
    {
        _members = members;
        return this;
    }

    public async Task<UserGroup> UpdateAsync(CancellationToken cancellationToken = default)
    {
        var updateUserGroupRequest = new UpdateUserGroupRequest(
            Name: _name,
            Handle: _handle,
            Members: _members
        );

        return await _userGroupsService.UpdateUserGroupAsync(
            _groupId,
            updateUserGroupRequest,
            cancellationToken
        );
    }
}