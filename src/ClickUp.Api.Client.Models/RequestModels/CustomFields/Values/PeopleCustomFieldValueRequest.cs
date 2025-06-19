using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

/// <summary>
/// Represents the request model for setting a people custom field value.
/// </summary>
public record class PeopleCustomFieldValueRequest
(
    [property: JsonPropertyName("value")]
    PeopleRelationshipActionValue Value
);
