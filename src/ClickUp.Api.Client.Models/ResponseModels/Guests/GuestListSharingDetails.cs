using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Lists;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the sharing details for a guest on a list.
/// </summary>
public record class GuestListSharingDetails
(
    [property: JsonPropertyName("tasks")]
    List<string> Tasks,

    [property: JsonPropertyName("lists")]
    List<List> Lists,

    [property: JsonPropertyName("folders")]
    List<string> Folders
);
