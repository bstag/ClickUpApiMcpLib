using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Checklists;

/// <summary>
/// Represents the request model for creating a checklist.
/// </summary>
public record class CreateChecklistRequest
(
    [property: JsonPropertyName("name")]
    string Name
);
