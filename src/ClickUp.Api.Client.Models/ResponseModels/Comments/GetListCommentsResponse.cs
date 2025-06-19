using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Comments;

namespace ClickUp.Api.Client.Models.ResponseModels.Comments;

/// <summary>
/// Represents the response model for getting list comments.
/// </summary>
public record class GetListCommentsResponse
(
    [property: JsonPropertyName("comments")]
    List<Comment> Comments
);
