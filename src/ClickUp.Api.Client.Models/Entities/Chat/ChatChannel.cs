using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat.Enums; // For enums
using System.Collections.Generic; // For List

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record ChatChannel
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string? Name, // Nullable for DMs, typically has a name for channels
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("topic")] string? Topic,
        [property: JsonPropertyName("type")] ChatRoomType Type,
        [property: JsonPropertyName("visibility")] ChatRoomVisibility? Visibility, // Nullable, might not apply to DMs
        [property: JsonPropertyName("parent")] ChatRoomParentDTO? Parent,
        [property: JsonPropertyName("creator")] ChatSimpleUser? Creator, // Or full User from Common
        [property: JsonPropertyName("created_at")] long? CreatedAt, // Timestamp
        [property: JsonPropertyName("workspace_id")] string WorkspaceId,
        [property: JsonPropertyName("archived")] bool? Archived,
        [property: JsonPropertyName("latest_comment_at")] long? LatestCommentAt, // Timestamp
        [property: JsonPropertyName("is_canonical_channel")] bool? IsCanonicalChannel,
        [property: JsonPropertyName("is_hidden")] bool? IsHidden,
        [property: JsonPropertyName("default_view")] ChatDefaultViewDTO? DefaultView,
        [property: JsonPropertyName("channel_type")] ChatSubcategoryType? ChannelType, // Using the enum, assuming string representation from API
        [property: JsonPropertyName("counts")] ChatLastReadAtData? Counts, // Represents unread counts etc.
        [property: JsonPropertyName("chat_room_category")] string? ChatRoomCategory, // e.g., "WELCOME_CHANNEL", could be an enum
        [property: JsonPropertyName("links")] ChatChannelLinks? Links,
        [property: JsonPropertyName("members")] List<ChatSimpleUser>? Members, // List of members in the channel
        [property: JsonPropertyName("last_message")] object? LastMessage // Could be a simplified ChatMessage summary
    );
}
