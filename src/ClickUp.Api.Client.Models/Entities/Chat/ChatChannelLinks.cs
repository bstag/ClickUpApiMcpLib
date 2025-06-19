using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the links related to a chat channel.
/// Corresponds to #/components/schemas/ChatChannel_links
/// </summary>
public record ChatChannelLinks
{
    [JsonPropertyName("channel")]
    public string Channel { get; init; }

    [JsonPropertyName("channel_settings")]
    public string ChannelSettings { get; init; }

    [JsonPropertyName("members")]
    public string Members { get; init; }

    [JsonPropertyName("messages")]
    public string Messages { get; init; }

    [JsonPropertyName("reactions")]
    public string Reactions { get; init; }

    [JsonPropertyName("uploads")]
    public string Uploads { get; init; }

    [JsonPropertyName("calls")]
    public string? Calls { get; init; }

    [JsonPropertyName("threads")]
    public string? Threads { get; init; }
}
