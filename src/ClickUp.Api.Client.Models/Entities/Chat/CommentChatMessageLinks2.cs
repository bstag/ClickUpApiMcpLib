using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents links related to a chat message.
/// Corresponds to #/components/schemas/CommentChatMessageLinks2
/// </summary>
public record CommentChatMessageLinks2(
    [property: JsonPropertyName("channel")]
    string Channel,
    [property: JsonPropertyName("channel_settings")]
    string ChannelSettings,
    [property: JsonPropertyName("members")]
    string Members,
    [property: JsonPropertyName("message")]
    string Message,
    [property: JsonPropertyName("message_settings")]
    string MessageSettings,
    [property: JsonPropertyName("reactions")]
    string Reactions,
    [property: JsonPropertyName("threads")]
    string Threads,
    [property: JsonPropertyName("uploads")]
    string Uploads,
    [property: JsonPropertyName("user")]
    string User
);
