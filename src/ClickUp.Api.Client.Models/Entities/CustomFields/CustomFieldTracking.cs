namespace ClickUp.Api.Client.Models.Entities.CustomFields;

/// <summary>
/// Represents the tracking configuration for certain Custom Field types, such as 'Progress (Automatic)' or 'Tasks' (relationship).
/// This determines what items contribute to the progress or are considered part of the relationship.
/// </summary>
public record CustomFieldTracking
{
    /// <summary>
    /// Gets a value indicating whether subtasks are included in the tracking.
    /// Relevant for 'Progress (Automatic)' type custom fields.
    /// </summary>
    public bool? Subtasks { get; init; }

    /// <summary>
    /// Gets a value indicating whether items in checklists are included in the tracking.
    /// Relevant for 'Progress (Automatic)' type custom fields.
    /// </summary>
    public bool? Checklists { get; init; }

    /// <summary>
    /// Gets a value indicating whether assigned comments are included in the tracking.
    /// Relevant for 'Progress (Automatic)' type custom fields.
    /// </summary>
    public bool? AssignedComments { get; init; }

    // Note: The 'Tasks' type custom field (a form of relationship) might also use this
    // or similar properties to define the scope of linked tasks.
    // This model primarily covers options seen with 'Progress (Automatic)'.
}
