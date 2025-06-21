using System.Text.Json.Serialization;
using System.Collections.Generic; // Required for List if any feature uses it.

namespace ClickUp.Api.Client.Models.Entities.Spaces
{
    /// <summary>
    /// Represents the configuration for the Due Dates feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Due Dates feature is enabled.</param>
    /// <param name="StartDateEnabled">Indicates if start dates are enabled.</param>
    /// <param name="RemapDueDatesEnabled">Indicates if remapping due dates is enabled.</param>
    /// <param name="DueDatesForSubtasksRollUpEnabled">Indicates if due dates for subtasks roll up.</param>
    public record DueDatesFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("start_date_enabled")] bool? StartDateEnabled,
        [property: JsonPropertyName("remap_due_dates_enabled")] bool? RemapDueDatesEnabled,
        [property: JsonPropertyName("due_dates_for_subtasks_roll_up_enabled")] bool? DueDatesForSubtasksRollUpEnabled
    );

    /// <summary>
    /// Represents the configuration for the Time Tracking feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Time Tracking feature is enabled.</param>
    /// <param name="HarvestEnabled">Indicates if Harvest integration for time tracking is enabled.</param>
    /// <param name="RollUpEnabled">Indicates if time tracking roll-ups are enabled.</param>
    public record TimeTrackingFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("harvest_enabled")] bool? HarvestEnabled,
        [property: JsonPropertyName("roll_up_enabled")] bool? RollUpEnabled
    );

    /// <summary>
    /// Represents the configuration for the Tags feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Tags feature is enabled.</param>
    public record TagsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the configuration for the Time Estimates feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Time Estimates feature is enabled.</param>
    /// <param name="RollUpEnabled">Indicates if time estimate roll-ups are enabled.</param>
    /// <param name="PerAssigneeEnabled">Indicates if time estimates per assignee are enabled.</param>
    public record TimeEstimatesFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("roll_up_enabled")] bool? RollUpEnabled,
        [property: JsonPropertyName("per_assignee_enabled")] bool? PerAssigneeEnabled
    );

    /// <summary>
    /// Represents the configuration for the Checklists feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Checklists feature is enabled.</param>
    public record ChecklistsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the configuration for the Custom Fields feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Custom Fields feature is enabled.</param>
    public record CustomFieldsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the configuration for the Remap Dependencies feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Remap Dependencies feature is enabled.</param>
    public record RemapDependenciesFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the configuration for the Dependency Warning feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Dependency Warning feature is enabled.</param>
    public record DependencyWarningFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the configuration for the Portfolios feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Portfolios feature is enabled.</param>
    public record PortfoliosFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the configuration for the Sprints feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Sprints feature is enabled.</param>
    /// <param name="LegacySprintsEnabled">Indicates if legacy sprints are enabled.</param>
    public record SprintsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("legacy_sprints_enabled")] bool? LegacySprintsEnabled
    );

    /// <summary>
    /// Represents the configuration for the Points feature (e.g., story points) in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Points feature is enabled.</param>
    public record PointsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the configuration for the Custom CuTask IDs feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Custom CuTask IDs feature is enabled.</param>
    public record CustomTaskIdsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the configuration for the Multiple Assignees feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Multiple Assignees feature is enabled.</param>
    public record MultipleAssigneesFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the configuration for the Emails feature in a Space.
    /// </summary>
    /// <param name="Enabled">Indicates if the Emails feature is enabled.</param>
    public record EmailsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    /// <summary>
    /// Represents the set of features and their configurations for a Space.
    /// </summary>
    /// <param name="DueDates">Configuration for the Due Dates feature.</param>
    /// <param name="Sprints">Configuration for the Sprints feature.</param>
    /// <param name="Points">Configuration for the Points feature.</param>
    /// <param name="CustomTaskIds">Configuration for the Custom CuTask IDs feature.</param>
    /// <param name="TimeTracking">Configuration for the Time Tracking feature.</param>
    /// <param name="Tags">Configuration for the Tags feature.</param>
    /// <param name="TimeEstimates">Configuration for the Time Estimates feature.</param>
    /// <param name="Checklists">Configuration for the Checklists feature.</param>
    /// <param name="CustomFields">Configuration for the Custom Fields feature.</param>
    /// <param name="RemapDependencies">Configuration for the Remap Dependencies feature.</param>
    /// <param name="DependencyWarning">Configuration for the Dependency Warning feature.</param>
    /// <param name="MultipleAssignees">Configuration for the Multiple Assignees feature.</param>
    /// <param name="Portfolios">Configuration for the Portfolios feature.</param>
    /// <param name="Emails">Configuration for the Emails feature.</param>
    public record Features
    (
        [property: JsonPropertyName("due_dates")] DueDatesFeature? DueDates,
        [property: JsonPropertyName("sprints")] SprintsFeature? Sprints,
        [property: JsonPropertyName("points")] PointsFeature? Points,
        [property: JsonPropertyName("custom_task_ids")] CustomTaskIdsFeature? CustomTaskIds,
        [property: JsonPropertyName("time_tracking")] TimeTrackingFeature? TimeTracking,
        [property: JsonPropertyName("tags")] TagsFeature? Tags,
        [property: JsonPropertyName("time_estimates")] TimeEstimatesFeature? TimeEstimates,
        [property: JsonPropertyName("checklists")] ChecklistsFeature? Checklists,
        [property: JsonPropertyName("custom_fields")] CustomFieldsFeature? CustomFields,
        [property: JsonPropertyName("remap_dependencies")] RemapDependenciesFeature? RemapDependencies,
        [property: JsonPropertyName("dependency_warning")] DependencyWarningFeature? DependencyWarning,
        [property: JsonPropertyName("multiple_assignees")] MultipleAssigneesFeature? MultipleAssignees,
        [property: JsonPropertyName("portfolios")] PortfoliosFeature? Portfolios,
        [property: JsonPropertyName("emails")] EmailsFeature? Emails
    );
}
