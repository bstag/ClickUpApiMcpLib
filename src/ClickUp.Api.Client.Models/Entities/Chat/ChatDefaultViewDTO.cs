using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

public record ChatDefaultViewDTO(
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("name")]
    string Name
)
{
    /// <summary>
    /// Gets the JSON string representing detailed view settings.
    /// The specific structure of this JSON string can vary based on the view type.
    /// </summary>
    [JsonPropertyName("view_settings")]
    public string? ViewSettings { get; init; }
}

/// <summary>
/// Represents the default view settings for a chat room.
/// Corresponds to #/components/schemas/ChatRoom_default_view
/// </summary>
/// <param name="Type">Type of the view. Example: "list"</param>
/// <param name="Id">ID of the view.</param>
/// <param name="Name">Name of the view.</param>
