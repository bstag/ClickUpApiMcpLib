using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents the details of what entities are shared with a guest.
/// </summary>
public record GuestSharingDetails
{
    /// <summary>
    /// Gets a list of task IDs shared with the guest.
    /// </summary>
    [JsonPropertyName("tasks")]
    public List<string> Tasks { get; init; } = new List<string>();

    /// <summary>
    /// Gets a list of list IDs shared with the guest.
    /// </summary>
    [JsonPropertyName("lists")]
    public List<string> Lists { get; init; } = new List<string>();

    /// <summary>
    /// Gets a list of folder IDs shared with the guest.
    /// </summary>
    [JsonPropertyName("folders")]
    public List<string> Folders { get; init; } = new List<string>();
}
