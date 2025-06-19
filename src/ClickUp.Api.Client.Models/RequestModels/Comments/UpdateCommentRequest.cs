using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Comments;

/// <summary>
/// Represents the request model for updating a comment.
/// </summary>
public record class UpdateCommentRequest
(
    [property: JsonPropertyName("comment_text")]
    string CommentText,

    [property: JsonPropertyName("assignee")]
    int Assignee,

    [property: JsonPropertyName("group_assignee")]
    string? GroupAssignee,

    [property: JsonPropertyName("resolved")]
    bool Resolved
);
