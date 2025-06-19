using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Webhooks; // For Webhook entity

namespace ClickUp.Api.Client.Models.ResponseModels.Webhooks
{
    public record GetWebhooksResponse
    (
        [property: JsonPropertyName("webhooks")] List<Webhook> Webhooks
    );
}
