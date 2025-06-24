using ClickUp.Api.Client.Models.Entities.Users;
using System.Collections.Generic;
using System.Text.Json.Serialization; // Ensured it's present once

namespace ClickUp.Api.Client.Models.Entities.Comments
{
    /// <summary>
    /// Represents a single text entry within a comment.
    /// Comments can be composed of multiple such entries.
    /// </summary>
    public class CommentTextEntry
    {
        /// <summary>
        /// Gets or sets the text content of this entry.
        /// </summary>
        [JsonPropertyName("text")]
        public required string Text { get; set; }
    }

    /// <summary>
    /// Represents a comment in ClickUp.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Gets or sets the unique identifier of the comment.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        /// <summary>
        /// Optional: Historical identifier for the comment, if provided by the API.
        /// </summary>
        [JsonPropertyName("hist_id")]
        public string? HistoryId { get; set; }

        /// <summary>
        /// Gets or sets the list of text entries that make up the comment's content.
        /// </summary>
        [JsonPropertyName("comment")]
        public required List<CommentTextEntry> CommentTextEntries { get; set; }

        /// <summary>
        /// Gets or sets the plain text representation of the comment content.
        /// </summary>
        [JsonPropertyName("comment_text")]
        public required string CommentText { get; set; }

        /// <summary>
        /// Gets or sets the user who posted the comment.
        /// </summary>
        [JsonPropertyName("user")]
        public required User User { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the comment has been resolved.
        /// </summary>
        [JsonPropertyName("resolved")]
        public bool Resolved { get; set; } // Assuming false by default if not present

        /// <summary>
        /// Gets or sets the user to whom the comment is assigned, if any.
        /// </summary>
        [JsonPropertyName("assignee")]
        public User? Assignee { get; set; }

        /// <summary>
        /// Gets or sets the user who assigned the comment, if any.
        /// </summary>
        [JsonPropertyName("assigned_by")]
        public User? AssignedBy { get; set; }

        /// <summary>
        /// Gets or sets the list of reactions to the comment.
        /// </summary>
        [JsonPropertyName("reactions")]
        public List<ReactionEntry> Reactions { get; set; } = new List<ReactionEntry>(); // Initialize to avoid null

        /// <summary>
        /// Gets or sets the date the comment was posted.
        /// The API specifies this as a string (Unix timestamp in milliseconds) but sometimes sends a number.
        /// </summary>
        [JsonPropertyName("date")]
        [JsonConverter(typeof(ClickUp.Api.Client.Models.Serialization.Converters.JsonStringOrNumberConverter))]
        public required string Date { get; set; }

        /// <summary>
        /// Gets or sets the number of replies to this comment.
        /// The API specifies this as a string but sometimes sends a number.
        /// </summary>
        [JsonPropertyName("reply_count")]
        [JsonConverter(typeof(ClickUp.Api.Client.Models.Serialization.Converters.JsonStringOrNumberConverter))]
        public required string ReplyCount {get; set; }

        /// <summary>
        /// Optional: Source of the comment, if provided by the API (e.g., "task_comment").
        /// </summary>
        [JsonPropertyName("source")]
        public string? Source { get; set; }
    }
}
