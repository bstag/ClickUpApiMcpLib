using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Webhooks;
using ClickUp.Api.Client.Models.RequestModels.Webhooks;
using ClickUp.Api.Client.Models.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class WebhookFluentUpdateRequest
{
    private string? _endpoint;
    private List<string>? _events;
    private string? _status; // e.g., "active", "disabled"
    private string? _secret;

    private readonly string _webhookId;
    private readonly IWebhooksService _webhooksService;
    private readonly List<string> _validationErrors = new List<string>();

    public WebhookFluentUpdateRequest(string webhookId, IWebhooksService webhooksService)
    {
        _webhookId = webhookId;
        _webhooksService = webhooksService;
    }

    public WebhookFluentUpdateRequest WithEndpoint(string endpoint)
    {
        _endpoint = endpoint;
        return this;
    }

    public WebhookFluentUpdateRequest WithEvents(List<string> events)
    {
        _events = events;
        return this;
    }

    public WebhookFluentUpdateRequest WithStatus(string status)
    {
        _status = status;
        return this;
    }

    public WebhookFluentUpdateRequest WithSecret(string secret)
    {
        _secret = secret;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_webhookId))
        {
            _validationErrors.Add("WebhookId is required.");
        }
        if (string.IsNullOrWhiteSpace(_endpoint) &&
            (_events == null || !_events.Any()) &&
            string.IsNullOrWhiteSpace(_status) &&
            string.IsNullOrWhiteSpace(_secret)) // Also check secret now
        {
            _validationErrors.Add("At least one property (Endpoint, Events, Status, or Secret) must be set for updating a Webhook.");
        }
        if (!string.IsNullOrWhiteSpace(_endpoint) && !Uri.TryCreate(_endpoint, UriKind.Absolute, out _))
        {
            _validationErrors.Add("If Endpoint URL is provided, it must be a valid absolute URI.");
        }
        if (_events != null && _events.Contains("*") && _events.Count > 1)
        {
            _validationErrors.Add("If '*' (all events) is specified, no other events should be listed.");
        }
        if (!string.IsNullOrWhiteSpace(_status) && !(_status.ToLower() == "active" || _status.ToLower() == "disabled"))
        {
            _validationErrors.Add("Status must be 'active' or 'disabled' if provided.");
        }
        // Secret validation (e.g. length, complexity) could be added if API has such rules.

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<Webhook> UpdateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
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