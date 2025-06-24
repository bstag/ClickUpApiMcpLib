using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Options;
using ClickUp.Api.Client.Http.Handlers;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace ClickUp.Api.Client.Tests.Http
{
    public class AuthenticationDelegatingHandlerTests
    {
        private const string DummyPersonalAccessToken = "pk_test_pat_key";
        private const string DummyOAuthToken = "oauth_test_token";

        private IOptions<ClickUpClientOptions> CreateOptions(string? pat = null, string? oauth = null)
        {
            return Options.Create(new ClickUpClientOptions { PersonalAccessToken = pat, OAuthAccessToken = oauth });
        }

        [Fact]
        public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AuthenticationDelegatingHandler(null!));
        }

        [Fact]
        public void Constructor_WithValidOptions_ShouldNotThrow()
        {
            var options = CreateOptions(pat: DummyPersonalAccessToken);
            var handler = new AuthenticationDelegatingHandler(options);
            Assert.NotNull(handler);
        }

        [Fact]
        public async Task SendAsync_WithPersonalAccessToken_ShouldAddCorrectAuthorizationHeader()
        {
            // Arrange
            var options = CreateOptions(pat: DummyPersonalAccessToken);
            var handler = new AuthenticationDelegatingHandler(options);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");

            var mockInnerHandler = new Mock<HttpMessageHandler>();
            mockInnerHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
                .Verifiable();
            handler.InnerHandler = mockInnerHandler.Object;
            var invoker = new HttpMessageInvoker(handler);

            // Act
            await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(request.Headers.Authorization);
            Assert.Equal(DummyPersonalAccessToken, request.Headers.Authorization.Scheme); // PAT is used as the scheme itself
            Assert.Null(request.Headers.Authorization.Parameter);
            mockInnerHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req == request), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendAsync_WithOAuthAccessToken_ShouldAddCorrectBearerAuthorizationHeader()
        {
            // Arrange
            var options = CreateOptions(oauth: DummyOAuthToken);
            var handler = new AuthenticationDelegatingHandler(options);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");

            var mockInnerHandler = new Mock<HttpMessageHandler>();
            mockInnerHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
                .Verifiable();
            handler.InnerHandler = mockInnerHandler.Object;
            var invoker = new HttpMessageInvoker(handler);

            // Act
            await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(request.Headers.Authorization);
            Assert.Equal("Bearer", request.Headers.Authorization.Scheme);
            Assert.Equal(DummyOAuthToken, request.Headers.Authorization.Parameter);
            mockInnerHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req == request), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendAsync_WithOAuthTakingPrecedenceOverPAT_ShouldUseOAuthToken()
        {
            // Arrange
            var options = CreateOptions(pat: DummyPersonalAccessToken, oauth: DummyOAuthToken); // Both provided
            var handler = new AuthenticationDelegatingHandler(options);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");

            var mockInnerHandler = new Mock<HttpMessageHandler>();
            mockInnerHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
                .Verifiable();
            handler.InnerHandler = mockInnerHandler.Object;
            var invoker = new HttpMessageInvoker(handler);

            // Act
            await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(request.Headers.Authorization);
            Assert.Equal("Bearer", request.Headers.Authorization.Scheme); // OAuth should be used
            Assert.Equal(DummyOAuthToken, request.Headers.Authorization.Parameter);
            mockInnerHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req == request), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendAsync_WithNoTokenConfigured_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var options = CreateOptions(); // No token
            var handler = new AuthenticationDelegatingHandler(options);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");

            var mockInnerHandler = new Mock<HttpMessageHandler>();
            mockInnerHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            handler.InnerHandler = mockInnerHandler.Object;
            var invoker = new HttpMessageInvoker(handler);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => invoker.SendAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task SendAsync_ShouldCallInnerHandler_WhenTokenIsValid()
        {
            // Arrange
            var options = CreateOptions(pat: DummyPersonalAccessToken);
            var handler = new AuthenticationDelegatingHandler(options);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clickup.com/api/v2/user");
            var expectedResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            var mockInnerHandler = new Mock<HttpMessageHandler>();
            mockInnerHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);
            handler.InnerHandler = mockInnerHandler.Object;
            var invoker = new HttpMessageInvoker(handler);

            // Act
            var response = await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.Same(expectedResponse, response);
            mockInnerHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req == request), ItExpr.IsAny<CancellationToken>());
        }
    }
}
