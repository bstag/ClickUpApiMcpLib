using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the links related to a chat channel.
/// Corresponds to #/components/schemas/ChatChannel_links
/// </summary>
public record ChatChannelLinks(
    [property: JsonPropertyName("channel")]
    string Channel,
    [property: JsonPropertyName("channel_settings")]
    string ChannelSettings,
    [property: JsonPropertyName("members")]
    string Members,
    [property: JsonPropertyName("messages")]
    string Messages,
    [property: JsonPropertyName("reactions")]
    string Reactions,
    [property: JsonPropertyName("uploads")]
    string Uploads
)
{
    [JsonPropertyName("calls")]
    public string? Calls { get; init; }

    [JsonPropertyName("threads")]
    public string? Threads { get; init; }
}
