using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Base exception class for all ClickUp API related errors.
    /// </summary>
    [Serializable]
    public class ClickUpApiException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with the error, if available.
        /// </summary>
        public HttpStatusCode? StatusCode { get; }

        /// <summary>
        /// Gets the error code returned by the ClickUp API, if available.
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// Gets the correlation ID for tracking the error across systems.
        /// </summary>
        public string CorrelationId { get; }

        /// <summary>
        /// Gets additional context information about the error.
        /// </summary>
        public Dictionary<string, object> Context { get; }

        /// <summary>
        /// Gets the raw response content from the API, if available.
        /// </summary>
        public string? RawResponse { get; }

        /// <summary>
        /// Gets the request URL that caused the error, if available.
        /// </summary>
        public string? RequestUrl { get; }

        /// <summary>
        /// Gets the HTTP method used in the request that caused the error.
        /// </summary>
        public string? HttpMethod { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiException"/> class.
        /// </summary>
        public ClickUpApiException() : this("An error occurred while communicating with the ClickUp API.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ClickUpApiException(string message) : base(message)
        {
            CorrelationId = Guid.NewGuid().ToString();
            Context = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiException"/> class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ClickUpApiException(string message, Exception innerException) : base(message, innerException)
        {
            CorrelationId = Guid.NewGuid().ToString();
            Context = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiException"/> class with detailed error information.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="errorCode">The API error code.</param>
        /// <param name="rawResponse">The raw response content.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClickUpApiException(
            string message,
            HttpStatusCode? statusCode = null,
            string? errorCode = null,
            string? rawResponse = null,
            string? requestUrl = null,
            string? httpMethod = null,
            Exception? innerException = null) : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            RawResponse = rawResponse;
            RequestUrl = requestUrl;
            HttpMethod = httpMethod;
            CorrelationId = Guid.NewGuid().ToString();
            Context = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpApiException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
#pragma warning disable SYSLIB0051
        protected ClickUpApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            StatusCode = (HttpStatusCode?)info.GetValue(nameof(StatusCode), typeof(HttpStatusCode?));
            ErrorCode = info.GetString(nameof(ErrorCode));
            CorrelationId = info.GetString(nameof(CorrelationId)) ?? Guid.NewGuid().ToString();
            RawResponse = info.GetString(nameof(RawResponse));
            RequestUrl = info.GetString(nameof(RequestUrl));
            HttpMethod = info.GetString(nameof(HttpMethod));
            Context = (Dictionary<string, object>?)info.GetValue(nameof(Context), typeof(Dictionary<string, object>)) ?? new Dictionary<string, object>();
        }
#pragma warning restore SYSLIB0051

        /// <summary>
        /// Sets the serialization data for the exception.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
#pragma warning disable SYSLIB0051
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(StatusCode), StatusCode);
            info.AddValue(nameof(ErrorCode), ErrorCode);
            info.AddValue(nameof(CorrelationId), CorrelationId);
            info.AddValue(nameof(RawResponse), RawResponse);
            info.AddValue(nameof(RequestUrl), RequestUrl);
            info.AddValue(nameof(HttpMethod), HttpMethod);
            info.AddValue(nameof(Context), Context);
        }
#pragma warning restore SYSLIB0051

        /// <summary>
        /// Adds context information to the exception.
        /// </summary>
        /// <param name="key">The context key.</param>
        /// <param name="value">The context value.</param>
        /// <returns>The current exception instance for method chaining.</returns>
        public ClickUpApiException WithContext(string key, object value)
        {
            Context[key] = value;
            return this;
        }

        /// <summary>
        /// Adds multiple context items to the exception.
        /// </summary>
        /// <param name="contextItems">The context items to add.</param>
        /// <returns>The current exception instance for method chaining.</returns>
        public ClickUpApiException WithContext(Dictionary<string, object> contextItems)
        {
            foreach (var item in contextItems)
            {
                Context[item.Key] = item.Value;
            }
            return this;
        }

        /// <summary>
        /// Creates a string representation of the exception with detailed information.
        /// </summary>
        /// <returns>A detailed string representation of the exception.</returns>
        public override string ToString()
        {
            var details = new List<string> { base.ToString() };

            if (StatusCode.HasValue)
                details.Add($"Status Code: {StatusCode.Value} ({(int)StatusCode.Value})");

            if (!string.IsNullOrEmpty(ErrorCode))
                details.Add($"Error Code: {ErrorCode}");

            if (!string.IsNullOrEmpty(RequestUrl))
                details.Add($"Request URL: {RequestUrl}");

            if (!string.IsNullOrEmpty(HttpMethod))
                details.Add($"HTTP Method: {HttpMethod}");

            details.Add($"Correlation ID: {CorrelationId}");

            if (Context.Count > 0)
            {
                details.Add("Context:");
                foreach (var item in Context)
                {
                    details.Add($"  {item.Key}: {item.Value}");
                }
            }

            if (!string.IsNullOrEmpty(RawResponse))
            {
                details.Add($"Raw Response: {RawResponse}");
            }

            return string.Join(Environment.NewLine, details);
        }
    }
}