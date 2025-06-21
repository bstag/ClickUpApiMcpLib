using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users; // Assuming User model exists here
using System.Collections.Generic;

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
        public required bool Resolved { get; set; }

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
        /// The structure of objects in this list might need a more specific type (e.g., a Reaction model).
        /// </summary>
        [JsonPropertyName("reactions")]
        public required List<object> Reactions { get; set; }

        /// <summary>
        /// Gets or sets the date the comment was posted, as a string (e.g., Unix timestamp in milliseconds).
        /// </summary>
        [JsonPropertyName("date")]
        public required string Date { get; set; }

        /// <summary>
        /// Gets or sets the number of replies to this comment, as a string.
        /// Consider changing to int if the API consistently provides a numerical value.
        /// </summary>
        [JsonPropertyName("reply_count")]
        public required string ReplyCount {get; set; }
    }
}
