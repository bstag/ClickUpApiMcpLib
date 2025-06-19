using System.Text.Json.Serialization;
using System.Collections.Generic; // For List

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    // Enum for Message Type, if applicable and distinct from PostType
    // [JsonConverter(typeof(JsonStringEnumConverter))]
    // public enum ChatMessageType { MESSAGE, POST }

    public record ChatMessage
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("assignee")] ChatSimpleUser? Assignee, // User who is assigned this message (if it's a task-like message)
        [property: JsonPropertyName("assigned_by")] ChatSimpleUser? AssignedBy, // User who assigned it
        [property: JsonPropertyName("content")] string? Content, // Actual message content (text, markdown, or JSON for block kit)
        [property: JsonPropertyName("content_format")] string? ContentFormat, // e.g., "text/plain", "text/md", "application/json+clickup_block_kit"
        [property: JsonPropertyName("date")] long Date, // Timestamp of message creation
        [property: JsonPropertyName("date_assigned")] long? DateAssigned, // Timestamp
        [property: JsonPropertyName("date_resolved")] long? DateResolved, // Timestamp
        [property: JsonPropertyName("date_updated")] long? DateUpdated, // Timestamp of last update
        [property: JsonPropertyName("group_assignee")] List<ChatSimpleUser>? GroupAssignee, // List of users if assigned to a group
        [property: JsonPropertyName("parent_channel")] ChatSimpleChannelInfo? ParentChannel, // Simplified info about the parent channel
        [property: JsonPropertyName("parent_message")] ChatSimpleMessageInfo? ParentMessage, // Simplified info if this is a reply
        [property: JsonPropertyName("post_data")] ChatPostData? PostData, // If type is "post"
        [property: JsonPropertyName("resolved")] bool? Resolved,
        [property: JsonPropertyName("resolved_by")] ChatSimpleUser? ResolvedBy,
        [property: JsonPropertyName("triaged_action")] string? TriagedAction, // e.g., "task_created"
        [property: JsonPropertyName("triaged_object_id")] string? TriagedObjectId, // ID of the object created via triage (e.g., task ID)
        [property: JsonPropertyName("triaged_object_type")] string? TriagedObjectType, // e.g., "task"
        [property: JsonPropertyName("type")] string Type, // "message", "post", etc. Could be an enum if values are fixed.
        [property: JsonPropertyName("user")] ChatSimpleUser User, // The user who sent the message. Changed from user_id to be object.
        [property: JsonPropertyName("links")] CommentChatMessageLinks2? Links,
        [property: JsonPropertyName("replies_count")] int? RepliesCount,
        [property: JsonPropertyName("reactions")] List<ChatReaction>? Reactions, // List of reactions on the message
        [property: JsonPropertyName("followers")] List<ChatSimpleUser>? Followers, // Users following this message/post
        [property: JsonPropertyName("workspace_id")] string WorkspaceId
    );

    // Simplified Channel Info for parent_channel reference
    public record ChatSimpleChannelInfo
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string? Name
    );

    // Simplified Message Info for parent_message reference
    public record ChatSimpleMessageInfo
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("user_id")] string UserId,
        [property: JsonPropertyName("date")] long Date
    );
}
