using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Folders;

public class CreateFolderFromTemplateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
