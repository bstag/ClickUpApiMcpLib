using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Authorization;

/// <summary>
/// Represents the response model for getting an access token.
/// </summary>
public record class GetAccessTokenResponse
(
    [property: JsonPropertyName("access_token")]
    string AccessToken
);
