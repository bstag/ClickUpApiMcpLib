using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers; // For MediaTypeWithQualityHeaderValue
using System.Text; // For StringContent
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Helpers;
using ClickUp.Api.Client.Models.Exceptions; // For custom exceptions

namespace ClickUp.Api.Client.Http
{
    /// <summary>
    /// Implementation of <see cref="IApiConnection"/> using <see cref="HttpClient"/>.
    /// </summary>
    public class ApiConnection : IApiConnection
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiConnection"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if httpClient is null.</exception>
        public ApiConnection(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc />
        public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent || response.Content == null)
                    {
                        return default;
                    }
                    return await response.Content.ReadFromJsonAsync<TResponse>(JsonSerializerOptionsHelper.Options, cancellationToken).ConfigureAwait(false);
                }

                await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
                return default; // Should not be reached due to HandleErrorResponseAsync throwing
            }
            catch (Exception ex) when (ex is not ClickUpApiException) // Catch non-ClickUpApi exceptions and wrap them
            {
                throw new ClickUpApiRequestException($"Request failed for GET {endpoint}: {ex.Message}", null, null, null, ex);
            }
        }

        /// <inheritdoc />
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
        {
            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload, JsonSerializerOptionsHelper.Options);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent || response.Content == null)
                    {
                        return default;
                    }
                    return await response.Content.ReadFromJsonAsync<TResponse>(JsonSerializerOptionsHelper.Options, cancellationToken).ConfigureAwait(false);
                }

                await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
                return default; // Should not be reached
            }
            catch (Exception ex) when (ex is not ClickUpApiException)
            {
                throw new ClickUpApiRequestException($"Request failed for POST {endpoint}: {ex.Message}", null, null, null, ex);
            }
        }

        /// <inheritdoc />
        public async Task PostAsync<TRequest>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
        {
            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload, JsonSerializerOptionsHelper.Options);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
                }
                // If successful, no content is expected to be returned or processed.
            }
            catch (Exception ex) when (ex is not ClickUpApiException)
            {
                throw new ClickUpApiRequestException($"Request failed for POST {endpoint} (no response body expected): {ex.Message}", null, null, null, ex);
            }
        }

        /// <inheritdoc />
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
        {
            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload, JsonSerializerOptionsHelper.Options);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Put, endpoint) { Content = content };
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent || response.Content == null)
                    {
                        return default;
                    }
                    return await response.Content.ReadFromJsonAsync<TResponse>(JsonSerializerOptionsHelper.Options, cancellationToken).ConfigureAwait(false);
                }

                await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
                return default; // Should not be reached
            }
            catch (Exception ex) when (ex is not ClickUpApiException)
            {
                throw new ClickUpApiRequestException($"Request failed for PUT {endpoint}: {ex.Message}", null, null, null, ex);
            }
        }

        /// <inheritdoc />
        public async Task PutAsync<TRequest>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
        {
            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload, JsonSerializerOptionsHelper.Options);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Put, endpoint) { Content = content };
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is not ClickUpApiException)
            {
                throw new ClickUpApiRequestException($"Request failed for PUT {endpoint} (no response body expected): {ex.Message}", null, null, null, ex);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is not ClickUpApiException)
            {
                throw new ClickUpApiRequestException($"Request failed for DELETE {endpoint}: {ex.Message}", null, null, null, ex);
            }
        }

        private async Task HandleErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            string rawErrorContent = string.Empty;
            if (response.Content != null)
            {
                rawErrorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }

            // Attempt to parse ClickUp specific error code and message if a standard format exists
            // For now, we'll use a generic message and pass rawErrorContent.
            // A more sophisticated approach would be to deserialize rawErrorContent into a ClickUpError DTO.
            string? apiErrorCode = null;
            string errorMessage = $"API request failed with status code {response.StatusCode}.";

            // Example: if ClickUp returns { "ECODE": "X_XXX", "err": "Error message" }
            // try
            // {
            //     var errorDto = JsonSerializer.Deserialize<ClickUpErrorDto>(rawErrorContent, JsonSerializerOptionsHelper.Options);
            //     if (errorDto != null)
            //     {
            //         apiErrorCode = errorDto.ErrorCode; // Assuming ClickUpErrorDto has ErrorCode property
            //         errorMessage = errorDto.ErrorMessage ?? errorMessage; // Assuming ClickUpErrorDto has ErrorMessage property
            //     }
            // }
            // catch (JsonException) { /* Ignore if content is not the expected error DTO */ }


            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    throw new ClickUpApiAuthenticationException(errorMessage, response.StatusCode, apiErrorCode, rawErrorContent);
                case HttpStatusCode.NotFound:
                    throw new ClickUpApiNotFoundException(errorMessage, response.StatusCode, apiErrorCode, rawErrorContent);
                case HttpStatusCode.TooManyRequests:
                    TimeSpan? retryAfterDelta = null;
                    DateTimeOffset? retryAfterDate = null;
                    if (response.Headers.RetryAfter?.Delta.HasValue ?? false)
                        retryAfterDelta = response.Headers.RetryAfter.Delta;
                    if (response.Headers.RetryAfter?.Date.HasValue ?? false)
                        retryAfterDate = response.Headers.RetryAfter.Date;
                    throw new ClickUpApiRateLimitException(errorMessage, response.StatusCode, apiErrorCode, rawErrorContent, retryAfterDelta, retryAfterDate);
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.UnprocessableEntity: // Often used for validation errors
                    // IReadOnlyDictionary<string, IReadOnlyList<string>>? validationErrors = null;
                    // if (!string.IsNullOrWhiteSpace(rawErrorContent)) { /* Try to parse validationErrors */ }
                    throw new ClickUpApiValidationException(errorMessage, response.StatusCode, apiErrorCode, rawErrorContent, null /* validationErrors */);
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                    throw new ClickUpApiServerException(errorMessage, response.StatusCode, apiErrorCode, rawErrorContent);
                default:
                    throw new ClickUpApiException(errorMessage, response.StatusCode, apiErrorCode, rawErrorContent);
            }
        }

        /// <inheritdoc />
        public async Task<TResponse?> PostMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask; // To allow async keyword
            throw new NotImplementedException("Actual HTTP POST multipart logic to be implemented.");
            // Or return await Task.FromResult<TResponse?>(default);
        }

        /// <inheritdoc />
        public async Task<TResponse?> DeleteAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent || response.Content == null)
                    {
                        return default;
                    }
                    return await response.Content.ReadFromJsonAsync<TResponse>(JsonSerializerOptionsHelper.Options, cancellationToken).ConfigureAwait(false);
                }

                await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
                return default; // Should not be reached
            }
            catch (Exception ex) when (ex is not ClickUpApiException)
            {
                throw new ClickUpApiRequestException($"Request failed for DELETE {endpoint}: {ex.Message}", null, null, null, ex);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync<TRequest>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
        {
            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload, JsonSerializerOptionsHelper.Options);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
                {
                    Content = content
                };
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(response, cancellationToken).ConfigureAwait(false);
                }
                // Typically, successful DELETEs with a body might still return 204 No Content or an empty 200 OK.
            }
            catch (Exception ex) when (ex is not ClickUpApiException)
            {
                throw new ClickUpApiRequestException($"Request failed for DELETE {endpoint} with payload: {ex.Message}", null, null, null, ex);
            }
        }
    }
}
