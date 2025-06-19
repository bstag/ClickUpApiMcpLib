using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents links related to a chat message.
/// Corresponds to #/components/schemas/CommentChatMessageLinks2
/// </summary>
public record CommentChatMessageLinks2
{
    [JsonPropertyName("channel")]
    public string Channel { get; init; }

    [JsonPropertyName("channel_settings")]
    public string ChannelSettings { get; init; }

    [JsonPropertyName("members")]
    public string Members { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; }

    [JsonPropertyName("message_settings")]
    public string MessageSettings { get; init; }

    [JsonPropertyName("reactions")]
    public string Reactions { get; init; }

    [JsonPropertyName("threads")]
    public string Threads { get; init; }

    [JsonPropertyName("uploads")]
    public string Uploads { get; init; }

    [JsonPropertyName("user")]
    public string User { get; init; }
}
