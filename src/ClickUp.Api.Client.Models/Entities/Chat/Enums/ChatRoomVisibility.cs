using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChatRoomVisibility
    {
        PUBLIC,
        PRIVATE
    }
}
