using System;
using System.Net.Http;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Exceptions;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Interface for centralized error handling in the ClickUp API client.
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Handles HTTP response errors and converts them to appropriate exceptions.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        /// <param name="requestUrl">The request URL that caused the error.</param>
        /// <param name="httpMethod">The HTTP method used in the request.</param>
        /// <returns>A task that represents the asynchronous error handling operation.</returns>
        /// <exception cref="ClickUpApiException">Thrown when an API error occurs.</exception>
        Task HandleHttpResponseErrorAsync(HttpResponseMessage response, string? requestUrl = null, string? httpMethod = null);

        /// <summary>
        /// Handles general exceptions and wraps them in appropriate ClickUp exceptions.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="context">Additional context information about the error.</param>
        /// <param name="requestUrl">The request URL that caused the error.</param>
        /// <param name="httpMethod">The HTTP method used in the request.</param>
        /// <returns>A ClickUp-specific exception.</returns>
        ClickUpApiException HandleException(Exception exception, string? context = null, string? requestUrl = null, string? httpMethod = null);

        /// <summary>
        /// Handles validation errors and creates a validation exception.
        /// </summary>
        /// <param name="propertyName">The name of the property that failed validation.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <param name="invalidValue">The invalid value.</param>
        /// <param name="validationRule">The validation rule that was violated.</param>
        /// <returns>A validation exception.</returns>
        ClickUpValidationException HandleValidationError(string propertyName, string errorMessage, object? invalidValue = null, string? validationRule = null);

        /// <summary>
        /// Determines if an exception is transient and should be retried.
        /// </summary>
        /// <param name="exception">The exception to evaluate.</param>
        /// <returns>True if the exception is transient and retryable; otherwise, false.</returns>
        bool IsTransientError(Exception exception);

        /// <summary>
        /// Extracts correlation ID from an exception or generates a new one.
        /// </summary>
        /// <param name="exception">The exception to extract correlation ID from.</param>
        /// <returns>The correlation ID.</returns>
        string GetOrCreateCorrelationId(Exception? exception = null);

        /// <summary>
        /// Logs an error with structured information.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="context">Additional context information.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns>A task that represents the asynchronous logging operation.</returns>
        Task LogErrorAsync(Exception exception, string? context = null, string? requestUrl = null, string? httpMethod = null);

        /// <summary>
        /// Creates an error context object with relevant information.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="additionalContext">Additional context information.</param>
        /// <returns>An error context object.</returns>
        ErrorContext CreateErrorContext(string? requestUrl = null, string? httpMethod = null, object? additionalContext = null);
    }

    /// <summary>
    /// Represents error context information.
    /// </summary>
    public class ErrorContext
    {
        /// <summary>
        /// Gets or sets the correlation ID for tracking the error.
        /// </summary>
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the request URL that caused the error.
        /// </summary>
        public string? RequestUrl { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method used in the request.
        /// </summary>
        public string? HttpMethod { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the error occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets or sets the user agent string.
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Gets or sets additional context information.
        /// </summary>
        public object? AdditionalContext { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string>? RequestHeaders { get; set; }

        /// <summary>
        /// Gets or sets the response headers.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string>? ResponseHeaders { get; set; }

        /// <summary>
        /// Gets or sets the request body content.
        /// </summary>
        public string? RequestBody { get; set; }

        /// <summary>
        /// Gets or sets the response body content.
        /// </summary>
        public string? ResponseBody { get; set; }

        /// <summary>
        /// Gets or sets the duration of the request.
        /// </summary>
        public TimeSpan? RequestDuration { get; set; }
    }

    /// <summary>
    /// Interface for error handling strategies.
    /// </summary>
    public interface IErrorHandlingStrategy
    {
        /// <summary>
        /// Gets a value indicating whether this strategy can handle the specified exception.
        /// </summary>
        /// <param name="exception">The exception to evaluate.</param>
        /// <returns>True if this strategy can handle the exception; otherwise, false.</returns>
        bool CanHandle(Exception exception);

        /// <summary>
        /// Handles the specified exception.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="context">The error context.</param>
        /// <returns>A ClickUp-specific exception.</returns>
        ClickUpApiException Handle(Exception exception, ErrorContext context);

        /// <summary>
        /// Gets the priority of this strategy (higher values have higher priority).
        /// </summary>
        int Priority { get; }
    }
}