using ClickUp.Api.Client.Models.Entities.Users;
using System;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Comments
{
    /// <summary>
    /// Represents a reaction to a comment.
    /// </summary>
    public class ReactionEntry
    {
        /// <summary>
        /// Gets or sets the reaction emoji or string.
        /// </summary>
        [JsonPropertyName("reaction")]
        public required string Reaction { get; set; }

        /// <summary>
        /// Gets or sets the date when the reaction was created.
        /// </summary>
        [JsonPropertyName("date_created")]
        public required DateTimeOffset DateCreated { get; set; } // Assuming ISO 8601 string from API

        /// <summary>
        /// Gets or sets the user who made the reaction.
        /// </summary>
        [JsonPropertyName("user")]
        public required User User { get; set; }
    }
}
