using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking;

/// <summary>
/// Represents the response model for getting all tags from time entries.
/// </summary>
public record class GetAllTagsFromTimeEntriesResponse
(
    [property: JsonPropertyName("data")]
    List<TimeEntryTagDetailsResponse> Data
);
