using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Webhooks
{
    /// <summary>
    /// Represents the request to update an existing Webhook.
    /// All properties are optional; only provided properties will be updated.
    /// Note: Changing scope (task_id, list_id, etc.) might require deleting and recreating the webhook.
    /// </summary>
    /// <param name="Endpoint">Optional: New URL endpoint for the webhook.</param>
    /// <param name="Events">Optional: New list of event names to subscribe to. This will replace the existing list of events.</param>
    /// <param name="Status">Optional: New status for the webhook (e.g., "active" or "inactive").</param>
    /// <param name="Secret">Optional: New secret key for verifying webhook payloads.</param>
    public record UpdateWebhookRequest
    (
        [property: JsonPropertyName("endpoint")] string? Endpoint,
        [property: JsonPropertyName("events")] List<string>? Events,
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("secret")] string? Secret
    );
}
