using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using ClickUp.Api.Client.Models.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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
    private readonly List<string> _validationErrors = new List<string>();

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

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_workspaceId))
        {
            _validationErrors.Add("WorkspaceId is required.");
        }
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors.Add("Doc name is required.");
        }
        // Add other validation rules as needed (e.g., for _visibility if it has specific allowed values)

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<Doc> CreateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
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