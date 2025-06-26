using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentFoldersApi
{
    private readonly IFoldersService _foldersService;

    public FluentFoldersApi(IFoldersService foldersService)
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

    public FluentCreateFolderRequest CreateFolder(string spaceId)
    {
        return new FluentCreateFolderRequest(spaceId, _foldersService);
    }

    public FluentUpdateFolderRequest UpdateFolder(string folderId)
    {
        return new FluentUpdateFolderRequest(folderId, _foldersService);
    }

    public FluentCreateFolderFromTemplateRequest CreateFolderFromTemplate(string spaceId, string templateId)
    {
        return new FluentCreateFolderFromTemplateRequest(spaceId, templateId, _foldersService);
    }
}
