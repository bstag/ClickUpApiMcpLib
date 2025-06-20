using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Authorization;

public class GetAccessTokenRequest
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; }

    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; }

    [JsonPropertyName("code")]
    public string Code { get; }

    public GetAccessTokenRequest(string clientId, string clientSecret, string code)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        Code = code;
    }
}
