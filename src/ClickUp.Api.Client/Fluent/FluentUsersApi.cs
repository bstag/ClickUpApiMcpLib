using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.Entities.Users;

public class FluentUsersApi
{
    private readonly IUsersService _usersService;

    public FluentUsersApi(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task<User> GetUserFromWorkspaceAsync(string workspaceId, string userId, bool? includeShared = null, CancellationToken cancellationToken = default)
    {
        return await _usersService.GetUserFromWorkspaceAsync(workspaceId, userId, includeShared, cancellationToken);
    }

    public FluentEditUserOnWorkspaceRequest EditUserOnWorkspace(string workspaceId, string userId)
    {
        return new FluentEditUserOnWorkspaceRequest(workspaceId, userId, _usersService);
    }

    public async Task RemoveUserFromWorkspaceAsync(string workspaceId, string userId, CancellationToken cancellationToken = default)
    {
        await _usersService.RemoveUserFromWorkspaceAsync(workspaceId, userId, cancellationToken);
    }
}
