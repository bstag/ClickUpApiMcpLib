using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request to set the value of a Location Custom Field.
/// </summary>
public class LocationCustomFieldValueRequest : SetCustomFieldValueRequest
{
    /// <summary>
    /// Gets or sets the location value, including coordinates and formatted address.
    /// </summary>
    [JsonPropertyName("value")]
    public required LocationValue Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationCustomFieldValueRequest"/> class.
    /// </summary>
    /// <param name="value">The location value.</param>
    public LocationCustomFieldValueRequest(LocationValue value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationCustomFieldValueRequest"/> class.
    /// Required for deserialization or if 'required' keyword is used on properties.
    /// </summary>
    public LocationCustomFieldValueRequest() { /* 'Value' will be initialized by deserializer if 'required' or needs a default if used directly without 'required' */ }
}
