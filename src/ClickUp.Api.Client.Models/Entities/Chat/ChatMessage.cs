using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.Chat;

/// <summary>
/// Represents a Chat Message in ClickUp.
/// Corresponds to #/components/schemas/ChatMessage
/// </summary>
public record ChatMessage(
    [property: JsonPropertyName("id")]
    string Id,
    /// <summary>
    /// Type of the message.
    /// </summary>
    /// <example>"comment"</example>
    /// <example>"system"</example>
    /// <example>"post"</example>
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("user")]
    User User, // From Common.User
    [property: JsonPropertyName("date")]
    long Date, // Unix timestamp
    [property: JsonPropertyName("group_id")]
    string GroupId,
    [property: JsonPropertyName("team_id")]
    string TeamId,
    [property: JsonPropertyName("channel_id")]
    string ChannelId,
    [property: JsonPropertyName("deleted")]
    bool Deleted,
    [property: JsonPropertyName("edited")]
    bool Edited
)
{
    [JsonPropertyName("text_content")]
    public string? TextContent { get; init; }

    [JsonPropertyName("edited_at")]
    public long? EditedAt { get; init; } // Nullable Unix timestamp

    [JsonPropertyName("data")]
    public ChatPostData? Data { get; init; } // Corresponds to ChatPostData schema

    /// <summary>
    /// Reactions to the message. The schema defines this as an array of "Reaction_object".
    /// "Reaction_object" is defined as {"type": "object"}, so using List<object>.
    /// A more specific Reaction model could be used if defined elsewhere or becomes clear.
    /// Using ChatReaction model found in the same directory.
    /// </summary>
    [JsonPropertyName("reactions")]
    public List<ChatReaction>? Reactions { get; init; }

    [JsonPropertyName("is_first_message_in_thread")]
    public bool? IsFirstMessageInThread { get; init; }

    [JsonPropertyName("thread_comment_ids")]
    public List<string>? ThreadCommentIds { get; init; }

    [JsonPropertyName("thread_count")]
    public int? ThreadCount { get; init; }

    [JsonPropertyName("parent_id")]
    public string? ParentId { get; init; }

    [JsonPropertyName("links")]
    public CommentChatMessageLinks2? Links { get; init; } // Corresponds to CommentChatMessageLinks2

    [JsonPropertyName("is_hidden")]
    public bool? IsHidden { get; init; }

    [JsonPropertyName("pinned")]
    public bool? Pinned { get; init; }

    [JsonPropertyName("pinned_at")]
    public long? PinnedAt { get; init; } // Nullable Unix timestamp

    [JsonPropertyName("pinned_by")]
    public User? PinnedBy { get; init; } // From Common.User

    [JsonPropertyName("system_event_type")]
    public string? SystemEventType { get; init; }

    /// <summary>
    /// Data associated with a system event. Structure varies.
    /// </summary>
    [JsonPropertyName("system_event_data")]
    public object? SystemEventData { get; init; }

    [JsonPropertyName("mentioned_user_ids")]
    public List<string>? MentionedUserIds { get; init; }

    [JsonPropertyName("mentioned_team_role_ids")]
    public List<string>? MentionedTeamRoleIds { get; init; }

    /// <summary>
    /// Additional properties for the message. Structure varies.
    /// </summary>
    [JsonPropertyName("message_props")]
    public object? MessageProps { get; init; }

    [JsonPropertyName("is_reply")]
    public bool? IsReply { get; init; }

    [JsonPropertyName("is_direct")]
    public bool? IsDirect { get; init; }

    [JsonPropertyName("is_muted")]
    public bool? IsMuted { get; init; }

    [JsonPropertyName("is_ai_generated")]
    public bool? IsAiGenerated { get; init; }

    [JsonPropertyName("ai_model_version")]
    public string? AiModelVersion { get; init; }

    [JsonPropertyName("ai_model_type")]
    public string? AiModelType { get; init; }

    [JsonPropertyName("is_read_only")]
    public bool? IsReadOnly { get; init; }

    [JsonPropertyName("is_urgent")]
    public bool? IsUrgent { get; init; }
}
