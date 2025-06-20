using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Comments;

public class CreateCommentRequest
{
    [JsonPropertyName("comment_text")]
    public string CommentText { get; set; } = null!;

    [JsonPropertyName("assignee")]
    public int? Assignee { get; set; }

    [JsonPropertyName("notify_all")]
    public bool? NotifyAll { get; set; }
}
