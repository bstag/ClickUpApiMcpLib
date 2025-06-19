using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks;

/// <summary>
/// Represents the request model for creating a task from a template.
/// </summary>
public record class CreateTaskFromTemplateRequest
(
    [property: JsonPropertyName("name")]
    string Name
);
