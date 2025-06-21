using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.Webhooks
{
    public record Webhook
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("userid")] int UserId, // Assuming int for user ID
        [property: JsonPropertyName("user")] User? User, // Optional User object
        [property: JsonPropertyName("team_id")] int TeamId, // Assuming int for team/workspace ID
        [property: JsonPropertyName("endpoint")] string Endpoint,
        [property: JsonPropertyName("client_id")] string? ClientId, // Nullable if not an OAuth app webhook
        [property: JsonPropertyName("events")] List<string> Events, // List of event names e.g. "taskCreated", "taskUpdated"
        [property: JsonPropertyName("task_id")] string? TaskId, // Nullable, scope to specific task
        [property: JsonPropertyName("list_id")] string? ListId, // Nullable, scope to specific list
        [property: JsonPropertyName("folder_id")] string? FolderId, // Nullable, scope to specific folder
        [property: JsonPropertyName("space_id")] string? SpaceId, // Nullable, scope to specific space
        [property: JsonPropertyName("health")] WebhookHealth? Health,
        [property: JsonPropertyName("secret")] string? Secret, // Secret key for verifying payload
        [property: JsonPropertyName("status")] string? Status, // e.g. "active", "disabled"
        [property: JsonPropertyName("date_created")] string? DateCreated // Assuming string, could be DateTimeOffset
    );
}
