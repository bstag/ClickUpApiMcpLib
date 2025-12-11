using System;
using System.Net;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Represents errors that occur due to authentication or authorization issues with the ClickUp API (HTTP 401/403).
    /// </summary>
    public class ClickUpApiAuthenticationException : ClickUpApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiAuthenticationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="httpStatus">The HTTP status code associated with the error (typically 401 or 403).</param>
        /// <param name="apiErrorCode">The ClickUp specific API error code.</param>
        /// <param name="rawErrorContent">The raw error content from the API response.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ClickUpApiAuthenticationException(
            string message,
            HttpStatusCode httpStatus,
            string? apiErrorCode = null,
            string? rawErrorContent = null,
            Exception? innerException = null)
            : base(message, httpStatus, apiErrorCode, rawErrorContent, null, null, innerException)
        {
        }
    }
}
