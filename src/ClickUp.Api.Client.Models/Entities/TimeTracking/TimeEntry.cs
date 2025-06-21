using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.TimeTracking
{
    /// <summary>
    /// Represents a simplified reference to a CuTask, used within Time Entry contexts.
    /// </summary>
    /// <param name="Id">The unique identifier of the task.</param>
    /// <param name="CustomId">The custom identifier of the task, if any.</param>
    /// <param name="Name">The name of the task.</param>
    /// <param name="Status">The current status of the task.</param>
    /// <param name="Url">The URL to access the task in ClickUp.</param>
    public record TimeEntryTaskReference
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("custom_id")] string? CustomId,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("status")] Status? Status,
        [property: JsonPropertyName("url")] string? Url
    );

    /// <summary>
    /// Represents a Time Entry in ClickUp.
    /// </summary>
    /// <param name="Id">The unique identifier of the time entry.</param>
    /// <param name="Task">A simplified reference to the task this time entry is associated with.</param>
    /// <param name="Wid">The identifier of the workspace this time entry belongs to.</param>
    /// <param name="User">The user who created this time entry.</param>
    /// <param name="Billable">Indicates whether this time entry is billable.</param>
    /// <param name="Start">The start time of the entry, as a string (e.g., Unix timestamp in milliseconds or ISO 8601).</param>
    /// <param name="End">The end time of the entry, as a string. Null if the timer is currently running.</param>
    /// <param name="Duration">The duration of the time entry in milliseconds. Can be negative if the timer is running.</param>
    /// <param name="Description">A description for the time entry.</param>
    /// <param name="Tags">A list of tags applied directly to this time entry.</param>
    /// <param name="Source">The source of the time entry (e.g., "clickup", "api").</param>
    /// <param name="At">The timestamp string for when the entry was last updated or created.</param>
    /// <param name="TaskLocationInfo">Location information (List, Folder, Space) for the associated task.</param>
    /// <param name="TaskTags">Tags associated with the task itself (distinct from tags on the time entry).</param>
    /// <param name="TaskUrl">The URL of the associated task.</param>
    /// <param name="IsLocked">Indicates if the time entry is locked from further editing.</param>
    /// <param name="LockedDetails">Additional details if the time entry is locked.</param>
    public record TimeEntry
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("task")] TimeEntryTaskReference? Task,
        [property: JsonPropertyName("wid")] string Wid,
        [property: JsonPropertyName("user")] User User,
        [property: JsonPropertyName("billable")] bool Billable,
        [property: JsonPropertyName("start")] string Start,
        [property: JsonPropertyName("end")] string? End,
        [property: JsonPropertyName("duration")] long Duration,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("tags")] List<TaskTag>? Tags,
        [property: JsonPropertyName("source")] string? Source,
        [property: JsonPropertyName("at")] string At,
        [property: JsonPropertyName("task_location")] TaskLocation? TaskLocationInfo,
        [property: JsonPropertyName("task_tags")] List<TaskTag>? TaskTags,
        [property: JsonPropertyName("task_url")] string? TaskUrl,
        [property: JsonPropertyName("is_locked")] bool? IsLocked,
        [property: JsonPropertyName("locked_details")] object? LockedDetails
    );
}
