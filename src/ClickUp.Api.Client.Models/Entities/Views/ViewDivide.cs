using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the dividing (or sub-grouping/sectioning) settings for a View.
/// This is often used for Board views where columns represent statuses, or for other sectioning logic.
/// </summary>
public record ViewDivide
{
    /// <summary>
    /// Gets the field used for dividing the view content (e.g., "status", "priority", "tag", or a custom field ID).
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    /// <summary>
    /// Gets the direction of division or sorting within the divided sections.
    /// This could be "asc", "desc", or an integer representation (e.g., 0 for ascending, 1 for descending).
    /// </summary>
    [JsonPropertyName("dir")]
    public string? Dir { get; init; }

    /// <summary>
    /// Gets a list of identifiers (e.g., status names or IDs) for sections that are collapsed by default.
    /// </summary>
    [JsonPropertyName("collapsed")]
    public List<string>? Collapsed { get; init; }

    // Note: Additional properties like "show_empty_statuses", "show_task_locations", etc.,
    // might be part of this object or within ViewSettings depending on the specific view type and API response.
}
