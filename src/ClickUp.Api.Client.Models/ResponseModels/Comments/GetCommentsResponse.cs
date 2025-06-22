using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Comments;

namespace ClickUp.Api.Client.Models.ResponseModels.Comments
{
    /// <summary>
    /// Represents the response when fetching a list of comments.
    /// </summary>
    public class GetCommentsResponse
    {
        /// <summary>
        /// Gets or sets the list of comments.
        /// </summary>
        [JsonPropertyName("comments")]
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
