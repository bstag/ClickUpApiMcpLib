using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking
{
    /// <summary>
    /// Represents the request to create a new manual Time Entry.
    /// </summary>
    /// <param name="Description">Optional: Description for the time entry.</param>
    /// <param name="Tags">Optional: A list of tags to apply to the time entry.</param>
    /// <param name="Start">The start time of the entry, as a Unix timestamp in milliseconds (required).</param>
    /// <param name="Duration">The duration of the time entry in milliseconds (required, must be positive).</param>
    /// <param name="Billable">Optional: Indicates whether this time entry is billable.</param>
    /// <param name="Assignee">Optional: The user ID of the person this time entry is for. Defaults to the authenticated user if not provided.</param>
    /// <param name="TaskId">Optional: The ID of the task to associate with this time entry.</param>
    /// <param name="WorkspaceId">Optional: The ID of the workspace. Usually inferred from the token or task context.</param>
    public record CreateTimeEntryRequest
    (
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("tags")] List<TimeTrackingTagDefinition>? Tags,
        [property: JsonPropertyName("start")] long Start,
        [property: JsonPropertyName("duration")] int Duration,
        [property: JsonPropertyName("billable")] bool? Billable,
        [property: JsonPropertyName("assignee")] int? Assignee,
        [property: JsonPropertyName("tid")] string? TaskId,
        [property: JsonPropertyName("wid")] string? WorkspaceId
    );
}
