using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Checklists;

namespace ClickUp.Api.Client.Models.ResponseModels.Checklists;

/// <summary>
/// Represents the response for creating a checklist.
/// </summary>
public record CreateChecklistResponse
{
    /// <summary>
    /// The checklist information.
    /// </summary>
    [JsonPropertyName("checklist")]
    public Checklist Checklist { get; init; } = null!;
}
