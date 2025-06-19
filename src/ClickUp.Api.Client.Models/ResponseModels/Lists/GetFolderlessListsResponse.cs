using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Lists;

namespace ClickUp.Api.Client.Models.ResponseModels.Lists;

/// <summary>
/// Represents the response model for getting folderless lists.
/// </summary>
public record class GetFolderlessListsResponse
(
    [property: JsonPropertyName("lists")]
    List<List> Lists
);
