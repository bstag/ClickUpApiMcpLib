using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Goals; // For KeyResult entity

namespace ClickUp.Api.Client.Models.ResponseModels.Goals
{
    public record CreateKeyResultResponse
    (
        [property: JsonPropertyName("key_result")] KeyResult KeyResult
    );
}
