using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Webhooks
{
    /// <summary>
    /// Represents the health status of a Webhook.
    /// </summary>
    /// <param name="Status">The current health status of the webhook (e.g., "active", "failing").</param>
    /// <param name="FailCount">The number of consecutive failed attempts to send a payload to the webhook endpoint.</param>
    /// <param name="LastAttempted">The date of the last attempt to send a payload, as a string (e.g., Unix timestamp or ISO 8601).</param>
    /// <param name="LastSuccessful">The date of the last successful payload delivery, as a string.</param>
    public record WebhookHealth
    (
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("fail_count")] int? FailCount,
        [property: JsonPropertyName("last_attempted")] string? LastAttempted,
        [property: JsonPropertyName("last_successful")] string? LastSuccessful
    );
}
