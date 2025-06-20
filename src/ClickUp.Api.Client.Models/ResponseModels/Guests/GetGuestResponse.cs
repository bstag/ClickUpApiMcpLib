using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response for getting a single guest.
/// </summary>
public record GetGuestResponse
{
    /// <summary>
    /// The guest information.
    /// </summary>
    [JsonPropertyName("guest")]
    public Guest Guest { get; init; } = null!;
}
