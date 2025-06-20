using System;
using System.Net;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Represents general client-side errors when making requests to the ClickUp API,
    /// such as network issues or unexpected client errors where a specific server error category doesn't apply.
    /// HttpStatus might be null if the request failed before receiving a response.
    /// </summary>
    public class ClickUpApiRequestException : ClickUpApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiRequestException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="httpStatus">The HTTP status code associated with the error, if available.</param>
        /// <param name="apiErrorCode">The ClickUp specific API error code, if available.</param>
        /// <param name="rawErrorContent">The raw error content from the API response, if available.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ClickUpApiRequestException(
            string message,
            HttpStatusCode? httpStatus = null,
            string? apiErrorCode = null,
            string? rawErrorContent = null,
            Exception? innerException = null)
            : base(message, httpStatus, apiErrorCode, rawErrorContent, innerException)
        {
        }
    }
}
