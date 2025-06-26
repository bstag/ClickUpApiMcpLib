using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TemplateFluentCreateFolderRequest
{
    private readonly CreateFolderFromTemplateRequest _request = new();
    private readonly string _spaceId;
    private readonly string _templateId;
    private readonly IFoldersService _foldersService;

    public TemplateFluentCreateFolderRequest(string spaceId, string templateId, IFoldersService foldersService)
    {
        _spaceId = spaceId;
        _templateId = templateId;
        _foldersService = foldersService;
    }

    public TemplateFluentCreateFolderRequest WithName(string name)
    {
        _request.Name = name;
        return this;
    }

    public async Task<Folder> CreateAsync(CancellationToken cancellationToken = default)
    {
        return await _foldersService.CreateFolderFromTemplateAsync(
            _spaceId,
            _templateId,
            _request,
            cancellationToken
        );
    }
}