using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents various settings for a View. Content can vary based on view type.
/// </summary>
public record ViewSettings
{
    /// <summary>
    /// Gets a value indicating whether changes to the view are automatically saved for all users.
    /// </summary>
    [JsonPropertyName("auto_save")]
    public bool? AutoSave { get; init; }

    /// <summary>
    /// Gets the identifier of the default view template, if this view is based on a template.
    /// </summary>
    [JsonPropertyName("default_view_template")]
    public string? DefaultViewTemplate { get; init; }

    /// <summary>
    /// Gets the sharing status or visibility of the view (e.g., "public", "private", "shared").
    /// Note: This might be a simplified representation; actual sharing details can be more complex.
    /// </summary>
    [JsonPropertyName("sharing")]
    public string? Sharing { get; init; }

    /// <summary>
    /// Gets a value indicating whether task locations (List, Folder, Space) are shown in the view.
    /// </summary>
    [JsonPropertyName("show_task_locations")]
    public bool? ShowTaskLocations { get; init; }

    /// <summary>
    /// Gets a value indicating whether the names of parent tasks are shown for subtasks.
    /// </summary>
    [JsonPropertyName("show_subtask_parent_names")]
    public bool? ShowSubtaskParentNames { get; init; }

    /// <summary>
    /// Gets a value indicating whether closed subtasks are shown in the view.
    /// This is distinct from the main task filter for closed tasks.
    /// </summary>
    [JsonPropertyName("show_closed_subtasks")]
    public bool? ShowClosedSubtasks { get; init; }

    /// <summary>
    /// Gets a value indicating whether assignees are shown in the view.
    /// </summary>
    [JsonPropertyName("show_assignees")]
    public bool? ShowAssignees { get; init; }

    /// <summary>
    /// Gets a value indicating whether due dates are shown in the view.
    /// </summary>
    [JsonPropertyName("show_due_date")]
    public bool? ShowDueDate { get; init; }

    /// <summary>
    /// Gets a value indicating whether time estimates are shown in the view.
    /// </summary>
    [JsonPropertyName("show_time_estimates")]
    public bool? ShowTimeEstimates { get; init; }

    /// <summary>
    /// Gets a value indicating whether time in status is shown in the view.
    /// </summary>
    [JsonPropertyName("show_time_in_status")]
    public bool? ShowTimeInStatus { get; init; }

    /// <summary>
    /// Gets a value indicating whether task priorities are shown in the view.
    /// </summary>
    [JsonPropertyName("show_priority")]
    public bool? ShowPriority { get; init; }

    /// <summary>
    /// Gets a value indicating whether tags are shown in the view.
    /// </summary>
    [JsonPropertyName("show_tags")]
    public bool? ShowTags { get; init; }

    /// <summary>
    /// Gets a value indicating whether empty statuses (columns) are shown, mainly relevant for Board views.
    /// </summary>
    [JsonPropertyName("show_empty_statuses")]
    public bool? ShowEmptyStatuses { get; init; }

    /// <summary>
    /// Gets a value indicating whether only comments involving the current user ("me") are shown or highlighted.
    /// </summary>
    [JsonPropertyName("me_comments")]
    public bool? MeComments { get; init; }

    /// <summary>
    /// Gets a value indicating whether only subtasks assigned to the current user ("me") are shown or highlighted.
    /// </summary>
    [JsonPropertyName("me_subtasks")]
    public bool? MeSubtasks { get; init; }

    /// <summary>
    /// Gets a value indicating whether only checklists involving the current user ("me") are shown or highlighted.
    /// </summary>
    [JsonPropertyName("me_checklists")]
    public bool? MeChecklists { get; init; }

    /// <summary>
    /// Gets a value indicating whether images are shown on task cards, relevant for Board views.
    /// </summary>
    [JsonPropertyName("show_images_on_cards")]
    public bool? ShowImagesOnCards { get; init; }

    /// <summary>
    /// Gets a value indicating whether empty columns are collapsed, relevant for Board views.
    /// </summary>
    [JsonPropertyName("collapse_empty_columns")]
    public bool? CollapseEmptyColumns { get; init; }

    /// <summary>
    /// Gets a value indicating if grouping is explicitly set to "none".
    /// </summary>
    [JsonPropertyName("group_by_none")]
    public bool? GroupByNone { get; init; }

    /// <summary>
    /// Gets a value indicating whether subtasks are shown in a separate group.
    /// </summary>
    [JsonPropertyName("subtasks_separate_group")]
    public bool? SubtasksSeparateGroup { get; init; }

    /// <summary>
    /// Gets a value indicating whether subtask information (e.g., time estimates) rolls up to the parent task.
    /// </summary>
    [JsonPropertyName("subtasks_parent_rollup")]
    public bool? SubtasksParentRollup { get; init; }

    /// <summary>
    /// Gets the width of the sidebar in Gantt views.
    /// </summary>
    [JsonPropertyName("show_gantt_sidebar_width")]
    public int? ShowGanttSidebarWidth { get; init; }

    /// <summary>
    /// Gets the time unit displayed in Gantt views (e.g., "days", "weeks").
    /// </summary>
    [JsonPropertyName("gantt_show_days")]
    public string? GanttShowDays { get; init; }

    /// <summary>
    /// Gets the type of Calendar view (e.g., "month", "week", "day").
    /// </summary>
    [JsonPropertyName("calendar_view_type")]
    public string? CalendarViewType { get; init; }

    /// <summary>
    /// Gets a value indicating whether weekends are shown in Calendar views.
    /// </summary>
    [JsonPropertyName("calendar_show_weekends")]
    public bool? CalendarShowWeekends { get; init; }

    /// <summary>
    /// Gets a value indicating whether reCAPTCHA is shown on Form views.
    /// </summary>
    [JsonPropertyName("form_show_recaptcha")]
    public bool? FormShowRecaptcha { get; init; }
}
