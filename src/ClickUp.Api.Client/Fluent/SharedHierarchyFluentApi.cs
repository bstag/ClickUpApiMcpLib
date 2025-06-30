using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Sharing;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class SharedHierarchyFluentApi
{
    private readonly ISharedHierarchyService _sharedHierarchyService;

    public SharedHierarchyFluentApi(ISharedHierarchyService sharedHierarchyService)
    {
        _sharedHierarchyService = sharedHierarchyService;
    }

    public async Task<SharedHierarchyResponse> GetSharedHierarchyAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId, cancellationToken);
    }

    /// <summary>
    /// Retrieves all shared task IDs for a specific workspace asynchronously.
    /// The underlying API is not paginated.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace (team).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="string"/> (Task IDs).</returns>
    public async IAsyncEnumerable<string> GetSharedTaskIdsAsyncEnumerableAsync(
        string workspaceId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId, cancellationToken).ConfigureAwait(false);
        if (response?.Shared?.Tasks != null)
        {
            foreach (var taskId in response.Shared.Tasks)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return taskId;
            }
        }
    }

    /// <summary>
    /// Retrieves all shared lists for a specific workspace asynchronously.
    /// The underlying API is not paginated.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace (team).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="SharedHierarchyListItem"/>.</returns>
    public async IAsyncEnumerable<SharedHierarchyListItem> GetSharedListsAsyncEnumerableAsync(
        string workspaceId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId, cancellationToken).ConfigureAwait(false);
        if (response?.Shared?.Lists != null)
        {
            foreach (var listItem in response.Shared.Lists)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return listItem;
            }
        }
    }

    /// <summary>
    /// Retrieves all shared folders for a specific workspace asynchronously.
    /// The underlying API is not paginated.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace (team).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="SharedHierarchyFolderItem"/>.</returns>
    public async IAsyncEnumerable<SharedHierarchyFolderItem> GetSharedFoldersAsyncEnumerableAsync(
        string workspaceId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId, cancellationToken).ConfigureAwait(false);
        if (response?.Shared?.Folders != null)
        {
            foreach (var folderItem in response.Shared.Folders)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return folderItem;
            }
        }
    }
}