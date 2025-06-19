namespace ClickUp.Api.Client.Models.Shared;

/// <summary>
/// Represents the health status of a Webhook.
/// </summary>
public record WebhookHealth
{
    public string Status { get; init; } = string.Empty; // e.g., "active", "failing"
    public int FailCount { get; init; }
}
