namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents a ClickUp User.
/// </summary>
public record User
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Color { get; init; }
    public string? ProfilePicture { get; init; }
    public string? Initials { get; init; }
    public int? WeekStartDay { get; init; } // 0 for Sunday, 1 for Monday
    public bool? GlobalFontSupport { get; init; }
    public string? Timezone { get; init; }
}
