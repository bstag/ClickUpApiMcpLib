using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FoldersFluentApi
{
    private readonly IFoldersService _foldersService;

    public FoldersFluentApi(IFoldersService foldersService)
    {
        _foldersService = foldersService;
    }

    public async Task<IEnumerable<Folder>> GetFoldersAsync(string spaceId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        return await _foldersService.GetFoldersAsync(spaceId, archived, cancellationToken);
    }

    public async Task<Folder> GetFolderAsync(string folderId, CancellationToken cancellationToken = default)
    {
        return await _foldersService.GetFolderAsync(folderId, cancellationToken);
    }

    public async Task DeleteFolderAsync(string folderId, CancellationToken cancellationToken = default)
    {
        await _foldersService.DeleteFolderAsync(folderId, cancellationToken);
    }

    public FolderFluentCreateRequest CreateFolder(string spaceId)
    {
        return new FolderFluentCreateRequest(spaceId, _foldersService);
    }

    public FolderFluentUpdateRequest UpdateFolder(string folderId)
    {
        return new FolderFluentUpdateRequest(folderId, _foldersService);
    }

    public TemplateFluentCreateFolderRequest CreateFolderFromTemplate(string spaceId, string templateId)
    {
        return new TemplateFluentCreateFolderRequest(spaceId, templateId, _foldersService);
    }

    /// <summary>
    /// Retrieves all folders within a specific space asynchronously.
    /// While this method returns an IAsyncEnumerable, the underlying ClickUp API for getting folders
    /// does not appear to be paginated, so all folders are typically fetched in a single call by the service.
    /// </summary>
    /// <param name="spaceId">The ID of the space.</param>
    /// <param name="archived">Optional. Whether to include archived folders. Defaults to false.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Folder"/>.</returns>
    public async IAsyncEnumerable<Folder> GetFoldersAsyncEnumerableAsync(
        string spaceId,
        bool? archived = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var folders = await _foldersService.GetFoldersAsync(spaceId, archived, cancellationToken).ConfigureAwait(false);
        if (folders != null)
        {
            foreach (var folder in folders)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return folder;
            }
        }
    }
}
