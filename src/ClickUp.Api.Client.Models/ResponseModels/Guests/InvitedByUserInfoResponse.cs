using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents information about the user who invited a guest.
/// </summary>
public record class InvitedByUserInfoResponse
(
    [property: JsonPropertyName("id")]
    int Id,

    [property: JsonPropertyName("color")]
    string? Color,

    [property: JsonPropertyName("username")]
    string Username,

    [property: JsonPropertyName("email")]
    string Email,

    [property: JsonPropertyName("initials")]
    string? Initials,

    [property: JsonPropertyName("profilePicture")]
    string? ProfilePicture
);
