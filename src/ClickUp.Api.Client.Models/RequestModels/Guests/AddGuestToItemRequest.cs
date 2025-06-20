using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Guests;

public class AddGuestToItemRequest
{
    [JsonPropertyName("permission_level")]
    public int PermissionLevel { get; set; }
}
