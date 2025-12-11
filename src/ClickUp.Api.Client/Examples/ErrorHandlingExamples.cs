using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Exceptions;
using ClickUp.Api.Client.Services;

namespace ClickUp.Api.Client.Examples
{
    /// <summary>
    /// Examples demonstrating the centralized error handling system.
    /// </summary>
    public static class ErrorHandlingExamples
    {
        /// <summary>
        /// Example of basic error handler usage in a service.
        /// </summary>
        public static async Task BasicErrorHandlingExample()
        {
            // Setup (normally done via DI)
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<ErrorHandler>();
            var errorHandler = new ErrorHandler(logger);

            try
            {
                // Simulate an HTTP response error
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("{\"error\": \"Invalid API token\"}")
                };

                await errorHandler.HandleHttpResponseErrorAsync(
                    response, 
                    "https://api.clickup.com/api/v2/team", 
                    "GET");
            }
            catch (ClickUpAuthenticationException authEx)
            {
                Console.WriteLine($"Authentication error: {authEx.Message}");
                Console.WriteLine($"Correlation ID: {authEx.CorrelationId}");
            }
        }

        /// <summary>
        /// Example of handling different types of HTTP errors.
        /// </summary>
        public static async Task HttpErrorHandlingExample()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<ErrorHandler>();
            var errorHandler = new ErrorHandler(logger);

            var testCases = new[]
            {
                (HttpStatusCode.Unauthorized, "{\"error\": \"Invalid token\"}", "Authentication"),
                (HttpStatusCode.Forbidden, "{\"error\": \"Access denied\"}", "Authorization"),
                (HttpStatusCode.NotFound, "{\"error\": \"Resource not found\"}", "Not Found"),
                (HttpStatusCode.TooManyRequests, "{\"error\": \"Rate limit exceeded\"}", "Rate Limit"),
                (HttpStatusCode.BadRequest, "{\"errors\": [{\"field\": \"name\", \"message\": \"Required\"}]}", "Validation"),
                (HttpStatusCode.InternalServerError, "{\"error\": \"Server error\"}", "Server Error")
            };

