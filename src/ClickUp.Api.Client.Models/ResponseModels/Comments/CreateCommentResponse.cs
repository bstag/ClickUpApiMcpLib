using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Comments; // For Comment entity

namespace ClickUp.Api.Client.Models.ResponseModels.Comments;

/// <summary>
/// Represents the response model for creating a general comment.
/// The actual structure might vary slightly based on the specific endpoint (e.g., task comment vs. list comment).
/// </summary>
public class CreateCommentResponse
{
    /// <summary>
    /// Gets or sets the ID of the newly created comment.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the full Comment object that was created.
    /// Some API endpoints might return the full object, potentially nested.
    /// For example, creating a task comment often returns a structure like {"id": "...", "hist_id": "...", "date": ..., "comment": { ...full comment object... } }.
    /// This model assumes a scenario where the full comment is available, possibly nested under a "comment" key.
    /// Adjust if specific endpoint responses differ.
    /// </summary>
    [JsonPropertyName("comment")]
    public Comment Comment { get; set; } = null!;

    // Additional properties like hist_id and date might be present at the root level for some comment creation responses.
    // Example:
    // [JsonPropertyName("hist_id")]
    // public string HistId { get; set; }
    //
    // [JsonPropertyName("date")]
    // public long Date { get; set; }
}
