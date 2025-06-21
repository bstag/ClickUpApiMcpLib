using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request to set the value of a Number or Currency Custom Field.
/// </summary>
public class NumberCustomFieldValueRequest : SetCustomFieldValueRequest
{
    /// <summary>
    /// Gets or sets the numeric value.
    /// </summary>
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberCustomFieldValueRequest"/> class.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    public NumberCustomFieldValueRequest(decimal value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberCustomFieldValueRequest"/> class.
    /// Required for deserialization.
    /// </summary>
    public NumberCustomFieldValueRequest() {}
}
