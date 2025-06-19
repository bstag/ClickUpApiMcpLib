using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the filter settings for a View.
/// </summary>
public record ViewFilters
{
    /// <summary>
    /// The logical operator to join filter fields (e.g., "AND", "OR").
    /// </summary>
    [JsonPropertyName("op")]
    public string? Operator { get; init; } // "AND" or "OR"

    /// <summary>
    /// List of filter criteria.
    /// </summary>
    [JsonPropertyName("fields")]
    public List<ViewFilterField>? Fields { get; init; }

    /// <summary>
    /// Text to search for within the view.
    /// </summary>
    [JsonPropertyName("search")]
    public string? Search { get; init; }

    /// <summary>
    /// Whether to show closed tasks.
    /// </summary>
    [JsonPropertyName("show_closed")]
    public bool? ShowClosed { get; init; }

    // Example from GetView response:
    // "filters": {
    //   "op": "AND",
    //   "fields": [ ... view filter field objects ... ],
    //   "search_custom_fields": true, // This might be part of 'settings' or here
    //   "search": null,
    //   "show_closed": false,
    //   "show_everything_level": false // This might be part of 'settings' or here
    // }
    // Adding fields that appear common in filter objects, to be verified with exact schema.

    [JsonPropertyName("search_custom_fields")]
    public bool? SearchCustomFields { get; init; }

    [JsonPropertyName("show_everything_level")]
    public bool? ShowEverythingLevel { get; init; } // Or "show_all_tasks_in_subfolders" etc.
}
