using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

public class LocationCustomFieldValueRequest : SetCustomFieldValueRequest
{
    [JsonPropertyName("value")]
    public required LocationValue Value { get; set; } // Assuming LocationValue is a defined type

    public LocationCustomFieldValueRequest(LocationValue value)
    {
        Value = value;
    }
    public LocationCustomFieldValueRequest() { /* Value will be initialized by deserializer or needs default if used directly */ }
}
