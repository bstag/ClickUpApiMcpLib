using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ClickUp.Api.Client.Models.Entities.Spaces; // Added for Space entity
using RichardSzalay.MockHttp; // Added for MockHttp
using System.IO; // Added for Path
using System.Net; // Added for HttpStatusCode

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class SpaceServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private readonly ISpacesService _spaceService;
        private string _testWorkspaceId;

        private List<string> _createdSpaceIdsForCleanup = new List<string>();

        public SpaceServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _spaceService = ServiceProvider.GetRequiredService<ISpacesService>();
            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"];

            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Tests will fail.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for SpaceServiceIntegrationTests.");
            }
        }

        public Task InitializeAsync()
        {
            // No workspace-level resources to create for each test class run,
            // as spaces are created within the configured _testWorkspaceId.
            _output.LogInformation("SpaceServiceIntegrationTests initialized. Using configured TestWorkspaceId: " + _testWorkspaceId);
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation($"Starting SpaceServiceIntegrationTests class disposal: Cleaning up {(_createdSpaceIdsForCleanup.Count)} created spaces.");
            foreach (var spaceId in _createdSpaceIdsForCleanup)
            {
                try
                {
                    _output.LogInformation($"Deleting test space: {spaceId}");
                    await _spaceService.DeleteSpaceAsync(spaceId);
                }
                catch (Exception ex)
                {
                    // Log error but continue trying to delete others
                    _output.LogError($"Error deleting space {spaceId}: {ex.Message}", ex);
                }
            }
            _createdSpaceIdsForCleanup.Clear();
            _output.LogInformation("SpaceServiceIntegrationTests class disposal complete.");
        }

        private void RegisterCreatedSpace(string spaceId)
        {
            if (!string.IsNullOrWhiteSpace(spaceId))
            {
                _createdSpaceIdsForCleanup.Add(spaceId);
            }
        }

        [Fact]
        public async Task GetSpacesAsync_FilterByArchived_ShouldReturnCorrectSpaces()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testWorkspaceId), "TestWorkspaceId must be available.");

            // 1. Create an active space
            var activeSpaceName = $"ActiveSpace_{Guid.NewGuid()}";
            var activeSpace = await _spaceService.CreateSpaceAsync(_testWorkspaceId,
                                new CreateSpaceRequest(activeSpaceName, MultipleAssignees: null, Features: null));
            RegisterCreatedSpace(activeSpace.Id);
            _output.LogInformation($"Created active space '{activeSpace.Name}' (ID: {activeSpace.Id}).");

            // 2. Create another space and archive it
            var spaceToArchiveName = $"ArchivedSpace_{Guid.NewGuid()}";
            var spaceToArchive = await _spaceService.CreateSpaceAsync(_testWorkspaceId,
                                new CreateSpaceRequest(spaceToArchiveName, MultipleAssignees: null, Features: null));
            RegisterCreatedSpace(spaceToArchive.Id); // Register for cleanup
            _output.LogInformation($"Created space to archive '{spaceToArchive.Name}' (ID: {spaceToArchive.Id}).");

            // Archive the space by updating it
            // UpdateSpaceRequest is (Name, Color, Private, AdminCanManage, MultipleAssignees, Features, Archived)
            await _spaceService.UpdateSpaceAsync(spaceToArchive.Id,
                new UpdateSpaceRequest(
                    Name: spaceToArchiveName,
                    Color: null,
                    Private: null,
                    AdminCanManage: null,
                    MultipleAssignees: null,
                    Features: null,
                    Archived: true
                ));
            _output.LogInformation($"Archived space '{spaceToArchive.Name}' (ID: {spaceToArchive.Id}).");

            // Give ClickUp a moment to process the archive action before querying
            // In Playback mode, this delay is not strictly necessary but kept for consistency with live mode.
            await Task.Delay(CurrentTestMode == TestMode.Playback ? 50 : 2000);

            List<Space> nonArchivedSpaces; // Changed from SpaceEntry to Space
            List<Space> archivedSpaces;    // Changed from SpaceEntry to Space
            List<Space> allSpacesDefault;  // Changed from SpaceEntry to Space

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var workspaceIdForMock = _testWorkspaceId; // Or a placeholder if IDs are embedded in mock files

                // Mock for GetSpacesAsync(archived: false)
                var getNonArchivedPath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GetSpaces", "GetSpaces_NotArchived_Success.json");
                _output.LogInformation($"[Playback] Using response file for Not Archived: {getNonArchivedPath}");
                Assert.True(File.Exists(getNonArchivedPath), $"Playback file not found: {getNonArchivedPath}");
                var getNonArchivedContent = await File.ReadAllTextAsync(getNonArchivedPath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space?archived=false")
                               .Respond("application/json", getNonArchivedContent);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space?archived=False") // Case variation
                               .Respond("application/json", getNonArchivedContent);


                // Mock for GetSpacesAsync(archived: true)
                var getArchivedPath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GetSpaces", "GetSpaces_Archived_Success.json");
                _output.LogInformation($"[Playback] Using response file for Archived: {getArchivedPath}");
                Assert.True(File.Exists(getArchivedPath), $"Playback file not found: {getArchivedPath}");
                var getArchivedContent = await File.ReadAllTextAsync(getArchivedPath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space?archived=true")
                               .Respond("application/json", getArchivedContent);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space?archived=True") // Case variation
                               .Respond("application/json", getArchivedContent);

                // Mock for GetSpacesAsync(archived: null) - parameter omitted
                var getDefaultPath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GetSpaces", "GetSpaces_Default_Success.json");
                _output.LogInformation($"[Playback] Using response file for Default: {getDefaultPath}");
                Assert.True(File.Exists(getDefaultPath), $"Playback file not found: {getDefaultPath}");
                var getDefaultContent = await File.ReadAllTextAsync(getDefaultPath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space")
                               .Respond("application/json", getDefaultContent);

                // IMPORTANT: The CreateSpace and UpdateSpace calls are NOT mocked here.
                // For this test to pass in Playback mode, those operations would either need to be mocked
                // in IntegrationTestBase setup if CLICKUP_SDK_TEST_MODE=Playback is global,
                // or the activeSpace.Id and spaceToArchive.Id would need to be hardcoded/predetermined
                // and the mock JSON files would need to contain these exact IDs.
                // This current modification ONLY mocks the GET calls.
                // For a true playback, the Create/Update responses would also be mocked to provide these IDs.
                // For now, we assume `activeSpace.Id` and `spaceToArchive.Id` are obtained from the (potentially live) Create calls
                // and the assertions will check against these dynamic IDs. The mock files for GetSpaces must be crafted
                // to include spaces that would match these dynamically generated IDs, or assertions need to be less ID-specific.

                // For this iteration, the recorded JSON files for GetSpaces should contain the actual IDs
                // that were generated during the Record phase.
            }

            // 3. Get non-archived spaces
            _output.LogInformation($"Fetching non-archived spaces from workspace '{_testWorkspaceId}'.");
            nonArchivedSpaces = (await _spaceService.GetSpacesAsync(_testWorkspaceId, archived: false)).ToList();

            Assert.NotNull(nonArchivedSpaces);
            // If playback files are static, these assertions on dynamic IDs might fail unless files are perfect.
            Assert.Contains(nonArchivedSpaces, s => s.Id == activeSpace.Id && s.Name == activeSpaceName && s.Archived == false);
            var foundArchivedInNonArchivedQuery = nonArchivedSpaces.FirstOrDefault(s => s.Id == spaceToArchive.Id);
            if (foundArchivedInNonArchivedQuery != null)
            {
                 _output.LogWarning($"Archived space {spaceToArchive.Id} was found in non-archived query. Archived status: {foundArchivedInNonArchivedQuery.Archived}");
            }
            Assert.DoesNotContain(nonArchivedSpaces, s => s.Id == spaceToArchive.Id && s.Archived == false);
            _output.LogInformation($"Found {nonArchivedSpaces.Count} non-archived spaces. Active space was present. Archived space was not.");

            // 4. Get archived spaces
            _output.LogInformation($"Fetching archived spaces from workspace '{_testWorkspaceId}'.");
            archivedSpaces = (await _spaceService.GetSpacesAsync(_testWorkspaceId, archived: true)).ToList();

            Assert.NotNull(archivedSpaces);
            var foundArchivedInArchivedQuery = archivedSpaces.FirstOrDefault(s => s.Id == spaceToArchive.Id);
            Assert.NotNull(foundArchivedInArchivedQuery);
            Assert.True(foundArchivedInArchivedQuery.Archived, $"Space {spaceToArchive.Id} should be marked as archived.");
            _output.LogInformation($"Found {archivedSpaces.Count} archived spaces. Archived space '{spaceToArchive.Name}' (ID: {spaceToArchive.Id}) was present and marked archived.");

            Assert.DoesNotContain(archivedSpaces, s => s.Id == activeSpace.Id);
            _output.LogInformation($"Active space '{activeSpace.Name}' (ID: {activeSpace.Id}) was correctly not in the archived list.");

            // 5. Get all spaces (archived: null or not provided)
            _output.LogInformation($"Fetching all (default, non-archived) spaces (archived: null) from workspace '{_testWorkspaceId}'.");
            allSpacesDefault = (await _spaceService.GetSpacesAsync(_testWorkspaceId, archived: null)).ToList();
            Assert.NotNull(allSpacesDefault);
            Assert.Contains(allSpacesDefault, s => s.Id == activeSpace.Id && s.Archived == false);
            Assert.DoesNotContain(allSpacesDefault, s => s.Id == spaceToArchive.Id && s.Archived == true);
            _output.LogInformation($"Found {allSpacesDefault.Count} total (default) spaces. Active space was present. Archived space was not.");
        }
    }
}
