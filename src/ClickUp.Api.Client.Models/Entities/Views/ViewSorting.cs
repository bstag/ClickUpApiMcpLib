using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the sorting settings for a View.
/// </summary>
public record ViewSorting
{
    /// <summary>
    /// List of fields to sort by, in order of precedence.
    /// </summary>
    [JsonPropertyName("fields")]
    public List<ViewSortField>? Fields { get; init; }

    // Sometimes there's a general "sort_by" or "order_by" if only single field sorting is simplified
    // but "fields" array is more common for multi-level sorting.
    // The GetView response example shows "sorting": { "fields": [ { "field": "priority", "dir": 0 } ] }
    // which confirms this structure.
}
