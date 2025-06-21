using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Goals
{
    /// <summary>
    /// Represents the request to update an existing Goal.
    /// All properties are optional; only provided properties will be updated.
    /// </summary>
    /// <param name="Name">Optional: New name for the goal.</param>
    /// <param name="DueDate">Optional: New due date for the goal, as a Unix timestamp in milliseconds.</param>
    /// <param name="Description">Optional: New description for the goal.</param>
    /// <param name="RemoveOwners">Optional: A list of user IDs to remove as owners.</param>
    /// <param name="AddOwners">Optional: A list of user IDs to add as owners.</param>
    /// <param name="Color">Optional: New color for the goal.</param>
    /// <param name="Archived">Optional: Set to true to archive the goal, false to unarchive.</param>
    public record UpdateGoalRequest
    (
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("due_date")] long? DueDate,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("rem_owners")] List<int>? RemoveOwners,
        [property: JsonPropertyName("add_owners")] List<int>? AddOwners,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("archived")] bool? Archived
    );
}
