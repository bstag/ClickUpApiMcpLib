using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.TimeTracking
{
    /// <summary>
    /// Represents the location (List, Folder, Space) of a task associated with a time entry.
    /// </summary>
    /// <param name="ListId">The identifier of the list.</param>
    /// <param name="FolderId">The identifier of the folder.</param>
    /// <param name="SpaceId">The identifier of the space.</param>
    /// <param name="ListName">The name of the list.</param>
    /// <param name="FolderName">The name of the folder.</param>
    /// <param name="SpaceName">The name of the space.</param>
    /// <param name="ListHidden">Indicates if the list is hidden.</param>
    /// <param name="FolderHidden">Indicates if the folder is hidden.</param>
    /// <param name="SpaceHidden">Indicates if the space is hidden.</param>
    public record TaskLocation
    (
        [property: JsonPropertyName("list_id")] string ListId,
        [property: JsonPropertyName("folder_id")] string FolderId,
        [property: JsonPropertyName("space_id")] string SpaceId,
        [property: JsonPropertyName("list_name")] string? ListName,
        [property: JsonPropertyName("folder_name")] string? FolderName,
        [property: JsonPropertyName("space_name")] string? SpaceName,
        [property: JsonPropertyName("list_hidden")] bool? ListHidden,
        [property: JsonPropertyName("folder_hidden")] bool? FolderHidden,
        [property: JsonPropertyName("space_hidden")] bool? SpaceHidden
    );
}
