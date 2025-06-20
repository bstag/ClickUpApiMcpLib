using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Comments; // For Comment entity

namespace ClickUp.Api.Client.Models.ResponseModels.Comments;

public class CreateCommentResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    // The API often returns the full comment object nested under a 'comment' key or similar.
    // However, some create endpoints might return the comment directly or just its ID.
    // For now, let's assume it could be the full comment object for maximum detail.
    // The API spec for POST /task/{task_id}/comment shows "comment": { ...Comment... }
    // If other create comment endpoints differ, this might need adjustment or specific response DTOs.
    [JsonPropertyName("comment")]
    public Comment Comment { get; set; } = null!;
}
