using System;
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Represents errors that occur due to invalid request parameters before sending to the ClickUp API.
    /// </summary>
    public class ClickUpRequestValidationException : ClickUpApiException
    {
        /// <summary>
        /// Gets the collection of validation error messages.
        /// </summary>
        public IEnumerable<string> ValidationErrors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpRequestValidationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="validationErrors">The collection of validation errors.</param>
        public ClickUpRequestValidationException(string message, IEnumerable<string> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpRequestValidationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="validationErrors">The collection of validation errors.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ClickUpRequestValidationException(string message, IEnumerable<string> validationErrors, Exception innerException)
            : base(message, innerException)
        {
            ValidationErrors = validationErrors ?? Enumerable.Empty<string>();
        }
    }
}
