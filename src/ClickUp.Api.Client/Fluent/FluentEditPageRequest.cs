using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentEditPageRequest
{
    private string? _name;
    private string? _subTitle;
    private string? _content;
    private string? _contentEditMode;
    private string? _contentFormat;
    private int? _orderIndex;
    private bool? _hidden;
    private string? _parentPageId;

    private readonly string _workspaceId;
    private readonly string _docId;
    private readonly string _pageId;
    private readonly IDocsService _docsService;

    public FluentEditPageRequest(string workspaceId, string docId, string pageId, IDocsService docsService)
    {
        _workspaceId = workspaceId;
        _docId = docId;
        _pageId = pageId;
        _docsService = docsService;
    }

    public FluentEditPageRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FluentEditPageRequest WithSubTitle(string subTitle)
    {
        _subTitle = subTitle;
        return this;
    }

    public FluentEditPageRequest WithContent(string content)
    {
        _content = content;
        return this;
    }

    public FluentEditPageRequest WithContentEditMode(string contentEditMode)
    {
        _contentEditMode = contentEditMode;
        return this;
    }

    public FluentEditPageRequest WithContentFormat(string contentFormat)
    {
        _contentFormat = contentFormat;
        return this;
    }

    public FluentEditPageRequest WithOrderIndex(int orderIndex)
    {
        _orderIndex = orderIndex;
        return this;
    }

    public FluentEditPageRequest WithHidden(bool hidden)
    {
        _hidden = hidden;
        return this;
    }

    public FluentEditPageRequest WithParentPageId(string parentPageId)
    {
        _parentPageId = parentPageId;
        return this;
    }

    public async Task EditAsync(CancellationToken cancellationToken = default)
    {
        var updatePageRequest = new EditPageRequest(
            Name: _name,
            SubTitle: _subTitle,
            Content: _content,
            ContentEditMode: _contentEditMode,
            ContentFormat: _contentFormat,
            OrderIndex: _orderIndex,
            Hidden: _hidden,
            ParentPageId: _parentPageId
        );

        await _docsService.EditPageAsync(
            _workspaceId,
            _docId,
            _pageId,
            updatePageRequest,
            cancellationToken
        );
    }
}