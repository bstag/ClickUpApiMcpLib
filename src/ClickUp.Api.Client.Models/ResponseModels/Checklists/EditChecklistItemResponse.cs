using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Checklists;

namespace ClickUp.Api.Client.Models.ResponseModels.Checklists;

/// <summary>
/// Represents the response model for editing a checklist item.
/// </summary>
public record class EditChecklistItemResponse
(
    [property: JsonPropertyName("checklist")]
    Checklist Checklist
);
