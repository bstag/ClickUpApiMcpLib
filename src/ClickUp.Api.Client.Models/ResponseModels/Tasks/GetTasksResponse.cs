using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tasks; // For Task entity

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks
{
    public record GetTasksResponse
    (
        [property: JsonPropertyName("tasks")] List<Task> Tasks,
        [property: JsonPropertyName("last_page")] bool? LastPage // For pagination if supported by the specific endpoint
    );
}
