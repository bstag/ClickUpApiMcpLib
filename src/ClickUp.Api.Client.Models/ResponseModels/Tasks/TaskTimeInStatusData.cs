using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks;

public class TaskTimeInStatusData
{
    [JsonPropertyName("by_minute")]
    public int? ByMinute { get; set; }

    [JsonPropertyName("since")]
    public string? Since { get; set; }
}
