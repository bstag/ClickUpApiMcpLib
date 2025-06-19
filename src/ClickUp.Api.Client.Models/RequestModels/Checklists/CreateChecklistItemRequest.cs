using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Checklists;

/// <summary>
/// Represents the request model for creating a checklist item.
/// </summary>
public record class CreateChecklistItemRequest
(
    [property: JsonPropertyName("name")]
    string? Name,

    [property: JsonPropertyName("assignee")]
    int? Assignee
);