            foreach (var (statusCode, content, description) in testCases)
            {
                try
                {
                    var response = new HttpResponseMessage(statusCode)
                    {
                        Content = new StringContent(content)
                    };

                    if (statusCode == HttpStatusCode.TooManyRequests)
                    {
                        response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(60));
                    }

                    await errorHandler.HandleHttpResponseErrorAsync(
                        response, 
                        $"https://api.clickup.com/api/v2/test/{description.ToLower()}", 
                        "GET");
                }
                catch (ClickUpApiException ex)
                {
                    Console.WriteLine($"{description} Error:");
                    Console.WriteLine($"  Type: {ex.GetType().Name}");
                    Console.WriteLine($"  Message: {ex.Message}");
                    Console.WriteLine($"  Status Code: {ex.StatusCode}");
                    Console.WriteLine($"  Error Code: {ex.ErrorCode}");
                    Console.WriteLine($"  Correlation ID: {ex.CorrelationId}");
                    
                    if (ex is ClickUpRateLimitException rateLimitEx)
                    {
                        Console.WriteLine($"  Retry After: {rateLimitEx.RetryAfterSeconds} seconds");
                    }
                    
                    if (ex is ClickUpValidationException validationEx && validationEx.ValidationErrors.Any())
                    {
                        Console.WriteLine("  Validation Errors:");
                        foreach (var error in validationEx.ValidationErrors)
                        {
                            Console.WriteLine($"    - {error.PropertyName}: {error.ErrorMessage}");
                        }
                    }
                    
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Example of handling general exceptions.
        /// </summary>
        public static void GeneralExceptionHandlingExample()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<ErrorHandler>();
            var errorHandler = new ErrorHandler(logger);

            var testExceptions = new Exception[]
            {
                new ArgumentNullException("apiToken", "API token cannot be null"),
                new ArgumentOutOfRangeException("pageSize", 1000, "Page size must be between 1 and 100"),
                new HttpRequestException("Network error occurred"),
                new TaskCanceledException("Request timeout", new TimeoutException()),
                new System.Text.Json.JsonException("Invalid JSON format"),
                new UnauthorizedAccessException("Access denied"),
                new InvalidOperationException("Invalid operation state")
            };

            foreach (var exception in testExceptions)
            {
                try
                {
                    var handledException = errorHandler.HandleException(
                        exception, 
                        "Test context", 
                        "https://api.clickup.com/api/v2/test", 
                        "GET");
                    
                    throw handledException;
                }
                catch (ClickUpApiException ex)
                {
                    Console.WriteLine($"Handled {exception.GetType().Name}:");
                    Console.WriteLine($"  Converted to: {ex.GetType().Name}");
                    Console.WriteLine($"  Message: {ex.Message}");
                    Console.WriteLine($"  Is Transient: {errorHandler.IsTransientError(ex)}");
                    Console.WriteLine($"  Correlation ID: {ex.CorrelationId}");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Example of validation error handling.
        /// </summary>
        public static void ValidationErrorExample()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<ErrorHandler>();
            var errorHandler = new ErrorHandler(logger);

            try
            {
                // Single validation error
                var validationEx = errorHandler.HandleValidationError(
                    "email", 
                    "Invalid email format", 
                    "invalid-email", 
                    "EmailFormat");
                
                throw validationEx;
            }
            catch (ClickUpValidationException ex)
            {
                Console.WriteLine("Validation Error:");
                Console.WriteLine($"  Property: {ex.PropertyName}");
                Console.WriteLine($"  Message: {ex.Message}");
                Console.WriteLine($"  Invalid Value: {ex.InvalidValue}");
                Console.WriteLine();
            }

            try
            {
                // Multiple validation errors
                var validationErrors = new[]
                {
                    new ValidationError("name", "Name is required"),
                    new ValidationError("email", "Invalid email format", "invalid@"),
                    new ValidationError("age", "Age must be between 18 and 120", 150, "Range")
                };
                
                throw new ClickUpValidationException(validationErrors);
            }
            catch (ClickUpValidationException ex)
            {
                Console.WriteLine("Multiple Validation Errors:");
                foreach (var error in ex.ValidationErrors)
                {
                    Console.WriteLine($"  - {error.PropertyName}: {error.ErrorMessage}");
                    if (error.InvalidValue != null)
                    {
                        Console.WriteLine($"    Invalid Value: {error.InvalidValue}");
                    }
                    if (!string.IsNullOrEmpty(error.ValidationRule))
                    {
                        Console.WriteLine($"    Rule: {error.ValidationRule}");
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Example of using custom error handling strategies.
        /// </summary>
        public static void CustomErrorHandlingStrategyExample()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<ErrorHandler>();
            
            // Create custom strategies
            var strategies = new IErrorHandlingStrategy[]
            {
                new NetworkErrorHandlingStrategy(),
                new ValidationErrorHandlingStrategy(),
                new JsonErrorHandlingStrategy(),
                new SecurityErrorHandlingStrategy(),
                new FallbackErrorHandlingStrategy()
            };
            
            var errorHandler = new ErrorHandler(logger, strategies);

            var testExceptions = new Exception[]
            {
                new HttpRequestException("Connection failed"),
                new ArgumentNullException("parameter"),
                new System.Text.Json.JsonException("Malformed JSON"),
                new UnauthorizedAccessException("Access denied"),
                new NotImplementedException("Feature not implemented")
            };

            foreach (var exception in testExceptions)
            {
                try
                {
                    var handledException = errorHandler.HandleException(exception);
                    throw handledException;
                }
                catch (ClickUpApiException ex)
                {
                    Console.WriteLine($"Strategy handled {exception.GetType().Name}:");
                    Console.WriteLine($"  Result: {ex.GetType().Name}");
                    Console.WriteLine($"  Message: {ex.Message}");
                    Console.WriteLine($"  Correlation ID: {ex.CorrelationId}");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Example of transient error detection.
        /// </summary>
        public static void TransientErrorDetectionExample()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<ErrorHandler>();
            var errorHandler = new ErrorHandler(logger);

            var testExceptions = new Exception[]
            {
                new ClickUpNetworkException("Network error", true),
                new ClickUpRateLimitException("Rate limit exceeded"),
                new ClickUpApiException("Server error", HttpStatusCode.InternalServerError),
                new ClickUpApiException("Bad request", HttpStatusCode.BadRequest),
                new ClickUpAuthenticationException("Invalid token"),
                new HttpRequestException("Connection timeout"),
                new TaskCanceledException("Request cancelled")
            };

            Console.WriteLine("Transient Error Detection:");
            foreach (var exception in testExceptions)
            {
                var isTransient = errorHandler.IsTransientError(exception);
                Console.WriteLine($"  {exception.GetType().Name}: {(isTransient ? "Transient" : "Not Transient")}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Example of error context and correlation ID usage.
        /// </summary>
        public static async Task ErrorContextExample()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<ErrorHandler>();
            var errorHandler = new ErrorHandler(logger);

            // Create error context
            var context = errorHandler.CreateErrorContext(
                "https://api.clickup.com/api/v2/team",
                "GET",
                new { UserId = 12345, Operation = "GetTeam" });

            Console.WriteLine("Error Context:");
            Console.WriteLine($"  Request URL: {context.RequestUrl}");
            Console.WriteLine($"  HTTP Method: {context.HttpMethod}");
            Console.WriteLine($"  Timestamp: {context.Timestamp}");
            Console.WriteLine($"  Additional Context: {context.AdditionalContext}");
            Console.WriteLine();

            try
            {
                // Simulate an error with context
                var exception = new InvalidOperationException("Test error");
                await errorHandler.LogErrorAsync(
                    exception, 
                    "Testing error logging", 
                    context.RequestUrl, 
                    context.HttpMethod);
                
                var handledException = errorHandler.HandleException(exception, "Test context");
                Console.WriteLine($"Generated Correlation ID: {handledException.CorrelationId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during example: {ex.Message}");
            }
        }

        /// <summary>
        /// Example of how to integrate error handling in a service method.
        /// </summary>
        public static async Task ServiceIntegrationExample()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<ErrorHandler>();
            var errorHandler = new ErrorHandler(logger);

            // Simulate a service method
            async Task<string> GetTeamDataAsync(string teamId)
            {
                var requestUrl = $"https://api.clickup.com/api/v2/team/{teamId}";
                const string httpMethod = "GET";

                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(teamId))
                    {
                        throw errorHandler.HandleValidationError(
                            nameof(teamId), 
                            "Team ID cannot be null or empty", 
                            teamId);
                    }

                    // Simulate HTTP call
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(requestUrl);

                    // Handle HTTP errors
                    if (!response.IsSuccessStatusCode)
                    {
                        await errorHandler.HandleHttpResponseErrorAsync(response, requestUrl, httpMethod);
                    }

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException httpEx)
                {
                    // Handle network errors
                    throw errorHandler.HandleException(httpEx, "Network error during team data retrieval", requestUrl, httpMethod);
                }
                catch (TaskCanceledException cancelEx)
                {
                    // Handle timeout errors
                    throw errorHandler.HandleException(cancelEx, "Timeout during team data retrieval", requestUrl, httpMethod);
                }
                catch (Exception ex) when (!(ex is ClickUpApiException))
                {
                    // Handle unexpected errors
                    throw errorHandler.HandleException(ex, "Unexpected error during team data retrieval", requestUrl, httpMethod);
                }
            }

            // Test the service method
            try
            {
                await GetTeamDataAsync(""); // This will trigger validation error
            }
            catch (ClickUpValidationException ex)
            {
                Console.WriteLine($"Service validation error: {ex.Message}");
                Console.WriteLine($"Property: {ex.PropertyName}");
                Console.WriteLine($"Correlation ID: {ex.CorrelationId}");
            }

            try
            {
                await GetTeamDataAsync("invalid-team-id"); // This will trigger HTTP error
            }
            catch (ClickUpApiException ex)
            {
                Console.WriteLine($"Service API error: {ex.Message}");
                Console.WriteLine($"Status Code: {ex.StatusCode}");
                Console.WriteLine($"Correlation ID: {ex.CorrelationId}");
                Console.WriteLine($"Is Transient: {errorHandler.IsTransientError(ex)}");
            }
        }
    }
}