using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Goals; // For KeyResult entity

namespace ClickUp.Api.Client.Models.ResponseModels.Goals
{
    /// <summary>
    /// Represents the response after creating a new Key Result.
    /// </summary>
    /// <param name="KeyResult">The newly created <see cref="Entities.Goals.KeyResult"/> object.</param>
    public record CreateKeyResultResponse
    (
        [property: JsonPropertyName("key_result")] KeyResult KeyResult
    );
}
