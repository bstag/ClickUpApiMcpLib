using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users; // Assuming User model exists here
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Entities.Comments
{
    public class CommentTextEntry
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class Comment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("comment")]
        public List<CommentTextEntry> CommentTextEntries { get; set; }

        [JsonPropertyName("comment_text")]
        public string CommentText { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; } // Assumes User model is in ClickUp.Api.Client.Models.Entities.Users

        [JsonPropertyName("resolved")]
        public bool Resolved { get; set; }

        [JsonPropertyName("assignee")]
        public User Assignee { get; set; } // Assumes User model

        [JsonPropertyName("assigned_by")]
        public User AssignedBy { get; set; } // Assumes User model

        [JsonPropertyName("reactions")]
        public List<object> Reactions { get; set; } // Define a more specific type if reaction structure is known

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("reply_count")]
        public string ReplyCount {get; set; }
    }
}
