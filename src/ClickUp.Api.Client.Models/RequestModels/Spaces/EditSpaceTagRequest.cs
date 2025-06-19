using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Spaces;

/// <summary>
/// Represents the request model for editing a space tag.
/// </summary>
public record class EditSpaceTagRequest
(
    [property: JsonPropertyName("tag")]
    EditSpaceTagInfo Tag
);
