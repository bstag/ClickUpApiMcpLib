using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat;

/// <summary>
/// Represents the request model for creating a chat view comment.
/// </summary>
public record class CreateChatViewCommentRequest
(
    [property: JsonPropertyName("comment_text")]
    string CommentText,

    [property: JsonPropertyName("notify_all")]
    bool NotifyAll
);
