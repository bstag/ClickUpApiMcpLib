using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request model for setting a label custom field value.
/// </summary>
public record class LabelCustomFieldValueRequest
(
    [property: JsonPropertyName("value")]
    List<string> Value
);
