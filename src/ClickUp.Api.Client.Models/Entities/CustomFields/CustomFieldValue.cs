using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.CustomFields
{
    /// <summary>
    /// Represents the value of a custom field on an entity like a task.
    /// </summary>
    /// <param name="Id">The unique identifier of the custom field definition.</param>
    /// <param name="Value">The actual value of the custom field. The type of this object depends on the custom field's type (e.g., string, number, array, object).</param>
    /// <param name="Type">The type of the custom field (e.g., "text", "number", "drop_down"). This might be redundant if <see cref="Id"/> is used to look up the full <see cref="CustomFields.CustomFieldDefinition"/>.</param>
    public record CustomFieldValue
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("value")] object Value, // Consider using JsonElement or a generic type if more type safety is needed during deserialization.
        [property: JsonPropertyName("type")] string? Type
    );
}
