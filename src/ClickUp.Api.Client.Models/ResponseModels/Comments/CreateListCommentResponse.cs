using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Comments;

/// <summary>
/// Represents the response model for creating a list comment.
/// </summary>
public record class CreateListCommentResponse
(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("hist_id")]
    string HistId,

    [property: JsonPropertyName("date")]
    DateTimeOffset Date
);