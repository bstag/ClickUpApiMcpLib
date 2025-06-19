using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request model for setting a manual progress custom field value.
/// </summary>
public record class ManualProgressCustomFieldValueRequest
(
    [property: JsonPropertyName("value")]
    ManualProgressValue Value
);
