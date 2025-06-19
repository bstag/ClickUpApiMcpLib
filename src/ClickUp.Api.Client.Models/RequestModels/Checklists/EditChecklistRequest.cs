using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Checklists;

/// <summary>
/// Represents the request model for editing a checklist.
/// </summary>
public record class EditChecklistRequest
(
    [property: JsonPropertyName("name")]
    string? Name,

    [property: JsonPropertyName("position")]
    int? Position
);
