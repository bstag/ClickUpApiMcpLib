using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask entity

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks
{
    public record GetTasksResponse
    (
        [property: JsonPropertyName("tasks")] List<CuTask> Tasks, // Changed from List<Task>
        [property: JsonPropertyName("last_page")] bool? LastPage
    );
}
