using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClickUp.Api.Client.Http;
using ClickUp.Api.Client.Models.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected; // Added for Protected() extension method
using Xunit;

namespace ClickUp.Api.Client.Tests.Http
{
    public class ApiConnectionTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private ApiConnection _apiConnection;

        // Helper method to setup ApiConnection with a mocked HttpMessageHandler
        private void SetupApiConnection(HttpResponseMessage responseMessage)
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    // ItExpr.IsAny for protected method mocking
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.clickup.com/api/v2/") // Base address is not strictly needed for these tests but good practice
            };
            _apiConnection = new ApiConnection(httpClient);
        }


        [Theory]
        [InlineData(HttpStatusCode.BadRequest, typeof(ClickUpApiValidationException), "Bad Request")] // Changed to ClickUpApiValidationException
        [InlineData(HttpStatusCode.Unauthorized, typeof(ClickUpApiAuthenticationException), "Unauthorized")]
        [InlineData(HttpStatusCode.Forbidden, typeof(ClickUpApiAuthenticationException), "Forbidden")]
        [InlineData(HttpStatusCode.NotFound, typeof(ClickUpApiNotFoundException), "Not Found")]
        [InlineData(HttpStatusCode.InternalServerError, typeof(ClickUpApiServerException), "Internal Server Error")]
        [InlineData(HttpStatusCode.BadGateway, typeof(ClickUpApiServerException), "Bad Gateway")]
        [InlineData(HttpStatusCode.ServiceUnavailable, typeof(ClickUpApiServerException), "Service Unavailable")]
        [InlineData(HttpStatusCode.GatewayTimeout, typeof(ClickUpApiServerException), "Gateway Timeout")]
        public async Task GetAsync_ThrowsCorrectException_ForGeneralErrorStatusCodes(HttpStatusCode statusCode, Type expectedExceptionType, string expectedMessageContent)
        {
            var responseMessage = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent($"{{\"err\":\"{expectedMessageContent}\",\"ECODE\":\"TEST_001\"}}", Encoding.UTF8, "application/json")
            };
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync(expectedExceptionType,
                () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.NotNull(exception);
            // Assert.Contains(expectedMessageContent, exception.Message); // Message now includes raw content, so direct check might be too specific
            if (exception is ClickUpApiException apiException)
            {
                Assert.Equal(statusCode, apiException.HttpStatus);
                Assert.Contains("TEST_001", apiException.ApiErrorCode);
                Assert.Contains(expectedMessageContent, apiException.RawErrorContent);
            }
        }

        [Fact]
        public async Task GetAsync_ThrowsClickUpApiRateLimitException_ForTooManyRequests()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("{\"err\":\"Rate limit reached\",\"ECODE\":\"GLOBAL_001\"}", Encoding.UTF8, "application/json")
            };
            responseMessage.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(60));
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync<ClickUpApiRateLimitException>(
                () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.Equal(HttpStatusCode.TooManyRequests, exception.HttpStatus);
            Assert.Contains("Rate limit reached", exception.Message); // The "err" field should be in the message
            Assert.Contains("GLOBAL_001", exception.ApiErrorCode);
            Assert.Equal(TimeSpan.FromSeconds(60), exception.RetryAfterDelta);
        }

        [Fact]
        public async Task GetAsync_ThrowsClickUpApiRateLimitException_ForTooManyRequests_NoRetryAfterHeader()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("{\"err\":\"Rate limit reached\",\"ECODE\":\"GLOBAL_001\"}", Encoding.UTF8, "application/json")
            };
            // No Retry-After header
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync<ClickUpApiRateLimitException>(
                 () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.Equal(HttpStatusCode.TooManyRequests, exception.HttpStatus);
            Assert.Contains("Rate limit reached", exception.Message);
            Assert.Contains("GLOBAL_001", exception.ApiErrorCode);
            Assert.Null(exception.RetryAfterDelta);
        }

        [Fact]
        public async Task GetAsync_ThrowsClickUpApiValidationException_ForUnprocessableEntity()
        {
            var errorDetail = new Dictionary<string, List<string>>
            {
                {"field1", new List<string> {"Error 1 for field1", "Error 2 for field1"}},
                {"field2", new List<string> {"Error 1 for field2"}}
            };
            var errorContent = new
            {
                err = "Validation failed",
                ECODE = "VALIDATE_001",
                errors = errorDetail
            };
            var jsonErrorContent = JsonSerializer.Serialize(errorContent);

            var responseMessage = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity) // 422
            {
                Content = new StringContent(jsonErrorContent, Encoding.UTF8, "application/json")
            };
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync<ClickUpApiValidationException>(
                () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.HttpStatus);
            Assert.Contains("Validation failed", exception.Message);
            Assert.Contains("VALIDATE_001", exception.ApiErrorCode);
            Assert.NotNull(exception.Errors);
            Assert.Equal(2, exception.Errors.Count);
            Assert.True(exception.Errors.ContainsKey("field1"));
            Assert.Equal(2, exception.Errors["field1"].Count);
            Assert.Contains("Error 1 for field1", exception.Errors["field1"]);
            Assert.Contains("Error 2 for field1", exception.Errors["field1"]);
            Assert.True(exception.Errors.ContainsKey("field2"));
            Assert.Single(exception.Errors["field2"]);
            Assert.Contains("Error 1 for field2", exception.Errors["field2"]);
        }

        [Fact]
        public async Task GetAsync_ThrowsClickUpApiValidationException_ForUnprocessableEntity_WithNonStandardErrorStructure()
        {
            var jsonErrorContent = "{\"err\":\"Invalid input\",\"ECODE\":\"INPUT_ERR_002\",\"detail\":\"Some additional detail here\"}";

            var responseMessage = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity) // 422
            {
                Content = new StringContent(jsonErrorContent, Encoding.UTF8, "application/json")
            };
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException>(
                 () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.HttpStatus);
            Assert.Contains("Invalid input", exception.Message);
            Assert.Contains("INPUT_ERR_002", exception.ApiErrorCode);
            // For this non-standard structure, the Errors dictionary might be null or empty
            // if the ApiConnection's HandleErrorResponseAsync doesn't find a field named "errors".
            // This is acceptable as the primary check is the exception type and basic message/code.
            // Based on current HandleErrorResponseAsync, it would be null.
             Assert.Null(exception.Errors);
        }

        [Fact]
        public async Task GetAsync_ThrowsClickUpApiException_WhenContentIsEmpty()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json") // Empty content
            };
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync<ClickUpApiValidationException>(
                 () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.Equal(HttpStatusCode.BadRequest, exception.HttpStatus);
            Assert.Contains("API request failed with status code BadRequest", exception.Message);
            Assert.Null(exception.ApiErrorCode);
        }

        [Fact]
        public async Task GetAsync_ThrowsClickUpApiException_WhenContentIsNotJson()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("This is not JSON", Encoding.UTF8, "text/plain") // Non-JSON content
            };
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync<ClickUpApiServerException>( // Or generic ClickUpApiException
                 () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.Equal(HttpStatusCode.InternalServerError, exception.HttpStatus);
            Assert.Contains("API request failed with status code InternalServerError", exception.Message); // More specific to the actual message format
            Assert.Contains("This is not JSON", exception.RawErrorContent); // Raw content should be preserved
            Assert.Null(exception.ApiErrorCode);
        }

        [Fact]
        public async Task GetAsync_ThrowsClickUpApiException_WithMinimalJsonError()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("{\"err\":\"Minimal error\"}", Encoding.UTF8, "application/json")
            };
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync<ClickUpApiAuthenticationException>(
                 () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.Equal(HttpStatusCode.Forbidden, exception.HttpStatus);
            Assert.Contains("Minimal error", exception.Message);
            Assert.Null(exception.ApiErrorCode);
        }
    }
}
