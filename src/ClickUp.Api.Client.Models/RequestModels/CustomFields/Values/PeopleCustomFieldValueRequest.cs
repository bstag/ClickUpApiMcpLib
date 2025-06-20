using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

public class PeopleCustomFieldValueRequest : SetCustomFieldValueRequest
{
    [JsonPropertyName("value")]
    public PeopleRelationshipActionValue Value { get; set; } // Assuming PeopleRelationshipActionValue is a defined type

    public PeopleCustomFieldValueRequest(PeopleRelationshipActionValue value)
    {
        Value = value;
    }
    public PeopleCustomFieldValueRequest() { /* Value will be initialized by deserializer or needs default if used directly */ }
}
