using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents a single column's settings in a View (typically for List or Table views).
/// </summary>
public record ViewColumnField
{
    /// <summary>
    /// ID of the field this column represents (e.g., "name", "status", "assignees", "due_date", or a custom field ID).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; }

    /// <summary>
    /// The specific field instance or sub-field if applicable. Often same as 'id' for standard fields.
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    /// <summary>
    /// Display name or title of the column.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Width of the column. Units or type (pixels, percentage) depend on API.
    /// </summary>
    [JsonPropertyName("width")]
    public int? Width { get; init; } // Or string if it includes units like "px"

    /// <summary>
    /// Order index of the column.
    /// </summary>
    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; init; } // Or float/double

    /// <summary>
    /// Whether the column is hidden.
    /// </summary>
    [JsonPropertyName("hidden")]
    public bool? Hidden { get; init; }

    /// <summary>
    /// Type of the field this column represents (e.g., "string", "user", "date", "cf").
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    // Example from GetView response:
    // {
    //   "id": "cf_abcdef", // custom field id
    //   "field": "cf_abcdef",
    //   "title": "My Custom Field",
    //   "type": "drop_down", // This is a custom field type
    //   "width": 150,
    //   "orderindex": 3,
    //   "hidden": false,
    //   "custom_field": { ... custom field definition ... } // This might be too detailed for ViewColumnField
    // }
    // Sticking to common column properties for now.
    // The "custom_field" full definition is likely not part of the column settings itself,
    // but rather the 'id' refers to an existing custom field.
}
