using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Http.Handlers;
using Moq;
using Moq.Protected;
using Xunit;

namespace ClickUp.Api.Client.Tests.Http
{
    public class AuthenticationDelegatingHandlerTests
    {
        private const string DummyApiKey = "pk_test_api_key";

        [Fact]
        public void Constructor_WithValidApiKey_ShouldNotThrow()
        {
            var handler = new AuthenticationDelegatingHandler(DummyApiKey);
            Assert.NotNull(handler);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_WithInvalidApiKey_ShouldThrowArgumentNullException(string? invalidApiKey)
        {
            Assert.Throws<ArgumentNullException>(() => new AuthenticationDelegatingHandler(invalidApiKey!));
        }

        [Fact]
        public async Task SendAsync_ShouldAddAuthorizationHeader_WithApiKey()
        {
            // Arrange
            var handler = new AuthenticationDelegatingHandler(DummyApiKey);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");

            var mockInnerHandler = new Mock<HttpMessageHandler>();
            mockInnerHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
                .Verifiable();

            handler.InnerHandler = mockInnerHandler.Object;

            var invoker = new HttpMessageInvoker(handler);

            // Act
            await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(request.Headers.Authorization);
            // Current implementation directly uses the API key as the scheme and parameter.
            // The scheme should be the API key itself.
            Assert.Equal(DummyApiKey, request.Headers.Authorization.Scheme);
            Assert.Null(request.Headers.Authorization.Parameter); // Parameter is null when scheme itself is the token

            mockInnerHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req == request),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task SendAsync_ShouldCallInnerHandler()
        {
            // Arrange
            var handler = new AuthenticationDelegatingHandler(DummyApiKey);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");
            var expectedResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            var mockInnerHandler = new Mock<HttpMessageHandler>();
            mockInnerHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            handler.InnerHandler = mockInnerHandler.Object;
            var invoker = new HttpMessageInvoker(handler);

            // Act
            var response = await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.Same(expectedResponse, response);
            mockInnerHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req == request),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
