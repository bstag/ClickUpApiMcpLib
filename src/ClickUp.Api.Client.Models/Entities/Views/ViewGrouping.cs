using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the grouping settings for a View.
/// </summary>
public record ViewGrouping
{
    /// <summary>
    /// Gets the field used for grouping items in the view (e.g., "status", "assignee", "priority", or a custom field ID).
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    /// <summary>
    /// Gets the direction of grouping (e.g., "asc" for ascending, "desc" for descending).
    /// Some API contexts might use integers (e.g., 0 for ascending, 1 for descending).
    /// </summary>
    [JsonPropertyName("dir")]
    public string? Dir { get; init; }

    /// <summary>
    /// Gets a list of identifiers (e.g., group names or IDs) for groups that are collapsed by default.
    /// </summary>
    [JsonPropertyName("collapsed")]
    public List<string>? Collapsed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the collapsed state of groups should be ignored when new items are added or the view is loaded.
    /// </summary>
    [JsonPropertyName("ignore_collapsed")]
    public bool? IgnoreCollapsed { get; init; }
}
