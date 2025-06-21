using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat.Enums; // For ChatRoomVisibility

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the request to create a new chat channel.
    /// </summary>
    /// <param name="Description">Optional description for the channel.</param>
    /// <param name="Name">The name of the channel (required).</param>
    /// <param name="Topic">Optional topic for the channel.</param>
    /// <param name="UserIds">A list of user IDs to initially add to the channel.</param>
    /// <param name="Visibility">The visibility of the channel (Public or Private).</param>
    /// <param name="WorkspaceId">The ID of the workspace where the channel will be created.</param>
    public record ChatCreateChatChannelRequest
    (
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("topic")] string? Topic,
        [property: JsonPropertyName("user_ids")] List<string> UserIds,
        [property: JsonPropertyName("visibility")] ChatRoomVisibility Visibility,
        [property: JsonPropertyName("workspace_id")] string WorkspaceId
    );
}
