using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Exceptions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Default error handling strategy for network-related exceptions.
    /// </summary>
    public class NetworkErrorHandlingStrategy : IErrorHandlingStrategy
    {
        /// <inheritdoc />
        public int Priority => 100;

        /// <inheritdoc />
        public bool CanHandle(Exception exception)
        {
            return exception is HttpRequestException or 
                   TaskCanceledException or 
                   TimeoutException or
                   SocketException;
        }

        /// <inheritdoc />
        public ClickUpApiException Handle(Exception exception, ErrorContext context)
        {
            return exception switch
            {
                HttpRequestException httpEx => new ClickUpNetworkException(
                    $"Network error: {httpEx.Message}",
                    true,
                    null,
                    httpEx)
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("RequestUrl", context.RequestUrl ?? "unknown")
                    .WithContext("HttpMethod", context.HttpMethod ?? "unknown"),

                TaskCanceledException cancelEx when cancelEx.InnerException is TimeoutException =>
                    new ClickUpNetworkException(
                        "Request timeout occurred",
                        true,
                        null,
                        cancelEx)
                        .WithContext("CorrelationId", context.CorrelationId ?? "unknown")
                    .WithContext("RequestUrl", context.RequestUrl ?? "unknown")
                    .WithContext("HttpMethod", context.HttpMethod ?? "unknown"),

                TaskCanceledException cancelEx => new ClickUpNetworkException(
                    "Request was cancelled",
                    false,
                    null,
                    cancelEx)
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("RequestUrl", context.RequestUrl ?? "unknown")
                    .WithContext("HttpMethod", context.HttpMethod ?? "unknown"),

                TimeoutException timeoutEx => new ClickUpNetworkException(
                    "Request timeout occurred",
                    true,
                    null,
                    timeoutEx)
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("RequestUrl", context.RequestUrl ?? "unknown")
                    .WithContext("HttpMethod", context.HttpMethod ?? "unknown"),

                _ => new ClickUpNetworkException(
                    $"Network error: {exception.Message}",
                    true,
                    null,
                    exception)
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("RequestUrl", context.RequestUrl ?? "unknown")
                    .WithContext("HttpMethod", context.HttpMethod ?? "unknown")
            };
        }
    }

    /// <summary>
    /// Error handling strategy for validation-related exceptions.
    /// </summary>
    public class ValidationErrorHandlingStrategy : IErrorHandlingStrategy
    {
        /// <inheritdoc />
        public int Priority => 90;

        /// <inheritdoc />
        public bool CanHandle(Exception exception)
        {
            return exception is ArgumentException or 
                   ArgumentNullException or 
                   ArgumentOutOfRangeException or
                   FormatException or
                   InvalidOperationException;
        }

        /// <inheritdoc />
        public ClickUpApiException Handle(Exception exception, ErrorContext context)
        {
            return exception switch
            {
                ArgumentNullException argNullEx => new ClickUpValidationException(
                    argNullEx.ParamName ?? "unknown",
                    $"Parameter cannot be null: {argNullEx.ParamName}")
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("ParameterName", argNullEx.ParamName ?? "unknown"),

                ArgumentOutOfRangeException argRangeEx => new ClickUpValidationException(
                    argRangeEx.ParamName ?? "unknown",
                    $"Parameter value is out of range: {argRangeEx.ParamName}",
                    argRangeEx.ActualValue)
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("ParameterName", argRangeEx.ParamName ?? "unknown")
                    .WithContext("ActualValue", argRangeEx.ActualValue ?? "null"),

                ArgumentException argEx => new ClickUpValidationException(
                    argEx.ParamName ?? "unknown",
                    argEx.Message)
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("ParameterName", argEx.ParamName ?? "unknown"),

                FormatException formatEx => new ClickUpValidationException(
                    "format",
                    $"Invalid format: {formatEx.Message}")
                    .WithContext("CorrelationId", context.CorrelationId),

                InvalidOperationException invalidOpEx => new ClickUpValidationException(
                    "operation",
                    $"Invalid operation: {invalidOpEx.Message}")
                    .WithContext("CorrelationId", context.CorrelationId),

                _ => new ClickUpValidationException(
                    "unknown",
                    $"Validation error: {exception.Message}")
                    .WithContext("CorrelationId", context.CorrelationId)
            };
        }
    }

    /// <summary>
    /// Error handling strategy for JSON serialization/deserialization exceptions.
    /// </summary>
    public class JsonErrorHandlingStrategy : IErrorHandlingStrategy
    {
        /// <inheritdoc />
        public int Priority => 80;

        /// <inheritdoc />
        public bool CanHandle(Exception exception)
        {
            return exception is System.Text.Json.JsonException;
        }

        /// <inheritdoc />
        public ClickUpApiException Handle(Exception exception, ErrorContext context)
        {
            var message = exception switch
            {
                System.Text.Json.JsonException jsonEx => $"JSON parsing error: {jsonEx.Message}",
                _ => $"JSON error: {exception.Message}"
            };

            return new ClickUpApiException(
                message,
                HttpStatusCode.BadRequest,
                "JSON_PARSE_ERROR",
                null,
                context.RequestUrl,
                context.HttpMethod,
                exception)
                .WithContext("CorrelationId", context.CorrelationId)
                .WithContext("ErrorType", "JsonParsingError");
        }
    }

    /// <summary>
    /// Error handling strategy for security-related exceptions.
    /// </summary>
    public class SecurityErrorHandlingStrategy : IErrorHandlingStrategy
    {
        /// <inheritdoc />
        public int Priority => 95;

        /// <inheritdoc />
        public bool CanHandle(Exception exception)
        {
            return exception is UnauthorizedAccessException or
                   System.Security.SecurityException or
                   System.Security.Authentication.AuthenticationException;
        }

        /// <inheritdoc />
        public ClickUpApiException Handle(Exception exception, ErrorContext context)
        {
            return exception switch
            {
                UnauthorizedAccessException unauthorizedEx => new ClickUpAuthenticationException(
                    $"Unauthorized access: {unauthorizedEx.Message}")
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("RequestUrl", context.RequestUrl ?? "unknown"),

                System.Security.SecurityException securityEx => new ClickUpAuthenticationException(
                    $"Security error: {securityEx.Message}")
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("RequestUrl", context.RequestUrl ?? "unknown"),

                System.Security.Authentication.AuthenticationException authEx => new ClickUpAuthenticationException(
                    $"Authentication error: {authEx.Message}")
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("RequestUrl", context.RequestUrl ?? "unknown"),

                _ => new ClickUpAuthenticationException(
                    $"Security error: {exception.Message}")
                    .WithContext("CorrelationId", context.CorrelationId)
                    .WithContext("RequestUrl", context.RequestUrl ?? "unknown")
            };
        }
    }

    /// <summary>
    /// Fallback error handling strategy for any unhandled exceptions.
    /// </summary>
    public class FallbackErrorHandlingStrategy : IErrorHandlingStrategy
    {
        /// <inheritdoc />
        public int Priority => 0; // Lowest priority - fallback

        /// <inheritdoc />
        public bool CanHandle(Exception exception)
        {
            return true; // Can handle any exception as fallback
        }

        /// <inheritdoc />
        public ClickUpApiException Handle(Exception exception, ErrorContext context)
        {
            return new ClickUpApiException(
                $"An unexpected error occurred: {exception.Message}",
                null,
                "UNEXPECTED_ERROR",
                null,
                context.RequestUrl,
                context.HttpMethod,
                exception)
                .WithContext("CorrelationId", context.CorrelationId)
                .WithContext("OriginalExceptionType", exception.GetType().FullName ?? "Unknown")
                .WithContext("StackTrace", exception.StackTrace ?? "No stack trace available");
        }
    }
}