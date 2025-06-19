using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Sharing;

/// <summary>
/// Represents an entry in an access control list.
/// </summary>
public record class AccessControlEntry
(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("kind")]
    string Kind,

    [property: JsonPropertyName("permission_level")]
    int? PermissionLevel
);
