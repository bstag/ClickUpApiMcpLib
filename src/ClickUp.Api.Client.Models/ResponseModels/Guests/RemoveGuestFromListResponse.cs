using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response model for removing a guest from a list.
/// </summary>
public record class RemoveGuestFromListResponse
(
    [property: JsonPropertyName("guest")]
    EditGuestOnWorkspaceResponseGuest Guest
);
