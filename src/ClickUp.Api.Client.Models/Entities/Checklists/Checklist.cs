using System.Collections.Generic;
using System.Text.Json.Serialization;
// No need to import ChecklistItem explicitly if it's in the same namespace.
// using ClickUp.Api.Client.Models.Entities.Checklists;

namespace ClickUp.Api.Client.Models.Entities.Checklists
{
    /// <summary>
    /// Represents a checklist associated with a task in ClickUp.
    /// </summary>
    /// <param name="Id">The unique identifier of the checklist.</param>
    /// <param name="TaskId">The identifier of the task this checklist belongs to.</param>
    /// <param name="Name">The name of the checklist.</param>
    /// <param name="OrderIndex">The order index of the checklist within the task.</param>
    /// <param name="Resolved">The number of resolved (completed) items in the checklist.</param>
    /// <param name="Unresolved">The number of unresolved (incomplete) items in the checklist.</param>
    /// <param name="Items">A list of items within this checklist. Can be null if the checklist is empty or items are not expanded.</param>
    public record Checklist
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("task_id")] string TaskId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("orderindex")] int OrderIndex,
        [property: JsonPropertyName("resolved")] int Resolved,
        [property: JsonPropertyName("unresolved")] int Unresolved,
        [property: JsonPropertyName("items")] List<ChecklistItem>? Items
    );
}
