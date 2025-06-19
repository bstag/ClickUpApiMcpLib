using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Sharing;

/// <summary>
/// Represents a folder item within a shared hierarchy.
/// </summary>
public record class SharedHierarchyFolderItem
(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("orderindex")]
    int OrderIndex,

    [property: JsonPropertyName("content")]
    string? Content,

    [property: JsonPropertyName("task_count")]
    string TaskCount,

    [property: JsonPropertyName("due_date")]
    string? DueDate,

    [property: JsonPropertyName("archived")]
    bool Archived
);
