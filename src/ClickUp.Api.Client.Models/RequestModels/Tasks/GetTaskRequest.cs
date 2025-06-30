using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    /// <summary>
    /// Represents the request parameters for getting a single task.
    /// </summary>
    public class GetTaskRequest
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

        /// <summary>
        /// Optional. If true, includes subtasks in the response.
        /// </summary>
        [JsonPropertyName("include_subtasks")]
        public bool? IncludeSubtasks { get; set; }

        /// <summary>
        /// Optional. If true, returns the Task description in Markdown format.
        /// </summary>
        [JsonPropertyName("include_markdown_description")]
        public bool? IncludeMarkdownDescription { get; set; }

        /// <summary>
        /// Optional. Page to retrieve for comments.
        /// </summary>
        [JsonPropertyName("page")]
        public int? Page { get; set; }

        public GetTaskRequest() { }

        public GetTaskRequest(
            bool? customTaskIds = null,
            string? teamId = null,
            bool? includeSubtasks = null,
            bool? includeMarkdownDescription = null,
            int? page = null)
        {
            CustomTaskIds = customTaskIds;
            TeamId = teamId;
            IncludeSubtasks = includeSubtasks;
            IncludeMarkdownDescription = includeMarkdownDescription;
            Page = page;
        }
    }
}
