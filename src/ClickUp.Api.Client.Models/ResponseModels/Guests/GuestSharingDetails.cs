using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the sharing details for a guest.
/// </summary>
public record class GuestSharingDetails
(
    [property: JsonPropertyName("tasks")]
    List<string> Tasks,

    [property: JsonPropertyName("lists")]
    List<string> Lists,

    [property: JsonPropertyName("folders")]
    List<string> Folders
);
