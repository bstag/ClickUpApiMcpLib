using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TaskRelationships
{
    /// <summary>
    /// Represents the request parameters for deleting a task dependency.
    /// All parameters are used in the query string.
    /// </summary>
    public class DeleteDependencyRequest
    {
        /// <summary>
        /// Optional. The unique identifier of the task that the main task currently depends on.
        /// Provide this to remove this specific "depends on" relationship.
        /// </summary>
        [JsonPropertyName("depends_on")]
        public string? DependsOnTaskId { get; set; }

        /// <summary>
        /// Optional. The unique identifier of the task that currently depends on the main task.
        /// Provide this to remove this specific "dependency of" relationship.
        /// </summary>
        [JsonPropertyName("dependency_of")]
        public string? DependencyOfTaskId { get; set; }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteDependencyRequest"/> class.
        /// </summary>
        /// <param name="dependsOnTaskId">The task that the main task depends on.</param>
        /// <param name="dependencyOfTaskId">The task that depends on the main task.</param>
        /// <param name="customTaskIds">Flag for custom task IDs.</param>
        /// <param name="teamId">Team ID if using custom task IDs.</param>
        /// <exception cref="System.ArgumentException">Thrown if both or neither of dependsOnTaskId and dependencyOfTaskId are provided.</exception>
        public DeleteDependencyRequest(string? dependsOnTaskId = null, string? dependencyOfTaskId = null, bool? customTaskIds = null, string? teamId = null)
        {
            if (string.IsNullOrWhiteSpace(dependsOnTaskId) && string.IsNullOrWhiteSpace(dependencyOfTaskId))
            {
                throw new System.ArgumentException("Either DependsOnTaskId or DependencyOfTaskId must be provided for deleting a dependency.");
            }
            if (!string.IsNullOrWhiteSpace(dependsOnTaskId) && !string.IsNullOrWhiteSpace(dependencyOfTaskId))
            {
                throw new System.ArgumentException("Only one of DependsOnTaskId or DependencyOfTaskId can be provided for deleting a dependency.");
            }

            DependsOnTaskId = dependsOnTaskId;
            DependencyOfTaskId = dependencyOfTaskId;
            CustomTaskIds = customTaskIds;
            TeamId = teamId;
        }
    }
}
