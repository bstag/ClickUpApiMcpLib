using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Goals
{
    /// <summary>
    /// Represents the request to create a new Goal.
    /// </summary>
    /// <param name="Name">The name of the goal.</param>
    /// <param name="DueDate">The due date of the goal, as a Unix timestamp in milliseconds.</param>
    /// <param name="Description">The description of the goal.</param>
    /// <param name="MultipleOwners">Indicates whether the goal can have multiple owners.</param>
    /// <param name="Owners">A list of user IDs to be assigned as owners of the goal.</param>
    /// <param name="Color">Optional: The color associated with the goal.</param>
    /// <param name="TeamId">The ID of the workspace (team) where the goal will be created.</param>
    /// <param name="FolderId">Optional: The ID of a goal folder to place this goal into.</param>
    public record CreateGoalRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("due_date")] long DueDate,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("multiple_owners")] bool MultipleOwners,
        [property: JsonPropertyName("owners")] List<int> Owners,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("team_id")] string TeamId,
        [property: JsonPropertyName("folder_id")] string? FolderId
    );
}
