using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat.Enums;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents a Chat Channel in ClickUp.
/// </summary>
/// <param name="Id">The unique identifier of the chat channel.</param>
/// <param name="Name">The name of the chat channel.</param>
/// <param name="UnreadCount">The number of unread messages in the channel for the current user.</param>
/// <param name="LastMessageAt">The Unix timestamp (milliseconds) of the last message in the channel.</param>
/// <param name="TeamId">The identifier of the team (workspace) this channel belongs to.</param>
/// <param name="CreatedAt">The Unix timestamp (milliseconds) when the channel was created.</param>
/// <param name="UpdatedAt">The Unix timestamp (milliseconds) when the channel was last updated.</param>
/// <param name="IsPrivate">Indicates if the channel is private.</param>
/// <param name="IsFavorite">Indicates if the channel is marked as a favorite by the current user.</param>
/// <param name="IsMuted">Indicates if the channel is muted by the current user.</param>
/// <param name="IsHidden">Indicates if the channel is hidden by the current user.</param>
/// <param name="IsDirect">Indicates if this is a direct message channel.</param>
/// <param name="IsReadOnly">Indicates if the channel is read-only for the current user.</param>
/// <param name="IsSystem">Indicates if this is a system channel.</param>
/// <param name="IsGroupChannel">Indicates if this is a group channel.</param>
/// <param name="Type">The type of the chat room.</param>
/// <param name="Visibility">The visibility setting of the chat room.</param>
/// <param name="IsAiEnabled">Indicates if AI features are enabled for this chat room.</param>
public record ChatChannel(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("unread_count")]
    int UnreadCount,
    [property: JsonPropertyName("last_message_at")]
    DateTimeOffset LastMessageAt,
    [property: JsonPropertyName("team_id")]
    string TeamId,
    [property: JsonPropertyName("created_at")]
    DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")]
    DateTimeOffset UpdatedAt,
    [property: JsonPropertyName("is_private")]
    bool IsPrivate,
    [property: JsonPropertyName("is_favorite")]
    bool IsFavorite,
    [property: JsonPropertyName("is_muted")]
    bool IsMuted,
    [property: JsonPropertyName("is_hidden")]
    bool IsHidden,
    [property: JsonPropertyName("is_direct")]
    bool IsDirect,
    [property: JsonPropertyName("is_read_only")]
    bool IsReadOnly,
    [property: JsonPropertyName("is_system")]
    bool IsSystem,
    [property: JsonPropertyName("is_group_channel")]
    bool IsGroupChannel,
    [property: JsonPropertyName("type")]
    ChatRoomType Type,
    [property: JsonPropertyName("visibility")]
    ChatRoomVisibility Visibility,
    [property: JsonPropertyName("is_ai_enabled")]
    bool IsAiEnabled
)
{
    /// <summary>
    /// Gets the last read information for the current user in this channel.
    /// </summary>
    [JsonPropertyName("last_read_at_data")]
    public ChatLastReadAtData? LastReadAtData { get; init; }

    /// <summary>
    /// Gets the parent entity of this chat room (e.g., task, list).
    /// </summary>
    [JsonPropertyName("room_parent")]
    public ChatRoomParentDTO? RoomParent { get; init; }

    /// <summary>
    /// Gets the default view settings for this chat room.
    /// </summary>
    [JsonPropertyName("default_view")]
    public ChatDefaultViewDTO? DefaultView { get; init; }

    /// <summary>
    /// Gets the user who created this channel.
    /// </summary>
    [JsonPropertyName("creator")]
    public User? Creator { get; init; }

    /// <summary>
    /// Gets the timestamp when the channel was archived, if applicable.
    /// </summary>
    [JsonPropertyName("archived_at")]
    public DateTimeOffset? ArchivedAt { get; init; }

    /// <summary>
    /// Gets the list of members in this channel.
    /// </summary>
    [JsonPropertyName("members")]
    public List<User>? Members { get; init; }

    /// <summary>
    /// Gets the list of guests in this channel.
    /// </summary>
    [JsonPropertyName("guests")]
    public List<User>? Guests { get; init; }

    /// <summary>
    /// Gets the API links related to this channel.
    /// </summary>
    [JsonPropertyName("links")]
    public ChatChannelLinks? Links { get; init; }

    /// <summary>
    /// Gets the identifier of the last message in this channel.
    /// </summary>
    [JsonPropertyName("last_message_id")]
    public string? LastMessageId { get; init; }

    /// <summary>
    /// Gets the identifier of the last reaction in this channel.
    /// </summary>
    [JsonPropertyName("last_reaction_id")]
    public string? LastReactionId { get; init; }

    /// <summary>
    /// Gets a list of custom role identifiers associated with this channel.
    /// </summary>
    [JsonPropertyName("custom_role_ids")]
    public List<string>? CustomRoleIds { get; init; }

    /// <summary>
    /// Gets the description of the chat channel.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the group identifier if this is a group channel.
    /// </summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; init; }

    /// <summary>
    /// Gets the subcategory type of the chat channel.
    /// </summary>
    [JsonPropertyName("subcategory_type")]
    public ChatSubcategoryType? SubcategoryType { get; init; }

    /// <summary>
    /// Gets the version of the AI model used in this channel, if applicable.
    /// </summary>
    [JsonPropertyName("ai_model_version")]
    public string? AiModelVersion { get; init; }

    /// <summary>
    /// Gets the type of the AI model used in this channel, if applicable.
    /// </summary>
    [JsonPropertyName("ai_model_type")]
    public string? AiModelType { get; init; }
}
