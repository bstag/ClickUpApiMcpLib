using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Comments;

public record UpdateCommentRequest
(
    [property: JsonPropertyName("comment_text")]
    string CommentText,

    [property: JsonPropertyName("assignee")]
    int? Assignee = null,

    [property: JsonPropertyName("resolved")]
    bool? Resolved = null,

    [property: JsonPropertyName("notify_all")]
    bool? NotifyAll = null
);
