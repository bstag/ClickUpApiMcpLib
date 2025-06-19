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

    [property: JsonPropertyName("profileInfo")]
    ProfileInfo? ProfileInfo // Added new property
);
