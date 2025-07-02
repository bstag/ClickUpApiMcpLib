using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Sharing;

/// <summary>
/// Represents the response model for shared hierarchy information.
/// </summary>
public record class SharedHierarchyResponse
(
    [property: JsonPropertyName("shared")]
    SharedHierarchyDetailsResponse Shared
);
