using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request to set the value of a CuTask Relationship Custom Field.
/// This typically involves adding or removing task IDs.
/// </summary>
public class TaskRelationshipCustomFieldValueRequest : SetCustomFieldValueRequest
{
    /// <summary>
    /// Gets or sets the action to perform on the task relationship, like adding or removing task IDs.
    /// </summary>
    [JsonPropertyName("value")]
    public required TaskRelationshipActionValue Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskRelationshipCustomFieldValueRequest"/> class.
    /// </summary>
    /// <param name="value">The task relationship action value.</param>
    public TaskRelationshipCustomFieldValueRequest(TaskRelationshipActionValue value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskRelationshipCustomFieldValueRequest"/> class.
    /// Required for deserialization or if 'required' keyword is used on properties.
    /// </summary>
    public TaskRelationshipCustomFieldValueRequest() { /* 'Value' will be initialized by deserializer if 'required' or needs a default if used directly without 'required' */ }
}
