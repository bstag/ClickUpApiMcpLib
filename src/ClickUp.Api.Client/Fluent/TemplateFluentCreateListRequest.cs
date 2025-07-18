using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.RequestModels.Lists;

using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TemplateFluentCreateListRequest
{
    private readonly CreateListFromTemplateRequest _request = new();
    private readonly string _containerId;
    private readonly string _templateId;
    private readonly IListsService _listsService;
    private readonly bool _inFolder;

    public TemplateFluentCreateListRequest(string containerId, string templateId, IListsService listsService, bool inFolder)
    {
        _containerId = containerId;
        _templateId = templateId;
        _listsService = listsService;
        _inFolder = inFolder;
    }

    public TemplateFluentCreateListRequest WithName(string name)
    {
        _request.Name = name;
        return this;
    }

    public async Task<ClickUpList> CreateAsync(CancellationToken cancellationToken = default)
    {
        if (_inFolder)
        {
            return await _listsService.CreateListFromTemplateInFolderAsync(
                _containerId,
                _templateId,
                _request,
                cancellationToken
            );
        }
        else
        {
            return await _listsService.CreateListFromTemplateInSpaceAsync(
                _containerId,
                _templateId,
                _request,
                cancellationToken
            );
        }
    }
}