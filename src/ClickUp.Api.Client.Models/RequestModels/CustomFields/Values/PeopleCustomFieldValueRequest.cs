using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request to set the value of a People (User) Custom Field.
/// This typically involves adding or removing user IDs.
/// </summary>
public class PeopleCustomFieldValueRequest : SetCustomFieldValueRequest
{
    /// <summary>
    /// Gets or sets the action to perform on the people relationship, like adding or removing user IDs.
    /// </summary>
    [JsonPropertyName("value")]
    public required PeopleRelationshipActionValue Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PeopleCustomFieldValueRequest"/> class.
    /// </summary>
    /// <param name="value">The people relationship action value.</param>
    public PeopleCustomFieldValueRequest(PeopleRelationshipActionValue value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PeopleCustomFieldValueRequest"/> class.
    /// Required for deserialization or if 'required' keyword is used on properties.
    /// </summary>
    public PeopleCustomFieldValueRequest() { /* 'Value' will be initialized by deserializer if 'required' or needs a default if used directly without 'required' */ }
}
