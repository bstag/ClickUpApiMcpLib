using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TaskRelationships
{
    /// <summary>
    /// Represents the request parameters for adding a task link.
    /// All parameters are used in the query string.
    /// </summary>
    public class AddTaskLinkRequest
    {
        /// <summary>
        /// Optional. If set to true, all task IDs provided are treated as custom task IDs.
        /// </summary>
        [JsonPropertyName("custom_task_ids")]
        public bool? CustomTaskIds { get; set; }

        /// <summary>
        /// Optional. The Workspace ID (formerly team_id). Required if CustomTaskIds is true.
        /// </summary>
        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; }

        public AddTaskLinkRequest(bool? customTaskIds = null, string? teamId = null)
        {
            CustomTaskIds = customTaskIds;
            TeamId = teamId;
        }
    }
}
