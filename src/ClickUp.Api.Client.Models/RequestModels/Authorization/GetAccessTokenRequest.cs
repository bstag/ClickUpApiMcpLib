using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Authorization;

/// <summary>
/// Represents the request body for obtaining an OAuth access token.
/// </summary>
public class GetAccessTokenRequest
{
    /// <summary>
    /// Gets the client ID of your ClickUp OAuth app.
    /// </summary>
    [JsonPropertyName("client_id")]
    public string ClientId { get; }

    /// <summary>
    /// Gets the client secret of your ClickUp OAuth app.
    /// </summary>
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; }

    /// <summary>
    /// Gets the authorization code received from the OAuth authorization flow.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAccessTokenRequest"/> class.
    /// </summary>
    /// <param name="clientId">The client ID of your ClickUp OAuth app.</param>
    /// <param name="clientSecret">The client secret of your ClickUp OAuth app.</param>
    /// <param name="code">The authorization code received from the OAuth authorization flow.</param>
    public GetAccessTokenRequest(string clientId, string clientSecret, string code)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        Code = code;
    }
}
