using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreateFolderRequest
{
    private string? _name;
    private readonly string _spaceId;
    private readonly IFoldersService _foldersService;

    public FluentCreateFolderRequest(string spaceId, IFoldersService foldersService)
    {
        _spaceId = spaceId;
        _foldersService = foldersService;
    }

    public FluentCreateFolderRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public async Task<Folder> CreateAsync(CancellationToken cancellationToken = default)
    {
        var createFolderRequest = new CreateFolderRequest(
            Name: _name ?? string.Empty
        );

        return await _foldersService.CreateFolderAsync(
            _spaceId,
            createFolderRequest,
            cancellationToken
        );
    }
}