using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Folders;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the sharing details for a guest on a folder.
/// </summary>
public record class GuestFolderSharingDetailsResponse
(
    [property: JsonPropertyName("tasks")]
    List<string> Tasks,

    [property: JsonPropertyName("lists")]
    List<string> Lists,

    [property: JsonPropertyName("folders")]
    List<Folder> Folders
);
