using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request model for setting a number custom field value.
/// </summary>
public record class NumberCustomFieldValueRequest
(
    [property: JsonPropertyName("value")]
    decimal Value
);
