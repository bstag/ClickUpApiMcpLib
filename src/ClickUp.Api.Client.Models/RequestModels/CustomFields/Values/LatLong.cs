using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents a latitude and longitude coordinate.
/// </summary>
public record class LatLong
(
    [property: JsonPropertyName("lat")]
    double Lat,

    [property: JsonPropertyName("lng")]
    double Lng
);
