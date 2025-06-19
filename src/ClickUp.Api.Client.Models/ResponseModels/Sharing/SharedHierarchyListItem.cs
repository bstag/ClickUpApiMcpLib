using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Sharing;

/// <summary>
/// Represents a list item within a shared hierarchy.
/// </summary>
public record class SharedHierarchyListItem
(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("orderindex")]
    int OrderIndex,

    [property: JsonPropertyName("content")]
    string? Content,

    [property: JsonPropertyName("status")]
    string? Status,

    [property: JsonPropertyName("priority")]
    string? Priority,

    [property: JsonPropertyName("assignee")]
    string? Assignee,

    [property: JsonPropertyName("task_count")]
    string TaskCount,

    [property: JsonPropertyName("due_date")]
    string? DueDate,

    [property: JsonPropertyName("start_date")]
    string? StartDate,

    [property: JsonPropertyName("archived")]
    bool Archived
);
