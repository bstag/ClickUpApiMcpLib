using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    /// <summary>
    /// Represents a reply to a chat message.
    /// This model shares many similarities with <see cref="ChatMessage"/> but is specific to replies.
    /// </summary>
    /// <param name="Id">The unique identifier of the reply message.</param>
    /// <param name="Assignee">The user to whom this reply (or its associated context, like a task comment) is assigned.</param>
    /// <param name="AssignedBy">The user who assigned this reply/comment.</param>
    /// <param name="Content">The text content of the reply message.</param>
    /// <param name="ContentFormat">The format of the content (e.g., "text", "markdown").</param>
    /// <param name="Date">The Unix timestamp (milliseconds) when the reply was created.</param>
    /// <param name="DateAssigned">The Unix timestamp (milliseconds) when this reply/comment was assigned.</param>
    /// <param name="DateResolved">The Unix timestamp (milliseconds) when this reply/comment was resolved.</param>
    /// <param name="DateUpdated">The Unix timestamp (milliseconds) when this reply was last updated.</param>
    /// <param name="GroupAssignee">A list of users if this reply/comment is assigned to a group.</param>
    /// <param name="ParentChannel">Basic information about the parent channel where this reply exists.</param>
    /// <param name="ParentMessage">Basic information about the parent message to which this is a reply.</param>
    /// <param name="Resolved">Indicates if this reply/comment has been resolved.</param>
    /// <param name="ResolvedBy">The user who resolved this reply/comment.</param>
    /// <param name="Type">The type of the message, typically "message" for a reply.</param>
    /// <param name="User">The user who created this reply message.</param>
    /// <param name="Links">API links related to this reply message.</param>
    /// <param name="Reactions">A list of reactions to this reply message.</param>
    /// <param name="Followers">A list of users following this reply/comment thread.</param>
    /// <param name="WorkspaceId">The identifier of the workspace this reply belongs to.</param>
    public record ReplyMessage
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("assignee")] ChatSimpleUser? Assignee,
        [property: JsonPropertyName("assigned_by")] ChatSimpleUser? AssignedBy,
        [property: JsonPropertyName("content")] string? Content,
        [property: JsonPropertyName("content_format")] string? ContentFormat,
        [property: JsonPropertyName("date")] System.DateTimeOffset Date,
        [property: JsonPropertyName("date_assigned")] System.DateTimeOffset? DateAssigned,
        [property: JsonPropertyName("date_resolved")] System.DateTimeOffset? DateResolved,
        [property: JsonPropertyName("date_updated")] System.DateTimeOffset? DateUpdated,
        [property: JsonPropertyName("group_assignee")] List<ChatSimpleUser>? GroupAssignee,
        [property: JsonPropertyName("parent_channel")] ChatSimpleChannelInfo? ParentChannel,
        [property: JsonPropertyName("parent_message")] ChatSimpleMessageInfo? ParentMessage,
        [property: JsonPropertyName("resolved")] bool? Resolved,
        [property: JsonPropertyName("resolved_by")] ChatSimpleUser? ResolvedBy,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("user")] ChatSimpleUser User,
        [property: JsonPropertyName("links")] CommentReplyMessageLinks2? Links,
        [property: JsonPropertyName("reactions")] List<ChatReaction>? Reactions,
        [property: JsonPropertyName("followers")] List<ChatSimpleUser>? Followers,
        [property: JsonPropertyName("workspace_id")] string WorkspaceId
    );
}
