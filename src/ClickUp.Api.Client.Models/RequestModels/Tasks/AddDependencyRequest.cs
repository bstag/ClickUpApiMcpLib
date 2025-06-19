using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks;

/// <summary>
/// Represents the request model for adding a task dependency.
/// </summary>
public record class AddDependencyRequest
(
    [property: JsonPropertyName("depends_on")]
    string? DependsOn,

    [property: JsonPropertyName("dependency_of")]
    string? DependencyOf
);
