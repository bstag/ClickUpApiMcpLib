using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Common
{
    public class Priority
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("priority")]
        public required string PriorityValue { get; set; } // Renamed to avoid conflict with class name

        [JsonPropertyName("color")]
        public required string Color { get; set; }

        [JsonPropertyName("orderindex")]
        public required string OrderIndex { get; set; }
    }
}
