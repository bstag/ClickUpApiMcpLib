namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents the priority display properties of a List itself.
/// </summary>
public record ListPriorityInfo
{
    public string? Priority { get; init; } // Corresponds to "priority" field in JSON
    public string? Color { get; init; }
}
