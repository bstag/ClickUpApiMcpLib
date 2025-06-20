using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.RequestModels.CustomFields; // For base class

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values;

public class DateCustomFieldValueRequest : SetCustomFieldValueRequest
{
    [JsonPropertyName("value")]
    public long Value { get; set; }

    [JsonPropertyName("value_options")]
    public CustomFieldValueOptions? ValueOptions { get; set; }

    // Constructor to match the previous record's primary constructor style
    public DateCustomFieldValueRequest(long value, CustomFieldValueOptions? valueOptions = null)
    {
        Value = value;
        ValueOptions = valueOptions;
    }

    // Parameterless constructor often needed for deserialization
    public DateCustomFieldValueRequest() {}
}
