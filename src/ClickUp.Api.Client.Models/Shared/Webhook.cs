using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Shared;

/// <summary>
/// Represents a Webhook configuration in ClickUp.
/// </summary>
public record Webhook
{
    public string Id { get; init; } = string.Empty;
    public int UserId { get; init; } // User who created it
    public int TeamId { get; init; } // Workspace ID
    public string Endpoint { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public List<string> Events { get; init; } = new();
    public string? TaskId { get; init; }
    public string? ListId { get; init; } // Using string? for flexibility as API spec/examples differ (string vs int)
    public string? FolderId { get; init; } // Using string?
    public string? SpaceId { get; init; } // Using string?
    public WebhookHealth Health { get; init; } = null!;
    public string Secret { get; init; } = string.Empty;
}
