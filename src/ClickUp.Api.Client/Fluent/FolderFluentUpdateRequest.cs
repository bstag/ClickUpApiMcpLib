using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FolderFluentUpdateRequest
{
    private string? _name;
    private bool? _archived;
    private readonly string _folderId;
    private readonly IFoldersService _foldersService;

    public FolderFluentUpdateRequest(string folderId, IFoldersService foldersService)
    {
        _folderId = folderId;
        _foldersService = foldersService;
    }

    public FolderFluentUpdateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FolderFluentUpdateRequest WithArchived(bool archived)
    {
        _archived = archived;
        return this;
    }

    public async Task<Folder> UpdateAsync(CancellationToken cancellationToken = default)
    {
        var updateFolderRequest = new UpdateFolderRequest(
            Name: _name ?? string.Empty,
            Archived: _archived
        );

        return await _foldersService.UpdateFolderAsync(
            _folderId,
            updateFolderRequest,
            cancellationToken
        );
    }
}