using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request model for setting a location custom field value.
/// </summary>
public record class LocationCustomFieldValueRequest
(
    [property: JsonPropertyName("value")]
    LocationValue Value
);
