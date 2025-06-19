using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Comments;

namespace ClickUp.Api.Client.Models.ResponseModels.Comments;

/// <summary>
/// Represents the response model for getting task comments.
/// </summary>
public record class GetTaskCommentsResponse
(
    [property: JsonPropertyName("comments")]
    List<Comment> Comments
);
