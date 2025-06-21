using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Comments;

/// <summary>
/// Represents the request model for creating a comment.
/// This is a general model; more specific ones like <see cref="CreateTaskCommentRequest"/> or <see cref="CreateListCommentRequest"/> might be used depending on the context.
/// </summary>
public class CreateCommentRequest
{
    /// <summary>
    /// Gets or sets the text content of the comment.
    /// </summary>
    [JsonPropertyName("comment_text")]
    public string CommentText { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the user to assign this comment to.
    /// </summary>
    [JsonPropertyName("assignee")]
    public int? Assignee { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to notify all involved parties.
    /// </summary>
    [JsonPropertyName("notify_all")]
    public bool? NotifyAll { get; set; }
}
