using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask (if that's the task DTO)
// Assuming ClickUpList and Space are in root Models or Entities. Adjust if needed.
// For simplicity, if specific simplified DTOs for task/list/space in history are not defined,
// we might use JsonElement or omit them if they are too complex for this context without further spec details.

namespace ClickUp.Api.Client.Models.Entities.TimeTracking;

public class TimeEntryHistory
{
    [JsonPropertyName("user")]
    public User? User { get; set; }

    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("field")]
    public string? Field { get; set; }

    [JsonPropertyName("before")]
    public JsonElement? Before { get; set; }

    [JsonPropertyName("after")]
    public JsonElement? After { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; } // Timestamp

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    // Optional related entities - might need specific simplified DTOs from API spec
    [JsonPropertyName("task")]
    public CuTask? Task { get; set; } // Assuming CuTask is the main task DTO

    // For List and Space, assuming simplified versions or direct IDs might be returned.
    // If full objects ClickUpList/Space are returned, using them directly is fine.
    // For now, let's assume they might be simplified or not always present.
    [JsonPropertyName("list")]
    public JsonElement? List { get; set; } // Placeholder if specific List DTO for history isn't defined

    [JsonPropertyName("space")]
    public JsonElement? Space { get; set; } // Placeholder if specific Space DTO for history isn't defined
}
