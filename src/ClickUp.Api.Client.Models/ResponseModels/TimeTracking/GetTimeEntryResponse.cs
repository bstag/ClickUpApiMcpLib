using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.TimeTracking;

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking;

public class GetTimeEntryResponse
{
    [JsonPropertyName("data")]
    public TimeEntry Data { get; set; } = null!;
}
