using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Webhooks; // For Webhook entity

namespace ClickUp.Api.Client.Models.ResponseModels.Webhooks
{
    public record UpdateWebhookResponse
    (
        // The ClickUp API documentation for "Update Webhook" shows the response is the full Webhook object.
        // So, this can inherit from Webhook or contain a Webhook property.
        // For consistency with CreateWebhookResponse, containing a 'webhook' property.
        [property: JsonPropertyName("id")] string Id, // ID of the webhook (should match the one in the path)
        [property: JsonPropertyName("webhook")] Webhook Webhook // The updated webhook details
    );
}
