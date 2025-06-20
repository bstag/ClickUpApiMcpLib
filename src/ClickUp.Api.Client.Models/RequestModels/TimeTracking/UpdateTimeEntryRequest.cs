using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking
{
    // Assuming TimeTrackingTagDefinition is accessible from CreateTimeEntryRequest.cs
    // or moved to a shared location within this namespace.

    public record UpdateTimeEntryRequest
    (
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("tags")] List<TimeTrackingTagDefinition>? Tags, // List of simple tag definitions or just names
        [property: JsonPropertyName("tag_action")] string? TagAction, // "add", "remove", or "replace". If "replace", 'tags' must be provided.
                                                                    // If "add" or "remove", 'tags' contains the tags to add/remove.
        [property: JsonPropertyName("start")] long? Start, // Unix timestamp in milliseconds for the start of the entry
        [property: JsonPropertyName("end")] long? End,     // Unix timestamp in milliseconds for the end of the entry
        [property: JsonPropertyName("tid")] string? TaskId, // CuTask ID to associate/re-associate with this time entry
        [property: JsonPropertyName("billable")] bool? Billable,
        [property: JsonPropertyName("duration")] int? Duration, // Duration in milliseconds. API might recalculate if start/end change.
        [property: JsonPropertyName("assignee")] int? Assignee, // User ID. To change the user this time entry is for.
        [property: JsonPropertyName("is_locked")] bool? IsLocked // To lock/unlock the time entry
    );
}
