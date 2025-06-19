using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Goals
{
    public record CreateGoalRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("due_date")] long DueDate, // Assuming Unix timestamp in milliseconds
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("multiple_owners")] bool MultipleOwners,
        [property: JsonPropertyName("owners")] List<int> Owners, // List of user IDs
        [property: JsonPropertyName("color")] string? Color, // Optional
        [property: JsonPropertyName("team_id")] string TeamId, // Workspace/Team ID
        [property: JsonPropertyName("folder_id")] string? FolderId // Optional: to place goal in a specific folder
    );
}
