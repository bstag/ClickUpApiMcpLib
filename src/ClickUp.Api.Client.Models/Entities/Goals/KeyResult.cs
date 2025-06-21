using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.Goals
{
    public record KeyResult
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("goal_id")] string GoalId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("type")] string Type, // e.g., "number", "currency", "boolean", "percentage", "task", "list"
        [property: JsonPropertyName("unit")] string? Unit, // e.g., "$", "%", or custom unit for "number" type
        [property: JsonPropertyName("creator")] User? CreatorUser, // Changed from Creator to avoid conflict
        [property: JsonPropertyName("date_created")] string DateCreated, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("goal_pretty_id")] string? GoalPrettyId,
        [property: JsonPropertyName("percent_completed")] int? PercentCompleted, // Nullable as it might not apply to all types or if not started
        [property: JsonPropertyName("completed")] bool Completed,
        [property: JsonPropertyName("task_ids")] List<string>? TaskIds, // For "task" type
        [property: JsonPropertyName("list_ids")] List<string>? ListIds, // For "list" type (unofficial, but plausible)
        [property: JsonPropertyName("subcategory_ids")] List<string>? SubcategoryIds, // ClickUp specific for some task categorizations
        [property: JsonPropertyName("owners")] List<User>? Owners,
        [property: JsonPropertyName("last_action")] LastAction? LastAction, // Using the LastAction record created
        [property: JsonPropertyName("steps_current")] object? StepsCurrent, // Can be int, bool, string depending on type
        [property: JsonPropertyName("steps_start")] object? StepsStart,   // Can be int, bool, string
        [property: JsonPropertyName("steps_end")] object? StepsEnd,     // Can be int, bool, string
        [property: JsonPropertyName("steps_taken")] int? StepsTaken, // Number of steps recorded
        [property: JsonPropertyName("history")] List<LastAction>? History, // History of changes, using LastAction as a likely type
        [property: JsonPropertyName("last_action_date")] string? LastActionDate, // Date of the last action
        [property: JsonPropertyName("active")] bool? Active // If the key result is currently active
    );
}
