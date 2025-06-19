using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Comments;

/// <summary>
/// Represents the request model for creating a task comment.
/// </summary>
public record class CreateTaskCommentRequest
(
    [property: JsonPropertyName("comment_text")]
    string CommentText,

    [property: JsonPropertyName("assignee")]
    int? Assignee,

    [property: JsonPropertyName("group_assignee")]
    string? GroupAssignee,

    [property: JsonPropertyName("notify_all")]
    bool NotifyAll
);
