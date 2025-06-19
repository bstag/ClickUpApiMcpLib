using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.UserGroups;

/// <summary>
/// Represents the avatar of a user group.
/// </summary>
public record UserGroupAvatar(
    [property: JsonPropertyName("attachment_id")] string? AttachmentId,
    [property: JsonPropertyName("color")] string? Color,
    [property: JsonPropertyName("source")] string? Source,
    [property: JsonPropertyName("icon")] string? Icon
);
