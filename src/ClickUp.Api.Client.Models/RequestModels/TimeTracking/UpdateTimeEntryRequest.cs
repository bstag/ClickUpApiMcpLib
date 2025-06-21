using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking
{
    /// <summary>
    /// Represents the request to update an existing Time Entry.
    /// All properties are optional; only provided properties will be updated.
    /// </summary>
    /// <param name="Description">Optional: New description for the time entry.</param>
    /// <param name="Tags">Optional: List of tags to add, remove, or replace, depending on <paramref name="TagAction"/>.</param>
    /// <param name="TagAction">Optional: Action to perform on tags ("add", "remove", or "replace").
    /// If "replace", the <paramref name="Tags"/> list will become the new set of tags.
    /// If "add" or "remove", <paramref name="Tags"/> contains the tags to add or remove.
    /// </param>
    /// <param name="Start">Optional: New start time as a Unix timestamp in milliseconds.</param>
    /// <param name="End">Optional: New end time as a Unix timestamp in milliseconds.</param>
    /// <param name="TaskId">Optional: New task ID to associate or re-associate with this time entry.</param>
    /// <param name="Billable">Optional: New billable status for the time entry.</param>
    /// <param name="Duration">Optional: New duration in milliseconds. The API might recalculate this if start and end times are also changed.</param>
    /// <param name="Assignee">Optional: New user ID to assign this time entry to.</param>
    /// <param name="IsLocked">Optional: Set to true to lock the time entry, false to unlock.</param>
    public record UpdateTimeEntryRequest
    (
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("tags")] List<TimeTrackingTagDefinition>? Tags,
        [property: JsonPropertyName("tag_action")] string? TagAction,
        [property: JsonPropertyName("start")] long? Start,
        [property: JsonPropertyName("end")] long? End,
        [property: JsonPropertyName("tid")] string? TaskId,
        [property: JsonPropertyName("billable")] bool? Billable,
        [property: JsonPropertyName("duration")] int? Duration,
        [property: JsonPropertyName("assignee")] int? Assignee,
        [property: JsonPropertyName("is_locked")] bool? IsLocked
    );
}
