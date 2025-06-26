using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using ClickUp.Api.Client.Models.ResponseModels.Docs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentSearchDocsRequest
{
    private string? _query;
    private string? _parentId;
    private int? _parentType;
    private string? _cursor;
    private int? _limit;

    private readonly string _workspaceId;
    private readonly IDocsService _docsService;

    public FluentSearchDocsRequest(string workspaceId, IDocsService docsService)
    {
        _workspaceId = workspaceId;
        _docsService = docsService;
    }

    public FluentSearchDocsRequest WithQuery(string query)
    {
        _query = query;
        return this;
    }

    public FluentSearchDocsRequest WithParentId(string parentId)
    {
        _parentId = parentId;
        return this;
    }

    public FluentSearchDocsRequest WithParentType(int parentType)
    {
        _parentType = parentType;
        return this;
    }

    public FluentSearchDocsRequest WithCursor(string cursor)
    {
        _cursor = cursor;
        return this;
    }

    public FluentSearchDocsRequest WithLimit(int limit)
    {
        _limit = limit;
        return this;
    }

    public async Task<SearchDocsResponse> SearchAsync(CancellationToken cancellationToken = default)
    {
        var request = new SearchDocsRequest
        {
            Query = _query ?? string.Empty,
            ParentId = _parentId,
            ParentType = _parentType,
            Cursor = _cursor,
            Limit = _limit
        };

        return await _docsService.SearchDocsAsync(
            _workspaceId,
            request,
            cancellationToken
        );
    }

    public IAsyncEnumerable<Doc> SearchAllAsync(CancellationToken cancellationToken = default)
    {
        var request = new SearchDocsRequest
        {
            Query = _query ?? string.Empty,
            ParentId = _parentId,
            ParentType = _parentType,
            Cursor = _cursor,
            Limit = _limit
        };

        return _docsService.SearchAllDocsAsync(
            _workspaceId,
            request,
            cancellationToken
        );
    }
}