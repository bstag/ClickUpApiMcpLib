using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Folders; // For Folder type

namespace ClickUp.Api.Client.Models.Entities.Lists
{
    /// <summary>
    /// Represents a List in ClickUp, which is a container for tasks.
    /// Note: This is distinct from <see cref="ClickUpList"/> which appears to be a more detailed model for a List.
    /// This model might be used in contexts where only basic list information is needed.
    /// </summary>
    /// <param name="Id">The unique identifier of the list.</param>
    /// <param name="Name">The name of the list.</param>
    /// <param name="Folder">The parent folder of the list, if any. Null for folderless lists.</param>
    /// <param name="Priority">The priority information for the list itself.</param>
    public record List
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("folder")] Folder? Folder,
        [property: JsonPropertyName("priority")] ListPriorityInfo? Priority
    );
}
