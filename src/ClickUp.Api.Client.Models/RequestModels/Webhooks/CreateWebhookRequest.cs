using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Webhooks
{
    /// <summary>
    /// Represents the request to create a new Webhook.
    /// </summary>
    /// <param name="Endpoint">The URL where webhook payloads will be sent.</param>
    /// <param name="Events">A list of event names to subscribe to (e.g., "taskCreated", "taskUpdated", or "*" for all events).</param>
    /// <param name="SpaceId">Optional: The ID of a Space to scope the webhook to.</param>
    /// <param name="FolderId">Optional: The ID of a Folder (Project) to scope the webhook to.</param>
    /// <param name="ListId">Optional: The ID of a List to scope the webhook to.</param>
    /// <param name="TaskId">Optional: The ID of a CuTask to scope the webhook to.</param>
    /// <param name="TeamId">Optional: The ID of the workspace (team). Often inferred from the API token or context, but can be specified.</param>
    public record CreateWebhookRequest
    (
        [property: JsonPropertyName("endpoint")] string Endpoint,
        [property: JsonPropertyName("events")] List<string> Events,
        [property: JsonPropertyName("space_id")] int? SpaceId,
        [property: JsonPropertyName("folder_id")] int? FolderId,
        [property: JsonPropertyName("list_id")] int? ListId,
        [property: JsonPropertyName("task_id")] string? TaskId,
        [property: JsonPropertyName("team_id")] int? TeamId
    );
}
