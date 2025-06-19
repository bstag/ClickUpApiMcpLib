using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the parent of a chat room.
/// Corresponds to #/components/schemas/ChatRoom_parent
/// </summary>
public record ChatRoomParentDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    /// <summary>
    /// The type of the parent item.
    /// </summary>
    /// <example>"task"</example>
    /// <example>"list"</example>
    /// <example>"folder"</example>
    /// <example>"space"</example>
    /// <example>"team"</example>
    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("url")]
    public string Url { get; init; }

    // "access" is not part of the ChatRoom_parent schema, removing it.
    // If it's needed for other contexts, it should be in a different DTO.
}
