using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat;       // For ChatChannelLocation
using ClickUp.Api.Client.Models.Entities.Chat.Enums; // For ChatRoomVisibility

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    public record ChatCreateLocationChatChannelRequest
    (
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("topic")] string? Topic,
        [property: JsonPropertyName("user_ids")] List<string>? UserIds, // Optional: users to add initially
        [property: JsonPropertyName("visibility")] ChatRoomVisibility Visibility,
        [property: JsonPropertyName("location")] ChatChannelLocation Location, // Required: folder, list, or space to associate with
        [property: JsonPropertyName("name")] string Name, // Name for the location-based channel
        [property: JsonPropertyName("workspace_id")] string WorkspaceId // Usually required
    );
}
