using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Shared
{
    /// <summary>
    /// Represents an error response from the ClickUp API with detailed validation errors.
    /// Extends <see cref="ClickUpErrorResponse"/> to include field-specific errors.
    /// </summary>
    public class ClickUpValidationErrorDetail : ClickUpErrorResponse
    {
        /// <summary>
        /// Gets or sets the detailed field-specific validation errors.
        /// The key is the field name, and the value is a list of error messages for that field.
        /// </summary>
        /// <remarks>
        /// This property name ("errors") is an assumption based on common API patterns.
        /// It might need adjustment if ClickUp API uses a different field name for detailed errors.
        /// </remarks>
        [JsonPropertyName("errors")]
        public Dictionary<string, List<string>>? DetailedErrors { get; set; }
    }
}
