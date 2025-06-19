using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.TimeTracking
{
    public record TaskLocation
    (
        [property: JsonPropertyName("list_id")] string ListId, // Changed to string to be more general
        [property: JsonPropertyName("folder_id")] string FolderId, // Changed to string
        [property: JsonPropertyName("space_id")] string SpaceId, // Changed to string
        [property: JsonPropertyName("list_name")] string? ListName,
        [property: JsonPropertyName("folder_name")] string? FolderName,
        [property: JsonPropertyName("space_name")] string? SpaceName,
        [property: JsonPropertyName("list_hidden")] bool? ListHidden, // Added as it's sometimes present
        [property: JsonPropertyName("folder_hidden")] bool? FolderHidden, // Added as it's sometimes present
        [property: JsonPropertyName("space_hidden")] bool? SpaceHidden // Added as it's sometimes present
    );
}
