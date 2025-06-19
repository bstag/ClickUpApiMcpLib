using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For Status type

namespace ClickUp.Api.Client.Models.Entities.Folders
{
    public record Folder
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("archived")] bool Archived,
        [property: JsonPropertyName("statuses")] List<Status>? Statuses
        // Add other properties based on OpenAPI spec as needed
    );
}
