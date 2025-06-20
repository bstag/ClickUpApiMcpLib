using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Common
{
    public class Priority
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("priority")]
        public string PriorityValue { get; set; } // Renamed to avoid conflict with class name

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("orderindex")]
        public string OrderIndex { get; set; }
    }
}
