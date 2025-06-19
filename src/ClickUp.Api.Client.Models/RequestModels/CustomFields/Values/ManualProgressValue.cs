using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the value for a manual progress custom field.
/// </summary>
public record class ManualProgressValue
(
    [property: JsonPropertyName("current")]
    decimal Current
);
