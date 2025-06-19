using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents detailed information about a guest user.
/// </summary>
public record GuestUserInfo
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

    [JsonPropertyName("role")]
    public int Role { get; init; }

    [JsonPropertyName("custom_role")]
    public CustomRole? CustomRole { get; init; }

    [JsonPropertyName("last_active")]
    public string? LastActive { get; init; }

    [JsonPropertyName("date_joined")]
    public string? DateJoined { get; init; }

    [JsonPropertyName("date_invited")]
    public string DateInvited { get; init; } = null!;
}
