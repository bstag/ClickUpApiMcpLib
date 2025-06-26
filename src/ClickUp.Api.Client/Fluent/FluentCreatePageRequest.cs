using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreatePageRequest
{
    private CreatePageRequest _request;
    private readonly string _workspaceId;
    private readonly string _docId;
    private readonly IDocsService _docsService;

    public FluentCreatePageRequest(string workspaceId, string docId, IDocsService docsService, string name, string content, string contentFormat)
    {
        _workspaceId = workspaceId;
        _docId = docId;
        _docsService = docsService;
        _request = new CreatePageRequest(ParentPageId: null, Name: name, SubTitle: null, Content: content, ContentFormat: contentFormat, OrderIndex: null, Hidden: null, TemplateId: null);
    }

    public FluentCreatePageRequest WithParentPageId(string parentPageId)
    {
        _request = _request with { ParentPageId = parentPageId };
        return this;
    }

    public FluentCreatePageRequest WithSubTitle(string subTitle)
    {
        _request = _request with { SubTitle = subTitle };
        return this;
    }

    public FluentCreatePageRequest WithOrderIndex(int orderIndex)
    {
        _request = _request with { OrderIndex = orderIndex };
        return this;
    }

    public FluentCreatePageRequest WithHidden(bool hidden)
    {
        _request = _request with { Hidden = hidden };
        return this;
    }

    public FluentCreatePageRequest WithTemplateId(string templateId)
    {
        _request = _request with { TemplateId = templateId };
        return this;
    }

    public async Task<Page> CreateAsync(CancellationToken cancellationToken = default)
    {
        return await _docsService.CreatePageAsync(
            _workspaceId,
            _docId,
            _request,
            cancellationToken
        );
    }
}