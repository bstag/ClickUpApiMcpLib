using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents the details of what entities are shared with a guest.
/// </summary>
public record GuestSharingDetails
{
    [JsonPropertyName("tasks")]
    public List<string> Tasks { get; init; } = new List<string>();

    [JsonPropertyName("lists")]
    public List<string> Lists { get; init; } = new List<string>();

    [JsonPropertyName("folders")]
    public List<string> Folders { get; init; } = new List<string>();
}
