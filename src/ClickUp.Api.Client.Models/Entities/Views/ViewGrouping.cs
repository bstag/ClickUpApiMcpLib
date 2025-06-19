using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the grouping settings for a View.
/// </summary>
public record ViewGrouping
{
    /// <summary>
    /// Field to group by (e.g., "status", "assignee", "priority", "tag", "due_date", or a custom field ID).
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    /// <summary>
    /// Direction of grouping (e.g., "asc", "desc").
    /// The API spec uses integer 0 for asc, 1 for desc for sorting, need to verify for grouping.
    /// Assuming string for now as it's common, but could be int.
    /// </summary>
    [JsonPropertyName("dir")]
    public string? Dir { get; init; } // Or potentially int (0 = asc, 1 = desc)

    [JsonPropertyName("collapsed")]
    public List<string>? Collapsed { get; init; } // List of IDs/names that are collapsed

    /// <summary>
    /// Whether to ignore collapsed state for new items or when view is loaded.
    /// </summary>
    [JsonPropertyName("ignore_collapsed")]
    public bool? IgnoreCollapsed { get; init; }

    // Other potential fields from spec:
    // "type": string (e.g. "status_group", "assignees_group") - need to verify if this exists
    // "orderindex": number - need to verify
}
