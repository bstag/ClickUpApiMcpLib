using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the request to update (patch) an existing chat message or post.
    /// All properties are optional; only provided properties will be updated.
    /// </summary>
    /// <param name="Assignee">Optional: New User ID to assign this message/comment to. Null to unassign.</param>
    /// <param name="GroupAssignee">Optional: New User Group ID for group assignment. Some APIs might use null or empty string to clear.</param>
    /// <param name="Content">Optional: New content for the message.</param>
    /// <param name="ContentFormat">Optional: The format of the new content (e.g., "text/plain", "text/md").</param>
    /// <param name="PostData">Optional: If updating a "post" type message, new data for the post.</param>
    /// <param name="Resolved">Optional: Set to true to resolve the message/comment, false to unresolve.</param>
    /// <param name="ParentMessageId">Optional: New parent message ID if changing what this message is a reply to, or making it a reply.</param>
    public record CommentPatchChatMessageRequest
    (
        [property: JsonPropertyName("assignee")] string? Assignee,
        [property: JsonPropertyName("group_assignee")] string? GroupAssignee,
        [property: JsonPropertyName("content")] string? Content,
        [property: JsonPropertyName("content_format")] string? ContentFormat,
        [property: JsonPropertyName("post_data")] UpdateCommentChatPostDataRequest? PostData,
        [property: JsonPropertyName("resolved")] bool? Resolved,
        [property: JsonPropertyName("parent_message_id")] string? ParentMessageId
    );
}
