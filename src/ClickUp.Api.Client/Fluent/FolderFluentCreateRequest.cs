using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using ClickUp.Api.Client.Models.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Fluent;

public class FolderFluentCreateRequest
{
    private string? _name;
    private readonly string _spaceId;
    private readonly IFoldersService _foldersService;
    private readonly List<string> _validationErrors = new List<string>();

    public FolderFluentCreateRequest(string spaceId, IFoldersService foldersService)
    {
        _spaceId = spaceId;
        _foldersService = foldersService;
    }

    public FolderFluentCreateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_spaceId))
        {
            _validationErrors.Add("SpaceId is required.");
        }
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors.Add("Folder name is required.");
        }

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<Folder> CreateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
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