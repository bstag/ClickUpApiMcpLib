using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.CustomFields
{
    /// <summary>
    /// Represents an option within a Custom Field (e.g., for Dropdown, Labels, or Users type).
    /// Note: This is similar to <see cref="ClickUp.Api.Client.Models.CustomFields.CustomFieldOption"/>. Consider consolidation.
    /// </summary>
    /// <param name="Id">The unique identifier of the custom field option.</param>
    /// <param name="Label">The label of the option, typically used for 'Dropdown' or 'Labels' types.</param>
    /// <param name="Name">The name associated with the option. For 'Users' type, this is often the user's full name.</param>
    /// <param name="Color">The color associated with the option.</param>
    /// <param name="Value">An optional value associated with the option. Its meaning varies by custom field type.</param>
    /// <param name="Type">The type of the option, e.g., "user" for options in a 'Users' custom field.</param>
    /// <param name="OrderIndex">The order index of the option.</param>
    /// <param name="Email">The email address, if this option represents a user.</param>
    /// <param name="ProfilePicture">The URL of the profile picture, if this option represents a user.</param>
    /// <param name="Username">The username, if this option represents a user.</param>
    public record Option
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("label")] string? Label,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("value")] string? Value,
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("orderindex")] int? OrderIndex,
        [property: JsonPropertyName("email")] string? Email,
        [property: JsonPropertyName("profilePicture")] string? ProfilePicture,
        [property: JsonPropertyName("username")] string? Username
    );
}
