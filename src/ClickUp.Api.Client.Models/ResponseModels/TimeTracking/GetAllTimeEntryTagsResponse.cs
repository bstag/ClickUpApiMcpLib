using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.TimeTracking; // For TaskTag

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking;

public class GetAllTimeEntryTagsResponse
{
    // Assuming the API returns a list of tags directly under a "tags" or "data" property.
    // Let's use "data" for consistency with other wrapper responses.
    [JsonPropertyName("data")]
    public List<TaskTag> Data { get; set; } = new();

    // If the actual response is just List<TaskTag>, this wrapper might be simplified
    // or the service method could deserialize directly to List<TaskTag>.
    // Using a wrapper for now for consistency and future flexibility.
}
