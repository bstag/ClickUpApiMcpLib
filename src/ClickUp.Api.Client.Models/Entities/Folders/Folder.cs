using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For Status type

namespace ClickUp.Api.Client.Models.Entities.Folders
{
    /// <summary>
    /// Represents a Folder in ClickUp, which can contain Lists.
    /// </summary>
    /// <param name="Id">The unique identifier of the folder.</param>
    /// <param name="Name">The name of the folder.</param>
    /// <param name="Archived">Indicates whether the folder is archived.</param>
    /// <param name="Statuses">A list of statuses available within this folder. This may be null if not requested or if the folder inherits statuses from its parent Space.</param>
    public record Folder
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("archived")] bool Archived,
        [property: JsonPropertyName("statuses")] List<Status>? Statuses
    );
}
