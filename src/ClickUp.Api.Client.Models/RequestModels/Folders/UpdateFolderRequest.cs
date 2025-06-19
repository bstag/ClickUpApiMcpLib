using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Folders;

/// <summary>
/// Represents the request model for updating a folder.
/// </summary>
public record class UpdateFolderRequest
(
    [property: JsonPropertyName("name")]
    string Name
);
