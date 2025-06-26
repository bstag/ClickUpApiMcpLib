using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using ClickUp.Api.Client.Models.RequestModels.UserGroups;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreateUserGroupRequest
{
    private string? _name;
    private string? _handle;
    private List<int>? _members;

    private readonly string _workspaceId;
    private readonly IUserGroupsService _userGroupsService;

    public FluentCreateUserGroupRequest(string workspaceId, IUserGroupsService userGroupsService)
    {
        _workspaceId = workspaceId;
        _userGroupsService = userGroupsService;
    }

    public FluentCreateUserGroupRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FluentCreateUserGroupRequest WithHandle(string handle)
    {
        _handle = handle;
        return this;
    }

    public FluentCreateUserGroupRequest WithMembers(List<int> members)
    {
        _members = members;
        return this;
    }

    public async Task<UserGroup> CreateAsync(CancellationToken cancellationToken = default)
    {
        var createUserGroupRequest = new CreateUserGroupRequest(
            Name: _name ?? string.Empty,
            Handle: _handle,
            Members: _members ?? new List<int>()
        );

        return await _userGroupsService.CreateUserGroupAsync(
            _workspaceId,
            createUserGroupRequest,
            cancellationToken
        );
    }
}