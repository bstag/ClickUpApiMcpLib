using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Goals; // For Goal and GoalFolder entities

namespace ClickUp.Api.Client.Models.ResponseModels.Goals
{
    /// <summary>
    /// Represents the response when retrieving multiple Goals, possibly including their containing folders.
    /// </summary>
    /// <param name="Goals">A list of <see cref="Entities.Goals.Goal"/> objects.</param>
    /// <param name="Folders">A list of <see cref="Entities.Goals.GoalFolder"/> objects that contain some of the retrieved goals.</param>
    public record GetGoalsResponse
    (
        [property: JsonPropertyName("goals")] List<Goal> Goals,
        [property: JsonPropertyName("folders")] List<GoalFolder> Folders
    );
}
