using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.CustomFields.Values
{
    public class CustomFieldValueOptions
    {
        [JsonPropertyName("time")]
        public bool Time { get; set; }
    }
}
