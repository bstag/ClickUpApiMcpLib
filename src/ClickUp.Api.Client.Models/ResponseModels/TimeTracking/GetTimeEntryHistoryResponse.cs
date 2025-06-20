using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.TimeTracking; // For TimeEntryHistory

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking;

public class GetTimeEntryHistoryResponse
{
    [JsonPropertyName("data")] // Assuming the history items are in a "data" array
    public List<TimeEntryHistory> History { get; set; } = new();
}
