using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the request to create a new chat message or post.
    /// </summary>
    /// <param name="Assignee">Optional: User ID to assign this message/comment to.</param>
    /// <param name="GroupAssignee">Optional: List of User IDs for group assignment.</param>
    /// <param name="TriagedAction">Optional: Triage action related to this message.</param>
    /// <param name="TriagedObjectId">Optional: ID of the object being triaged.</param>
    /// <param name="TriagedObjectType">Optional: Type of the object being triaged.</param>
    /// <param name="Type">The type of the message, either "message" or "post" (required).</param>
    /// <param name="Content">The content of the message (required).</param>
    /// <param name="Reactions">Optional: List of reaction emojis to add (e.g., [":smile:", ":thumbsup:"]).</param>
    /// <param name="Followers">Optional: List of User IDs to add as followers.</param>
    /// <param name="ContentFormat">Optional: The format of the content (e.g., "text/plain", "text/md").</param>
    /// <param name="PostData">Required if 'Type' is "post". Contains data for the post like title and subtype.</param>
    /// <param name="ParentMessageId">Optional: If this message is a reply, the ID of the parent message.</param>
    public record CommentCreateChatMessageRequest
    (
        [property: JsonPropertyName("assignee")] string? Assignee,
        [property: JsonPropertyName("group_assignee")] List<string>? GroupAssignee,
        [property: JsonPropertyName("triaged_action")] string? TriagedAction,
        [property: JsonPropertyName("triaged_object_id")] string? TriagedObjectId,
        [property: JsonPropertyName("triaged_object_type")] string? TriagedObjectType,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("content")] string Content,
        [property: JsonPropertyName("reactions")] List<string>? Reactions,
        [property: JsonPropertyName("followers")] List<string>? Followers,
        [property: JsonPropertyName("content_format")] string? ContentFormat,
        [property: JsonPropertyName("post_data")] CommentChatPostDataCreate? PostData,
        [property: JsonPropertyName("parent_message_id")] string? ParentMessageId
    );
}
