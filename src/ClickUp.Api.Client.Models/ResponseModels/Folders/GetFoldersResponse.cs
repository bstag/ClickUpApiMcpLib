using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Folders;

namespace ClickUp.Api.Client.Models.ResponseModels.Folders;

/// <summary>
/// Represents the response model for getting folders.
/// </summary>
public record class GetFoldersResponse
(
    [property: JsonPropertyName("folders")]
    List<Folder> Folders
);
