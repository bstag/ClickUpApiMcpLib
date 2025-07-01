using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Webhooks;
using ClickUp.Api.Client.Models.RequestModels.Webhooks;
using System.Collections.Generic;
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
    private int? _teamId;

    private readonly string _workspaceId;
    private readonly IWebhooksService _webhooksService;

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
        _teamId = teamId;
        return this;
    }

    public async Task<Webhook> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var createWebhookRequest = new CreateWebhookRequest(
            Endpoint: _endpoint ?? string.Empty,
            Events: _events ?? new List<string>(),
            SpaceId: _spaceId,
            FolderId: _folderId,
            ListId: _listId,
            TaskId: _taskId,
            TeamId: _teamId
        );

        return await _webhooksService.CreateWebhookAsync(
            _workspaceId,
            createWebhookRequest,
            cancellationToken
        );
    }
}