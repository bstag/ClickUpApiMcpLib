using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Comments;

/// <summary>
/// Represents the request model for updating an existing comment.
/// </summary>
/// <param name="CommentText">The new text content for the comment.</param>
/// <param name="Assignee">Optional: The new user ID to assign the comment to. Use null to unassign if the API supports it, or check API docs for specific unassignment value.</param>
/// <param name="Resolved">Optional: Set to true to mark the comment as resolved, false to mark as unresolved.</param>
/// <param name="NotifyAll">Optional: Set to true to notify all involved parties of the update.</param>
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
