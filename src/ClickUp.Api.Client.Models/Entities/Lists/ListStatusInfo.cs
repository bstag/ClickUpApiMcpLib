namespace ClickUp.Api.Client.Models.Entities.Lists;

/// <summary>
/// Represents the status display properties of a List itself (color, label).
/// This is distinct from the task statuses within the list.
/// </summary>
public record ListStatusInfo
{
    /// <summary>
    /// Gets the textual representation of the list's status or label.
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Gets the color associated with the list's status display.
    /// </summary>
    /// <example>"#FF0000"</example>
    public string? Color { get; init; }

    /// <summary>
    /// Gets a value indicating whether the label for the list's status should be hidden.
    /// </summary>
    public bool? HideLabel { get; init; }
}
