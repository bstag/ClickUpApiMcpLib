using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Goals
{
    /// <summary>
    /// Represents the request to create a new Key Result for a Goal.
    /// </summary>
    /// <param name="Name">The name of the key result.</param>
    /// <param name="Owners">A list of user IDs to be assigned as owners of the key result.</param>
    /// <param name="Type">The type of the key result (e.g., "number", "currency", "boolean", "percentage", "task").</param>
    /// <param name="StepsStart">The starting value for the key result's progress. Type depends on <paramref name="Type"/> (e.g., int, bool, string). Null if not applicable.</param>
    /// <param name="StepsEnd">The target value for the key result's progress. Type depends on <paramref name="Type"/>.</param>
    /// <param name="Unit">Optional: The unit of measurement for "number", "currency", or "percentage" types (e.g., "$", "%").</param>
    /// <param name="TaskIds">Optional: A list of task IDs to link to this key result, if <paramref name="Type"/> is "task".</param>
    /// <param name="ListIds">Optional: A list of list IDs to link to this key result, if <paramref name="Type"/> is "list" (check API support).</param>
    /// <param name="GoalId">The ID of the parent Goal this key result belongs to. Often part of the URL path but included here if needed in the body.</param>
    public record CreateKeyResultRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("owners")] List<int> Owners,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("steps_start")] object? StepsStart,
        [property: JsonPropertyName("steps_end")] object StepsEnd,
        [property: JsonPropertyName("unit")] string? Unit,
        [property: JsonPropertyName("task_ids")] List<string>? TaskIds,
        [property: JsonPropertyName("list_ids")] List<string>? ListIds,
        [property: JsonPropertyName("goal_id")] string GoalId
    );
}
