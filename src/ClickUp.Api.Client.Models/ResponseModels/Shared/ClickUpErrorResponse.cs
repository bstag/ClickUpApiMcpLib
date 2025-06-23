using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Shared;

/// <summary>
/// Represents an error response from the ClickUp API.
/// </summary>
public class ClickUpErrorResponse
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [JsonPropertyName("err")]
    public required string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    [JsonPropertyName("ECODE")]
    public required string ErrorCode { get; set; }
}