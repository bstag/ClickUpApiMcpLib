using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.CustomFields
{
    public record Option
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("label")] string? Label, // Used in Dropdown, Labels
        [property: JsonPropertyName("name")] string? Name,   // Used in Users custom field for full name
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("value")] string? Value, // Sometimes used, e.g. for text content in some option types
        [property: JsonPropertyName("type")] string? Type,   // e.g. "user" for user type in users custom field options
        [property: JsonPropertyName("orderindex")] int? OrderIndex,
        [property: JsonPropertyName("email")] string? Email, // For user type options
        [property: JsonPropertyName("profilePicture")] string? ProfilePicture, // For user type options
        [property: JsonPropertyName("username")] string? Username // For user type options
    );
}
