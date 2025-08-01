using System;
using System.Net;
using System.Runtime.Serialization;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Exception thrown when authentication fails with the ClickUp API.
    /// </summary>
    [Serializable]
    public class ClickUpAuthenticationException : ClickUpApiException
    {
        /// <summary>
        /// Gets the authentication method that failed.
        /// </summary>
        public string? AuthenticationMethod { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpAuthenticationException"/> class.
        /// </summary>
        public ClickUpAuthenticationException() : base("Authentication failed.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpAuthenticationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ClickUpAuthenticationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpAuthenticationException"/> class with detailed information.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="authenticationMethod">The authentication method that failed.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClickUpAuthenticationException(string message, string? authenticationMethod = null, Exception? innerException = null)
            : base(message, HttpStatusCode.Unauthorized, "AUTHENTICATION_FAILED", null, null, null, innerException)
        {
            AuthenticationMethod = authenticationMethod;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpAuthenticationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
#pragma warning disable SYSLIB0051
        protected ClickUpAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            AuthenticationMethod = info.GetString(nameof(AuthenticationMethod));
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
            info.AddValue(nameof(AuthenticationMethod), AuthenticationMethod);
        }
#pragma warning restore SYSLIB0051
    }

    /// <summary>
    /// Exception thrown when rate limiting occurs with the ClickUp API.
    /// </summary>
    [Serializable]
    public class ClickUpRateLimitException : ClickUpApiException
    {
        /// <summary>
        /// Gets the time when the rate limit will reset.
        /// </summary>
        public DateTimeOffset? ResetTime { get; }

        /// <summary>
        /// Gets the number of seconds to wait before retrying.
        /// </summary>
        public int? RetryAfterSeconds { get; }

        /// <summary>
        /// Gets the current rate limit.
        /// </summary>
        public int? RateLimit { get; }

        /// <summary>
        /// Gets the remaining requests in the current rate limit window.
        /// </summary>
        public int? RemainingRequests { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpRateLimitException"/> class.
        /// </summary>
        public ClickUpRateLimitException() : base("Rate limit exceeded.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpRateLimitException"/> class with detailed rate limit information.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="retryAfterSeconds">The number of seconds to wait before retrying.</param>
        /// <param name="resetTime">The time when the rate limit will reset.</param>
        /// <param name="rateLimit">The current rate limit.</param>
        /// <param name="remainingRequests">The remaining requests in the current window.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClickUpRateLimitException(
            string message,
            int? retryAfterSeconds = null,
            DateTimeOffset? resetTime = null,
            int? rateLimit = null,
            int? remainingRequests = null,
            Exception? innerException = null)
            : base(message, HttpStatusCode.TooManyRequests, "RATE_LIMIT_EXCEEDED", null, null, null, innerException)
        {
            RetryAfterSeconds = retryAfterSeconds;
            ResetTime = resetTime;
            RateLimit = rateLimit;
            RemainingRequests = remainingRequests;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpRateLimitException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
#pragma warning disable SYSLIB0051
        protected ClickUpRateLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ResetTime = (DateTimeOffset?)info.GetValue(nameof(ResetTime), typeof(DateTimeOffset?));
            RetryAfterSeconds = (int?)info.GetValue(nameof(RetryAfterSeconds), typeof(int?));
            RateLimit = (int?)info.GetValue(nameof(RateLimit), typeof(int?));
            RemainingRequests = (int?)info.GetValue(nameof(RemainingRequests), typeof(int?));
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
            info.AddValue(nameof(ResetTime), ResetTime);
            info.AddValue(nameof(RetryAfterSeconds), RetryAfterSeconds);
            info.AddValue(nameof(RateLimit), RateLimit);
            info.AddValue(nameof(RemainingRequests), RemainingRequests);
        }
#pragma warning restore SYSLIB0051
    }

    /// <summary>
    /// Exception thrown when a requested resource is not found.
    /// </summary>
    [Serializable]
    public class ClickUpNotFoundException : ClickUpApiException
    {
        /// <summary>
        /// Gets the type of resource that was not found.
        /// </summary>
        public string? ResourceType { get; }

        /// <summary>
        /// Gets the identifier of the resource that was not found.
        /// </summary>
        public string? ResourceId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpNotFoundException"/> class.
        /// </summary>
        public ClickUpNotFoundException() : base("The requested resource was not found.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpNotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ClickUpNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpNotFoundException"/> class with resource information.
        /// </summary>
        /// <param name="resourceType">The type of resource that was not found.</param>
        /// <param name="resourceId">The identifier of the resource that was not found.</param>
        /// <param name="message">The custom error message, or null to use default.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClickUpNotFoundException(
            string resourceType,
            string resourceId,
            string? message = null,
            Exception? innerException = null)
            : base(message ?? $"{resourceType} with ID '{resourceId}' was not found.", HttpStatusCode.NotFound, "RESOURCE_NOT_FOUND", null, null, null, innerException)
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpNotFoundException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
#pragma warning disable SYSLIB0051
        protected ClickUpNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ResourceType = info.GetString(nameof(ResourceType));
            ResourceId = info.GetString(nameof(ResourceId));
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
            info.AddValue(nameof(ResourceType), ResourceType);
            info.AddValue(nameof(ResourceId), ResourceId);
        }
#pragma warning restore SYSLIB0051
    }

    /// <summary>
    /// Exception thrown when access to a resource is forbidden.
    /// </summary>
    [Serializable]
    public class ClickUpForbiddenException : ClickUpApiException
    {
        /// <summary>
        /// Gets the required permission that is missing.
        /// </summary>
        public string? RequiredPermission { get; }

        /// <summary>
        /// Gets the resource that access was denied to.
        /// </summary>
        public string? Resource { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpForbiddenException"/> class.
        /// </summary>
        public ClickUpForbiddenException() : base("Access to the requested resource is forbidden.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpForbiddenException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ClickUpForbiddenException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpForbiddenException"/> class with permission information.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="requiredPermission">The required permission that is missing.</param>
        /// <param name="resource">The resource that access was denied to.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClickUpForbiddenException(
            string message,
            string? requiredPermission = null,
            string? resource = null,
            Exception? innerException = null)
            : base(message, HttpStatusCode.Forbidden, "ACCESS_FORBIDDEN", null, null, null, innerException)
        {
            RequiredPermission = requiredPermission;
            Resource = resource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpForbiddenException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
#pragma warning disable SYSLIB0051
        protected ClickUpForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RequiredPermission = info.GetString(nameof(RequiredPermission));
            Resource = info.GetString(nameof(Resource));
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
            info.AddValue(nameof(RequiredPermission), RequiredPermission);
            info.AddValue(nameof(Resource), Resource);
        }
#pragma warning restore SYSLIB0051
    }

    /// <summary>
    /// Exception thrown when a network or connectivity error occurs.
    /// </summary>
    [Serializable]
    public class ClickUpNetworkException : ClickUpApiException
    {
        /// <summary>
        /// Gets a value indicating whether the error is likely transient and retryable.
        /// </summary>
        public bool IsTransient { get; }

        /// <summary>
        /// Gets the timeout value that was exceeded, if applicable.
        /// </summary>
        public TimeSpan? Timeout { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpNetworkException"/> class.
        /// </summary>
        public ClickUpNetworkException() : base("A network error occurred while communicating with the ClickUp API.")
        {
            IsTransient = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpNetworkException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="isTransient">Whether the error is likely transient and retryable.</param>
        /// <param name="timeout">The timeout value that was exceeded.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClickUpNetworkException(
            string message,
            bool isTransient = true,
            TimeSpan? timeout = null,
            Exception? innerException = null)
            : base(message, null, "NETWORK_ERROR", null, null, null, innerException)
        {
            IsTransient = isTransient;
            Timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpNetworkException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
#pragma warning disable SYSLIB0051
        protected ClickUpNetworkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            IsTransient = info.GetBoolean(nameof(IsTransient));
            Timeout = (TimeSpan?)info.GetValue(nameof(Timeout), typeof(TimeSpan?));
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
            info.AddValue(nameof(IsTransient), IsTransient);
            info.AddValue(nameof(Timeout), Timeout);
        }
#pragma warning restore SYSLIB0051
    }
}