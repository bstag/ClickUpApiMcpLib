using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    // This model represents a reply to a message.
    // It's likely very similar to ChatMessage, but the links might differ (e.g., no "replies" link for a reply itself).
    // The 'type' might also be constrained.
    public record ReplyMessage
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("assignee")] ChatSimpleUser? Assignee,
        [property: JsonPropertyName("assigned_by")] ChatSimpleUser? AssignedBy,
        [property: JsonPropertyName("content")] string? Content,
        [property: JsonPropertyName("content_format")] string? ContentFormat,
        [property: JsonPropertyName("date")] long Date,
        [property: JsonPropertyName("date_assigned")] long? DateAssigned,
        [property: JsonPropertyName("date_resolved")] long? DateResolved,
        [property: JsonPropertyName("date_updated")] long? DateUpdated,
        [property: JsonPropertyName("group_assignee")] List<ChatSimpleUser>? GroupAssignee,
        [property: JsonPropertyName("parent_channel")] ChatSimpleChannelInfo? ParentChannel, // From ChatMessage.cs
        [property: JsonPropertyName("parent_message")] ChatSimpleMessageInfo? ParentMessage, // From ChatMessage.cs, this would be the message it's replying to
        // No post_data for replies typically
        [property: JsonPropertyName("resolved")] bool? Resolved,
        [property: JsonPropertyName("resolved_by")] ChatSimpleUser? ResolvedBy,
        // Triage fields might not apply to replies
        [property: JsonPropertyName("type")] string Type, // Typically "message" for a reply
        [property: JsonPropertyName("user")] ChatSimpleUser User,
        [property: JsonPropertyName("links")] CommentReplyMessageLinks2? Links, // Uses CommentReplyMessageLinks2
        [property: JsonPropertyName("reactions")] List<ChatReaction>? Reactions,
        [property: JsonPropertyName("followers")] List<ChatSimpleUser>? Followers,
        [property: JsonPropertyName("workspace_id")] string WorkspaceId
    );
}
