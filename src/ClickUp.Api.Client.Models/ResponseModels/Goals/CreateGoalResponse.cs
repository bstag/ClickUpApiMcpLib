using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Goals; // For Goal entity

namespace ClickUp.Api.Client.Models.ResponseModels.Goals
{
    /// <summary>
    /// Represents the response after creating a new Goal.
    /// </summary>
    /// <param name="Goal">The newly created <see cref="Entities.Goals.Goal"/> object.</param>
    public record CreateGoalResponse
    (
        [property: JsonPropertyName("goal")] Goal Goal
    );
}
