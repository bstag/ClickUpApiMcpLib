using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

public class LabelCustomFieldValueRequest : SetCustomFieldValueRequest
{
    [JsonPropertyName("value")]
    public List<string> Value { get; set; }

    public LabelCustomFieldValueRequest(List<string> value)
    {
        Value = value;
    }
    public LabelCustomFieldValueRequest() { Value = new List<string>(); } // Ensure list is initialized
}
