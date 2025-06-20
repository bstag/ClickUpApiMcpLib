using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatReaction

namespace ClickUp.Api.Client.Models.ResponseModels.Chat;

public record GetChatReactionsResponse
(
    [property: JsonPropertyName("data")]
    List<ChatReaction> Data,

    [property: JsonPropertyName("meta")]
    GetChatReactionsResponse.ResponseMeta Meta
)
{
    public record ResponseMeta(
        [property: JsonPropertyName("next_cursor")]
        string? NextCursor,

        [property: JsonPropertyName("previous_cursor")]
        string? PreviousCursor
    );
}
