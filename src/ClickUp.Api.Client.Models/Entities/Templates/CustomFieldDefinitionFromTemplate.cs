using System.Text.Json;
using System.Text.Json.Serialization;

using ClickUp.Api.Client.Models.Entities.CustomFields; // For CustomFieldTypeConfig

namespace ClickUp.Api.Client.Models.Entities.Templates;

/// <summary>
/// Represents the definition of a Custom Field as part of a Task Template.
/// </summary>
public class CustomFieldDefinitionFromTemplate
{
    /// <summary>
    /// Gets or sets the unique identifier of the custom field.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the custom field.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of the custom field (e.g., "text", "drop_down").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type-specific configuration for this custom field.
    /// </summary>
    [JsonPropertyName("type_config")]
    public CustomFieldTypeConfig? TypeConfig { get; set; }

    /// <summary>
    /// Gets or sets the default or pre-filled value for this custom field in the template.
    /// The structure of this value depends on the custom field type and is captured as a <see cref="JsonElement"/>.
    /// </summary>
    [JsonPropertyName("value")]
    public JsonElement? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this custom field is required when the template is used.
    /// </summary>
    [JsonPropertyName("required")]
    public bool? Required { get; set; }
}
