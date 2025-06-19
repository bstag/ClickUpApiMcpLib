using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Goals
{
    public record UpdateGoalRequest
    (
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("due_date")] long? DueDate, // Assuming Unix timestamp in milliseconds
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("rem_owners")] List<int>? RemoveOwners, // List of user IDs to remove
        [property: JsonPropertyName("add_owners")] List<int>? AddOwners, // List of user IDs to add
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("archived")] bool? Archived // To archive/unarchive a goal
    );
}
