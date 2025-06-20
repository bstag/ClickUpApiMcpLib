using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.CustomFields; // For CustomFieldTypeConfig

namespace ClickUp.Api.Client.Models.Entities.Templates;

public class CustomFieldDefinitionFromTemplate
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("type_config")]
    public CustomFieldTypeConfig? TypeConfig { get; set; }

    // Using JsonElement to capture potentially varied structures for 'value'
    // Deserialization might require custom converters if specific types are needed later.
    [JsonPropertyName("value")]
    public JsonElement? Value { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }
}
