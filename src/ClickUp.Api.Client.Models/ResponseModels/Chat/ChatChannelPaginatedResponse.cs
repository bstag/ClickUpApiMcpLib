using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatChannel

namespace ClickUp.Api.Client.Models.ResponseModels.Chat
{
    /// <summary>
    /// Represents a paginated response for a list of <see cref="ChatChannel"/> items.
    /// </summary>
    /// <param name="NextCursor">A cursor string to retrieve the next page of results. Null if no more pages.</param>
    /// <param name="Data">The list of <see cref="ChatChannel"/> items for the current page.</param>
    public record ChatChannelPaginatedResponse
    (
        [property: JsonPropertyName("next_cursor")] string? NextCursor,
        [property: JsonPropertyName("data")] List<ChatChannel> Data
    );
}
