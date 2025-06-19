using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Guests;

/// <summary>
/// Represents the request model for adding a guest to a list.
/// </summary>
public record class AddGuestToListRequest
(
    [property: JsonPropertyName("permission_level")]
    string PermissionLevel
);
