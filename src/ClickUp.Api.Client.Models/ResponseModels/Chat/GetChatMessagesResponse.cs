using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatMessage

namespace ClickUp.Api.Client.Models.ResponseModels.Chat;

public record GetChatMessagesResponse
(
    [property: JsonPropertyName("data")]
    List<ChatMessage> Data,

    [property: JsonPropertyName("meta")]
    GetChatMessagesResponse.ResponseMeta Meta
)
{
    public record ResponseMeta(
        [property: JsonPropertyName("next_cursor")]
        string? NextCursor,

        [property: JsonPropertyName("previous_cursor")]
        string? PreviousCursor
    );
}
