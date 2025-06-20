using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models; // For ClickUpWorkspace

namespace ClickUp.Api.Client.Models.ResponseModels.Workspaces;

public class GetAuthorizedWorkspacesResponse
{
    [JsonPropertyName("teams")] // ClickUp API refers to workspaces as 'teams' in this response
    public List<ClickUpWorkspace> Workspaces { get; set; } = new();
}
