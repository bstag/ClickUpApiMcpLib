using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    public record CommentPatchChatMessageRequest
    (
        [property: JsonPropertyName("assignee")] string? Assignee, // User ID
        [property: JsonPropertyName("group_assignee")] string? GroupAssignee, // Not typically a list for patch, usually set/unset one group or clear
        [property: JsonPropertyName("content")] string? Content,
        [property: JsonPropertyName("content_format")] string? ContentFormat, // e.g., "text/plain", "text/md"
        [property: JsonPropertyName("post_data")] CommentChatPostDataPatch? PostData,
        [property: JsonPropertyName("resolved")] bool? Resolved,
        [property: JsonPropertyName("parent_message_id")] string? ParentMessageId // To make it a reply or change replied-to message
    );
}
