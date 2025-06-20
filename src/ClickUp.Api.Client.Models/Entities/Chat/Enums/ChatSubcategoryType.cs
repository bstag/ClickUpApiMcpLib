using System.Runtime.Serialization;
using System.Text.Json.Serialization;
// using ClickUp.Api.Client.Models.JsonConverters; // Removed this line

namespace ClickUp.Api.Client.Models.Entities.Chat.Enums;

/// <summary>
/// Defines the subcategory type of a chat room.
/// Corresponds to #/components/schemas/ChatRoom_subcategory_type
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))] // Changed to standard converter
public enum ChatSubcategoryType
{
    // Unknown = 0, // See comment in ChatRoomType.cs

    /// <summary>
    /// Default subcategory type.
    /// </summary>
    [EnumMember(Value = "default")]
    Default,

    /// <summary>
    /// Form comments subcategory type.
    /// </summary>
    [EnumMember(Value = "form_comments")]
    FormComments,

    /// <summary>
    /// CuTask comments subcategory type.
    /// </summary>
    [EnumMember(Value = "task_comments")]
    TaskComments,

    /// <summary>
    /// Doc comments subcategory type.
    /// </summary>
    [EnumMember(Value = "doc_comments")]
    DocComments,

    /// <summary>
    /// Chat view comments subcategory type.
    /// </summary>
    [EnumMember(Value = "chat_view_comments")]
    ChatViewComments,

    /// <summary>
    /// Goal comments subcategory type.
    /// </summary>
    [EnumMember(Value = "goal_comments")]
    GoalComments,

    /// <summary>
    /// Thread comments subcategory type.
    /// </summary>
    [EnumMember(Value = "thread_comments")]
    ThreadComments
}
