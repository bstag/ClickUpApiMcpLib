using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents various settings for a View. Content can vary based on view type.
/// </summary>
public record ViewSettings
{
    /// <summary>
    /// Whether to automatically save the view for everyone.
    /// </summary>
    [JsonPropertyName("auto_save")]
    public bool? AutoSave { get; init; }

    /// <summary>
    /// Default view template ID if this view is a template.
    /// </summary>
    [JsonPropertyName("default_view_template")]
    public string? DefaultViewTemplate { get; init; }

    /// <summary>
    /// Who the view is shared with or visibility settings (e.g., "public", "private", "shared").
    /// </summary>
    [JsonPropertyName("sharing")]
    public string? Sharing { get; init; } // Simplified, could be a complex object

    // Common settings that might appear (many are view-type specific)
    // From GetView response example for a List view:
    [JsonPropertyName("show_task_locations")]
    public bool? ShowTaskLocations { get; init; }

    [JsonPropertyName("show_subtask_parent_names")]
    public bool? ShowSubtaskParentNames { get; init; }

    [JsonPropertyName("show_closed_subtasks")]
    public bool? ShowClosedSubtasks { get; init; } // Different from filters.show_closed for tasks

    [JsonPropertyName("show_assignees")]
    public bool? ShowAssignees { get; init; }

    [JsonPropertyName("show_due_date")]
    public bool? ShowDueDate { get; init; }

    [JsonPropertyName("show_time_estimates")]
    public bool? ShowTimeEstimates { get; init; }

    [JsonPropertyName("show_time_in_status")]
    public bool? ShowTimeInStatus { get; init; }

    [JsonPropertyName("show_priority")]
    public bool? ShowPriority { get; init; }

    [JsonPropertyName("show_tags")]
    public bool? ShowTags { get; init; }

    [JsonPropertyName("show_empty_statuses")] // For Board views mainly
    public bool? ShowEmptyStatuses { get; init; }

    [JsonPropertyName("me_comments")]
    public bool? MeComments { get; init; }

    [JsonPropertyName("me_subtasks")]
    public bool? MeSubtasks { get; init; }

    [JsonPropertyName("me_checklists")]
    public bool? MeChecklists { get; init; }

    [JsonPropertyName("show_images_on_cards")] // For Board views
    public bool? ShowImagesOnCards { get; init; }

    [JsonPropertyName("collapse_empty_columns")] // For Board views
    public bool? CollapseEmptyColumns { get; init; }

    [JsonPropertyName("group_by_none")]
    public bool? GroupByNone { get; init; }

    [JsonPropertyName("subtasks_separate_group")]
    public bool? SubtasksSeparateGroup { get; init; }

    [JsonPropertyName("subtasks_parent_rollup")]
    public bool? SubtasksParentRollup { get; init; }

    [JsonPropertyName("show_gantt_sidebar_width")] // Gantt specific
    public int? ShowGanttSidebarWidth { get; init; }

    [JsonPropertyName("gantt_show_days")] // Gantt specific (e.g., "days", "weeks")
    public string? GanttShowDays { get; init; }

    // Calendar specific settings
    [JsonPropertyName("calendar_view_type")] // e.g., "month", "week", "day"
    public string? CalendarViewType { get; init; }

    [JsonPropertyName("calendar_show_weekends")]
    public bool? CalendarShowWeekends { get; init; }

    // Form specific settings
    [JsonPropertyName("form_show_recaptcha")]
    public bool? FormShowRecaptcha { get; init; }


    // This is a flexible object, often a dictionary in less strictly typed languages.
    // For C#, specific known properties are best. If it's truly dynamic,
    // Dictionary<string, JsonElement> might be an option, but less type-safe.
    // The GetView response shows a large number of boolean flags directly in the settings object.
    // I've added many common ones based on that example and general ClickUp knowledge.
}
