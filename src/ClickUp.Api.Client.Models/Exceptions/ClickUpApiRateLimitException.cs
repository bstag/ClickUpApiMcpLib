using System;
using System.Net;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Represents errors that occur due to rate limiting by the ClickUp API (HTTP 429).
    /// </summary>
    public class ClickUpApiRateLimitException : ClickUpApiException
    {
        /// <summary>
        /// Gets the suggested time to wait before retrying the request, if provided by the API.
        /// </summary>
        public TimeSpan? RetryAfterDelta { get; }

        /// <summary>
        /// Gets the specific date and time after which the request may be retried, if provided by the API.
        /// </summary>
        public DateTimeOffset? RetryAfterDate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiRateLimitException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="httpStatus">The HTTP status code associated with the error (typically 429).</param>
        /// <param name="apiErrorCode">The ClickUp specific API error code.</param>
        /// <param name="rawErrorContent">The raw error content from the API response.</param>
        /// <param name="retryAfterDelta">The suggested time to wait before retrying.</param>
        /// <param name="retryAfterDate">The specific date and time after which to retry.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ClickUpApiRateLimitException(
            string message,
            HttpStatusCode httpStatus,
            string? apiErrorCode = null,
            string? rawErrorContent = null,
            TimeSpan? retryAfterDelta = null,
            DateTimeOffset? retryAfterDate = null,
            Exception? innerException = null)
            : base(message, httpStatus, apiErrorCode, rawErrorContent, innerException)
        {
            RetryAfterDelta = retryAfterDelta;
            RetryAfterDate = retryAfterDate;
        }
    }
}
