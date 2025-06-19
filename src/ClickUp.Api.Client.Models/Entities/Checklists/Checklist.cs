using System.Collections.Generic;
using System.Text.Json.Serialization;
// No need to import ChecklistItem explicitly if it's in the same namespace.
// using ClickUp.Api.Client.Models.Entities.Checklists;

namespace ClickUp.Api.Client.Models.Entities.Checklists
{
    public record Checklist
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("task_id")] string TaskId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("orderindex")] int OrderIndex,
        [property: JsonPropertyName("resolved")] int Resolved, // Count of resolved items
        [property: JsonPropertyName("unresolved")] int Unresolved, // Count of unresolved items
        [property: JsonPropertyName("items")] List<ChecklistItem>? Items // Nullable if checklist is empty
    );
}
