using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Webhooks;
using ClickUp.Api.Client.Models.RequestModels.Webhooks;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class WebhookFluentCreateRequest
{
    private string? _endpoint;
    private List<string>? _events;
    private int? _spaceId;
    private int? _folderId;
    private int? _listId;
    private string? _taskId;
    private int? _teamId; // This seems redundant if _workspaceId is the team/workspace ID for the webhook itself. Clarify API.

    private readonly string _workspaceId; // This is typically the team_id for webhook creation
    private readonly IWebhooksService _webhooksService;
    private readonly List<string> _validationErrors = new List<string>();

    public WebhookFluentCreateRequest(string workspaceId, IWebhooksService webhooksService)
    {
        _workspaceId = workspaceId;
        _webhooksService = webhooksService;
    }

    public WebhookFluentCreateRequest WithEndpoint(string endpoint)
    {
        _endpoint = endpoint;
        return this;
    }

    public WebhookFluentCreateRequest WithEvents(List<string> events)
    {
        _events = events;
        return this;
    }

    public WebhookFluentCreateRequest WithSpaceId(int spaceId)
    {
        _spaceId = spaceId;
        return this;
    }

    public WebhookFluentCreateRequest WithFolderId(int folderId)
    {
        _folderId = folderId;
        return this;
    }

    public WebhookFluentCreateRequest WithListId(int listId)
    {
        _listId = listId;
        return this;
    }

    public WebhookFluentCreateRequest WithTaskId(string taskId)
    {
        _taskId = taskId;
        return this;
    }

    public WebhookFluentCreateRequest WithTeamId(int teamId)
    {
        // This might be intended for filtering events to a specific team if the webhook is workspace-global.
        // Or it might be the same as _workspaceId. Assuming it's for event filtering for now.
        _teamId = teamId;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_workspaceId)) // This is the team_id in the path
        {
            _validationErrors.Add("WorkspaceId (TeamId for webhook registration) is required.");
        }
        if (string.IsNullOrWhiteSpace(_endpoint))
        {
            _validationErrors.Add("Endpoint URL is required.");
        }
        if (!Uri.TryCreate(_endpoint, UriKind.Absolute, out _))
        {
            _validationErrors.Add("Endpoint URL must be a valid absolute URI.");
        }
        if (_events == null || !_events.Any())
        {
            _validationErrors.Add("At least one event must be specified.");
        }
        else if (_events.Contains("*") && _events.Count > 1)
        {
            _validationErrors.Add("If '*' (all events) is specified, no other events should be listed.");
        }
        // Further validation: Ensure that if a specific resource ID (spaceId, folderId, listId, taskId) is provided,
        // the events are relevant to that resource type, or that wildcard isn't used with specific resource IDs
        // unless the API supports it. This level of detail depends on API specifics.

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<Webhook> CreateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
        var createWebhookRequest = new CreateWebhookRequest(
            Endpoint: _endpoint ?? string.Empty,
            Events: _events ?? new List<string>(),
            SpaceId: _spaceId,
            FolderId: _folderId,
            ListId: _listId,
            TaskId: _taskId,
            TeamId: _teamId // This is part of the body, distinct from workspaceId in path
        );

        return await _webhooksService.CreateWebhookAsync(
            _workspaceId, // This is team_id for the path /team/{team_id}/webhook
            createWebhookRequest,
            cancellationToken
        );
    }
}