using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request model for setting a text-like custom field value.
/// </summary>
public record class TextLikeCustomFieldValueRequest
(
    [property: JsonPropertyName("value")]
    string Value
);
