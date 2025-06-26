using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using ClickUp.Api.Client.Models.ResponseModels.Docs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class DocsFluentApi
{
    private readonly IDocsService _docsService;

    public DocsFluentApi(IDocsService docsService)
    {
        _docsService = docsService;
    }

    public SearchDocsFluentRequest SearchDocs(string workspaceId,
        string? query = null,
        List<string>? spaceIds = null,
        List<string>? folderIds = null,
        List<string>? listIds = null,
        List<string>? taskIds = null,
        bool? includeArchived = null,
        int? limit = null,
        string? cursor = null,
        string? parentId = null,
        int? parentType = null,
        bool? includeDeleted = null,
        int? creatorId = null)
    {
        return new SearchDocsFluentRequest(workspaceId, _docsService);
    }

    public DocFluentCreateRequest CreateDoc(string workspaceId, string name)
    {
        return new DocFluentCreateRequest(workspaceId, _docsService, name);
    }

    public async Task<Doc> GetDocAsync(string workspaceId, string docId, CancellationToken cancellationToken = default)
    {
        return await _docsService.GetDocAsync(workspaceId, docId, cancellationToken);
    }

    public async Task<IEnumerable<DocPageListingItem>> GetDocPageListingAsync(string workspaceId, string docId, int? maxPageDepth = null, CancellationToken cancellationToken = default)
    {
        return await _docsService.GetDocPageListingAsync(workspaceId, docId, maxPageDepth, cancellationToken);
    }

    public async Task<IEnumerable<Page>> GetDocPagesAsync(string workspaceId, string docId, int? maxPageDepth = null, string? contentFormat = null, CancellationToken cancellationToken = default)
    {
        return await _docsService.GetDocPagesAsync(workspaceId, docId, maxPageDepth, contentFormat, cancellationToken);
    }

    public PageFluentCreateRequest CreatePage(string workspaceId, string docId, string name, string content, string contentFormat)
    {
        return new PageFluentCreateRequest(workspaceId, docId, _docsService, name, content, contentFormat);
    }

    public async Task<Page> GetPageAsync(string workspaceId, string docId, string pageId, string? contentFormat = null, CancellationToken cancellationToken = default)
    {
        return await _docsService.GetPageAsync(workspaceId, docId, pageId, contentFormat, cancellationToken);
    }

    public PageFluentEditRequest EditPage(string workspaceId, string docId, string pageId)
    {
        return new PageFluentEditRequest(workspaceId, docId, pageId, _docsService);
    }
}
