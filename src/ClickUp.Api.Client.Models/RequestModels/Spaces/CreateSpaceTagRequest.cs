using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tags;

namespace ClickUp.Api.Client.Models.RequestModels.Spaces;

/// <summary>
/// Represents the request model for creating a space tag.
/// </summary>
public record class CreateSpaceTagRequest
(
    [property: JsonPropertyName("tag")]
    Tag Tag
);
