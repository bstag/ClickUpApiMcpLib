namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents a Task Status.
/// </summary>
public record Status
{
    public string StatusText { get; init; } = string.Empty; // Renamed from 'status' to avoid conflict with record's generated methods if any, and for clarity
    public string Color { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public string Type { get; init; } = string.Empty; // "open", "custom", "closed"
}
