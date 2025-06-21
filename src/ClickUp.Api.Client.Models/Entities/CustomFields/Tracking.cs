using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.CustomFields
{
    /// <summary>
    /// Represents the tracking configuration for a Custom Field, typically for 'Progress (Automatic)' or similar types.
    /// Note: This is similar to <see cref="ClickUp.Api.Client.Models.CustomFields.CustomFieldTracking"/>. Consider consolidation.
    /// </summary>
    /// <param name="Subtasks">Indicates whether subtasks are included in progress calculation.</param>
    /// <param name="Checklists">Indicates whether checklist items are included in progress calculation.</param>
    /// <param name="AssignedComments">Indicates whether assigned comments are included in progress calculation.</param>
    /// <param name="AllComments">Indicates whether all comments are included (less common for progress, might apply to other tracking).</param>
    /// <param name="TimeEstimates">Indicates whether time estimates are considered.</param>
    /// <param name="TimeLogged">Indicates whether logged time is considered.</param>
    public record Tracking
    (
        [property: JsonPropertyName("subtasks")] bool? Subtasks,
        [property: JsonPropertyName("checklists")] bool? Checklists,
        [property: JsonPropertyName("assigned_comments")] bool? AssignedComments,
        [property: JsonPropertyName("all_comments")] bool? AllComments,
        [property: JsonPropertyName("time_estimates")] bool? TimeEstimates,
        [property: JsonPropertyName("time_logged")] bool? TimeLogged
    );
}
