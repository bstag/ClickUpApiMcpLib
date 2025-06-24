using System;
using System.IO; // Added for Path
using System.Linq;
using System.Net; // Added for HttpStatusCode
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using RichardSzalay.MockHttp; // Added for MockHttp extension methods
using ClickUp.Api.Client.Models.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ClickUp.Api.Client.Extensions; // Added for AddClickUpClient
using Xunit.Abstractions; // Required for ITestOutputHelper
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure; // Added for ITestOutputHelper extensions and TestMode

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class AuthorizationServiceIntegrationTests : IntegrationTestBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ITestOutputHelper _output;

        public AuthorizationServiceIntegrationTests(ITestOutputHelper output) // Inject ITestOutputHelper
            : base() // Ensure base constructor is called
        {
            _output = output; // Store it
            _authorizationService = ServiceProvider.GetRequiredService<IAuthorizationService>();

            if (CurrentTestMode != TestMode.Playback) // Only log token if not in playback to avoid issues if token is not set for playback
            {
                 _output.LogInformation($"API Token Used: {(string.IsNullOrWhiteSpace(ClientOptions.PersonalAccessToken) ? "NOT SET" : ClientOptions.PersonalAccessToken.Substring(0, Math.Min(ClientOptions.PersonalAccessToken.Length, 7)) + "...")}");
            }
        }

        [Fact]
        public async Task GetAuthorizedUserAsync_WithValidToken_ReturnsUser()
        {
            // Arrange
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler); // Ensure MockHttpHandler is available in Playback mode
                var responsePath = Path.Combine(RecordedResponsesBasePath, "AuthorizationService", "GetAuthorizedUser", "Success.json");
                _output.LogInformation($"[Playback] Using response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When("https://api.clickup.com/api/v2/user")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }
            // In Record or Passthrough mode, this test will hit the live API. Token is configured in IntegrationTestBase.

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
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "AuthorizationService", "GetAuthorizedTeams", "Success.json"); // Note: Original name was GetAuthorizedTeams
                _output.LogInformation($"[Playback] Using response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When("https://api.clickup.com/api/v2/team") // This is the endpoint for "teams" which are workspaces
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }
            // In Record or Passthrough mode, this test will hit the live API.

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
}
