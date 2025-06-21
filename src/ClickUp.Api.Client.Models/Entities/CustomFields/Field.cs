using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.CustomFields
{
    /// <summary>
    /// Represents the definition of a Custom Field.
    /// Note: This is similar to <see cref="ClickUp.Api.Client.Models.CustomFields.CustomFieldDefinition"/>. Consider consolidation if they represent the same concept.
    /// </summary>
    /// <param name="Id">The unique identifier of the custom field.</param>
    /// <param name="Name">The name of the custom field.</param>
    /// <param name="Type">The type of the custom field (e.g., "text", "drop_down", "number").</param>
    /// <param name="TypeConfig">The type-specific configuration for this custom field.</param>
    /// <param name="DateCreated">The date when the custom field was created, as a string (e.g., Unix timestamp in milliseconds).</param>
    /// <param name="HideFromGuests">Indicates whether this custom field is hidden from guests.</param>
    /// <param name="Required">Indicates whether this custom field is required.</param>
    /// <param name="Creator">The user who created this custom field.</param>
    public record Field
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("type_config")] TypeConfig? TypeConfig,
        [property: JsonPropertyName("date_created")] string? DateCreated,
        [property: JsonPropertyName("hide_from_guests")] bool? HideFromGuests,
        [property: JsonPropertyName("required")] bool? Required,
        [property: JsonPropertyName("creator")] User? Creator
    );
}
