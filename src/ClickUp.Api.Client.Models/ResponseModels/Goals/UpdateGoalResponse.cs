using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Goals; // For Goal entity

namespace ClickUp.Api.Client.Models.ResponseModels.Goals
{
    /// <summary>
    /// Represents the response after updating an existing Goal.
    /// </summary>
    /// <param name="Goal">The updated <see cref="Entities.Goals.Goal"/> object.</param>
    public record UpdateGoalResponse
    (
        [property: JsonPropertyName("goal")] Goal Goal
    );
}
