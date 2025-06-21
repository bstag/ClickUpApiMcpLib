using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the parent of a chat room.
/// Corresponds to #/components/schemas/ChatRoom_parent
/// </summary>
public record ChatRoomParentDTO(
    [property: JsonPropertyName("id")]
    string Id,
    /// <summary>
    /// The type of the parent item.
    /// </summary>
    /// <example>"task"</example>
    /// <example>"list"</example>
    /// <example>"folder"</example>
    /// <example>"space"</example>
    /// <example>"team"</example>
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("url")]
    string Url
)
{
    // "access" is not part of the ChatRoom_parent schema, removing it.
    // If it's needed for other contexts, it should be in a different DTO.
}
