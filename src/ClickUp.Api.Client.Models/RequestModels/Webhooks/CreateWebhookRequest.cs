using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Webhooks
{
    public record CreateWebhookRequest
    (
        [property: JsonPropertyName("endpoint")] string Endpoint,
        [property: JsonPropertyName("events")] List<string> Events, // e.g., ["taskCreated", "taskUpdated", "*"]
        [property: JsonPropertyName("space_id")] int? SpaceId, // Scope to a specific Space
        [property: JsonPropertyName("folder_id")] int? FolderId, // Scope to a specific Folder (Project)
        [property: JsonPropertyName("list_id")] int? ListId, // Scope to a specific List
        [property: JsonPropertyName("task_id")] string? TaskId, // Scope to a specific Task
        [property: JsonPropertyName("team_id")] int? TeamId // Workspace ID, often required if not inferred from token or context
    );
}
