using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.Webhooks
{
    /// <summary>
    /// Represents a Webhook configuration in ClickUp.
    /// </summary>
    /// <param name="Id">The unique identifier of the webhook.</param>
    /// <param name="UserId">The identifier of the user who created the webhook.</param>
    /// <param name="User">The user object for who created the webhook, if available.</param>
    /// <param name="TeamId">The identifier of the team (workspace) this webhook belongs to.</param>
    /// <param name="Endpoint">The URL endpoint where webhook payloads will be sent.</param>
    /// <param name="ClientId">The client ID if this webhook is associated with an OAuth application.</param>
    /// <param name="Events">A list of event names that trigger this webhook (e.g., "taskCreated", "taskUpdated").</param>
    /// <param name="TaskId">The identifier of a specific task this webhook is scoped to, if any.</param>
    /// <param name="ListId">The identifier of a specific list this webhook is scoped to, if any.</param>
    /// <param name="FolderId">The identifier of a specific folder this webhook is scoped to, if any.</param>
    /// <param name="SpaceId">The identifier of a specific space this webhook is scoped to, if any.</param>
    /// <param name="Health">Information about the health status of the webhook.</param>
    /// <param name="Secret">The secret key used for verifying the authenticity of webhook payloads.</param>
    /// <param name="Status">The current status of the webhook (e.g., "active", "disabled").</param>
    /// <param name="DateCreated">The date the webhook was created, as a string (e.g., Unix timestamp in milliseconds or ISO 8601).</param>
    public record Webhook
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("userid")] int UserId,
        [property: JsonPropertyName("user")] User? User,
        [property: JsonPropertyName("team_id")] int TeamId,
        [property: JsonPropertyName("endpoint")] string Endpoint,
        [property: JsonPropertyName("client_id")] string? ClientId,
        [property: JsonPropertyName("events")] List<string> Events,
        [property: JsonPropertyName("task_id")] string? TaskId,
        [property: JsonPropertyName("list_id")] string? ListId,
        [property: JsonPropertyName("folder_id")] string? FolderId,
        [property: JsonPropertyName("space_id")] string? SpaceId,
        [property: JsonPropertyName("health")] WebhookHealth? Health,
        [property: JsonPropertyName("secret")] string? Secret,
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("date_created")] string? DateCreated
    );
}
