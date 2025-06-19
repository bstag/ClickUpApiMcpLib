using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Checklists;

/// <summary>
/// Represents the request model for editing a checklist item.
/// </summary>
public record class EditChecklistItemRequest
(
    [property: JsonPropertyName("name")]
    string? Name,

    [property: JsonPropertyName("assignee")]
    int? Assignee,

    [property: JsonPropertyName("resolved")]
    bool? Resolved,

    [property: JsonPropertyName("parent")]
    string? Parent
);
