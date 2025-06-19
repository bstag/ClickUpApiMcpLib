using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    public record CommentCreateChatMessageRequest
    (
        [property: JsonPropertyName("assignee")] string? Assignee, // User ID for assignee
        [property: JsonPropertyName("group_assignee")] List<string>? GroupAssignee, // List of User IDs for group assignment
        [property: JsonPropertyName("triaged_action")] string? TriagedAction,
        [property: JsonPropertyName("triaged_object_id")] string? TriagedObjectId,
        [property: JsonPropertyName("triaged_object_type")] string? TriagedObjectType,
        [property: JsonPropertyName("type")] string Type, // "message" or "post"
        [property: JsonPropertyName("content")] string Content, // Message content
        [property: JsonPropertyName("reactions")] List<string>? Reactions, // List of reaction emojis to add (e.g., [":smile:", ":thumbsup:"])
        [property: JsonPropertyName("followers")] List<string>? Followers, // List of User IDs to add as followers
        [property: JsonPropertyName("content_format")] string? ContentFormat, // e.g., "text/plain", "text/md"
        [property: JsonPropertyName("post_data")] CommentChatPostDataCreate? PostData, // Required if type is "post"
        [property: JsonPropertyName("parent_message_id")] string? ParentMessageId // If this is a reply
    );
}
