using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Folders;

/// <summary>
/// Represents the request model for creating a Folder from a template.
/// </summary>
public class CreateFolderFromTemplateRequest
{
    /// <summary>
    /// Gets or sets the name for the new folder being created from the template.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
