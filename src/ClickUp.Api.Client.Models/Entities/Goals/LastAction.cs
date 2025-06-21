using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User, if userid maps to a User object

namespace ClickUp.Api.Client.Models.Entities.Goals
{
    /// <summary>
    /// Represents the last action or a historical action taken on a Key Result.
    /// </summary>
    /// <param name="Id">The unique identifier of the action.</param>
    /// <param name="KeyResultId">The identifier of the key result this action pertains to.</param>
    /// <param name="UserId">The identifier of the user who performed the action.</param>
    /// <param name="User">The user object for who performed the action, if available.</param>
    /// <param name="DateModified">The date the action was performed, as a string (e.g., Unix timestamp in milliseconds).</param>
    /// <param name="StepsTaken">The number of steps taken or recorded in this action.</param>
    /// <param name="Note">A textual note associated with the action.</param>
    /// <param name="NoteHtml">An HTML version of the note, if available.</param>
    /// <param name="StepsBefore">The value or state of the key result before this action. Type depends on key result type.</param>
    /// <param name="StepsCurrent">The value or state of the key result after this action. Type depends on key result type.</param>
    /// <param name="StepsBeforeFloat">The float value of steps before, if applicable.</param>
    /// <param name="StepsCurrentFloat">The float value of steps current, if applicable.</param>
    /// <param name="StepsBeforeString">The string value of steps before, if applicable.</param>
    /// <param name="StepsCurrentString">The string value of steps current, if applicable.</param>
    /// <param name="Type">The type of action performed.</param>
    public record LastAction
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("key_result_id")] string KeyResultId,
        [property: JsonPropertyName("userid")] int UserId,
        [property: JsonPropertyName("user")] User? User,
        [property: JsonPropertyName("date_modified")] string DateModified,
        [property: JsonPropertyName("steps_taken")] int? StepsTaken,
        [property: JsonPropertyName("note")] string? Note,
        [property: JsonPropertyName("note_html")] string? NoteHtml,
        [property: JsonPropertyName("steps_before")] object? StepsBefore,
        [property: JsonPropertyName("steps_current")] object? StepsCurrent,
        [property: JsonPropertyName("steps_before_float")] double? StepsBeforeFloat,
        [property: JsonPropertyName("steps_current_float")] double? StepsCurrentFloat,
        [property: JsonPropertyName("steps_before_string")] string? StepsBeforeString,
        [property: JsonPropertyName("steps_current_string")] string? StepsCurrentString,
        [property: JsonPropertyName("type")] string? Type
    );
}
