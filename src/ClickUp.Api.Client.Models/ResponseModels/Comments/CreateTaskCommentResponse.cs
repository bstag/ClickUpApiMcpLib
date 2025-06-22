using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Comments;

/// <summary>
/// Represents the response model for creating a task comment.
/// </summary>
public record class CreateTaskCommentResponse
(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("hist_id")]
    string HistId,

    [property: JsonPropertyName("date")]
    DateTimeOffset Date
);