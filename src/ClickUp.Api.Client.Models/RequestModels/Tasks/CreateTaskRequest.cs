using System.Collections.Generic;
using System.Text.Json.Serialization;
using System; // For Nullable

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    /// <summary>
    /// Represents the request to create a new CuTask.
    /// </summary>
    /// <param name="Name">The name of the new task.</param>
    /// <param name="Description">Optional: Markdown supported description for the task.</param>
    /// <param name="Assignees">Optional: List of user IDs to assign to the task.</param>
    /// <param name="GroupAssignees">Optional: List of group IDs to assign to the task.</param>
    /// <param name="Tags">Optional: List of tag names to apply to the task.</param>
    /// <param name="Status">Optional: The status to set for the new task (name or ID).</param>
    /// <param name="Priority">Optional: The priority level for the task (e.g., 1 for Urgent, 2 High, 3 Normal, 4 Low).</param>
    /// <param name="DueDate">Optional: Due date for the task as a Unix timestamp in milliseconds.</param>
    /// <param name="DueDateTime">Optional: If true, <paramref name="DueDate"/> includes a time component; if false, it's an all-day task.</param>
    /// <param name="TimeEstimate">Optional: Estimated time for the task in milliseconds.</param>
    /// <param name="StartDate">Optional: Start date for the task as a Unix timestamp in milliseconds.</param>
    /// <param name="StartDateTime">Optional: If true, <paramref name="StartDate"/> includes a time component; if false, it's an all-day task.</param>
    /// <param name="NotifyAll">Optional: If true, all watchers of the task will be notified.</param>
    /// <param name="Parent">Optional: The ID of the parent task if this is a subtask.</param>
    /// <param name="LinksTo">Optional: The ID of a task to link to this new task.</param>
    /// <param name="CheckRequiredCustomFields">Optional: If true, the API will check for required custom fields and may return an error if they are not provided.</param>
    /// <param name="CustomFields">Optional: A list of custom field values to set for the new task.</param>
    /// <param name="CustomItemId">Optional: A custom item ID for use with an external system, requires the `custom_task_ids` Space feature.</param>
    /// <param name="ListId">Optional: The ID of the list where the task will be created. While often part of the URL, it might be included here in some contexts.</param>
    public record CreateTaskRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("assignees")] List<int>? Assignees,
        [property: JsonPropertyName("group_assignees")] List<string>? GroupAssignees,
        [property: JsonPropertyName("tags")] List<string>? Tags,
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("priority")] int? Priority,
        [property: JsonPropertyName("due_date")] System.DateTimeOffset? DueDate,
        [property: JsonPropertyName("due_date_time")] bool? DueDateTime,
        [property: JsonPropertyName("time_estimate")] int? TimeEstimate,
        [property: JsonPropertyName("start_date")] System.DateTimeOffset? StartDate,
        [property: JsonPropertyName("start_date_time")] bool? StartDateTime,
        [property: JsonPropertyName("notify_all")] bool? NotifyAll,
        [property: JsonPropertyName("parent")] string? Parent,
        [property: JsonPropertyName("links_to")] string? LinksTo,
        [property: JsonPropertyName("check_required_custom_fields")] bool? CheckRequiredCustomFields,
        [property: JsonPropertyName("custom_fields")] List<CustomTaskFieldToSet>? CustomFields,
        [property: JsonPropertyName("custom_item_id")] long? CustomItemId,
        [property: JsonPropertyName("list_id")] string? ListId
    );
}
