using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response model for editing a guest on a workspace.
/// </summary>
public record class EditGuestOnWorkspaceResponse
(
    [property: JsonPropertyName("guest")]
    EditGuestOnWorkspaceResponseGuest Guest
);
