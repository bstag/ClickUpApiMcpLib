using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Folders;

/// <summary>
/// Represents the request model for creating a folder.
/// </summary>
public record class CreateFolderRequest
(
    [property: JsonPropertyName("name")]
    string Name
);
