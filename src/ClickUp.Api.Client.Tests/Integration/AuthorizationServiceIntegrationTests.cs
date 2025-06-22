using System;
using System.Linq;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ClickUp.Api.Client.Extensions; // Added for AddClickUpClient
using Xunit.Abstractions; // Required for ITestOutputHelper

namespace ClickUp.Api.Client.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class AuthorizationServiceIntegrationTests : IntegrationTestBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ITestOutputHelper _output;

        public AuthorizationServiceIntegrationTests(ITestOutputHelper output) // Inject ITestOutputHelper
        {
            _output = output; // Store it
            _authorizationService = ServiceProvider.GetRequiredService<IAuthorizationService>();
            _output.LogInformation($"API Token Used: {(string.IsNullOrWhiteSpace(ClientOptions.PersonalAccessToken) ? "NOT SET" : ClientOptions.PersonalAccessToken.Substring(0, Math.Min(ClientOptions.PersonalAccessToken.Length, 7)) + "...")}");
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_WithValidToken_ReturnsUser()
        {
            // Arrange
            // Token is configured in IntegrationTestBase

            // Act
            var user = await _authorizationService.GetAuthorizedUserAsync();

            // Assert
            Assert.NotNull(user);
            Assert.False(string.IsNullOrWhiteSpace(user.Username));
            Assert.False(string.IsNullOrWhiteSpace(user.Email));
            _output.LogInformation($"Successfully fetched authorized user: {user.Username} ({user.Email})");
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_WithInvalidToken_ThrowsClickUpApiAuthenticationException()
        {
            // Arrange
            var services = new ServiceCollection();
            // Configure with a deliberately invalid token
            services.AddClickUpClient(opts =>
            {
                opts.PersonalAccessToken = "invalid_dummy_token_for_testing_auth_failure";
                if (!string.IsNullOrWhiteSpace(ClientOptions.BaseAddress)) // Keep base address if configured
                {
                    opts.BaseAddress = ClientOptions.BaseAddress;
                }
            });
            var serviceProviderWithInvalidToken = services.BuildServiceProvider();
            var authServiceWithInvalidToken = serviceProviderWithInvalidToken.GetRequiredService<IAuthorizationService>();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ClickUpApiAuthenticationException>(
                () => authServiceWithInvalidToken.GetAuthorizedUserAsync());

            _output.LogInformation($"Received expected exception: {exception.Message}");
            Assert.NotNull(exception);
            // Optionally, check status code if the exception includes it and it's consistent
            // Assert.Equal(System.Net.HttpStatusCode.Unauthorized, exception.StatusCode);
        }

        [Fact]
        public async Task GetAuthorizedWorkspacesAsync_WithValidToken_ReturnsWorkspaces()
        {
            // Arrange
            // Token is configured in IntegrationTestBase

            // Act
            var workspaces = await _authorizationService.GetAuthorizedWorkspacesAsync();

            // Assert
            Assert.NotNull(workspaces);
            Assert.NotEmpty(workspaces);
            var firstWorkspace = workspaces.First();
            Assert.False(string.IsNullOrWhiteSpace(firstWorkspace.Id));
            Assert.False(string.IsNullOrWhiteSpace(firstWorkspace.Name));
            _output.LogInformation($"Successfully fetched {workspaces.Count()} workspaces. First workspace: {firstWorkspace.Name} (ID: {firstWorkspace.Id})");
        }
    }

    // Helper extension for ITestOutputHelper to make logging easier
    public static class TestOutputHelperExtensions
    {
        public static void LogInformation(this ITestOutputHelper output, string message)
        {
            output.WriteLine($"[INFO] {DateTime.UtcNow:O} | {message}");
        }

        public static void LogError(this ITestOutputHelper output, string message, Exception? ex = null)
        {
            output.WriteLine($"[ERROR] {DateTime.UtcNow:O} | {message}" + (ex != null ? $"\nException: {ex}" : ""));
        }
    }
}
