using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.CustomFields
{
    public record Tracking
    (
        [property: JsonPropertyName("subtasks")] bool? Subtasks,
        [property: JsonPropertyName("checklists")] bool? Checklists,
        [property: JsonPropertyName("assigned_comments")] bool? AssignedComments,
        [property: JsonPropertyName("all_comments")] bool? AllComments, // Common in some tracking configs
        [property: JsonPropertyName("time_estimates")] bool? TimeEstimates, // Common in some tracking configs
        [property: JsonPropertyName("time_logged")] bool? TimeLogged // Common in some tracking configs
    );
}
