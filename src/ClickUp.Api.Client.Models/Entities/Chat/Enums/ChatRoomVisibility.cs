using System.Runtime.Serialization;
using System.Text.Json.Serialization;
// using ClickUp.Api.Client.Models.JsonConverters; // Removed this line

namespace ClickUp.Api.Client.Models.Entities.Chat.Enums;

/// <summary>
/// Defines the visibility of a chat room.
/// Corresponds to #/components/schemas/ChatRoom_visibility
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))] // Changed to standard converter
public enum ChatRoomVisibility
{
    // Unknown = 0, // See comment in ChatRoomType.cs

    /// <summary>
    /// Public visibility.
    /// </summary>
    [EnumMember(Value = "public")]
    Public,

    /// <summary>
    /// Private visibility.
    /// </summary>
    [EnumMember(Value = "private")]
    Private,

    /// <summary>
    /// Members only visibility.
    /// </summary>
    [EnumMember(Value = "members_only")]
    MembersOnly
}
