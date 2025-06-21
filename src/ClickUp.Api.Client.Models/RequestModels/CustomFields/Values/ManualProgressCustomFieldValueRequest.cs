using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

public class ManualProgressCustomFieldValueRequest : SetCustomFieldValueRequest
{
    [JsonPropertyName("value")]
    public required ManualProgressValue Value { get; set; } // Assuming ManualProgressValue is a defined type

    public ManualProgressCustomFieldValueRequest(ManualProgressValue value)
    {
        Value = value;
    }
    public ManualProgressCustomFieldValueRequest() { /* Value will be initialized by deserializer or needs default if used directly */ }
}
