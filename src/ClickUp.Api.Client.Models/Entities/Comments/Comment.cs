using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users; // Assuming User model exists here
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Entities.Comments
{
    public class CommentTextEntry
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; }
    }

    public class Comment
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("comment")]
        public required List<CommentTextEntry> CommentTextEntries { get; set; }

        [JsonPropertyName("comment_text")]
        public required string CommentText { get; set; }

        [JsonPropertyName("user")]
        public required User User { get; set; } // Assumes User model is in ClickUp.Api.Client.Models.Entities.Users

        [JsonPropertyName("resolved")]
        public required bool Resolved { get; set; }

        [JsonPropertyName("assignee")]
        public User? Assignee { get; set; } // Assumes User model, can be null

        [JsonPropertyName("assigned_by")]
        public User? AssignedBy { get; set; } // Assumes User model, can be null

        [JsonPropertyName("reactions")]
        public required List<object> Reactions { get; set; } // Define a more specific type if reaction structure is known

        [JsonPropertyName("date")]
        public required string Date { get; set; }

        [JsonPropertyName("reply_count")]
        public required string ReplyCount {get; set; } // Consider changing to int if appropriate for API
    }
}
