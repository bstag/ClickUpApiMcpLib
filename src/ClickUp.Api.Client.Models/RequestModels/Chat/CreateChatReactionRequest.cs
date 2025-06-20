using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat;

public class CreateChatReactionRequest
{
    [JsonPropertyName("reaction")]
    public string Reaction { get; set; } = null!;

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
}
