using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking
{
    /// <summary>
    /// Represents the request to start a new Time Entry (timer).
    /// </summary>
    /// <param name="Description">Optional: Description for the time entry.</param>
    /// <param name="Tags">Optional: A list of tags to apply to the time entry.</param>
    /// <param name="TaskId">Optional: The ID of the task to associate with this timer. Can be null if starting a timer not linked to a specific task.</param>
    /// <param name="Billable">Optional: Indicates whether this time entry should be billable.</param>
    /// <param name="WorkspaceId">Optional: The ID of the workspace. Often inferred from the API token, but can be specified.</param>
    /// <param name="ProjectId_Legacy">Optional: Legacy project ID, if applicable. Prefer using <paramref name="TaskId"/> for linking to tasks.</param>
    /// <param name="CreatedWith">Optional: Identifier for the client or application creating the time entry.</param>
    public record StartTimeEntryRequest
    (
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("tags")] List<TimeTrackingTagDefinition>? Tags,
        [property: JsonPropertyName("tid")] string? TaskId,
        [property: JsonPropertyName("billable")] bool? Billable,
        [property: JsonPropertyName("wid")] string? WorkspaceId,
        [property: JsonPropertyName("pid")] int? ProjectId_Legacy,
        [property: JsonPropertyName("created_with")] string? CreatedWith
    );
}
