using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the parent of a chat room.
/// Corresponds to #/components/schemas/ChatRoom_parent
/// </summary>
/// <param name="Id">The ID of the parent item.</param>
/// <param name="Type">The type of the parent item. Examples: "task", "list", "folder", "space", "team".</param>
/// <param name="Name">The name of the parent item.</param>
/// <param name="Url">The URL of the parent item.</param>
public record ChatRoomParentDTO(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("url")]
    string Url
);
