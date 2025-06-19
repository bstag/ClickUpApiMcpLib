using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the default view settings for a chat room.
/// Corresponds to #/components/schemas/ChatRoom_default_view
/// </summary>
public record ChatDefaultViewDTO
{
    /// <summary>
    /// Type of the view.
    /// </summary>
    /// <example>"list"</example>
    [JsonPropertyName("type")]
    public string Type { get; init; }

    /// <summary>
    /// ID of the view.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; }

    /// <summary>
    /// Name of the view.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; }

    /// <summary>
    /// JSON string representing view settings.
    /// </summary>
    [JsonPropertyName("view_settings")]
    public string? ViewSettings { get; init; }
}
