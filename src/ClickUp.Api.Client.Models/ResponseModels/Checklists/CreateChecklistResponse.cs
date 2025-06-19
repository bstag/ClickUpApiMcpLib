using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Checklists;

namespace ClickUp.Api.Client.Models.ResponseModels.Checklists;

/// <summary>
/// Represents the response model for creating a checklist.
/// </summary>
public record class CreateChecklistResponse
(
    [property: JsonPropertyName("checklist")]
    Checklist Checklist
);
