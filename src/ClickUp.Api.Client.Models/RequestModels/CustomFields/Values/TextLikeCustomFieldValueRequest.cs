using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

public class TextLikeCustomFieldValueRequest : SetCustomFieldValueRequest
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    public TextLikeCustomFieldValueRequest(string value)
    {
        Value = value;
    }
    public TextLikeCustomFieldValueRequest() { Value = string.Empty; } // Initialize to empty string
}
