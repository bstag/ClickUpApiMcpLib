using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For User

namespace ClickUp.Api.Client.Models.Entities.TimeTracking
{
    // Simplified CuTask Reference for Time Entries
    public record TimeEntryTaskReference
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("custom_id")] string? CustomId,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("status")] Status? Status, // Simplified status
        [property: JsonPropertyName("url")] string? Url
    );

    public record TimeEntry
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("task")] TimeEntryTaskReference? Task, // Simplified task object
        [property: JsonPropertyName("wid")] string Wid, // Workspace ID
        [property: JsonPropertyName("user")] ComUser User, // Full User object from Common
        [property: JsonPropertyName("billable")] bool Billable,
        [property: JsonPropertyName("start")] string Start, // Timestamp string (ISO 8601 or Unix ms string)
        [property: JsonPropertyName("end")] string? End, // Timestamp string, null if timer is running
        [property: JsonPropertyName("duration")] long Duration, // Duration in milliseconds. Negative if timer is running.
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("tags")] List<TaskTag>? Tags, // Using TaskTag from this namespace. Could be Common.Tag.
                                                                 // OpenAPI 'Datum1' refers to 'task_tags' which are simple name/color.
                                                                 // 'tags' on a time entry itself might be richer.
                                                                 // Let's assume 'tags' are like TaskTag for now.
        [property: JsonPropertyName("source")] string? Source, // e.g., "clickup", "api"
        [property: JsonPropertyName("at")] string At, // Timestamp string for when the entry was last updated or created

        // These are often part of the main time entry object directly or nested under a 'task' object
        // For Gettimeentrieswithinadaterangeresponse (Datum1), task_location and task_tags are separate top-level fields in the response item.
        // However, a singular time entry GET might embed them.
        // For now, assuming they can be part of the TimeEntry model directly if the API flattens them.
        [property: JsonPropertyName("task_location")] TaskLocation? TaskLocationInfo, // Renamed to avoid conflict
        [property: JsonPropertyName("task_tags")] List<TaskTag>? TaskTags, // Distinct from 'tags' on the time entry itself. These are tags on the task.
        [property: JsonPropertyName("task_url")] string? TaskUrl, // URL of the task
        [property: JsonPropertyName("is_locked")] bool? IsLocked, // If the time entry is locked
        [property: JsonPropertyName("locked_details")] object? LockedDetails // Details if locked
    );
}
