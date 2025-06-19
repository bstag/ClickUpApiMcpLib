using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Checklists;

/// <summary>
/// Represents the request to create a checklist.
/// Based on the inline schema `CreateChecklistrequest` from POST /v2/task/{task_id}/checklist.
/// </summary>
public record CreateChecklistRequest
{
    /// <summary>
    /// The name of the checklist to be created.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; }

    public CreateChecklistRequest(string name)
    {
        Name = name;
    }
}
