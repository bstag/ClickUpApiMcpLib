using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Workspaces;

/// <summary>
/// Represents the response model for getting workspace plan information.
/// </summary>
public record class GetWorkspacePlanResponse
(
    [property: JsonPropertyName("plan_name")]
    string PlanName,

    [property: JsonPropertyName("plan_id")]
    int PlanId
);
