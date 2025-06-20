using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking
{
    // Assuming TimeTrackingTagDefinition is accessible from its own file now.

    public record StartTimeEntryRequest
    (
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("tags")] List<TimeTrackingTagDefinition>? Tags,
        [property: JsonPropertyName("tid")] string? TaskId, // CuTask ID to associate with this timer. Can be null.
        [property: JsonPropertyName("billable")] bool? Billable,
        [property: JsonPropertyName("wid")] string? WorkspaceId, // Workspace ID. Often inferred from token, but can be specified.
        [property: JsonPropertyName("pid")] int? ProjectId_Legacy, // Legacy project ID, if applicable. Use `tid` for tasks.
        [property: JsonPropertyName("created_with")] string? CreatedWith // Optional: client identification
    );
}
