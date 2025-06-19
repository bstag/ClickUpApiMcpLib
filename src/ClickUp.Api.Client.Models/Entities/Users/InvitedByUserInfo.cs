using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents information about the user who invited a guest.
/// </summary>
public record InvitedByUserInfo
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("username")]
    public string? Username { get; init; }

    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;

    [JsonPropertyName("color")]
    public string? Color { get; init; }

    [JsonPropertyName("profile_picture")]
    public string? ProfilePicture { get; init; }

    [JsonPropertyName("initials")]
    public string Initials { get; init; } = null!;
}
