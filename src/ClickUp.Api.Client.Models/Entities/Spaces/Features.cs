using System.Text.Json.Serialization;
using System.Collections.Generic; // Required for List if any feature uses it.

namespace ClickUp.Api.Client.Models.Entities.Spaces
{
    // Sub-record for Due Dates feature configuration
    public record DueDatesFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("start_date_enabled")] bool? StartDateEnabled, // ClickUp specific
        [property: JsonPropertyName("remap_due_dates_enabled")] bool? RemapDueDatesEnabled, // ClickUp specific
        [property: JsonPropertyName("due_dates_for_subtasks_roll_up_enabled")] bool? DueDatesForSubtasksRollUpEnabled // ClickUp specific
    );

    // Sub-record for Time Tracking feature configuration
    public record TimeTrackingFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("harvest_enabled")] bool? HarvestEnabled, // ClickUp specific for Harvest integration
        [property: JsonPropertyName("roll_up_enabled")] bool? RollUpEnabled // ClickUp specific for time rollups
    );

    // Sub-record for Tags feature configuration
    public record TagsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Sub-record for Time Estimates feature configuration
    public record TimeEstimatesFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("roll_up_enabled")] bool? RollUpEnabled, // ClickUp specific
        [property: JsonPropertyName("per_assignee_enabled")] bool? PerAssigneeEnabled // ClickUp specific
    );

    // Sub-record for Checklists feature configuration
    public record ChecklistsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Sub-record for Custom Fields feature configuration
    public record CustomFieldsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Sub-record for Remap Dependencies feature configuration
    public record RemapDependenciesFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Sub-record for Dependency Warning feature configuration
    public record DependencyWarningFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Sub-record for Portfolios feature configuration
    public record PortfoliosFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Sub-record for Sprints feature configuration
    public record SprintsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("legacy_sprints_enabled")] bool? LegacySprintsEnabled
    );

    // Sub-record for Points feature configuration
    public record PointsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Sub-record for Custom CuTask IDs feature configuration
    public record CustomTaskIdsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Sub-record for Multiple Assignees feature configuration
    public record MultipleAssigneesFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Sub-record for Emails feature configuration
    public record EmailsFeature
    (
        [property: JsonPropertyName("enabled")] bool Enabled
    );

    // Main Features record
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
        [property: JsonPropertyName("remap_dependencies")] RemapDependenciesFeature? RemapDependencies, // ClickUp specific: "remap_dependencies_enabled"
        [property: JsonPropertyName("dependency_warning")] DependencyWarningFeature? DependencyWarning, // ClickUp specific: "dependency_warning_enabled"
        [property: JsonPropertyName("multiple_assignees")] MultipleAssigneesFeature? MultipleAssignees,
        [property: JsonPropertyName("portfolios")] PortfoliosFeature? Portfolios, // ClickUp specific: "portfolio_enabled"
        [property: JsonPropertyName("emails")] EmailsFeature? Emails // ClickUp specific: "emails_enabled"
    );
}
