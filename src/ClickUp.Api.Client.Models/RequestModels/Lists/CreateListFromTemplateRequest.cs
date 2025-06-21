using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Lists;

/// <summary>
/// Represents the request model for creating a List from a template.
/// </summary>
public class CreateListFromTemplateRequest
{
    /// <summary>
    /// Gets or sets the name for the new list being created from the template.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
