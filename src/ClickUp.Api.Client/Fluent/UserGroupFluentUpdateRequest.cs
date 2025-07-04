using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using ClickUp.Api.Client.Models.RequestModels.UserGroups;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Linq;
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
    private readonly List<string> _validationErrors = new List<string>();

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

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_groupId))
        {
            _validationErrors.Add("GroupId is required.");
        }
        // For an update, at least one field should ideally be provided.
        if (string.IsNullOrWhiteSpace(_name) && string.IsNullOrWhiteSpace(_handle) && _members == null)
        {
            _validationErrors.Add("At least one property (Name, Handle, or Members) must be set for updating a User Group.");
        }
        if (_members != null)
        {
            if ((_members.Add == null || !_members.Add.Any()) && (_members.Rem == null || !_members.Rem.Any()))
            {
                _validationErrors.Add("If Members object is provided, it must specify members to add or remove.");
            }
        }

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<UserGroup> UpdateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
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