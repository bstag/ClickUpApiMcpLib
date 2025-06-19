using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat;       // For ChatChannelLocation
using ClickUp.Api.Client.Models.Entities.Chat.Enums; // For ChatRoomVisibility

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    public record ChatUpdateChatChannelRequest
    (
        [property: JsonPropertyName("content_format")] string? ContentFormat, // e.g. "text/plain" for description/topic
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("location")] ChatChannelLocation? Location, // To associate with a folder, list, or space
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("topic")] string? Topic,
        [property: JsonPropertyName("visibility")] ChatRoomVisibility? Visibility, // PUBLIC or PRIVATE
        [property: JsonPropertyName("archived")] bool? Archived // To archive/unarchive the channel
    );
}
