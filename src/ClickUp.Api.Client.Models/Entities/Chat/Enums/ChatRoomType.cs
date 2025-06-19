using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChatRoomType
    {
        CHANNEL,
        DM, // Direct Message
        GROUP_DM // Group Direct Message
    }
}
