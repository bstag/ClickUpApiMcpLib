using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Sharing;

/// <summary>
/// Represents the request model for updating privacy and access settings.
/// </summary>
public record class UpdatePrivacyAndAccessRequest
(
    [property: JsonPropertyName("entries")]
    List<AccessControlEntry>? Entries,

    [property: JsonPropertyName("private")]
    bool? Private
);
