using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Checklists;

/// <summary>
/// Represents the request to edit a checklist.
/// Based on the inline schema `EditChecklistrequest` from PUT /v2/checklist/{checklist_id}.
/// </summary>
public record EditChecklistRequest
{
    /// <summary>
    /// The new name for the checklist.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// The new position (order index) for the checklist.
    /// </summary>
    [JsonPropertyName("position")]
    public int? Position { get; init; }
}
