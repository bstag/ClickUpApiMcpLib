using System.Collections.Generic;
using System.Text.Json.Serialization;
using System; // For Nullable

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    // CustomTaskFieldToSet and CustomTaskFieldValueOptions are now in CustomTaskFieldModels.cs

    public record CreateTaskRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("description")] string? Description, // Markdown supported
        [property: JsonPropertyName("assignees")] List<int>? Assignees, // User IDs
        [property: JsonPropertyName("group_assignees")] List<string>? GroupAssignees, // Group IDs (string)
        [property: JsonPropertyName("tags")] List<string>? Tags, // Tag names
        [property: JsonPropertyName("status")] string? Status, // Status name or ID (prefer name for robustness if API allows)
        [property: JsonPropertyName("priority")] int? Priority, // Priority level (e.g., 1 for Urgent, 2 High, 3 Normal, 4 Low)
        [property: JsonPropertyName("due_date")] long? DueDate, // Unix timestamp in milliseconds
        [property: JsonPropertyName("due_date_time")] bool? DueDateTime, // If false, due_date is treated as an all-day task
        [property: JsonPropertyName("time_estimate")] int? TimeEstimate, // Milliseconds
        [property: JsonPropertyName("start_date")] long? StartDate, // Unix timestamp in milliseconds
        [property: JsonPropertyName("start_date_time")] bool? StartDateTime, // If false, start_date is treated as an all-day task
        [property: JsonPropertyName("notify_all")] bool? NotifyAll,
        [property: JsonPropertyName("parent")] string? Parent, // Task ID of the parent task
        [property: JsonPropertyName("links_to")] string? LinksTo, // Task ID to link to
        [property: JsonPropertyName("check_required_custom_fields")] bool? CheckRequiredCustomFields,
        [property: JsonPropertyName("custom_fields")] List<CustomTaskFieldToSet>? CustomFields,
        [property: JsonPropertyName("custom_item_id")] long? CustomItemId, // For use with an external system's ID, requires `custom_task_ids` Space feature
        [property: JsonPropertyName("list_id")] string? ListId // Not in spec for CreateTaskrequest body, but often required in URL or if creating task without a default list
                                                              // Adding here if it's ever part of a generic task creation model.
                                                              // Usually, task creation is scoped to a list via URL.
    );
}
