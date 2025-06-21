using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents the last read data for a chat channel.
/// Corresponds to #/components/schemas/ChatChannel_last_read_at_data
/// </summary>
public record ChatLastReadAtData
{
    /// <summary>
    /// Gets the Unix timestamp (milliseconds) when the current user last read messages in the channel.
    /// </summary>
    [JsonPropertyName("last_read_at")]
    public long LastReadAt { get; init; }

    /// <summary>
    /// Gets the ID of the last message read by the current user.
    /// </summary>
    [JsonPropertyName("last_read_message_id")]
    public string? LastReadMessageId { get; init; }

    /// <summary>
    /// Gets the ID of the last message seen by the current user (may differ from last read).
    /// </summary>
    [JsonPropertyName("last_seen_message_id")]
    public string? LastSeenMessageId { get; init; }

    /// <summary>
    /// Gets a dictionary representing the comment version vector.
    /// This is used for synchronization and tracking message versions across clients.
    /// Keys are typically user IDs or client identifiers, and values are version numbers.
    /// </summary>
    [JsonPropertyName("comment_version_vector")]
    public Dictionary<string, int>? CommentVersionVector { get; init; }

    /// <summary>
    /// Gets a dictionary representing the comment vector.
    /// Similar to the version vector, this is used for synchronization.
    /// Keys are typically user IDs or client identifiers, and values are counts or versions.
    /// </summary>
    [JsonPropertyName("comment_vector")]
    public Dictionary<string, int>? CommentVector { get; init; }

    /// <summary>
    /// Gets the number of unread messages for the current user in this channel.
    /// </summary>
    [JsonPropertyName("unread_count")]
    public int? UnreadCount { get; init; }

    /// <summary>
    /// Gets the number of times the current user has been mentioned in unread messages.
    /// </summary>
    [JsonPropertyName("mentioned_count")]
    public int? MentionedCount { get; init; }
}
