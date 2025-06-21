using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request to set the value of a Label Custom Field.
/// </summary>
public class LabelCustomFieldValueRequest : SetCustomFieldValueRequest
{
    /// <summary>
    /// Gets or sets the list of label names or IDs to apply.
    /// The API might expect label names or specific label option IDs.
    /// </summary>
    [JsonPropertyName("value")]
    public List<string> Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelCustomFieldValueRequest"/> class.
    /// </summary>
    /// <param name="value">The list of label names or IDs.</param>
    public LabelCustomFieldValueRequest(List<string> value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelCustomFieldValueRequest"/> class with an empty list of values.
    /// Required for deserialization or default instantiation.
    /// </summary>
    public LabelCustomFieldValueRequest() { Value = new List<string>(); }
}
