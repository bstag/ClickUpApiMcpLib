using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Goals; // For Goal and GoalFolder entities

namespace ClickUp.Api.Client.Models.ResponseModels.Goals
{
    public record GetGoalsResponse
    (
        [property: JsonPropertyName("goals")] List<Goal> Goals,
        [property: JsonPropertyName("folders")] List<GoalFolder> Folders
    );
}
