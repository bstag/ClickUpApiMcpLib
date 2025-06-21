using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the filter settings for a View.
/// </summary>
public record ViewFilters
{
    /// <summary>
    /// Gets the logical operator (e.g., "AND", "OR") used to join the filter criteria in the <see cref="Fields"/> list.
    /// </summary>
    [JsonPropertyName("op")]
    public string? Operator { get; init; }

    /// <summary>
    /// Gets the list of individual filter criteria applied to the view.
    /// </summary>
    [JsonPropertyName("fields")]
    public List<ViewFilterField>? Fields { get; init; }

    /// <summary>
    /// Gets the search text applied as a filter to the view.
    /// </summary>
    [JsonPropertyName("search")]
    public string? Search { get; init; }

    /// <summary>
    /// Gets a value indicating whether closed tasks are shown in the view.
    /// </summary>
    [JsonPropertyName("show_closed")]
    public bool? ShowClosed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the search should include custom fields.
    /// </summary>
    [JsonPropertyName("search_custom_fields")]
    public bool? SearchCustomFields { get; init; }

    /// <summary>
    /// Gets a value indicating whether the view shows items from all levels (e.g., including subtasks from other locations if applicable).
    /// </summary>
    [JsonPropertyName("show_everything_level")]
    public bool? ShowEverythingLevel { get; init; }
}
