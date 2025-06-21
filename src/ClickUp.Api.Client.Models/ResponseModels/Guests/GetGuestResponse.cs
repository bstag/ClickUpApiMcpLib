using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response when retrieving information about a single guest.
/// </summary>
public record GetGuestResponse
{
    /// <summary>
    /// Gets the detailed information about the guest.
    /// </summary>
    [JsonPropertyName("guest")]
    public Guest Guest { get; init; } = null!;
}
