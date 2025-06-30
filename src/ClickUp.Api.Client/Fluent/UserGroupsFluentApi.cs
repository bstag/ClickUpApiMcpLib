using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using System.Collections.Generic;
using System.Runtime.CompilerServices; // Added for EnumeratorCancellation
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

    /// <summary>
    /// Retrieves all user groups for a specific workspace asynchronously, optionally filtered by group IDs.
    /// While this method returns an IAsyncEnumerable, the underlying ClickUp API for user groups
    /// does not appear to be paginated, so all groups are typically fetched in a single call by the service.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace (team).</param>
    /// <param name="groupIds">Optional list of group IDs to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="UserGroup"/>.</returns>
    public async IAsyncEnumerable<UserGroup> GetUserGroupsAsyncEnumerableAsync(
        string workspaceId,
        List<string>? groupIds = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var userGroups = await _userGroupsService.GetUserGroupsAsync(workspaceId, groupIds, cancellationToken).ConfigureAwait(false);
        if (userGroups != null)
        {
            foreach (var group in userGroups)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return group;
            }
        }
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
