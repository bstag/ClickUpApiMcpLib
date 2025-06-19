using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the last read data for a chat channel.
/// Corresponds to #/components/schemas/ChatChannel_last_read_at_data
/// </summary>
public record ChatLastReadAtData
{
    [JsonPropertyName("last_read_at")]
    public long LastReadAt { get; init; } // Unix timestamp

    [JsonPropertyName("last_read_message_id")]
    public string? LastReadMessageId { get; init; }

    [JsonPropertyName("last_seen_message_id")]
    public string? LastSeenMessageId { get; init; }

    /// <summary>
    /// A dictionary representing the comment version vector.
    /// Keys are typically user IDs or similar identifiers, values are version numbers.
    /// </summary>
    [JsonPropertyName("comment_version_vector")]
    public Dictionary<string, int>? CommentVersionVector { get; init; }

    /// <summary>
    /// A dictionary representing the comment vector.
    /// Keys are typically user IDs or similar identifiers, values are counts or versions.
    /// </summary>
    [JsonPropertyName("comment_vector")]
    public Dictionary<string, int>? CommentVector { get; init; }

    [JsonPropertyName("unread_count")]
    public int? UnreadCount { get; init; }

    [JsonPropertyName("mentioned_count")]
    public int? MentionedCount { get; init; }
}
