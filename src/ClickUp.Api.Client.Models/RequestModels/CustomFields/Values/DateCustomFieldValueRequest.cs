using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.Tasks;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request model for setting a date custom field value.
/// </summary>
public record class DateCustomFieldValueRequest
(
    [property: JsonPropertyName("value")]
    long Value,

    [property: JsonPropertyName("value_options")]
    CustomFieldValueOptions? ValueOptions
);
