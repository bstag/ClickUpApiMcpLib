using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TaskRelationships
{
    /// <summary>
    /// Request model for adding a task dependency.
    /// </summary>
    public class AddDependencyRequest
    {
        /// <summary>
        /// Gets or sets the ID of the task that the main task will depend on.
        /// </summary>
        [JsonPropertyName("depends_on")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DependsOn { get; set; }

        /// <summary>
        /// Gets or sets the ID of the task that will depend on the main task.
        /// </summary>
        [JsonPropertyName("dependency_of")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DependencyOf { get; set; }

        public AddDependencyRequest(string? dependsOn, string? dependencyOf)
        {
            DependsOn = dependsOn;
            DependencyOf = dependencyOf;
        }
    }
}
