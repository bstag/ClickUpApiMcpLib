using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking;

/// <summary>
/// Represents the request model for removing tags from time entries.
/// </summary>
public record class RemoveTagsFromTimeEntriesRequest
(
    [property: JsonPropertyName("time_entry_ids")]
    List<string> TimeEntryIds,

    [property: JsonPropertyName("tags")]
    List<TimeTrackingTagDefinition> Tags
);
