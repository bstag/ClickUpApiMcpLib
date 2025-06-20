using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks;

public class StatusHistoryItem
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; set; }

    [JsonPropertyName("total_time")]
    public TaskTimeInStatusData? TotalTime { get; set; }
}
