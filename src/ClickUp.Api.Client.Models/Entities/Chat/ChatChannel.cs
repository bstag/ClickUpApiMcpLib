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
public record ChatChannel
{
    // Properties from ChatChannel schema
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("unread_count")]
    public int UnreadCount { get; init; }

    [JsonPropertyName("last_message_at")]
    public long LastMessageAt { get; init; } // Unix timestamp

    [JsonPropertyName("last_read_at_data")]
    public ChatLastReadAtData? LastReadAtData { get; init; }

    [JsonPropertyName("team_id")]
    public string TeamId { get; init; }

    [JsonPropertyName("room_parent")]
    public ChatRoomParentDTO? RoomParent { get; init; }

    [JsonPropertyName("default_view")]
    public ChatDefaultViewDTO? DefaultView { get; init; }

    [JsonPropertyName("creator")]
    public User? Creator { get; init; } // Assuming User model from Common

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; init; } // Unix timestamp

    [JsonPropertyName("updated_at")]
    public long UpdatedAt { get; init; } // Unix timestamp

    [JsonPropertyName("archived_at")]
    public long? ArchivedAt { get; init; } // Nullable Unix timestamp

    [JsonPropertyName("members")]
    public List<User>? Members { get; init; } // Assuming User model from Common

    [JsonPropertyName("guests")]
    public List<User>? Guests { get; init; } // Assuming User model from Common

    [JsonPropertyName("links")]
    public ChatChannelLinks? Links { get; init; }

    [JsonPropertyName("is_private")]
    public bool IsPrivate { get; init; }

    [JsonPropertyName("is_favorite")]
    public bool IsFavorite { get; init; }

    [JsonPropertyName("is_muted")]
    public bool IsMuted { get; init; }

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; init; }

    [JsonPropertyName("is_direct")]
    public bool IsDirect { get; init; }

    [JsonPropertyName("last_message_id")]
    public string? LastMessageId { get; init; }

    [JsonPropertyName("last_reaction_id")]
    public string? LastReactionId { get; init; }

    [JsonPropertyName("is_read_only")]
    public bool IsReadOnly { get; init; }

    [JsonPropertyName("is_system")]
    public bool IsSystem { get; init; }

    [JsonPropertyName("custom_role_ids")]
    public List<string>? CustomRoleIds { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("group_id")]
    public string? GroupId { get; init; }

    [JsonPropertyName("is_group_channel")]
    public bool IsGroupChannel { get; init; }

    // Properties from ChatRoom schema (base type)
    [JsonPropertyName("type")]
    public ChatRoomType Type { get; init; }

    [JsonPropertyName("visibility")]
    public ChatRoomVisibility Visibility { get; init; }

    [JsonPropertyName("subcategory_type")]
    public ChatSubcategoryType? SubcategoryType { get; init; } // Nullable enum

    [JsonPropertyName("is_ai_enabled")]
    public bool IsAiEnabled { get; init; }

    [JsonPropertyName("ai_model_version")]
    public string? AiModelVersion { get; init; }

    [JsonPropertyName("ai_model_type")]
    public string? AiModelType { get; init; }
}
