using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.ResponseModels.Views;

public record CreateSpaceViewResponse
{
    [JsonPropertyName("view")]
    public View View { get; init; } = null!;
}
