using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat.Enums; // Updated namespace
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User if it's there

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents a Chat Channel in ClickUp, inheriting from ChatRoom.
/// </summary>
public record ChatChannel(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("unread_count")]
    int UnreadCount,
    [property: JsonPropertyName("last_message_at")]
    long LastMessageAt, // Unix timestamp
    [property: JsonPropertyName("team_id")]
    string TeamId,
    [property: JsonPropertyName("created_at")]
    long CreatedAt, // Unix timestamp
    [property: JsonPropertyName("updated_at")]
    long UpdatedAt, // Unix timestamp
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
    // Properties from ChatRoom schema (base type)
    [property: JsonPropertyName("type")]
    ChatRoomType Type,
    [property: JsonPropertyName("visibility")]
    ChatRoomVisibility Visibility,
    [property: JsonPropertyName("is_ai_enabled")]
    bool IsAiEnabled
)
{
    // Nullable properties or those with complex types not suitable for primary constructor
    [JsonPropertyName("last_read_at_data")]
    public ChatLastReadAtData? LastReadAtData { get; init; }

    [JsonPropertyName("room_parent")]
    public ChatRoomParentDTO? RoomParent { get; init; }

    [JsonPropertyName("default_view")]
    public ChatDefaultViewDTO? DefaultView { get; init; }

    [JsonPropertyName("creator")]
    public User? Creator { get; init; } // Assuming User model from Common

    [JsonPropertyName("archived_at")]
    public long? ArchivedAt { get; init; } // Nullable Unix timestamp

    [JsonPropertyName("members")]
    public List<User>? Members { get; init; } // Assuming User model from Common

    [JsonPropertyName("guests")]
    public List<User>? Guests { get; init; } // Assuming User model from Common

    [JsonPropertyName("links")]
    public ChatChannelLinks? Links { get; init; }

    [JsonPropertyName("last_message_id")]
    public string? LastMessageId { get; init; }

    [JsonPropertyName("last_reaction_id")]
    public string? LastReactionId { get; init; }

    [JsonPropertyName("custom_role_ids")]
    public List<string>? CustomRoleIds { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("group_id")]
    public string? GroupId { get; init; }

    [JsonPropertyName("subcategory_type")]
    public ChatSubcategoryType? SubcategoryType { get; init; } // Nullable enum

    [JsonPropertyName("ai_model_version")]
    public string? AiModelVersion { get; init; }

    [JsonPropertyName("ai_model_type")]
    public string? AiModelType { get; init; }
}
