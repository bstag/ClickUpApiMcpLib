using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Goals; // For Goal entity

namespace ClickUp.Api.Client.Models.ResponseModels.Goals
{
    /// <summary>
    /// Represents the response when retrieving a single Goal.
    /// </summary>
    /// <param name="Goal">The retrieved <see cref="Entities.Goals.Goal"/> object.</param>
    public record GetGoalResponse
    (
        [property: JsonPropertyName("goal")] Goal Goal
    );
}
