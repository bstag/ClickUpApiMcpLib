using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Space entity

namespace ClickUp.Api.Client.Models.ResponseModels.Spaces
{
    /// <summary>
    /// Represents the response when retrieving multiple Spaces.
    /// </summary>
    /// <param name="Spaces">A list of <see cref="Entities.Spaces.Space"/> objects.</param>
    public record GetSpacesResponse
    (
        [property: JsonPropertyName("spaces")] List<Space> Spaces
    );
}
