using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Guests;

/// <summary>
/// Represents the request model for adding a guest to a task.
/// </summary>
public record class AddGuestToTaskRequest
(
    [property: JsonPropertyName("permission_level")]
    string PermissionLevel
);
