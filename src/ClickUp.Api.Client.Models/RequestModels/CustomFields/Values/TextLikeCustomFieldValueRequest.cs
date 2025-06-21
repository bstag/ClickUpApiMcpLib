using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request to set the value of a text-like Custom Field (e.g., Text, Short Text, URL, Email, Phone).
/// </summary>
public class TextLikeCustomFieldValueRequest : SetCustomFieldValueRequest
{
    /// <summary>
    /// Gets or sets the string value for the custom field.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextLikeCustomFieldValueRequest"/> class.
    /// </summary>
    /// <param name="value">The string value to set.</param>
    public TextLikeCustomFieldValueRequest(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextLikeCustomFieldValueRequest"/> class with an empty string value.
    /// Required for deserialization or default instantiation.
    /// </summary>
    public TextLikeCustomFieldValueRequest() { Value = string.Empty; }
}
