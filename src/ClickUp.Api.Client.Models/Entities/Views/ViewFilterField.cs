using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents a single filter field criterion in a View.
/// </summary>
public record ViewFilterField
{
    /// <summary>
    /// ID of the field to filter by (e.g., "status", "assignees", "priority", "tags", "due_date", or a custom field ID).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; }

    /// <summary>
    /// The specific field instance or sub-field if applicable (e.g. for custom fields).
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }


    /// <summary>
    /// The filter operator. Values depend on the field type.
    /// Examples: "IS NULL", "IS NOT NULL", "EQUALS", "NOT EQUALS", "GT", "LT", "ANY", "ALL".
    /// The actual values might be numeric IDs in some API versions/parts.
    /// The GetView response example shows string operators like "IS NULL".
    /// </summary>
    [JsonPropertyName("operator")]
    public string Operator { get; init; } // e.g. "IS NULL", "EQUALS", "ANY"

    /// <summary>
    /// Value(s) for the filter. Can be a single value or an array of values.
    /// The type depends on the field being filtered.
    /// Using JsonElement to allow for flexibility (string, number, array).
    /// </summary>
    [JsonPropertyName("values")]
    public JsonElement? Values { get; init; }

    /// <summary>
    /// Path for nested conditions, if applicable. (More common in advanced query builders)
    /// </summary>
    [JsonPropertyName("path")]
    public List<string>? Path { get; init; }

    /// <summary>
    /// Type of the field being filtered, helps in interpreting operator and values.
    /// e.g. "dropdownSelect", "number", "date"
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }


    // Example structure from GetView response for a filter field:
    // {
    //   "id": "assignees",
    //   "field": "assignees",
    //   "operator": "ANY",
    //   "values": [ 12345 ], // User ID
    //   "path": [ "assignees" ],
    //   "type": "user"
    // }
    // Another example:
    // {
    //   "id": "cf_abcdef", // Custom field ID
    //   "operator": "IS NULL",
    //   "type": "short_text"
    // }
}
