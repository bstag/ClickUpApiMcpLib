using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat.Enums; // For ChatRoomVisibility

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    public record ChatCreateChatChannelRequest
    (
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("name")] string Name, // Required for channels
        [property: JsonPropertyName("topic")] string? Topic,
        [property: JsonPropertyName("user_ids")] List<string> UserIds, // IDs of users to add to the channel
        [property: JsonPropertyName("visibility")] ChatRoomVisibility Visibility, // PUBLIC or PRIVATE
        [property: JsonPropertyName("workspace_id")] string WorkspaceId // Usually required
    );
}
