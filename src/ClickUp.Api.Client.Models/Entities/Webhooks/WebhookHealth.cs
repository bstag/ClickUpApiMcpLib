using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Webhooks
{
    public record WebhookHealth
    (
        [property: JsonPropertyName("status")] string? Status, // e.g., "active", "failing"
        [property: JsonPropertyName("fail_count")] int? FailCount,
        [property: JsonPropertyName("last_attempted")] string? LastAttempted, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("last_successful")] string? LastSuccessful // Assuming string, could be DateTimeOffset
    );
}
