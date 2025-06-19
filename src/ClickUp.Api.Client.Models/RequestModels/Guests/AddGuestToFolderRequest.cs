using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Guests;

/// <summary>
/// Represents the request model for adding a guest to a folder.
/// </summary>
public record class AddGuestToFolderRequest
(
    [property: JsonPropertyName("permission_level")]
    string PermissionLevel
);
