using System.Collections.Generic;
using System.Text.Json.Serialization;
// Note: ClickUpWorkspace is in the root Models namespace
using ClickUp.Api.Client.Models.Entities.WorkSpaces;


namespace ClickUp.Api.Client.Models.ResponseModels.Authorization;

/// <summary>
/// Represents the response for getting authorized workspaces (teams).
/// </summary>
public record GetAuthorizedWorkspacesResponse
{
    [JsonPropertyName("teams")]
    public List<ClickUpWorkspace> Workspaces { get; init; } = new();
}
