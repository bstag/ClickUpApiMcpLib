using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Authorization;

public class GetAccessTokenRequest
{
    /// <summary>
    /// Gets the client ID of your ClickUp OAuth app.
    /// </summary>
    [JsonPropertyName("client_id")]
    [Required]
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets the client secret of your ClickUp OAuth app.
    /// </summary>
    [JsonPropertyName("client_secret")]
    [Required]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets the authorization code received from the OAuth authorization flow.
    /// </summary>
    [JsonPropertyName("code")]
    [Required]
    public string? Code { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAccessTokenRequest"/> class.
    /// </summary>
    /// <param name="clientId">The client ID of your ClickUp OAuth app.</param>
    /// <param name="clientSecret">The client secret of your ClickUp OAuth app.</param>
    /// <param name="code">The authorization code received from the OAuth authorization flow.</param>
    public GetAccessTokenRequest(string? clientId = null, string? clientSecret = null, string? code = null) {
        ClientId = clientId;
        ClientSecret = clientSecret;
        Code = code;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAccessTokenRequest"/> class.
    /// </summary>
    public GetAccessTokenRequest() { }
    
}

