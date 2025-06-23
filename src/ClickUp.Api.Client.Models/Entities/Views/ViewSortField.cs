using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents a single field used for sorting in a View.
/// </summary>
/// <param name="Field">The field to sort by (e.g., "status", "priority", "due_date", or a custom field ID).</param>
/// <param name="Dir">Direction of sorting. The API spec typically uses integer 0 for 'asc' and 1 for 'desc'.</param>
public record ViewSortField(
    [property: JsonPropertyName("field")]
    string Field,
    [property: JsonPropertyName("dir")]
    int Dir
);
