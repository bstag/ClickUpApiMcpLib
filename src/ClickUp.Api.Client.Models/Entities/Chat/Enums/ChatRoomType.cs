using System.Runtime.Serialization;
using System.Text.Json.Serialization;
// using ClickUp.Api.Client.Models.JsonConverters; // Removed this line

namespace ClickUp.Api.Client.Models.Entities.Chat.Enums;

/// <summary>
/// Defines the type of a chat room.
/// Corresponds to #/components/schemas/ChatRoom_type
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))] // Changed to standard converter
public enum ChatRoomType
{
    // Unknown = 0, // Standard converter doesn't inherently support an "Unknown" mapping without custom logic
                 // For now, relying on default(ChatRoomType) if value is not matched.
                 // Or, the API might strictly only send defined values.

    /// <summary>
    /// Channel chat room type.
    /// </summary>
    [EnumMember(Value = "channel")]
    Channel,

    /// <summary>
    /// Direct message chat room type.
    /// </summary>
    [EnumMember(Value = "dm")]
    Dm,

    /// <summary>
    /// Group chat room type.
    /// </summary>
    [EnumMember(Value = "group")]
    Group
}
