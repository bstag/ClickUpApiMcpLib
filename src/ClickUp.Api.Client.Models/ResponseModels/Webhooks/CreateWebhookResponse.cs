using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Webhooks; // For Webhook entity

namespace ClickUp.Api.Client.Models.ResponseModels.Webhooks
{
    public record CreateWebhookResponse
    (
        [property: JsonPropertyName("id")] string Id, // ID of the created webhook resource itself
        [property: JsonPropertyName("webhook")] Webhook Webhook // The details of the webhook created
    );
}
