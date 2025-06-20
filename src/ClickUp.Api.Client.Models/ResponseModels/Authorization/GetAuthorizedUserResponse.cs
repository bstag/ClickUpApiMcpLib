using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.Authorization;

/// <summary>
/// Represents the response for getting the authorized user.
/// </summary>
public record GetAuthorizedUserResponse
{
    [JsonPropertyName("user")]
    public User User { get; init; } = null!;
}
