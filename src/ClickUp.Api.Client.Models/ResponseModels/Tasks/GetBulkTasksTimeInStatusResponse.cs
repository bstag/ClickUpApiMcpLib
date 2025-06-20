using System.Collections.Generic;
// No specific JsonPropertyName for the dictionary itself, as it's the root object with dynamic keys.

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks;

/// <summary>
/// Represents the response for getting time in status for multiple tasks.
/// This is a dictionary where the key is the task ID and the value is the TaskTimeInStatusResponse for that task.
/// </summary>
public class GetBulkTasksTimeInStatusResponse : Dictionary<string, TaskTimeInStatusResponse>
{
    // Inherits Dictionary functionality. No additional members needed for now.
    // Deserialization should handle populating this dictionary based on the JSON structure.
}
