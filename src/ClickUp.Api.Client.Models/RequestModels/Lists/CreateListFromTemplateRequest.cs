using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Lists;

public class CreateListFromTemplateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
