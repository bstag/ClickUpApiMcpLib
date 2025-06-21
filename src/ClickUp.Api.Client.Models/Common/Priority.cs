using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Common
{
    /// <summary>
    /// Represents a priority level in ClickUp (e.g., for tasks or lists).
    /// </summary>
    public class Priority
    {
        /// <summary>
        /// Gets or sets the unique identifier of the priority.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the textual representation of the priority (e.g., "High", "Normal").
        /// </summary>
        [JsonPropertyName("priority")]
        public required string PriorityValue { get; set; }

        /// <summary>
        /// Gets or sets the color associated with the priority.
        /// </summary>
        /// <example>"#FF0000"</example>
        [JsonPropertyName("color")]
        public required string Color { get; set; }

        /// <summary>
        /// Gets or sets the order index of the priority, determining its position in a list.
        /// </summary>
        [JsonPropertyName("orderindex")]
        public required string OrderIndex { get; set; }
    }
}
