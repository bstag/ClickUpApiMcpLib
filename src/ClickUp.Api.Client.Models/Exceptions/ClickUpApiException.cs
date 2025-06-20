using System;
using System.Net;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Base class for all ClickUp API specific exceptions.
    /// </summary>
    public class ClickUpApiException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code returned by the API, if available.
        /// </summary>
        public HttpStatusCode? HttpStatus { get; }

        /// <summary>
        /// Gets the specific error code returned by the ClickUp API (e.g., "OAUTH_023"), if available.
        /// </summary>
        public string? ApiErrorCode { get; }

        /// <summary>
        /// Gets the raw error content (e.g., JSON response) from the API, if available, for debugging purposes.
        /// </summary>
        public string? RawErrorContent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="httpStatus">The HTTP status code associated with the error.</param>
        /// <param name="apiErrorCode">The ClickUp specific API error code.</param>
        /// <param name="rawErrorContent">The raw error content from the API response.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ClickUpApiException(
            string message,
            HttpStatusCode? httpStatus = null,
            string? apiErrorCode = null,
            string? rawErrorContent = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            HttpStatus = httpStatus;
            ApiErrorCode = apiErrorCode;
            RawErrorContent = rawErrorContent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        /// <param name="httpStatus">The HTTP status code associated with the error.</param>
        /// <param name="apiErrorCode">The ClickUp specific API error code.</param>
        /// <param name="rawErrorContent">The raw error content from the API response.</param>
        public ClickUpApiException(
            string message,
            Exception innerException,
            HttpStatusCode? httpStatus = null,
            string? apiErrorCode = null,
            string? rawErrorContent = null)
            : base(message, innerException)
        {
            HttpStatus = httpStatus;
            ApiErrorCode = apiErrorCode;
            RawErrorContent = rawErrorContent;
        }
    }
}
