using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Docs;

public class SearchDocsRequest
{
    [JsonPropertyName("query")]
    public string Query { get; set; } = null!;

    [JsonPropertyName("space_ids")]
    public List<string>? SpaceIds { get; set; }

    [JsonPropertyName("folder_ids")]
    public List<string>? FolderIds { get; set; }

    [JsonPropertyName("list_ids")]
    public List<string>? ListIds { get; set; }

    [JsonPropertyName("task_ids")]
    public List<string>? TaskIds { get; set; }

    [JsonPropertyName("include_archived")]
    public bool? IncludeArchived { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}
