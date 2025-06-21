using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the API links related to a specific chat channel.
/// These are typically URLs to other related API endpoints.
/// </summary>
/// <param name="Channel">Link to the channel itself.</param>
/// <param name="ChannelSettings">Link to the channel settings.</param>
/// <param name="Members">Link to the members of the channel.</param>
/// <param name="Messages">Link to the messages in the channel.</param>
/// <param name="Reactions">Link to the reactions in the channel.</param>
/// <param name="Uploads">Link to the uploads in the channel.</param>
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
    /// <summary>
    /// Gets the link to calls within the channel, if applicable.
    /// </summary>
    [JsonPropertyName("calls")]
    public string? Calls { get; init; }

    /// <summary>
    /// Gets the link to threads within the channel, if applicable.
    /// </summary>
    [JsonPropertyName("threads")]
    public string? Threads { get; init; }
}
