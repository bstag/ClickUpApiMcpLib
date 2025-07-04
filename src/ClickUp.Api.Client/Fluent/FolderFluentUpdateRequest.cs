using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using ClickUp.Api.Client.Models.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Fluent;

public class FolderFluentUpdateRequest
{
    private string? _name;
    private bool? _archived;
    private readonly string _folderId;
    private readonly IFoldersService _foldersService;
    private readonly List<string> _validationErrors = new List<string>();

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

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_folderId))
        {
            _validationErrors.Add("FolderId is required.");
        }
        if (string.IsNullOrWhiteSpace(_name) && !_archived.HasValue)
        {
            _validationErrors.Add("Either Name or Archived status must be provided for an update.");
        }

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<Folder> UpdateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
        var updateFolderRequest = new UpdateFolderRequest(
            Name: _name ?? string.Empty, // API might require name even if only archiving, or might ignore if empty and archive is true. Assuming API handles this.
            Archived: _archived
        );

        return await _foldersService.UpdateFolderAsync(
            _folderId,
            updateFolderRequest,
            cancellationToken
        );
    }
}