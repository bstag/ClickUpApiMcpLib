using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tags;

namespace ClickUp.Api.Client.Models.ResponseModels.Spaces;

/// <summary>
/// Represents the response model for getting space tags.
/// </summary>
public record class GetSpaceTagsResponse
(
    [property: JsonPropertyName("tags")]
    List<Tag> Tags
);
