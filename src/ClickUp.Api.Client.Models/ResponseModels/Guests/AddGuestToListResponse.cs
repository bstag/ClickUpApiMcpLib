using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response model for adding a guest to a list.
/// </summary>
public record class AddGuestToListResponse
(
    [property: JsonPropertyName("guest")]
    AddGuestToListResponseGuest Guest
);
