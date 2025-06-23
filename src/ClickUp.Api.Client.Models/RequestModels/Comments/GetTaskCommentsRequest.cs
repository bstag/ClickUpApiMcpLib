using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Comments
{
    /// <summary>
    /// Represents the request parameters for getting task comments.
    /// </summary>
    public class GetTaskCommentsRequest
    {
        /// <summary>
        /// The ID of the task for which to retrieve comments.
        /// </summary>
        [JsonIgnore] // TaskId is part of the URL path, not query params usually for this type of GET
        public string TaskId { get; }

        [JsonPropertyName("custom_task_ids")]
        public bool? CustomTaskIds { get; set; }

        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; } // Required if CustomTaskIds is true

        [JsonPropertyName("start")]
        public long? Start { get; set; } // Unix timestamp

        [JsonPropertyName("start_id")]
        public string? StartId { get; set; } // Comment ID to start after

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTaskCommentsRequest"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        public GetTaskCommentsRequest(string taskId)
        {
            TaskId = taskId;
        }
    }
}
