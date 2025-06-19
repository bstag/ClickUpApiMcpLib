using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response model for removing a guest from a folder.
/// </summary>
public record class RemoveGuestFromFolderResponse
(
    [property: JsonPropertyName("guest")]
    EditGuestOnWorkspaceResponseGuest Guest
);
