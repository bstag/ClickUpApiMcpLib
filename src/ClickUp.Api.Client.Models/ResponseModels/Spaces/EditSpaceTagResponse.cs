using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.Spaces; // Assuming EditSpaceTagInfo is in RequestModels

namespace ClickUp.Api.Client.Models.ResponseModels.Spaces;

/// <summary>
/// Represents the response model for editing a space tag.
/// </summary>
public record class EditSpaceTagResponse
(
    [property: JsonPropertyName("tag")]
    EditSpaceTagInfo Tag
);
