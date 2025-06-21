using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    /// <summary>
    /// Represents a structure for modifying assignees (users or groups) by specifying lists to add and remove.
    /// </summary>
    /// <typeparam name="T">The type of the identifier for assignees (e.g., int for user IDs, string for group IDs).</typeparam>
    /// <param name="Add">A list of assignee identifiers to add.</param>
    /// <param name="Remove">A list of assignee identifiers to remove.</param>
    public record AssigneeModification<T>
    (
        [property: JsonPropertyName("add")] List<T>? Add,
        [property: JsonPropertyName("rem")] List<T>? Remove
    );

    /// <summary>
    /// Represents the request to update an existing CuTask.
    /// All properties are optional; only provided properties will be updated.
    /// </summary>
    /// <param name="Name">Optional: New name for the task.</param>
    /// <param name="Description">Optional: New Markdown supported description for the task.</param>
    /// <param name="Status">Optional: New status for the task (name or ID).</param>
    /// <param name="Priority">Optional: New priority level for the task.</param>
    /// <param name="DueDate">Optional: New due date as a Unix timestamp in milliseconds.</param>
    /// <param name="DueDateTime">Optional: Indicates if the new <paramref name="DueDate"/> includes time.</param>
    /// <param name="Parent">Optional: The ID of a new parent task to move this task under (making it a subtask).</param>
    /// <param name="TimeEstimate">Optional: New time estimate in milliseconds.</param>
    /// <param name="StartDate">Optional: New start date as a Unix timestamp in milliseconds.</param>
    /// <param name="StartDateTime">Optional: Indicates if the new <paramref name="StartDate"/> includes time.</param>
    /// <param name="Assignees">Optional: Modifications to user assignees (add/remove lists of user IDs).</param>
    /// <param name="GroupAssignees">Optional: Modifications to group assignees (add/remove lists of group IDs).</param>
    /// <param name="Archived">Optional: Set to true to archive the task, false to unarchive.</param>
    /// <param name="CustomFields">Optional: A list of custom field values to update.</param>
    public record UpdateTaskRequest
    (
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("priority")] int? Priority,
        [property: JsonPropertyName("due_date")] long? DueDate,
        [property: JsonPropertyName("due_date_time")] bool? DueDateTime,
        [property: JsonPropertyName("parent")] string? Parent,
        [property: JsonPropertyName("time_estimate")] int? TimeEstimate,
        [property: JsonPropertyName("start_date")] long? StartDate,
        [property: JsonPropertyName("start_date_time")] bool? StartDateTime,
        [property: JsonPropertyName("assignees")] AssigneeModification<int>? Assignees,
        [property: JsonPropertyName("group_assignees")] AssigneeModification<string>? GroupAssignees,
        [property: JsonPropertyName("archived")] bool? Archived,
        [property: JsonPropertyName("custom_fields")] List<CustomTaskFieldToSet>? CustomFields
    );
}
