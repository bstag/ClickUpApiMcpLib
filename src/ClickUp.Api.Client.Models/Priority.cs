namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents a Task Priority.
/// </summary>
public record Priority
{
    public string? Id { get; init; }
    public string PriorityText { get; init; } = string.Empty; // Renamed from 'priority' for clarity
    public string Color { get; init; } = string.Empty;
    public string OrderIndex { get; init; } = string.Empty; // Spec example shows this as string for task priorities
}
