using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the value for a location custom field.
/// </summary>
public record class LocationValue
(
    [property: JsonPropertyName("location")]
    LatLong Location,

    [property: JsonPropertyName("formatted_address")]
    string FormattedAddress
);
