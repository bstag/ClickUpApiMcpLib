namespace ClickUp.Api.Client.Models.RequestModels.Authorization;

public class GetAccessTokenRequest
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Code { get; set; }
}