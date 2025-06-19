using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents a single field used for sorting in a View.
/// </summary>
public record ViewSortField
{
    /// <summary>
    /// The field to sort by (e.g., "status", "priority", "due_date", or a custom field ID).
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; init; }

    /// <summary>
    /// Direction of sorting. The API spec typically uses integer 0 for 'asc' and 1 for 'desc'.
    /// </summary>
    [JsonPropertyName("dir")]
    public int Dir { get; init; } // 0 = asc, 1 = desc
}
