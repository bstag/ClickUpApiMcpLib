using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the dividing (or sub-grouping/sectioning) settings for a View.
/// This is often called "Columns" in Board views (representing statuses) or similar sectioning.
/// </summary>
public record ViewDivide
{
    /// <summary>
    /// Field to divide by (e.g., "status", "priority", "tag", or a custom field ID).
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    /// <summary>
    /// Direction of division/sorting within divisions.
    /// (e.g., "asc", "desc", or 0 for asc, 1 for desc).
    /// </summary>
    [JsonPropertyName("dir")]
    public string? Dir { get; init; } // Or int

    [JsonPropertyName("collapsed")]
    public List<string>? Collapsed { get; init; } // List of IDs/names of sections that are collapsed

    // Other potential fields from spec for "divide" or board column settings:
    // "show_empty_statuses": bool
    // "show_task_locations": bool
    // "show_subtask_parent_names": bool
    // "me_comments": bool
    // "me_subtasks": bool
    // "me_checklists": bool
}
