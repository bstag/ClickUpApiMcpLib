using System;
using System.Collections.Generic;
using System.Net;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Represents errors that occur due to input validation issues with the ClickUp API (HTTP 400/422).
    /// </summary>
    public class ClickUpApiValidationException : ClickUpApiException
    {
        /// <summary>
        /// Gets a dictionary of field-specific validation errors, if provided by the API.
        /// The key is the field name, and the value is a list of error messages for that field.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>>? Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiValidationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="httpStatus">The HTTP status code associated with the error (typically 400 or 422).</param>
        /// <param name="apiErrorCode">The ClickUp specific API error code.</param>
        /// <param name="rawErrorContent">The raw error content from the API response.</param>
        /// <param name="errors">A dictionary containing field-specific validation errors.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ClickUpApiValidationException(
            string message,
            HttpStatusCode httpStatus,
            string? apiErrorCode = null,
            string? rawErrorContent = null,
            IReadOnlyDictionary<string, IReadOnlyList<string>>? errors = null,
            Exception? innerException = null)
            : base(message, httpStatus, apiErrorCode, rawErrorContent, innerException)
        {
            Errors = errors;
        }
    }
}
