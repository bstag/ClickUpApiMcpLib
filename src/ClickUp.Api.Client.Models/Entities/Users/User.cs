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
    DateTimeOffset? LastActive = null,

    [property: JsonPropertyName("date_joined")]
    DateTimeOffset? DateJoined = null,

    [property: JsonPropertyName("date_invited")]
    DateTimeOffset? DateInvited = null,

    [property: JsonPropertyName("profileInfo")]
    ProfileInfo? ProfileInfo = null
);