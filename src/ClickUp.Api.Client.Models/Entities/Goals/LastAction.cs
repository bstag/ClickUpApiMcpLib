using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For User, if userid maps to a User object

namespace ClickUp.Api.Client.Models.Entities.Goals
{
    public record LastAction
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("key_result_id")] string KeyResultId,
        [property: JsonPropertyName("userid")] int UserId, // Assuming int for user ID, could be User object
        [property: JsonPropertyName("user")] User? User, // Optional User object if API provides it
        [property: JsonPropertyName("date_modified")] string DateModified, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("steps_taken")] int? StepsTaken, // Assuming int, based on common usage for "steps"
        [property: JsonPropertyName("note")] string? Note,
        [property: JsonPropertyName("note_html")] string? NoteHtml, // Added as notes often have html versions
        [property: JsonPropertyName("steps_before")] object? StepsBefore, // Type depends on how steps are defined (e.g., int, decimal, string)
        [property: JsonPropertyName("steps_current")] object? StepsCurrent, // Type depends on how steps are defined
        [property: JsonPropertyName("steps_before_float")] double? StepsBeforeFloat, // Added for potential float values
        [property: JsonPropertyName("steps_current_float")] double? StepsCurrentFloat, // Added for potential float values
        [property: JsonPropertyName("steps_before_string")] string? StepsBeforeString, // Added for potential string values
        [property: JsonPropertyName("steps_current_string")] string? StepsCurrentString, // Added for potential string values
        [property: JsonPropertyName("type")] string? Type // Type of last action
    );
}
