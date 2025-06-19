using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response model for adding a guest to a folder.
/// </summary>
public record class AddGuestToFolderResponse
(
    [property: JsonPropertyName("guest")]
    AddGuestToFolderResponseGuest Guest
);
