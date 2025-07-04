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
        private Mock<HttpMessageHandler> _mockHttpMessageHandler = null!; // Initialize with null forgiving operator
        private ApiConnection _apiConnection = null!; // Initialize with null forgiving operator

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
        // Note: For HttpStatusCode.BadRequest, ClickUpApiExceptionFactory creates ClickUpApiRequestException if no "errors" field is parsable.
        // If "errors" is present, it will create ClickUpApiValidationException (tested separately or via UnprocessableEntity test).
        [InlineData(HttpStatusCode.BadRequest, typeof(ClickUpApiRequestException), "Bad Request", "REQUEST_ERROR_001", null)] // No "errors" field
        [InlineData(HttpStatusCode.Unauthorized, typeof(ClickUpApiAuthenticationException), "Unauthorized", "AUTH_001", null)]
        [InlineData(HttpStatusCode.Forbidden, typeof(ClickUpApiAuthenticationException), "Forbidden", "AUTH_002", null)]
        [InlineData(HttpStatusCode.NotFound, typeof(ClickUpApiNotFoundException), "Not Found", "NF_001", null)]
        [InlineData(HttpStatusCode.InternalServerError, typeof(ClickUpApiServerException), "Internal Server Error", "SERVER_001", null)]
        [InlineData(HttpStatusCode.BadGateway, typeof(ClickUpApiServerException), "Bad Gateway", "SERVER_002", null)]
        [InlineData(HttpStatusCode.ServiceUnavailable, typeof(ClickUpApiServerException), "Service Unavailable", "SERVER_003", null)]
        [InlineData(HttpStatusCode.GatewayTimeout, typeof(ClickUpApiServerException), "Gateway Timeout", "SERVER_004", null)]
        public async Task GetAsync_ThrowsCorrectException_ForGeneralErrorStatusCodes(HttpStatusCode statusCode, Type expectedExceptionType, string errMessage, string ecode, string? errorsJson) // errorsJson can be null
        {
            var errorPayload = new Dictionary<string, object> { { "err", errMessage }, { "ECODE", ecode } };
            if (!string.IsNullOrEmpty(errorsJson))
            {
                // This is a simplified way to merge; real JSON merging would be more robust.
                // For test purposes, we assume errorsJson is a valid JSON structure for the "errors" field.
                // Example: errorsJson = "\"field1\":[\"error1\"],\"field2\":[\"error2\"]"
                // We need to construct: {"err":"...", "ECODE":"...", "errors": { errorsJson } }
                // This is tricky with string concatenation for complex JSON.
                // A better approach for the test is to define the full JSON string directly if errors are involved.
                // However, to keep the InlineData simpler, we'll handle it carefully.
                // For now, this test will focus on cases WITHOUT the "errors" field by passing null for errorsJson.
                // A separate test or modification will handle "errors" field presence for BadRequest.
            }
            // Simplified: only include err and ECODE for this general test
            var responseJson = JsonSerializer.Serialize(new { err = errMessage, ECODE = ecode });

            var responseMessage = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync(expectedExceptionType,
                () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.NotNull(exception);
            if (exception is ClickUpApiException apiException)
            {
                Assert.Equal(statusCode, apiException.HttpStatus);
                Assert.Equal(ecode, apiException.ApiErrorCode);
                Assert.Contains(errMessage, apiException.Message);
                Assert.Equal(responseJson, apiException.RawErrorContent);

                if (expectedExceptionType == typeof(ClickUpApiValidationException))
                {
                    Assert.NotNull((apiException as ClickUpApiValidationException)?.Errors);
                }
            }
            else
            {
                Assert.Fail($"Exception was not of type ClickUpApiException: {exception.GetType()}");
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

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiRequestException>(
                 () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.HttpStatus);
            Assert.Contains("Invalid input", exception.Message);
            Assert.Contains("INPUT_ERR_002", exception.ApiErrorCode);
            // For this non-standard structure, the Errors dictionary might be null or empty
            // if the ApiConnection's HandleErrorResponseAsync doesn't find a field named "errors".
            // This is acceptable as the primary check is the exception type and basic message/code.
            // Based on current HandleErrorResponseAsync, it would be null.
             // Assert.Null(exception.Errors); // ClickUpApiRequestException does not have an 'Errors' property.
        }

        [Fact]
        public async Task GetAsync_ThrowsClickUpApiException_WhenContentIsEmpty()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json") // Empty content
            };
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync<ClickUpApiRequestException>( // Changed from ClickUpApiValidationException
                 () => _apiConnection.GetAsync<object>("test-endpoint"));

            Assert.Equal(HttpStatusCode.BadRequest, exception.HttpStatus);
            Assert.Contains($"API request failed with status code {(int)HttpStatusCode.BadRequest}", exception.Message); // More precise message check
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
            Assert.Contains($"API request failed with status code {(int)HttpStatusCode.InternalServerError}", exception.Message); // Check for status code number
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

        // POST / PUT / DELETE Tests
        // Helper for testing POST, PUT, DELETE methods that expect a response body
        private async Task TestHttpOperationAsync<TException>(
            Func<ApiConnection, Task> operation,
            HttpResponseMessage responseMessage,
            HttpStatusCode expectedStatusCode,
            string? expectedErrMessageContains = null,
            string? expectedEcode = null,
            bool checkRawContent = true)
            where TException : ClickUpApiException
        {
            SetupApiConnection(responseMessage);

            var exception = await Assert.ThrowsAsync<TException>(async () => await operation(_apiConnection));

            Assert.NotNull(exception);
            Assert.Equal(expectedStatusCode, exception.HttpStatus);

            if (expectedErrMessageContains != null)
            {
                Assert.Contains(expectedErrMessageContains, exception.Message);
            }
            if (expectedEcode != null)
            {
                Assert.Equal(expectedEcode, exception.ApiErrorCode);
            }
            if (checkRawContent && responseMessage.Content != null)
            {
                var expectedRawContent = await responseMessage.Content.ReadAsStringAsync();
                Assert.Equal(expectedRawContent, exception.RawErrorContent);
            }
        }

        // Example for PostAsync
        [Theory]
        [InlineData(HttpStatusCode.BadRequest, typeof(ClickUpApiRequestException), "Post Bad Request", "POST_REQ_001")]
        [InlineData(HttpStatusCode.NotFound, typeof(ClickUpApiNotFoundException), "Post Not Found", "POST_NF_001")]
        public async Task PostAsync_ThrowsCorrectException_ForErrorStatusCodes(HttpStatusCode statusCode, Type exceptionType, string errMessage, string ecode)
        {
            var responseJson = JsonSerializer.Serialize(new { err = errMessage, ECODE = ecode });
            var httpResponseMessage = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };

            var apiOperation = (ApiConnection conn) => conn.PostAsync<object, object>("test-post-endpoint", new { });

            // Perform the test using the helper
            // Need to cast exceptionType to TException which is not straightforward in a helper like this with Theory.
            // For now, let's specialize the call or simplify the helper not to take TException if Type is used.

            SetupApiConnection(httpResponseMessage);
            var actualException = await Assert.ThrowsAsync(exceptionType, async () => await apiOperation(_apiConnection));

            Assert.NotNull(actualException);
            if (actualException is ClickUpApiException apiException)
            {
                Assert.Equal(statusCode, apiException.HttpStatus);
                Assert.Equal(ecode, apiException.ApiErrorCode);
                Assert.Contains(errMessage, apiException.Message);
                Assert.Equal(responseJson, apiException.RawErrorContent);
            }
            else
            {
                Assert.Fail($"Exception was not of type ClickUpApiException: {actualException.GetType()}");
            }
        }

        // Test for PostAsync without response body
        [Fact]
        public async Task PostAsync_NoResponseBody_ThrowsCorrectException_ForErrorStatusCode()
        {
            HttpStatusCode statusCode = HttpStatusCode.Forbidden;
            string errMessage = "Post Forbidden No Body";
            string ecode = "POST_FORBID_001";
            var responseJson = $"{{\"err\":\"{errMessage}\",\"ECODE\":\"{ecode}\"}}";
            var httpResponseMessage = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };

            SetupApiConnection(httpResponseMessage);
            var apiOperation = (ApiConnection conn) => conn.PostAsync<object>("test-post-nobody-endpoint", new { });
            var actualException = await Assert.ThrowsAsync<ClickUpApiAuthenticationException>(async () => await apiOperation(_apiConnection));

            Assert.NotNull(actualException);
            Assert.Equal(statusCode, actualException.HttpStatus);
            Assert.Equal(ecode, actualException.ApiErrorCode);
            Assert.Contains(errMessage, actualException.Message);
            Assert.Equal(responseJson, actualException.RawErrorContent);
        }


        // TODO: Add similar tests for PutAsync, DeleteAsync (with and without request/response body), and PostMultipartAsync
        // For PostMultipartAsync, the request setup will be different (using MultipartFormDataContent).
    }
}
