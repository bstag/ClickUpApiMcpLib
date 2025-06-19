namespace ClickUp.Api.Client.Models.CustomFields;

/// <summary>
/// Represents the tracking configuration for some Custom Field types (e.g., progress, tasks).
/// </summary>
public record CustomFieldTracking
{
    public bool? Subtasks { get; init; }
    public bool? Checklists { get; init; } // Seen for progress (automatic)
    public bool? AssignedComments { get; init; } // Seen for progress (automatic)
    // The 'tasks' type custom field might have different tracking properties
    // For now, these are the common ones seen in progress tracking.
}
