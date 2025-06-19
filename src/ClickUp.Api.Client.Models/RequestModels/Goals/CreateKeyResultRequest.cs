using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Goals
{
    public record CreateKeyResultRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("owners")] List<int> Owners, // List of user IDs
        [property: JsonPropertyName("type")] string Type, // e.g., "number", "currency", "boolean", "percentage", "task", "list"
        [property: JsonPropertyName("steps_start")] object? StepsStart, // Can be int, bool, string, null if not applicable
        [property: JsonPropertyName("steps_end")] object StepsEnd, // Can be int, bool, string
        [property: JsonPropertyName("unit")] string? Unit, // e.g., "$", "%", or custom unit for "number" type
        [property: JsonPropertyName("task_ids")] List<string>? TaskIds, // For "task" type
        [property: JsonPropertyName("list_ids")] List<string>? ListIds, // For "list" type (if supported)
        [property: JsonPropertyName("goal_id")] string GoalId // Not in spec, but typically required for path or body
    );
}
