using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatChannel

namespace ClickUp.Api.Client.Models.ResponseModels.Chat
{
    // This is a specific implementation for ChatChannel.
    // A truly generic ChatPaginatedResponse<T> would require different handling
    // or be used with known types.
    public record ChatChannelPaginatedResponse
    (
        [property: JsonPropertyName("next_cursor")] string? NextCursor,
        [property: JsonPropertyName("data")] List<ChatChannel> Data // Specific to ChatChannel here
    );

    // Placeholder for a more generic concept if needed, though not directly usable
    // public record ChatPaginatedResponse<T>
    // (
    //     [property: JsonPropertyName("next_cursor")] string? NextCursor,
    //     [property: JsonPropertyName("data")] List<T> Data
    // );
}
