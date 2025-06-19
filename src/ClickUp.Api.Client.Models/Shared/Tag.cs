namespace ClickUp.Api.Client.Models.Shared;

/// <summary>
/// Represents a Tag in ClickUp.
/// </summary>
public record Tag
{
    public string Name { get; init; } = string.Empty;
    public string TagFg { get; init; } = string.Empty; // Foreground color
    public string TagBg { get; init; } = string.Empty; // Background color
    public ulong? Creator { get; init; } // User ID of the creator, optional
}
