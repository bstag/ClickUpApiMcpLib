using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record ChatLastReadAtData
    (
        [property: JsonPropertyName("parent_id")] string ParentId, // ID of the parent context (e.g., channel ID)
        [property: JsonPropertyName("parent_type")] int ParentType, // Type of the parent context
        [property: JsonPropertyName("root_parent_id")] string? RootParentId, // Root parent ID (e.g., workspace ID)
        [property: JsonPropertyName("root_parent_type")] int? RootParentType, // Type of the root parent
        [property: JsonPropertyName("date")] long? Date, // Timestamp of last read
        [property: JsonPropertyName("version")] int? Version, // Version of the read state
        [property: JsonPropertyName("has_unread")] bool? HasUnread,
        [property: JsonPropertyName("num_unread")] int? NumUnread, // Number of unread messages
        [property: JsonPropertyName("latest_comment_at")] long? LatestCommentAt, // Timestamp of the latest comment
        [property: JsonPropertyName("badge_count")] int? BadgeCount, // Count for badging purposes
        [property: JsonPropertyName("thread_count")] int? ThreadCount, // Number of threads (if applicable)
        [property: JsonPropertyName("mention_count")] int? MentionCount // Number of mentions
    );
}
