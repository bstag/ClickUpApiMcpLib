using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat;       // For ChatChannelLocation
using ClickUp.Api.Client.Models.Entities.Chat.Enums; // For ChatRoomVisibility

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the request to update an existing chat channel.
    /// All properties are optional; only provided properties will be updated.
    /// </summary>
    /// <param name="ContentFormat">The format of the description and topic content (e.g., "text/plain").</param>
    /// <param name="Description">New description for the channel.</param>
    /// <param name="Location">New location (Folder, List, or Space) to associate the channel with.</param>
    /// <param name="Name">New name for the channel.</param>
    /// <param name="Topic">New topic for the channel.</param>
    /// <param name="Visibility">New visibility for the channel (Public or Private).</param>
    /// <param name="Archived">Set to true to archive the channel, false to unarchive.</param>
    public record ChatUpdateChatChannelRequest
    (
        [property: JsonPropertyName("content_format")] string? ContentFormat,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("location")] ChatChannelLocation? Location,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("topic")] string? Topic,
        [property: JsonPropertyName("visibility")] ChatRoomVisibility? Visibility,
        [property: JsonPropertyName("archived")] bool? Archived
    );
}
