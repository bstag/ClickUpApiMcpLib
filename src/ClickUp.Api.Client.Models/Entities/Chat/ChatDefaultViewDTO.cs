using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the default view settings for a chat room.
/// Corresponds to #/components/schemas/ChatRoom_default_view
/// </summary>
public record ChatDefaultViewDTO(
    /// <summary>
    /// Type of the view.
    /// </summary>
    /// <example>"list"</example>
    [property: JsonPropertyName("type")]
    string Type,
    /// <summary>
    /// ID of the view.
    /// </summary>
    [property: JsonPropertyName("id")]
    string Id,
    /// <summary>
    /// Name of the view.
    /// </summary>
    [property: JsonPropertyName("name")]
    string Name
)
{
    /// <summary>
    /// JSON string representing view settings.
    /// </summary>
    [JsonPropertyName("view_settings")]
    public string? ViewSettings { get; init; }
}
