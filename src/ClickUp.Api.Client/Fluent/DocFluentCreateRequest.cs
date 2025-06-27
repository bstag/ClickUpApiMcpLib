using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class DocFluentCreateRequest
{
    private ParentDocIdentifier? _parent;
    private string? _visibility;
    private bool? _createPage;
    private string? _templateId;
    private long? _workspaceIdForDoc;

    private readonly string _workspaceId;
    private readonly IDocsService _docsService;
    private readonly string _name;

    public DocFluentCreateRequest(string workspaceId, IDocsService docsService, string name)
    {
        _workspaceId = workspaceId;
        _docsService = docsService;
        _name = name;
    }

    public DocFluentCreateRequest WithParent(ParentDocIdentifier parent)
    {
        _parent = parent;
        return this;
    }

    public DocFluentCreateRequest WithVisibility(string visibility)
    {
        _visibility = visibility;
        return this;
    }

    public DocFluentCreateRequest WithCreatePage(bool createPage)
    {
        _createPage = createPage;
        return this;
    }

    public DocFluentCreateRequest WithTemplateId(string templateId)
    {
        _templateId = templateId;
        return this;
    }

    public DocFluentCreateRequest WithWorkspaceIdForDoc(long workspaceIdForDoc)
    {
        _workspaceIdForDoc = workspaceIdForDoc;
        return this;
    }

    public async Task<Doc> CreateAsync(CancellationToken cancellationToken = default)
    {
        var createDocRequest = new CreateDocRequest(
            Name: _name,
            Parent: _parent,
            Visibility: _visibility,
            CreatePage: _createPage,
            TemplateId: _templateId,
            WorkspaceId: _workspaceIdForDoc
        );

        return await _docsService.CreateDocAsync(
            _workspaceId,
            createDocRequest,
            cancellationToken
        );
    }
}