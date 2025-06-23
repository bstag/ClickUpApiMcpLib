using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Common;

/// <summary>
/// Represents the location of a task (List, Folder, Space).
/// Placeholder DTO based on usage in TimeTrackingServiceTests.
/// </summary>
/// <param name="ListId">ID of the List.</param>
/// <param name="ListName">Name of the List.</param>
/// <param name="FolderId">ID of the Folder.</param>
/// <param name="FolderName">Name of the Folder.</param>
/// <param name="SpaceId">ID of the Space.</param>
/// <param name="SpaceName">Name of the Space.</param>
/// <param name="ListHidden">Indicates if the List is hidden.</param>
/// <param name="FolderHidden">Indicates if the Folder is hidden.</param>
/// <param name="SpaceHidden">Indicates if the Space is hidden.</param>
public record TaskLocation(
    [property: JsonPropertyName("list_id")] string ListId,
    [property: JsonPropertyName("list_name")] string ListName,
    [property: JsonPropertyName("folder_id")] string FolderId,
    [property: JsonPropertyName("folder_name")] string FolderName,
    [property: JsonPropertyName("space_id")] string SpaceId,
    [property: JsonPropertyName("space_name")] string SpaceName,
    [property: JsonPropertyName("list_hidden")] bool? ListHidden = null,
    [property: JsonPropertyName("folder_hidden")] bool? FolderHidden = null,
    [property: JsonPropertyName("space_hidden")] bool? SpaceHidden = null
);
