using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Webhooks;
using ClickUp.Api.Client.Models.RequestModels.Webhooks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentUpdateWebhookRequest
{
    private string? _endpoint;
    private List<string>? _events;
    private string? _status;
    private string? _secret;

    private readonly string _webhookId;
    private readonly IWebhooksService _webhooksService;

    public FluentUpdateWebhookRequest(string webhookId, IWebhooksService webhooksService)
    {
        _webhookId = webhookId;
        _webhooksService = webhooksService;
    }

    public FluentUpdateWebhookRequest WithEndpoint(string endpoint)
    {
        _endpoint = endpoint;
        return this;
    }

    public FluentUpdateWebhookRequest WithEvents(List<string> events)
    {
        _events = events;
        return this;
    }

    public FluentUpdateWebhookRequest WithStatus(string status)
    {
        _status = status;
        return this;
    }

    public FluentUpdateWebhookRequest WithSecret(string secret)
    {
        _secret = secret;
        return this;
    }

    public async Task<Webhook> UpdateAsync(CancellationToken cancellationToken = default)
    {
        var updateWebhookRequest = new UpdateWebhookRequest(
            Endpoint: _endpoint,
            Events: _events,
            Status: _status,
            Secret: _secret
        );

        return await _webhooksService.UpdateWebhookAsync(
            _webhookId,
            updateWebhookRequest,
            cancellationToken
        );
    }
}