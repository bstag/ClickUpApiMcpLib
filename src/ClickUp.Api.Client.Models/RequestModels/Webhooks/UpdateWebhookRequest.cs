using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Webhooks
{
    public record UpdateWebhookRequest
    (
        [property: JsonPropertyName("endpoint")] string? Endpoint, // Optional: new endpoint URL
        [property: JsonPropertyName("events")] List<string>? Events, // Optional: new list of events to subscribe to
        [property: JsonPropertyName("status")] string? Status, // Optional: "active" or "inactive"
                                                                // Note: To change scope (task_id, list_id etc.),
                                                                // it might require deleting and recreating the webhook.
                                                                // This request typically only updates endpoint, events, and status.
        [property: JsonPropertyName("secret")] string? Secret // Optional: new secret for the webhook
    );
}
