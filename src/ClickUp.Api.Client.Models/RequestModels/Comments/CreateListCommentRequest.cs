using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Comments;

/// <summary>
/// Represents the request model for creating a list comment.
/// </summary>
public record class CreateListCommentRequest
(
    [property: JsonPropertyName("comment_text")]
    string CommentText,

    [property: JsonPropertyName("assignee")]
    int Assignee,

    [property: JsonPropertyName("notify_all")]
    bool NotifyAll
);
