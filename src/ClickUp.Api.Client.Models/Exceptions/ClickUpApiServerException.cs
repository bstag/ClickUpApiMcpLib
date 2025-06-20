using System;
using System.Net;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Represents errors that occur on the ClickUp API server side (HTTP 5xx).
    /// </summary>
    public class ClickUpApiServerException : ClickUpApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiServerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="httpStatus">The HTTP status code associated with the error (typically 5xx).</param>
        /// <param name="apiErrorCode">The ClickUp specific API error code.</param>
        /// <param name="rawErrorContent">The raw error content from the API response.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ClickUpApiServerException(
            string message,
            HttpStatusCode httpStatus,
            string? apiErrorCode = null,
            string? rawErrorContent = null,
            Exception? innerException = null)
            : base(message, httpStatus, apiErrorCode, rawErrorContent, innerException)
        {
        }
    }
}
