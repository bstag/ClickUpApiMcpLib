using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks;

public class TaskTimeInStatusResponse
{
    [JsonPropertyName("total_time")]
    public TaskTimeInStatusData TotalTime { get; set; } = new();

    [JsonPropertyName("status_history")]
    public List<StatusHistoryItem> StatusHistory { get; set; } = new();

    [JsonPropertyName("current_status")]
    public StatusHistoryItem CurrentStatus { get; set; } = new(); // CurrentStatus has same structure as a StatusHistoryItem
}
