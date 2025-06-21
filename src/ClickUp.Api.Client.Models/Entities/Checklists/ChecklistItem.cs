using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.Checklists
{
    public record ChecklistItem
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("orderindex")] int OrderIndex,
        [property: JsonPropertyName("assignee")] User? Assignee,
        [property: JsonPropertyName("resolved")] bool Resolved,
        [property: JsonPropertyName("parent")] string? Parent, // Nullable if it's a top-level item
        [property: JsonPropertyName("date_created")] string? DateCreated, // Assuming string, OpenAPI spec might specify DateTimeOffset
        [property: JsonPropertyName("children")] List<ChecklistItem>? Children // Nullable if no sub-items
    );
}
