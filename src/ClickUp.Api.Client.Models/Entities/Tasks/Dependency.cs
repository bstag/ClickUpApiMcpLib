using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Tasks
{
    /// <summary>
    /// Represents a dependency relationship between tasks.
    /// </summary>
    /// <param name="TaskId">The identifier of the task that has a dependency.</param>
    /// <param name="DependsOn">The identifier of the task that <see cref="TaskId"/> depends on.</param>
    /// <param name="TypeOfDependency">The type of dependency (e.g., "waiting_on", "blocking").</param>
    public record Dependency
    (
        [property: JsonPropertyName("task_id")] string TaskId,
        [property: JsonPropertyName("depends_on")] string DependsOn,
        [property: JsonPropertyName("type_of_dependency")] string TypeOfDependency
    );
}
