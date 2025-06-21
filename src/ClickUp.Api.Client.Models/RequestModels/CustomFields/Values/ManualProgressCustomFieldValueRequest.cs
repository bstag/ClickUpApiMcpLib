using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request to set the value of a Manual Progress Custom Field.
/// </summary>
public class ManualProgressCustomFieldValueRequest : SetCustomFieldValueRequest
{
    /// <summary>
    /// Gets or sets the current progress value.
    /// </summary>
    [JsonPropertyName("value")]
    public required ManualProgressValue Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualProgressCustomFieldValueRequest"/> class.
    /// </summary>
    /// <param name="value">The manual progress value.</param>
    public ManualProgressCustomFieldValueRequest(ManualProgressValue value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualProgressCustomFieldValueRequest"/> class.
    /// Required for deserialization or if 'required' keyword is used on properties.
    /// </summary>
    public ManualProgressCustomFieldValueRequest() { /* 'Value' will be initialized by deserializer if 'required' or needs a default if used directly without 'required' */ }
}
