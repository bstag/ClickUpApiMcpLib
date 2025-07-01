using System.Collections.Generic;
using System.Text.Json.Serialization; // Added for JsonPropertyName

namespace ClickUp.Api.Client.Models.RequestModels.Chat;

public class GetChatChannelsRequest
{
    [JsonPropertyName("description_format")]
    public string? DescriptionFormat { get; set; }

    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    [JsonPropertyName("is_follower")]
    public bool? IsFollower { get; set; }

    [JsonPropertyName("include_hidden")]
    public bool? IncludeHidden { get; set; }

    [JsonPropertyName("with_comment_since")]
    public long? WithCommentSince { get; set; }

    [JsonPropertyName("room_types")]
    public IEnumerable<string>? RoomTypes { get; set; }

    public GetChatChannelsRequest() { } // Adding a default constructor
}