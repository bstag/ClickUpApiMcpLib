using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks;

/// <summary>
/// Represents the request body for merging tasks.
/// </summary>
public record MergeTasksRequest
{
    /// <summary>
    /// Array of task IDs to merge into the target task.
    /// </summary>
    [JsonPropertyName("source_task_ids")]
    public List<string> SourceTaskIds { get; init; } = new();
}
