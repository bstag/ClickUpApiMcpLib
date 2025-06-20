using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

/// <summary>
/// Represents the response model for getting tasks in a view.
/// </summary>
public record class GetViewTasksResponse
(
    [property: JsonPropertyName("tasks")]
    List<CuTask> Tasks, // Changed from List<Task>

    [property: JsonPropertyName("last_page")]
    bool LastPage
);
