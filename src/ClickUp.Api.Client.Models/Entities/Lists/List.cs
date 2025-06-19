using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Folders; // For Folder type

namespace ClickUp.Api.Client.Models.Entities.Lists
{
    public record List
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("folder")] Folder? Folder, // Nullable Folder
        [property: JsonPropertyName("priority")] ListPriorityInfo? Priority // Using ListPriorityInfo
        // Add other properties based on OpenAPI spec as needed
    );
}
