using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields; // For base class

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request to set the value of a Date Custom Field.
/// </summary>
public class DateCustomFieldValueRequest : SetCustomFieldValueRequest
{
    /// <summary>
    /// Gets or sets the date value, typically as a Unix timestamp in milliseconds.
    /// </summary>
    [JsonPropertyName("value")]
    public long Value { get; set; }

    /// <summary>
    /// Gets or sets additional options for the date value, such as whether time is included.
    /// </summary>
    [JsonPropertyName("value_options")]
    public CustomFieldValueOptions? ValueOptions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateCustomFieldValueRequest"/> class.
    /// </summary>
    /// <param name="value">The date value (Unix timestamp in milliseconds).</param>
    /// <param name="valueOptions">Optional: Additional options for the date value.</param>
    public DateCustomFieldValueRequest(long value, CustomFieldValueOptions? valueOptions = null)
    {
        Value = value;
        ValueOptions = valueOptions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateCustomFieldValueRequest"/> class.
    /// Required for deserialization.
    /// </summary>
    public DateCustomFieldValueRequest() {}
}
