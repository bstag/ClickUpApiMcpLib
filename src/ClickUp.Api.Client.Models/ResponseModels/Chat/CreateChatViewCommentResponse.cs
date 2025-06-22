using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Chat;

/// <summary>
/// Represents the response model for creating a chat view comment.
/// </summary>
public record class CreateChatViewCommentResponse
(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("hist_id")]
    string HistId,

    [property: JsonPropertyName("date")]
    DateTimeOffset Date
);