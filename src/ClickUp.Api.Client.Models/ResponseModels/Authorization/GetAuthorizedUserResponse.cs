using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.Authorization;

public class GetAuthorizedUserResponse
{
    [JsonPropertyName("user")]
    public User User { get; set; } = null!;
}
