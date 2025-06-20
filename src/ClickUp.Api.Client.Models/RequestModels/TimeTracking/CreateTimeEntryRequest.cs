using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking
{
    // TimeTrackingTagDefinition is now in TimeTrackingTagDefinition.cs

    public record CreateTimeEntryRequest
    (
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("tags")] List<TimeTrackingTagDefinition>? Tags, // List of simple tag definitions or just names
        [property: JsonPropertyName("start")] long Start, // Unix timestamp in milliseconds for the start of the entry
        [property: JsonPropertyName("duration")] int Duration, // Duration in milliseconds (must be positive)
        [property: JsonPropertyName("billable")] bool? Billable,
        [property: JsonPropertyName("assignee")] int? Assignee, // User ID of the person who this time entry is for
        [property: JsonPropertyName("tid")] string? TaskId, // CuTask ID to associate with this time entry
        [property: JsonPropertyName("wid")] string? WorkspaceId // Workspace ID, sometimes required if not inferred from auth/task
    );
}
