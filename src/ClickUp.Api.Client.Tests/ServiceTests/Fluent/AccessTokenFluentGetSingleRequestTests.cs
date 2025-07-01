using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent
{
    public class AccessTokenFluentGetSingleRequestTests : IDisposable
    {
        private readonly Mock<IAuthorizationService> _authServiceMock;
        private readonly AccessTokenFluentGetSingleRequest _sut;

        public AccessTokenFluentGetSingleRequestTests()
        {
            _authServiceMock = new Mock<IAuthorizationService>();
            _sut = new AccessTokenFluentGetSingleRequest(_authServiceMock.Object);
        }

        public void Dispose()
        {
            _authServiceMock.VerifyAll();
        }

        [Fact]
        public void WithClientId_ShouldSetClientId_AndReturnSelf()
        {
            // Arrange
            var clientId = "test-client-id";

            // Act
            var result = _sut.WithClientId(clientId);

            // Assert
            Assert.Same(_sut, result);
        }

        [Fact]
        public void WithClientSecret_ShouldSetClientSecret_AndReturnSelf()
        {
            // Arrange
            var clientSecret = "test-client-secret";

            // Act
            var result = _sut.WithClientSecret(clientSecret);

            // Assert
            Assert.Same(_sut, result);
        }

        [Fact]
        public void WithCode_ShouldSetCode_AndReturnSelf()
        {
            // Arrange
            var code = "test-code";

            // Act
            var result = _sut.WithCode(code);

            // Assert
            Assert.Same(_sut, result);
        }

        [Fact]
        public async Task GetAsync_WithAllParametersSet_ShouldCallAuthorizationServiceWithCorrectParameters()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var code = "test-code";
            var cancellationToken = new CancellationToken();
            var expectedResponse = new GetAccessTokenResponse("test-access-token");

            _authServiceMock
                .Setup(x => x.GetAccessTokenAsync(clientId, clientSecret, code, cancellationToken))
                .ReturnsAsync(expectedResponse)
                .Verifiable();

            // Act
            var result = await _sut
                .WithClientId(clientId)
                .WithClientSecret(clientSecret)
                .WithCode(code)
                .GetAsync(cancellationToken);

            // Assert
            Assert.Same(expectedResponse, result);
        }

        [Fact]
        public async Task GetAsync_WithNullClientId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var clientSecret = "test-client-secret";
            var code = "test-code";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut
                .WithClientId(null!)
                .WithClientSecret(clientSecret)
                .WithCode(code)
                .GetAsync());
        }

        [Fact]
        public async Task GetAsync_WithNullClientSecret_ShouldThrowArgumentNullException()
        {
            // Arrange
            var clientId = "test-client-id";
            var code = "test-code";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut
                .WithClientId(clientId)
                .WithClientSecret(null!)
                .WithCode(code)
                .GetAsync());
        }

        [Fact]
        public async Task GetAsync_WithNullCode_ShouldThrowArgumentNullException()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut
                .WithClientId(clientId)
                .WithClientSecret(clientSecret)
                .WithCode(null!)
                .GetAsync());
        }

        [Fact]
        public async Task GetAsync_WithEmptyClientId_ShouldThrowArgumentException()
        {
            // Arrange
            var clientSecret = "test-client-secret";
            var code = "test-code";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut
                .WithClientId("")
                .WithClientSecret(clientSecret)
                .WithCode(code)
                .GetAsync());
        }

        [Fact]
        public async Task GetAsync_WithEmptyClientSecret_ShouldThrowArgumentException()
        {
            // Arrange
            var clientId = "test-client-id";
            var code = "test-code";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut
                .WithClientId(clientId)
                .WithClientSecret("")
                .WithCode(code)
                .GetAsync());
        }

        [Fact]
        public async Task GetAsync_WithEmptyCode_ShouldThrowArgumentException()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut
                .WithClientId(clientId)
                .WithClientSecret(clientSecret)
                .WithCode("")
                .GetAsync());
        }

        [Fact]
        public async Task GetAsync_WhenServiceThrows_ShouldPropagateException()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var code = "test-code";
            var expectedException = new InvalidOperationException("Test exception");

            _authServiceMock
                .Setup(x => x.GetAccessTokenAsync(clientId, clientSecret, code, It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut
                .WithClientId(clientId)
                .WithClientSecret(clientSecret)
                .WithCode(code)
                .GetAsync());

            Assert.Same(expectedException, actualException);
        }
    }
}
