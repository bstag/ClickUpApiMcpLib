using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Sharing;

/// <summary>
/// Represents the details of a shared hierarchy.
/// </summary>
public record class SharedHierarchyDetailsResponse
(
    [property: JsonPropertyName("tasks")]
    List<string> Tasks,

    [property: JsonPropertyName("lists")]
    List<SharedHierarchyListItem> Lists,

    [property: JsonPropertyName("folders")]
    List<SharedHierarchyFolderItem> Folders
);
