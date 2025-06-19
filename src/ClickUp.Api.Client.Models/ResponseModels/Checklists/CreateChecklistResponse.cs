using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Checklists; // For Checklist model

namespace ClickUp.Api.Client.Models.ResponseModels.Checklists;

/// <summary>
/// Represents the response after creating a checklist.
/// Based on the inline schema `CreateChecklistresponse` from POST /v2/task/{task_id}/checklist.
/// </summary>
public record CreateChecklistResponse
{
    /// <summary>
    /// The checklist that was created.
    /// </summary>
    [JsonPropertyName("checklist")]
    public Checklist Checklist { get; init; }
}
