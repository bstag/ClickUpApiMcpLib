using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For User

namespace ClickUp.Api.Client.Models.Entities.CustomFields
{
    public record Field // Represents a Custom Field Definition
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("type")] string Type, // e.g., "text", "drop_down", "number", "currency", "date", "users", "labels", "progress", etc.
        [property: JsonPropertyName("type_config")] TypeConfig? TypeConfig,
        [property: JsonPropertyName("date_created")] string? DateCreated, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("hide_from_guests")] bool? HideFromGuests,
        [property: JsonPropertyName("required")] bool? Required,
        [property: JsonPropertyName("creator")] ComUser? Creator // User who created the custom field
    );
}
