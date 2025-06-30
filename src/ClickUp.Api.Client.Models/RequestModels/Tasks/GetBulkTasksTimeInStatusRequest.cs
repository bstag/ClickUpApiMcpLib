using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    /// <summary>
    /// Represents the request parameters for getting time in status for multiple tasks.
    /// </summary>
    public class GetBulkTasksTimeInStatusRequest
    {
        /// <summary>
        /// A collection of Task IDs for which to retrieve time-in-status data.
        /// </summary>
        [JsonPropertyName("task_ids")] // This will be converted to comma-separated string for query
        public IEnumerable<string> TaskIds { get; }

        /// <summary>
        /// Optional. If set to true, the task IDs are treated as custom task IDs.
        /// </summary>
        [JsonPropertyName("custom_task_ids")]
        public bool? CustomTaskIds { get; set; }

        /// <summary>
        /// Optional. The Workspace ID (formerly team_id). Required if CustomTaskIds is true.
        /// </summary>
        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; }

        public GetBulkTasksTimeInStatusRequest(IEnumerable<string> taskIds)
        {
            TaskIds = taskIds ?? throw new System.ArgumentNullException(nameof(taskIds));
        }

        public GetBulkTasksTimeInStatusRequest(
            IEnumerable<string> taskIds,
            bool? customTaskIds = null,
            string? teamId = null) : this(taskIds)
        {
            CustomTaskIds = customTaskIds;
            TeamId = teamId;
        }
    }
}
