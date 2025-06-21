using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the sorting settings for a View.
/// </summary>
public record ViewSorting
{
    /// <summary>
    /// Gets the list of fields by which the view is sorted, in order of precedence.
    /// </summary>
    [JsonPropertyName("fields")]
    public List<ViewSortField>? Fields { get; init; }
}
