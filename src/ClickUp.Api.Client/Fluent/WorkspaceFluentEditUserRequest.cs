using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Users;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class WorkspaceFluentEditUserRequest
{
    private string? _username;
    private int? _roleId;
    private bool? _admin;

    private readonly string _workspaceId;
    private readonly string _userId;
    private readonly IUsersService _usersService;

    public WorkspaceFluentEditUserRequest(string workspaceId, string userId, IUsersService usersService)
    {
        _workspaceId = workspaceId;
        _userId = userId;
        _usersService = usersService;
    }

    public WorkspaceFluentEditUserRequest WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public WorkspaceFluentEditUserRequest WithRoleId(int roleId)
    {
        _roleId = roleId;
        return this;
    }

    public WorkspaceFluentEditUserRequest WithAdmin(bool admin)
    {
        _admin = admin;
        return this;
    }

    public async Task<User> EditAsync(CancellationToken cancellationToken = default)
    {
        var editUserRequest = new EditUserOnWorkspaceRequest(
            Username: _username ?? string.Empty,
            Admin: _admin ?? false,
            CustomRoleId: _roleId ?? 0
        );

        return await _usersService.EditUserOnWorkspaceAsync(
            _workspaceId,
            _userId,
            editUserRequest,
            cancellationToken
        );
    }
}