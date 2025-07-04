using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using ClickUp.Api.Client.Models.RequestModels.UserGroups;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class UserGroupFluentCreateRequest
{
    private string? _name;
    private string? _handle;
    private List<int>? _members;

    private readonly string _workspaceId;
    private readonly IUserGroupsService _userGroupsService;
    private readonly List<string> _validationErrors = new List<string>();

    public UserGroupFluentCreateRequest(string workspaceId, IUserGroupsService userGroupsService)
    {
        _workspaceId = workspaceId;
        _userGroupsService = userGroupsService;
    }

    public UserGroupFluentCreateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserGroupFluentCreateRequest WithHandle(string handle)
    {
        _handle = handle;
        return this;
    }

    public UserGroupFluentCreateRequest WithMembers(List<int> members)
    {
        _members = members;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_workspaceId))
        {
            _validationErrors.Add("WorkspaceId (TeamId) is required.");
        }
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors.Add("User group name is required.");
        }
        if (_members == null || !_members.Any())
        {
            _validationErrors.Add("At least one member must be added to the user group.");
        }
        // Handle is optional.

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<UserGroup> CreateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
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