using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Users;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure; // For TestMode & base class
using ClickUp.Api.Client.Models.Exceptions; // For ClickUpApiException
using RichardSzalay.MockHttp; // For MockHttpHandler extension methods like .When()

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class UsersServiceIntegrationTests : IntegrationTestBase
    {
        private readonly IUsersService _usersService;
        private readonly IAuthorizationService _authorizationService; // To get current user ID
        private readonly ITestOutputHelper _output;
        private readonly string _workspaceId;

        public UsersServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _usersService = ServiceProvider.GetRequiredService<IUsersService>();
            _authorizationService = ServiceProvider.GetRequiredService<IAuthorizationService>(); // For fetching current user

            _workspaceId = ClientOptions.TestWorkspaceId; // From base class, loaded from config

            if (CurrentTestMode != TestMode.Playback && string.IsNullOrWhiteSpace(_workspaceId))
            {
                throw new InvalidOperationException("TestWorkspaceId is not configured. Required for UsersService tests in Record/Passthrough mode.");
            }
            _output.LogInformation($"Running UsersServiceTests in {CurrentTestMode} mode for workspace: {_workspaceId}");
        }

        private async Task<string> GetCurrentUserIdAsync()
        {
            // In Playback mode, we need to mock the GetAuthorizedUserAsync call as well,
            // or use a hardcoded known ID that matches recorded data.
            if (CurrentTestMode == TestMode.Playback)
            {
                // This assumes that the recordings for UsersService tests were made
                // using a specific user ID, and that ID should be returned here.
                // For simplicity, let's assume a common "test_user_id" placeholder was used in recordings.
                // The actual ID would be in the recorded JSON for GetAuthorizedUser for AuthorizationService tests.
                // Let's use a placeholder that the user would replace in their JSONs or know from their recording session.
                // A more robust way would be to have this ID configured or derived consistently.
                // For now, we'll use a fixed ID that should match what's in the recorded files for UsersService.
                // This ID should also be the one used in the path for recorded responses of GetUserFromWorkspaceAsync.
                // Example: "/api/v2/team/{workspaceId}/user/123456" -> "123456" is this ID.

                // Attempt to read the user ID from the GetAuthorizedUser recording if it exists.
                // This makes the test more self-contained if that recording is present.
                var authUserResponsePath = Path.Combine(RecordedResponsesBasePath, "AuthorizationService", "GETAuthorizedUser", "Success.json");
                if (File.Exists(authUserResponsePath))
                {
                    var content = await File.ReadAllTextAsync(authUserResponsePath);
                    // Crude parsing, ideally use System.Text.Json.JsonDocument
                    // {"user":{"id":123,...}}
                    var idStr = System.Text.RegularExpressions.Regex.Match(content, "\"id\":(\\d+)").Groups[1].Value;
                    if (!string.IsNullOrEmpty(idStr))
                    {
                        _output.LogInformation($"[Playback] Using current user ID '{idStr}' from AuthorizationService recording.");
                        return idStr;
                    }
                }
                // Fallback to a known placeholder if the above fails or is not desired.
                // The user MUST ensure their recorded files for UsersService use this ID.
                _output.LogWarning("[Playback] Could not derive user ID from AuthorizationService recording. Using placeholder 'playback_user_id_123'. Ensure your UsersService recordings match this ID.");
                return "playback_user_id_123"; // Placeholder for playback
            }

            var currentUser = await _authorizationService.GetAuthorizedUserAsync();
            return currentUser.Id.ToString();
        }

        [Fact]
        public async Task GetUserFromWorkspaceAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            string userIdToTest;
            if (CurrentTestMode == TestMode.Playback)
            {
                userIdToTest = await GetCurrentUserIdAsync(); // Get the ID used in recordings
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "UsersService", $"GETUser", $"Success_query{userIdToTest}.json");
                // The RecordingDelegatingHandler for UsersService might generate paths like:
                // UsersService/GETUser/Success_body...json (if it interprets user ID as part of body/query for generation)
                // Or more likely: UsersService/GETUser (if endpoint is /team/{team_id}/user/{user_id}, user_id is part of path)
                // The handler's GenerateFilePath needs to be checked.
                // Assuming /team/{team_id}/user/{user_id} means {user_id} is part of the resource path.
                // The handler uses primaryResource "team", then sub-resource "user".
                // It might generate "TeamService" or "UsersService" based on "user" segment.
                // Let's assume it generates "UsersService" and method "GETUser".
                // And the file name includes the user_id to differentiate, if not part of folder structure.
                // For simplicity, let's assume a file structure like:
                // UsersService/GETUser_workspaceId_userId/Success.json or UsersService/GETUser/user_userId_Success.json
                // The current handler logic based on "/team/{team_id}/user/{user_id}":
                // serviceName = "UsersService" (due to "user" segment after "team/{id}")
                // derivedMethodNamePart = "User"
                // fullMethodName = "GETUser"
                // scenarioName = "Success.json" (if no query/body hash)
                // This means the path would be: UsersService/GETUser/Success.json
                // This is problematic if we need to test different users.
                // The path should ideally include the user ID if it's part of the URL path.
                // For now, we'll assume one "Success.json" for the primary test user.

                var specificUserResponsePath = Path.Combine(RecordedResponsesBasePath, "UsersService", "GETUser", $"user_{userIdToTest}_Success.json");
                 _output.LogInformation($"[Playback] Attempting to use specific user response file: {specificUserResponsePath}");

                if (!File.Exists(specificUserResponsePath))
                {
                    _output.LogWarning($"[Playback] Specific user response file not found: {specificUserResponsePath}. Falling back to generic 'Success.json'.");
                    specificUserResponsePath = Path.Combine(RecordedResponsesBasePath, "UsersService", "GETUser", "Success.json");
                }

                Assert.True(File.Exists(specificUserResponsePath), $"Playback file not found: {specificUserResponsePath}");
                var responseContent = await File.ReadAllTextAsync(specificUserResponsePath);
                MockHttpHandler.When(HttpMethod.Get, $"https{"://"}api.clickup.com/api/v2/team/{_workspaceId}/user/{userIdToTest}")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }
            else
            {
                userIdToTest = await GetCurrentUserIdAsync(); // Get current authenticated user's ID for live test
            }

            _output.LogInformation($"Testing GetUserFromWorkspaceAsync for User ID: {userIdToTest} in Workspace: {_workspaceId}");

            // Act
            var user = await _usersService.GetUserFromWorkspaceAsync(_workspaceId, userIdToTest);

            // Assert
            Assert.NotNull(user);
            Assert.IsType<User>(user);
            Assert.Equal(int.Parse(userIdToTest), user.Id); // User ID should match
            Assert.False(string.IsNullOrWhiteSpace(user.Username));
            _output.LogInformation($"Successfully fetched user: {user.Username} (ID: {user.Id}) from workspace {_workspaceId}");

            if (CurrentTestMode == TestMode.Record)
            {
                _output.LogInformation($"[Record] Ensure response for GET /team/{_workspaceId}/user/{userIdToTest} is saved as 'UsersService/GETUser/user_{userIdToTest}_Success.json' or 'UsersService/GETUser/Success.json'.");
            }
        }

        [Fact]
        public async Task GetUserFromWorkspaceAsync_ShouldThrowNotFound_WhenUserDoesNotExistOrNotInWorkspace()
        {
            // Arrange
            var nonExistentUserId = "000000"; // A user ID that is unlikely to exist
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "UsersService", "GETUser", $"user_{nonExistentUserId}_Error_404.json");
                 if (!File.Exists(responsePath))
                {
                     _output.LogWarning($"[Playback] Specific error response file not found: {responsePath}. Falling back to generic 'Error_404.json'.");
                    responsePath = Path.Combine(RecordedResponsesBasePath, "UsersService", "GETUser", "Error_404.json");
                }
                Assert.True(File.Exists(responsePath), $"Playback file not found for 404 scenario: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(HttpMethod.Get, $"https{"://"}api.clickup.com/api/v2/team/{_workspaceId}/user/{nonExistentUserId}")
                               .Respond(HttpStatusCode.NotFound, "application/json", responseContent);
            }
            // In Record/Passthrough, this will hit the live API with a non-existent user ID.

            _output.LogInformation($"Testing GetUserFromWorkspaceAsync for non-existent User ID: {nonExistentUserId} in Workspace: {_workspaceId}");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ClickUpApiNotFoundException>(
                () => _usersService.GetUserFromWorkspaceAsync(_workspaceId, nonExistentUserId));

            Assert.NotNull(exception);
            // Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode); // If your custom exception surfaces status code
            _output.LogInformation($"Received expected ClickUpApiNotFoundException for user {nonExistentUserId}.");

            if (CurrentTestMode == TestMode.Record)
            {
                _output.LogInformation($"[Record] Ensure response for GET /team/{_workspaceId}/user/{nonExistentUserId} (resulting in 404) is saved, e.g., as 'UsersService/GETUser/user_{nonExistentUserId}_Error_404.json' or 'UsersService/GETUser/Error_404.json'.");
            }
        }

        // **Note on EditUserOnWorkspaceAsync and RemoveUserFromWorkspaceAsync:**
        // Implementing and consistently running tests for user editing and removal requires careful consideration:
        // 1. Idempotency: Tests should be runnable multiple times without side effects or reliance on previous state.
        //    - Editing requires knowing the original state to revert, or editing to a known test state.
        //    - Removing a user means they can't be removed again.
        // 2. Test User Management:
        //    - Ideal: Programmatically invite a new test user, perform actions, then remove them. This requires:
        //        - An email address that can be used for invites (e.g., using mailinator or similar).
        //        - API permissions for the test token to invite and remove users.
        //        - Handling the invitation acceptance flow (often not possible purely via API for security).
        //    - Alternative: Use a pre-existing, dedicated test user account within the test workspace
        //      that can be modified and whose state can be reset (e.g., always edit role to X, then back to Y).
        //      This user should not be critical.
        // 3. Permissions: The API token used for tests must have permissions to edit/remove users.
        //    Personal Access Tokens might not always have full user management rights depending on the user's role.
        //
        // Given these complexities, robust tests for edit/remove are often deferred or require significant
        // infrastructure setup. For this iteration, we will focus on GetUserFromWorkspaceAsync.
        // If specific, safe test users are available and the ClickUp plan/permissions allow,
        // these tests could be added. For now, they are omitted to prevent accidental modification
        // of important users or test failures due to permission issues.

        // Example (Conceptual - DO NOT RUN without proper test user setup):
        // [Fact]
        // [Trait("Category", "UsersService_Mutation")] // Separate trait for destructive tests
        // public async Task EditUserOnWorkspaceAsync_ShouldModifyUser_WhenUserExistsAndPermitted() { /* ... */ }

        // [Fact]
        // [Trait("Category", "UsersService_Mutation")]
        // public async Task RemoveUserFromWorkspaceAsync_ShouldRemoveUser_WhenUserExistsAndPermitted() { /* ... */ }
    }
}
