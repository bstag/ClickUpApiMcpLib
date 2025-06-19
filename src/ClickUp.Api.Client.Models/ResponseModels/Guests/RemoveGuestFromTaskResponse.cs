using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response model for removing a guest from a task.
/// </summary>
public record class RemoveGuestFromTaskResponse
(
    [property: JsonPropertyName("guest")]
    AddGuestToTaskResponseGuest Guest
);
