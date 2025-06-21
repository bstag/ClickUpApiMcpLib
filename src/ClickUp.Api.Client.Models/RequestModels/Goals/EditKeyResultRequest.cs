using System.Text.Json.Serialization;
using System.Collections.Generic; // For List

namespace ClickUp.Api.Client.Models.RequestModels.Goals
{
    /// <summary>
    /// Represents the request to edit an existing Key Result.
    /// All properties are optional; only provided properties will be updated.
    /// </summary>
    /// <param name="StepsCurrent">Optional: The new current progress or value of the key result. Type depends on the key result type.</param>
    /// <param name="Note">Optional: A new note for the key result.</param>
    /// <param name="Name">Optional: New name for the key result.</param>
    /// <param name="Owners">Optional: A list of user IDs to set as the owners. This will replace existing owners.</param>
    /// <param name="AddOwners">Optional: A list of user IDs to add as owners.</param>
    /// <param name="RemoveOwners">Optional: A list of user IDs to remove as owners.</param>
    /// <param name="TaskIds">Optional: New list of task IDs to associate with this key result (for "task" type key results).</param>
    /// <param name="ListIds">Optional: New list of list IDs to associate with this key result (for "list" type key results, check API support).</param>
    /// <param name="Archived">Optional: Set to true to archive the key result, false to unarchive.</param>
    public record EditKeyResultRequest
    (
        [property: JsonPropertyName("steps_current")] object? StepsCurrent,
        [property: JsonPropertyName("note")] string? Note,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("owners")] List<int>? Owners,
        [property: JsonPropertyName("add_owners")] List<int>? AddOwners,
        [property: JsonPropertyName("rem_owners")] List<int>? RemoveOwners,
        [property: JsonPropertyName("task_ids")] List<string>? TaskIds,
        [property: JsonPropertyName("list_ids")] List<string>? ListIds,
        [property: JsonPropertyName("archived")] bool? Archived
    );
}
