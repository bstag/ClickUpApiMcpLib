using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Exceptions;
using ValidationError = ClickUp.Api.Client.Models.Exceptions.ValidationError;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Centralized error handler for the ClickUp API client.
    /// </summary>
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogger<ErrorHandler> _logger;
        private readonly List<IErrorHandlingStrategy> _strategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="strategies">The error handling strategies.</param>
        public ErrorHandler(ILogger<ErrorHandler> logger, IEnumerable<IErrorHandlingStrategy>? strategies = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _strategies = strategies?.OrderByDescending(s => s.Priority).ToList() ?? new List<IErrorHandlingStrategy>();
            
            // Add default strategies if none provided
            if (!_strategies.Any())
            {
                AddDefaultStrategies();
            }
        }

        /// <inheritdoc />
        public async Task HandleHttpResponseErrorAsync(HttpResponseMessage response, string? requestUrl = null, string? httpMethod = null)
        {
            if (response.IsSuccessStatusCode)
                return;

            var context = CreateErrorContext(requestUrl, httpMethod);
            context.ResponseHeaders = ExtractHeaders(response.Headers);
            
            string? responseContent = null;
            try
            {
                responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                context.ResponseBody = responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read response content for error handling. CorrelationId: {CorrelationId}", context.CorrelationId);
            }

            var exception = response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => CreateAuthenticationException(responseContent, context),
                HttpStatusCode.Forbidden => CreateForbiddenException(responseContent, context),
                HttpStatusCode.NotFound => CreateNotFoundException(responseContent, context),
                HttpStatusCode.TooManyRequests => CreateRateLimitException(response, responseContent, context),
                HttpStatusCode.BadRequest => CreateValidationException(responseContent, context),
                HttpStatusCode.InternalServerError or 
                HttpStatusCode.BadGateway or 
                HttpStatusCode.ServiceUnavailable or 
                HttpStatusCode.GatewayTimeout => CreateServerException(response.StatusCode, responseContent, context),
                _ => CreateGenericApiException(response.StatusCode, responseContent, context)
            };

            await LogErrorAsync(exception, "HTTP response error", requestUrl, httpMethod).ConfigureAwait(false);
            throw exception;
        }

        /// <inheritdoc />
        public ClickUpApiException HandleException(Exception exception, string? context = null, string? requestUrl = null, string? httpMethod = null)
        {
            if (exception is ClickUpApiException clickUpException)
                return clickUpException;

            var errorContext = CreateErrorContext(requestUrl, httpMethod, context);
            
            // Try to find a strategy that can handle this exception
            foreach (var strategy in _strategies)
            {
                if (strategy.CanHandle(exception))
                {
                    return strategy.Handle(exception, errorContext);
                }
            }

            // Default handling
            return exception switch
            {
                HttpRequestException httpEx => new ClickUpNetworkException(
                    $"Network error occurred: {httpEx.Message}", 
                    true, 
                    null, 
                    httpEx)
                    .WithContext("OriginalException", exception.GetType().Name)
                    .WithContext("CorrelationId", errorContext.CorrelationId),
                    
                TaskCanceledException timeoutEx when timeoutEx.InnerException is TimeoutException => 
                    new ClickUpNetworkException(
                        "Request timeout occurred", 
                        true, 
                        null, 
                        timeoutEx)
                        .WithContext("OriginalException", exception.GetType().Name)
                        .WithContext("CorrelationId", errorContext.CorrelationId),
                        
                ArgumentException argEx => new ClickUpValidationException(
                    argEx.ParamName ?? "unknown", 
                    argEx.Message)
                    .WithContext("OriginalException", exception.GetType().Name)
                    .WithContext("CorrelationId", errorContext.CorrelationId),
                    
                _ => new ClickUpApiException(
                    $"An unexpected error occurred: {exception.Message}", 
                    null, 
                    "UNEXPECTED_ERROR", 
                    null, 
                    requestUrl, 
                    httpMethod, 
                    exception)
                    .WithContext("OriginalException", exception.GetType().Name)
                    .WithContext("CorrelationId", errorContext.CorrelationId)
            };
        }

        /// <inheritdoc />
        public ClickUpValidationException HandleValidationError(string propertyName, string errorMessage, object? invalidValue = null, string? validationRule = null)
        {
            var validationError = new ValidationError(propertyName, errorMessage, invalidValue, validationRule);
            return new ClickUpValidationException(new[] { validationError });
        }

        /// <inheritdoc />
        public bool IsTransientError(Exception exception)
        {
            return exception switch
            {
                ClickUpNetworkException networkEx => networkEx.IsTransient,
                ClickUpRateLimitException => true,
                ClickUpApiException apiEx when apiEx.StatusCode.HasValue => apiEx.StatusCode.Value switch
                {
                    HttpStatusCode.InternalServerError or
                    HttpStatusCode.BadGateway or
                    HttpStatusCode.ServiceUnavailable or
                    HttpStatusCode.GatewayTimeout or
                    HttpStatusCode.RequestTimeout => true,
                    _ => false
                },
                HttpRequestException => true,
                TaskCanceledException => true,
                TimeoutException => true,
                _ => false
            };
        }

        /// <inheritdoc />
        public string GetOrCreateCorrelationId(Exception? exception = null)
        {
            if (exception is ClickUpApiException clickUpEx)
                return clickUpEx.CorrelationId;
                
            return Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public async Task LogErrorAsync(Exception exception, string? context = null, string? requestUrl = null, string? httpMethod = null)
        {
            var correlationId = GetOrCreateCorrelationId(exception);
            
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["RequestUrl"] = requestUrl ?? "unknown",
                ["HttpMethod"] = httpMethod ?? "unknown",
                ["Context"] = context ?? "unknown"
            });

            if (exception is ClickUpApiException clickUpEx)
            {
                _logger.LogError(exception, 
                    "ClickUp API error occurred. StatusCode: {StatusCode}, ErrorCode: {ErrorCode}, CorrelationId: {CorrelationId}",
                    clickUpEx.StatusCode,
                    clickUpEx.ErrorCode,
                    clickUpEx.CorrelationId);
            }
            else
            {
                _logger.LogError(exception, 
                    "Unexpected error occurred. ExceptionType: {ExceptionType}, CorrelationId: {CorrelationId}",
                    exception.GetType().Name,
                    correlationId);
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public ErrorContext CreateErrorContext(string? requestUrl = null, string? httpMethod = null, object? additionalContext = null)
        {
            return new ErrorContext
            {
                RequestUrl = requestUrl,
                HttpMethod = httpMethod,
                AdditionalContext = additionalContext,
                Timestamp = DateTimeOffset.UtcNow
            };
        }

        private void AddDefaultStrategies()
        {
            // Add default error handling strategies here if needed
            // This allows for extensibility while providing sensible defaults
        }

        private ClickUpApiException CreateAuthenticationException(string? responseContent, ErrorContext context)
        {
            var message = ExtractErrorMessage(responseContent) ?? "Authentication failed. Please check your API token.";
            return new ClickUpApiAuthenticationException(message, HttpStatusCode.Unauthorized, "AUTHENTICATION_FAILED", responseContent)
                .WithContext("CorrelationId", context.CorrelationId)
                .WithContext("ResponseBody", responseContent);
        }

        private ClickUpApiException CreateForbiddenException(string? responseContent, ErrorContext context)
        {
            var message = ExtractErrorMessage(responseContent) ?? "Access to the requested resource is forbidden.";
            return new ClickUpForbiddenException(message)
                .WithContext("CorrelationId", context.CorrelationId)
                .WithContext("ResponseBody", responseContent);
        }

        private ClickUpApiException CreateNotFoundException(string? responseContent, ErrorContext context)
        {
            var message = ExtractErrorMessage(responseContent) ?? "The requested resource was not found.";
            return new ClickUpNotFoundException(message)
                .WithContext("CorrelationId", context.CorrelationId)
                .WithContext("ResponseBody", responseContent);
        }

        private ClickUpApiException CreateRateLimitException(HttpResponseMessage response, string? responseContent, ErrorContext context)
        {
            var message = ExtractErrorMessage(responseContent) ?? "Rate limit exceeded.";
            
            // Extract rate limit information from headers
            int? retryAfter = null;
            if (response.Headers.RetryAfter?.Delta.HasValue == true)
            {
                retryAfter = (int)response.Headers.RetryAfter.Delta.Value.TotalSeconds;
            }

            DateTimeOffset? resetTime = null;
            if (response.Headers.RetryAfter?.Date.HasValue == true)
            {
                resetTime = response.Headers.RetryAfter.Date.Value;
            }
            
            return new ClickUpRateLimitException(
                message, 
                retryAfter,
                resetTime)
                .WithContext("CorrelationId", context.CorrelationId)
                .WithContext("ResponseBody", responseContent);
        }

        private ClickUpApiException CreateValidationException(string? responseContent, ErrorContext context)
        {
            var message = ExtractErrorMessage(responseContent) ?? "Validation failed.";
            var validationErrors = ExtractValidationErrors(responseContent);
            
            return new ClickUpValidationException(message, validationErrors)
                .WithContext("CorrelationId", context.CorrelationId)
                .WithContext("ResponseBody", responseContent);
        }

        private ClickUpApiException CreateServerException(HttpStatusCode statusCode, string? responseContent, ErrorContext context)
        {
            var message = ExtractErrorMessage(responseContent) ?? $"Server error occurred: {statusCode}";
            return new ClickUpApiException(message, statusCode, "SERVER_ERROR", responseContent, context.RequestUrl, context.HttpMethod)
                .WithContext("CorrelationId", context.CorrelationId);
        }

        private ClickUpApiException CreateGenericApiException(HttpStatusCode statusCode, string? responseContent, ErrorContext context)
        {
            var message = ExtractErrorMessage(responseContent) ?? $"API error occurred: {statusCode}";
            return new ClickUpApiException(message, statusCode, "API_ERROR", responseContent, context.RequestUrl, context.HttpMethod)
                .WithContext("CorrelationId", context.CorrelationId);
        }

        private string? ExtractErrorMessage(string? responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent))
                return null;

            try
            {
                using var document = JsonDocument.Parse(responseContent);
                
                // Try common error message fields
                if (document.RootElement.TryGetProperty("error", out var errorElement))
                {
                    if (errorElement.ValueKind == JsonValueKind.String)
                        return errorElement.GetString();
                    if (errorElement.TryGetProperty("message", out var messageElement))
                        return messageElement.GetString();
                }
                
                if (document.RootElement.TryGetProperty("message", out var msgElement))
                    return msgElement.GetString();
                    
                if (document.RootElement.TryGetProperty("err", out var errElement))
                    return errElement.GetString();
            }
            catch (JsonException)
            {
                // If JSON parsing fails, return the raw content (truncated if too long)
                return responseContent.Length > 500 ? responseContent.Substring(0, 500) + "..." : responseContent;
            }

            return null;
        }

        private List<ValidationError>? ExtractValidationErrors(string? responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent))
                return null;

            try
            {
                using var document = JsonDocument.Parse(responseContent);
                var errors = new List<ValidationError>();
                
                // Try to extract validation errors from common structures
                if (document.RootElement.TryGetProperty("errors", out var errorsElement) && 
                    errorsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var errorElement in errorsElement.EnumerateArray())
                    {
                        var field = errorElement.TryGetProperty("field", out var fieldElement) ? fieldElement.GetString() : "unknown";
                        var message = errorElement.TryGetProperty("message", out var msgElement) ? msgElement.GetString() : "Validation error";
                        
                        if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(message))
                        {
                            errors.Add(new ValidationError(field, message));
                        }
                    }
                }
                
                return errors.Any() ? errors : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private Dictionary<string, string> ExtractHeaders(System.Net.Http.Headers.HttpHeaders headers)
        {
            var result = new Dictionary<string, string>();
            
            foreach (var header in headers)
            {
                result[header.Key] = string.Join(", ", header.Value);
            }
            
            return result;
        }
    }
}