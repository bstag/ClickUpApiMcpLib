namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents the status display properties of a List itself (color, label).
/// </summary>
public record ListStatusInfo
{
    public string? Status { get; init; }
    public string? Color { get; init; }
    public bool? HideLabel { get; init; }
}
