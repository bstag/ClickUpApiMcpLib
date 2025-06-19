using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Comments;

namespace ClickUp.Api.Client.Models.ResponseModels.Chat;

/// <summary>
/// Represents the response model for getting chat view comments.
/// </summary>
public record class GetChatViewCommentsResponse
(
    [property: JsonPropertyName("comments")]
    List<Comment> Comments
);
