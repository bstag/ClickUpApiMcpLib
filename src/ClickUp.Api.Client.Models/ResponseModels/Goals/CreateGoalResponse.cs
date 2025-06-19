using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Goals; // For Goal entity

namespace ClickUp.Api.Client.Models.ResponseModels.Goals
{
    public record CreateGoalResponse
    (
        [property: JsonPropertyName("goal")] Goal Goal
    );
}
