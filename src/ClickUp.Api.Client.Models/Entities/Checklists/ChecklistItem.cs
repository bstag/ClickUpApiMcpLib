using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.Checklists
{
    /// <summary>
    /// Represents an individual item within a checklist.
    /// </summary>
    /// <param name="Id">The unique identifier of the checklist item.</param>
    /// <param name="Name">The name or content of the checklist item.</param>
    /// <param name="OrderIndex">The order index of the item within its checklist (or parent item).</param>
    /// <param name="Assignee">The user to whom this checklist item is assigned, if any.</param>
    /// <param name="Resolved">Indicates whether the checklist item has been resolved (completed).</param>
    /// <param name="Parent">The identifier of the parent checklist item if this is a sub-item. Null for top-level items.</param>
    /// <param name="DateCreated">The date when the checklist item was created, as a string (e.g., Unix timestamp in milliseconds).</param>
    /// <param name="Children">A list of child checklist items (sub-items). Null if no children or not expanded.</param>
    public record ChecklistItem
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("orderindex")] int OrderIndex,
        [property: JsonPropertyName("assignee")] User? Assignee,
        [property: JsonPropertyName("resolved")] bool Resolved,
        [property: JsonPropertyName("parent")] string? Parent,
        [property: JsonPropertyName("date_created")] string? DateCreated,
        [property: JsonPropertyName("children")] List<ChecklistItem>? Children
    );
}
