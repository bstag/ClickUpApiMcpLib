using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.Goals
{
    /// <summary>
    /// Represents a Key Result associated with a Goal in ClickUp.
    /// </summary>
    /// <param name="Id">The unique identifier of the key result.</param>
    /// <param name="GoalId">The identifier of the parent goal this key result belongs to.</param>
    /// <param name="Name">The name of the key result.</param>
    /// <param name="Type">The type of the key result (e.g., "number", "currency", "boolean", "percentage", "task").</param>
    /// <param name="Unit">The unit of measurement for the key result (e.g., "$", "%", or a custom unit for "number" type).</param>
    /// <param name="CreatorUser">The user who created the key result.</param>
    /// <param name="DateCreated">The date the key result was created, as a string (e.g., Unix timestamp in milliseconds).</param>
    /// <param name="GoalPrettyId">The user-friendly identifier of the parent goal.</param>
    /// <param name="PercentCompleted">The percentage of the key result that has been completed. Nullable if not applicable or not started.</param>
    /// <param name="Completed">Indicates whether the key result is completed.</param>
    /// <param name="TaskIds">A list of task identifiers associated with this key result, if applicable (for "task" type).</param>
    /// <param name="ListIds">A list of list identifiers associated with this key result (potentially for a "list" type, though less common).</param>
    /// <param name="SubcategoryIds">A list of subcategory identifiers, specific to ClickUp task categorizations.</param>
    /// <param name="Owners">A list of users who own this key result.</param>
    /// <param name="LastAction">The last action performed on this key result.</param>
    /// <param name="StepsCurrent">The current progress or value of the key result. Type depends on the key result type.</param>
    /// <param name="StepsStart">The starting value or baseline for the key result. Type depends on the key result type.</param>
    /// <param name="StepsEnd">The target value for the key result. Type depends on the key result type.</param>
    /// <param name="StepsTaken">The number of discrete steps taken towards the key result.</param>
    /// <param name="History">A list of historical actions or changes related to this key result.</param>
    /// <param name="LastActionDate">The date of the last action performed on this key result, as a string.</param>
    /// <param name="Active">Indicates whether the key result is currently active.</param>
    public record KeyResult
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("goal_id")] string GoalId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("unit")] string? Unit,
        [property: JsonPropertyName("creator")] User? CreatorUser,
        [property: JsonPropertyName("date_created")] string DateCreated,
        [property: JsonPropertyName("goal_pretty_id")] string? GoalPrettyId,
        [property: JsonPropertyName("percent_completed")] int? PercentCompleted,
        [property: JsonPropertyName("completed")] bool Completed,
        [property: JsonPropertyName("task_ids")] List<string>? TaskIds,
        [property: JsonPropertyName("list_ids")] List<string>? ListIds,
        [property: JsonPropertyName("subcategory_ids")] List<string>? SubcategoryIds,
        [property: JsonPropertyName("owners")] List<User>? Owners,
        [property: JsonPropertyName("last_action")] LastAction? LastAction,
        [property: JsonPropertyName("steps_current")] object? StepsCurrent,
        [property: JsonPropertyName("steps_start")] object? StepsStart,
        [property: JsonPropertyName("steps_end")] object? StepsEnd,
        [property: JsonPropertyName("steps_taken")] int? StepsTaken,
        [property: JsonPropertyName("history")] List<LastAction>? History,
        [property: JsonPropertyName("last_action_date")] string? LastActionDate,
        [property: JsonPropertyName("active")] bool? Active
    );
}
