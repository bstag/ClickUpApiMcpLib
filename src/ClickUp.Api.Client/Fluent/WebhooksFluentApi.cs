using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Webhooks;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class WebhooksFluentApi
{
    private readonly IWebhooksService _webhooksService;

    public WebhooksFluentApi(IWebhooksService webhooksService)
    {
        _webhooksService = webhooksService;
    }

    public async Task<IEnumerable<Webhook>> GetWebhooksAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _webhooksService.GetWebhooksAsync(workspaceId, cancellationToken);
    }

    public async IAsyncEnumerable<Webhook> GetWebhooksAsyncEnumerableAsync(string workspaceId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var webhooks = await _webhooksService.GetWebhooksAsync(workspaceId, cancellationToken).ConfigureAwait(false);
        if (webhooks == null)
        {
            yield break;
        }

        foreach (var webhook in webhooks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return webhook;
        }
    }

    public WebhookFluentCreateRequest CreateWebhook(string workspaceId)
    {
        return new WebhookFluentCreateRequest(workspaceId, _webhooksService);
    }

    public WebhookFluentUpdateRequest UpdateWebhook(string webhookId)
    {
        return new WebhookFluentUpdateRequest(webhookId, _webhooksService);
    }

    public async Task DeleteWebhookAsync(string webhookId, CancellationToken cancellationToken = default)
    {
        await _webhooksService.DeleteWebhookAsync(webhookId, cancellationToken);
    }
}