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
/// <param name="Id">The unique identifier of the chat message.</param>
/// <param name="Type">The type of the message (e.g., "comment", "system", "post").</param>
/// <param name="User">The user who sent or generated the message.</param>
/// <param name="Date">The Unix timestamp (milliseconds) when the message was created.</param>
/// <param name="GroupId">The group identifier associated with the message.</param>
/// <param name="TeamId">The team (workspace) identifier associated with the message.</param>
/// <param name="ChannelId">The channel identifier where the message resides.</param>
/// <param name="Deleted">Indicates if the message has been deleted.</param>
/// <param name="Edited">Indicates if the message has been edited.</param>
public record ChatMessage(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("user")]
    User User,
    [property: JsonPropertyName("date")]
    DateTimeOffset Date,
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
    /// <summary>
    /// Gets the text content of the message.
    /// </summary>
    [JsonPropertyName("text_content")]
    public string? TextContent { get; init; }

    /// <summary>
    /// Gets the timestamp when the message was last edited, if applicable.
    /// </summary>
    [JsonPropertyName("edited_at")]
    public DateTimeOffset? EditedAt { get; init; }

    /// <summary>
    /// Gets additional data associated with the message, typically for "post" type messages.
    /// </summary>
    [JsonPropertyName("data")]
    public ChatPostData? Data { get; init; }

    /// <summary>
    /// Gets the list of reactions to this message.
    /// </summary>
    [JsonPropertyName("reactions")]
    public List<ChatReaction>? Reactions { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is the first message in a thread.
    /// </summary>
    [JsonPropertyName("is_first_message_in_thread")]
    public bool? IsFirstMessageInThread { get; init; }

    /// <summary>
    /// Gets a list of comment IDs that are part of this message's thread.
    /// </summary>
    [JsonPropertyName("thread_comment_ids")]
    public List<string>? ThreadCommentIds { get; init; }

    /// <summary>
    /// Gets the count of messages in this message's thread.
    /// </summary>
    [JsonPropertyName("thread_count")]
    public int? ThreadCount { get; init; }

    /// <summary>
    /// Gets the ID of the parent message if this message is part of a thread.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; init; }

    /// <summary>
    /// Gets the API links related to this chat message.
    /// </summary>
    [JsonPropertyName("links")]
    public CommentChatMessageLinks2? Links { get; init; }

    /// <summary>
    /// Gets a value indicating whether this message is hidden.
    /// </summary>
    [JsonPropertyName("is_hidden")]
    public bool? IsHidden { get; init; }

    /// <summary>
    /// Gets a value indicating whether this message is pinned.
    /// </summary>
    [JsonPropertyName("pinned")]
    public bool? Pinned { get; init; }

    /// <summary>
    /// Gets the timestamp when this message was pinned, if applicable.
    /// </summary>
    [JsonPropertyName("pinned_at")]
    public DateTimeOffset? PinnedAt { get; init; }

    /// <summary>
    /// Gets the user who pinned this message.
    /// </summary>
    [JsonPropertyName("pinned_by")]
    public User? PinnedBy { get; init; }

    /// <summary>
    /// Gets the type of system event, if this is a "system" type message.
    /// </summary>
    /// <example>"user_joined", "task_created"</example>
    [JsonPropertyName("system_event_type")]
    public string? SystemEventType { get; init; }

    /// <summary>
    /// Gets the data associated with a system event. The structure of this object varies depending on the <see cref="SystemEventType"/>.
    /// </summary>
    [JsonPropertyName("system_event_data")]
    public object? SystemEventData { get; init; }

    /// <summary>
    /// Gets a list of user IDs mentioned in this message.
    /// </summary>
    [JsonPropertyName("mentioned_user_ids")]
    public List<string>? MentionedUserIds { get; init; }

    /// <summary>
    /// Gets a list of team role IDs mentioned in this message.
    /// </summary>
    [JsonPropertyName("mentioned_team_role_ids")]
    public List<string>? MentionedTeamRoleIds { get; init; }

    /// <summary>
    /// Gets additional properties for the message. The structure of this object can vary.
    /// </summary>
    [JsonPropertyName("message_props")]
    public object? MessageProps { get; init; }

    /// <summary>
    /// Gets a value indicating whether this message is a reply to another message.
    /// </summary>
    [JsonPropertyName("is_reply")]
    public bool? IsReply { get; init; }

    /// <summary>
    /// Gets a value indicating whether this message is part of a direct message channel.
    /// </summary>
    [JsonPropertyName("is_direct")]
    public bool? IsDirect { get; init; }

    /// <summary>
    /// Gets a value indicating whether notifications for this message are muted for the current user.
    /// </summary>
    [JsonPropertyName("is_muted")]
    public bool? IsMuted { get; init; }

    /// <summary>
    /// Gets a value indicating whether this message was generated by AI.
    /// </summary>
    [JsonPropertyName("is_ai_generated")]
    public bool? IsAiGenerated { get; init; }

    /// <summary>
    /// Gets the version of the AI model used to generate this message, if applicable.
    /// </summary>
    [JsonPropertyName("ai_model_version")]
    public string? AiModelVersion { get; init; }

    /// <summary>
    /// Gets the type of the AI model used to generate this message, if applicable.
    /// </summary>
    [JsonPropertyName("ai_model_type")]
    public string? AiModelType { get; init; }

    /// <summary>
    /// Gets a value indicating whether this message is read-only.
    /// </summary>
    [JsonPropertyName("is_read_only")]
    public bool? IsReadOnly { get; init; }

    /// <summary>
    /// Gets a value indicating whether this message is marked as urgent.
    /// </summary>
    [JsonPropertyName("is_urgent")]
    public bool? IsUrgent { get; init; }
}
