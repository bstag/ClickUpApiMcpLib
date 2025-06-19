using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Space entity

namespace ClickUp.Api.Client.Models.ResponseModels.Spaces
{
    public record GetSpacesResponse
    (
        [property: JsonPropertyName("spaces")] List<Space> Spaces
    );
}
