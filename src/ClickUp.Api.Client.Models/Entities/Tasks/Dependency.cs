using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Tasks
{
    public record Dependency
    (
        [property: JsonPropertyName("task_id")] string TaskId,
        [property: JsonPropertyName("depends_on")] string DependsOn,
        [property: JsonPropertyName("type_of_dependency")] string TypeOfDependency // e.g., "waiting_on", "blocking"
    );
}
