using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tasks;

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks;

/// <summary>
/// Represents the response model for adding a task link.
/// </summary>
public record class AddTaskLinkResponse
(
    [property: JsonPropertyName("task")]
    Task Task
);
