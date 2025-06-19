using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response model for adding a guest to a task.
/// </summary>
public record class AddGuestToTaskResponse
(
    [property: JsonPropertyName("guest")]
    AddGuestToTaskResponseGuest Guest
);
