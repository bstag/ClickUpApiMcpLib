using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents a ClickUp User.
/// </summary>
public record class User
(
    [property: JsonPropertyName("id")]
    int Id,

    [property: JsonPropertyName("username")]
    string? Username,

    [property: JsonPropertyName("email")]
    string Email,

    [property: JsonPropertyName("color")]
    string? Color,

    [property: JsonPropertyName("profilePicture")]
    string? ProfilePicture,

    [property: JsonPropertyName("initials")]
    string? Initials,

    // Fields from User9 schema in OpenAPI spec
    [property: JsonPropertyName("role")]
    int? Role = null,

    [property: JsonPropertyName("custom_role")]
    CustomRole? CustomRole = null,

    [property: JsonPropertyName("last_active")]
    string? LastActive = null, // String representation of Unix ms timestamp

    [property: JsonPropertyName("date_joined")]
    string? DateJoined = null, // String representation of Unix ms timestamp

    [property: JsonPropertyName("date_invited")]
    string? DateInvited = null, // String representation of Unix ms timestamp

    [property: JsonPropertyName("profileInfo")]
    ProfileInfo? ProfileInfo = null
);
