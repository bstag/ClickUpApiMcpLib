using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Goals; // For KeyResult entity

namespace ClickUp.Api.Client.Models.ResponseModels.Goals
{
    /// <summary>
    /// Represents the response after editing an existing Key Result.
    /// </summary>
    /// <param name="KeyResult">The updated <see cref="Entities.Goals.KeyResult"/> object.</param>
    public record EditKeyResultResponse
    (
        [property: JsonPropertyName("key_result")] KeyResult KeyResult
    );
}
