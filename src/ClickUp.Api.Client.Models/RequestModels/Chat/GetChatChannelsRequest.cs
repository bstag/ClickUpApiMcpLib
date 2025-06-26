using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.RequestModels.Chat;

public class GetChatChannelsRequest
{
    public string? DescriptionFormat { get; set; }
    public string? Cursor { get; set; }
    public int? Limit { get; set; }
    public bool? IsFollower { get; set; }
    public bool? IncludeHidden { get; set; }
    public long? WithCommentSince { get; set; }
    public IEnumerable<string>? RoomTypes { get; set; }
}