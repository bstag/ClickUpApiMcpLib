// src/ClickUp.Api.Client/Http/ClickUpApiExceptionFactory.cs
namespace ClickUp.Api.Client.Http;

using ClickUp.Api.Client.Models.Exceptions;
using System;
using System.Net;
using System.Net.Http; // For HttpResponseMessage
using System.Text.Json;

public static class ClickUpApiExceptionFactory
{
    // Helper to attempt to parse ClickUp's specific error format
    private static bool TryParseClickUpError(string? jsonContent, out string? errorCode, out string? errorExplain)
    {
        errorCode = null;
        errorExplain = null;
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            return false;
        }

        try
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonContent!))
            {
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("err", out JsonElement errElement))
                {
                    errorExplain = errElement.GetString();
                }
                if (root.TryGetProperty("ECODE", out JsonElement ecodeElement))
                {
                    errorCode = ecodeElement.GetString();
                }
                return !string.IsNullOrWhiteSpace(errorCode) || !string.IsNullOrWhiteSpace(errorExplain);
            }
        }
        catch (JsonException)
        {
            // Invalid JSON, cannot parse
            return false;
        }
    }

    public static ClickUpApiException Create(
        HttpResponseMessage response,
        string? responseContent,
        string? customMessage = null)
    {
        string? apiErrorCode = null;
        string? apiErrorExplain = null;

        if (responseContent != null)
        {
            TryParseClickUpError(responseContent, out apiErrorCode, out apiErrorExplain);
        }

        var baseMessage = customMessage ?? $"API request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).";
        if (!string.IsNullOrWhiteSpace(apiErrorExplain))
        {
            baseMessage += $" ClickUp Error: {apiErrorExplain}";
        }
        if (!string.IsNullOrWhiteSpace(apiErrorCode))
        {
            baseMessage += $" (ECODE: {apiErrorCode})";
        }

        // Specific handling for validation errors (400 or 422) if they contain structured error details
        if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.UnprocessableEntity)
        {
            if (TryParseValidationErrors(responseContent, out var validationErrors))
            {
                return new ClickUpApiValidationException(baseMessage, response.StatusCode, apiErrorCode, responseContent, validationErrors);
            }
        }

        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => new ClickUpApiAuthenticationException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
            HttpStatusCode.Forbidden => new ClickUpApiAuthenticationException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
            HttpStatusCode.NotFound => new ClickUpApiNotFoundException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
            HttpStatusCode.TooManyRequests => new ClickUpApiRateLimitException(baseMessage, response.StatusCode, apiErrorCode, responseContent, GetRetryAfter(response)),
            // HttpStatusCode.BadRequest and HttpStatusCode.UnprocessableEntity are now handled above if they have details,
            // otherwise they fall into the generic ClickUpApiRequestException.
            >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError => new ClickUpApiRequestException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
            >= HttpStatusCode.InternalServerError => new ClickUpApiServerException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
            _ => new ClickUpApiException(baseMessage, response.StatusCode, apiErrorCode, responseContent)
        };
    }

    private static bool TryParseValidationErrors(string? jsonContent, out IReadOnlyDictionary<string, IReadOnlyList<string>>? errors)
    {
        errors = null;
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            return false;
        }

        try
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonContent!))
            {
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("errors", out JsonElement errorsElement) && errorsElement.ValueKind == JsonValueKind.Object)
                {
                    var parsedErrors = new Dictionary<string, IReadOnlyList<string>>();
                    foreach (JsonProperty property in errorsElement.EnumerateObject())
                    {
                        if (property.Value.ValueKind == JsonValueKind.Array)
                        {
                            var fieldErrors = new List<string>();
                            foreach (JsonElement errorValue in property.Value.EnumerateArray())
                            {
                                fieldErrors.Add(errorValue.GetString() ?? string.Empty);
                            }
                            parsedErrors[property.Name] = fieldErrors.AsReadOnly();
                        }
                    }
                    if (parsedErrors.Any())
                    {
                        errors = parsedErrors;
                        return true;
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Invalid JSON, cannot parse validation errors
            return false;
        }
        return false;
    }

    public static ClickUpApiException Create(Exception innerException, string message)
    {
        // For non-HTTP related issues that should still be wrapped
        return new ClickUpApiException(message, innerException);
    }

    private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
    {
        if (response.Headers.RetryAfter?.Delta.HasValue)
            return response.Headers.RetryAfter.Delta.Value;
        if (response.Headers.RetryAfter?.Date.HasValue)
            return response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
        return null;
    }
}
