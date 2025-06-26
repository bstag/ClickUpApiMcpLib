using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Webhooks;
using ClickUp.Api.Client.Models.RequestModels.Webhooks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreateWebhookRequest
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

    public FluentCreateWebhookRequest(string workspaceId, IWebhooksService webhooksService)
    {
        _workspaceId = workspaceId;
        _webhooksService = webhooksService;
    }

    public FluentCreateWebhookRequest WithEndpoint(string endpoint)
    {
        _endpoint = endpoint;
        return this;
    }

    public FluentCreateWebhookRequest WithEvents(List<string> events)
    {
        _events = events;
        return this;
    }

    public FluentCreateWebhookRequest WithSpaceId(int spaceId)
    {
        _spaceId = spaceId;
        return this;
    }

    public FluentCreateWebhookRequest WithFolderId(int folderId)
    {
        _folderId = folderId;
        return this;
    }

    public FluentCreateWebhookRequest WithListId(int listId)
    {
        _listId = listId;
        return this;
    }

    public FluentCreateWebhookRequest WithTaskId(string taskId)
    {
        _taskId = taskId;
        return this;
    }

    public FluentCreateWebhookRequest WithTeamId(int teamId)
    {
        _teamId = teamId;
        return this;
    }

    public async Task<Webhook> CreateAsync(CancellationToken cancellationToken = default)
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