using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

public class NumberCustomFieldValueRequest : SetCustomFieldValueRequest
{
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    public NumberCustomFieldValueRequest(decimal value)
    {
        Value = value;
    }
    public NumberCustomFieldValueRequest() {}
}
