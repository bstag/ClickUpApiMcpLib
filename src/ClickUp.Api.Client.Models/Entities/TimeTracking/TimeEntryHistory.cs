using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask (if that's the task DTO)
// Assuming ClickUpList and Space are in root Models or Entities. Adjust if needed.
// For simplicity, if specific simplified DTOs for task/list/space in history are not defined,
// we might use JsonElement or omit them if they are too complex for this context without further spec details.

namespace ClickUp.Api.Client.Models.Entities.TimeTracking;

/// <summary>
/// Represents a historical record of changes to a Time Entry.
/// </summary>
public class TimeEntryHistory
{
    /// <summary>
    /// Gets or sets the user who performed the action.
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the type of action performed (e.g., "create", "update", "delete").
    /// </summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }

    /// <summary>
    /// Gets or sets the field that was changed, if applicable.
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    /// <summary>
    /// Gets or sets the value of the field before the change. Captured as <see cref="JsonElement"/> due to potential variability.
    /// </summary>
    [JsonPropertyName("before")]
    public JsonElement? Before { get; set; }

    /// <summary>
    /// Gets or sets the value of the field after the change. Captured as <see cref="JsonElement"/> due to potential variability.
    /// </summary>
    [JsonPropertyName("after")]
    public JsonElement? After { get; set; }

    /// <summary>
    /// Gets or sets the date of the historical action, as a string (e.g., Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>
    /// Gets or sets the source of the change (e.g., "clickup", "api").
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets any note associated with this historical change.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the task associated with this time entry history, if applicable.
    /// </summary>
    [JsonPropertyName("task")]
    public CuTask? Task { get; set; }

    /// <summary>
    /// Gets or sets information about the list associated with this time entry's task.
    /// Captured as <see cref="JsonElement"/> as the structure might be a simplified reference.
    /// </summary>
    [JsonPropertyName("list")]
    public JsonElement? List { get; set; }

    /// <summary>
    /// Gets or sets information about the space associated with this time entry's task.
    /// Captured as <see cref="JsonElement"/> as the structure might be a simplified reference.
    /// </summary>
    [JsonPropertyName("space")]
    public JsonElement? Space { get; set; }
}
