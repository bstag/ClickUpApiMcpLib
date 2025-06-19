using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents user profile information.
/// </summary>
public record class ProfileInfo
(
    [property: JsonPropertyName("display_profile")]
    string? DisplayProfile,

    [property: JsonPropertyName("verified_ambassador")]
    string? VerifiedAmbassador,

    [property: JsonPropertyName("verified_consultant")]
    string? VerifiedConsultant,

    [property: JsonPropertyName("top_tier_user")]
    string? TopTierUser,

    [property: JsonPropertyName("viewed_verified_ambassador")]
    string? ViewedVerifiedAmbassador,

    [property: JsonPropertyName("viewed_verified_consultant")]
    string? ViewedVerifiedConsultant,

    [property: JsonPropertyName("viewed_top_tier_user")]
    string? ViewedTopTierUser
);
