using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    // Structure for modifying assignees (users or groups)
    public record AssigneeModification<T>
    (
        [property: JsonPropertyName("add")] List<T>? Add,
        [property: JsonPropertyName("rem")] List<T>? Remove
    );

    public record UpdateTaskRequest
    (
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("description")] string? Description, // Markdown supported
        [property: JsonPropertyName("status")] string? Status, // Status name or ID
        [property: JsonPropertyName("priority")] int? Priority, // Priority level
        [property: JsonPropertyName("due_date")] long? DueDate, // Unix timestamp in milliseconds
        [property: JsonPropertyName("due_date_time")] bool? DueDateTime,
        [property: JsonPropertyName("parent")] string? Parent, // Task ID of the parent task to move to
        [property: JsonPropertyName("time_estimate")] int? TimeEstimate, // Milliseconds
        [property: JsonPropertyName("start_date")] long? StartDate, // Unix timestamp in milliseconds
        [property: JsonPropertyName("start_date_time")] bool? StartDateTime,
        [property: JsonPropertyName("assignees")] AssigneeModification<int>? Assignees, // User IDs
        [property: JsonPropertyName("group_assignees")] AssigneeModification<string>? GroupAssignees, // Group IDs
        [property: JsonPropertyName("archived")] bool? Archived,
        [property: JsonPropertyName("custom_fields")] List<CustomTaskFieldToSet>? CustomFields // Using the same CustomTaskFieldToSet from CreateTaskRequest
                                                                                             // Ensure CustomTaskFieldToSet is accessible here (e.g. via using or if in same namespace)
                                                                                             // If it's in CreateTaskRequest.cs, it might need to be moved to a common location or duplicated.
                                                                                             // For now, assuming it's resolvable.
    );
}
