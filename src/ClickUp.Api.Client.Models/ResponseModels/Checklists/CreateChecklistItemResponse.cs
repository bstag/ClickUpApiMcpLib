using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Checklists;

namespace ClickUp.Api.Client.Models.ResponseModels.Checklists;

/// <summary>
/// Represents the response for creating a checklist item.
/// </summary>
public record CreateChecklistItemResponse
{
    /// <summary>
    /// The parent checklist information, updated after item creation.
    /// </summary>
    [JsonPropertyName("checklist")]
    public Checklist Checklist { get; init; } = null!;
}
