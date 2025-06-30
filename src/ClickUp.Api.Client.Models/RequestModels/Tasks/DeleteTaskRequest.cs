using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    /// <summary>
    /// Represents the request parameters for deleting a task.
    /// </summary>
    public class DeleteTaskRequest
    {
        /// <summary>
        /// Optional. If set to true, the taskId is treated as a custom task ID.
        /// </summary>
        [JsonPropertyName("custom_task_ids")]
        public bool? CustomTaskIds { get; set; }

        /// <summary>
        /// Optional. The Workspace ID (formerly team_id). Required if CustomTaskIds is true.
        /// </summary>
        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; }

        public DeleteTaskRequest() { }

        public DeleteTaskRequest(bool? customTaskIds = null, string? teamId = null)
        {
            CustomTaskIds = customTaskIds;
            TeamId = teamId;
        }
    }
}
