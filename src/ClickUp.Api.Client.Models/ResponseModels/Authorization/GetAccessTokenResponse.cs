using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Authorization;

/// <summary>
/// Represents the response for obtaining an OAuth access token.
/// Based on the inline schema `GetAccessTokenresponse` from POST /v2/oauth/token.
/// </summary>
public record GetAccessTokenResponse
{
    /// <summary>
    /// The OAuth access token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    // OAuth token responses often include other fields like:
    // "token_type": "Bearer",
    // "expires_in": 3600, // seconds
    // "refresh_token": "string",
    // "scope": "read write"
    // However, the prompt specifically mentions "access_token" as the primary property
    // based on the schema. If the actual schema includes more, they should be added.
    // For now, adhering to the minimal requirement from the prompt.
}
