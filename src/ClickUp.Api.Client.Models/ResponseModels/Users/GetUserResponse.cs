using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Users;

/// <summary>
/// Represents the response model for getting a user.
/// </summary>
public record class GetUserResponse
(
    [property: JsonPropertyName("member")]
    GetUserResponseMember Member
);
