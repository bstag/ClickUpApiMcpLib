using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Entities.Views;

public record ViewFilterField
{
    /// <summary>
    /// Represents a single filter field criterion in a View.
    /// </summary>
    /// <param name="id">ID of the field to filter by (e.g., "status", "assignees", "priority", "tags", "due_date", or a custom field ID).</param>
    /// <param name="operator">The filter operator. Examples: "IS NULL", "IS NOT NULL", "EQUALS", "NOT EQUALS", "GT", "LT", "ANY", "ALL".</param>
    public ViewFilterField(string id, string @operator)
    {
        Id = id;
        Operator = @operator;
    }

    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("operator")]
    public string Operator { get; }

    /// <summary>
    /// The specific field instance or sub-field if applicable (e.g. for custom fields).
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }

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
