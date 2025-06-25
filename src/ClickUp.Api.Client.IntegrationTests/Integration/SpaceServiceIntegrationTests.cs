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

            string activeSpaceIdForTest = "space_active_123"; // Predefined for playback
            string activeSpaceNameForTest = "My Active Playback Space";
            string archivedSpaceIdForTest = "space_archived_789";
            string archivedSpaceNameForTest = "My Archived Playback Space";

            List<Space> nonArchivedSpaces;
            List<Space> archivedSpaces;
            List<Space> allSpacesDefault;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var workspaceIdForMock = _testWorkspaceId;

                // Mock for GetSpacesAsync(archived: false)
                var getNonArchivedPath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GetSpaces", "GetSpaces_NotArchived_Success.json");
                _output.LogInformation($"[Playback] Using response file for Not Archived: {getNonArchivedPath}");
                Assert.True(File.Exists(getNonArchivedPath), $"Playback file not found: {getNonArchivedPath}");
                var getNonArchivedContent = await File.ReadAllTextAsync(getNonArchivedPath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space?archived=false")
                               .Respond("application/json", getNonArchivedContent);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space?archived=False")
                               .Respond("application/json", getNonArchivedContent);

                // Mock for GetSpacesAsync(archived: true)
                var getArchivedPath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GetSpaces", "GetSpaces_Archived_Success.json");
                _output.LogInformation($"[Playback] Using response file for Archived: {getArchivedPath}");
                Assert.True(File.Exists(getArchivedPath), $"Playback file not found: {getArchivedPath}");
                var getArchivedContent = await File.ReadAllTextAsync(getArchivedPath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space?archived=true")
                               .Respond("application/json", getArchivedContent);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space?archived=True")
                               .Respond("application/json", getArchivedContent);

                // Mock for GetSpacesAsync(archived: null) - parameter omitted
                var getDefaultPath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GetSpaces", "GetSpaces_Default_Success.json");
                _output.LogInformation($"[Playback] Using response file for Default: {getDefaultPath}");
                Assert.True(File.Exists(getDefaultPath), $"Playback file not found: {getDefaultPath}");
                var getDefaultContent = await File.ReadAllTextAsync(getDefaultPath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/team/{workspaceIdForMock}/space")
                               .Respond("application/json", getDefaultContent);

                // No API calls for create/update in Playback mode for this test
                _output.LogInformation("[Playback] Skipped live creation/archiving of spaces.");
            }
            else // Record or Passthrough mode
            {
                _output.LogInformation("[Record/Passthrough] Performing live space creation and archiving.");
                // 1. Create an active space
                activeSpaceNameForTest = $"ActiveSpace_{Guid.NewGuid()}";
                var liveActiveSpace = await _spaceService.CreateSpaceAsync(_testWorkspaceId,
                                    new CreateSpaceRequest(activeSpaceNameForTest, MultipleAssignees: null, Features: null));
                RegisterCreatedSpace(liveActiveSpace.Id);
                activeSpaceIdForTest = liveActiveSpace.Id; // Use live ID for assertions in this mode
                _output.LogInformation($"Created active space '{liveActiveSpace.Name}' (ID: {liveActiveSpace.Id}).");

                // 2. Create another space and archive it
                archivedSpaceNameForTest = $"ArchivedSpace_{Guid.NewGuid()}";
                var liveSpaceToArchive = await _spaceService.CreateSpaceAsync(_testWorkspaceId,
                                    new CreateSpaceRequest(archivedSpaceNameForTest, MultipleAssignees: null, Features: null));
                RegisterCreatedSpace(liveSpaceToArchive.Id);
                archivedSpaceIdForTest = liveSpaceToArchive.Id; // Use live ID
                _output.LogInformation($"Created space to archive '{liveSpaceToArchive.Name}' (ID: {liveSpaceToArchive.Id}).");

                await _spaceService.UpdateSpaceAsync(liveSpaceToArchive.Id,
                    new UpdateSpaceRequest(
                        Name: archivedSpaceNameForTest, // Use the generated name
                        Color: null, Private: null, AdminCanManage: null, MultipleAssignees: null, Features: null, Archived: true
                    ));
                _output.LogInformation($"Archived space '{liveSpaceToArchive.Name}' (ID: {liveSpaceToArchive.Id}).");

                await Task.Delay(2000); // Give ClickUp time to process
            }

            // 3. Get non-archived spaces
            _output.LogInformation($"Fetching non-archived spaces from workspace '{_testWorkspaceId}'.");
            nonArchivedSpaces = (await _spaceService.GetSpacesAsync(_testWorkspaceId, archived: false)).ToList();

            Assert.NotNull(nonArchivedSpaces);
            Assert.Contains(nonArchivedSpaces, s => s.Id == activeSpaceIdForTest && s.Name == activeSpaceNameForTest && s.Archived == false);
            var foundArchivedInNonArchivedQuery = nonArchivedSpaces.FirstOrDefault(s => s.Id == archivedSpaceIdForTest);
            if (foundArchivedInNonArchivedQuery != null)
            {
                _output.LogWarning($"Archived space {archivedSpaceIdForTest} was found in non-archived query. Archived status: {foundArchivedInNonArchivedQuery.Archived}");
            }
            //This assertion might be tricky if other non-archived spaces exist with the same ID by chance, but primary check is above.
            Assert.DoesNotContain(nonArchivedSpaces, s => s.Id == archivedSpaceIdForTest && s.Archived == false); // Check it's not there AND marked non-archived
            _output.LogInformation($"Found {nonArchivedSpaces.Count} non-archived spaces. Active space '{activeSpaceNameForTest}' (ID: {activeSpaceIdForTest}) was present. Archived space '{archivedSpaceNameForTest}' (ID: {archivedSpaceIdForTest}) was not present or not marked as non-archived.");


            // 4. Get archived spaces
            _output.LogInformation($"Fetching archived spaces from workspace '{_testWorkspaceId}'.");
            archivedSpaces = (await _spaceService.GetSpacesAsync(_testWorkspaceId, archived: true)).ToList();

            Assert.NotNull(archivedSpaces);
            var foundArchivedInArchivedQuery = archivedSpaces.FirstOrDefault(s => s.Id == archivedSpaceIdForTest);
            Assert.NotNull(foundArchivedInArchivedQuery);
            Assert.True(foundArchivedInArchivedQuery.Archived, $"Space {archivedSpaceIdForTest} should be marked as archived.");
            Assert.Equal(archivedSpaceNameForTest, foundArchivedInArchivedQuery.Name);
            _output.LogInformation($"Found {archivedSpaces.Count} archived spaces. Archived space '{archivedSpaceNameForTest}' (ID: {archivedSpaceIdForTest}) was present and marked archived.");

            Assert.DoesNotContain(archivedSpaces, s => s.Id == activeSpaceIdForTest);
            _output.LogInformation($"Active space '{activeSpaceNameForTest}' (ID: {activeSpaceIdForTest}) was correctly not in the archived list.");

            // 5. Get all spaces (archived: null or not provided) - typically means non-archived
            _output.LogInformation($"Fetching all (default, non-archived) spaces (archived: null) from workspace '{_testWorkspaceId}'.");
            allSpacesDefault = (await _spaceService.GetSpacesAsync(_testWorkspaceId, archived: null)).ToList();
            Assert.NotNull(allSpacesDefault);
            Assert.Contains(allSpacesDefault, s => s.Id == activeSpaceIdForTest && s.Name == activeSpaceNameForTest && s.Archived == false);
            Assert.DoesNotContain(allSpacesDefault, s => s.Id == archivedSpaceIdForTest && s.Archived == true); // Check it's not there AND marked archived
            _output.LogInformation($"Found {allSpacesDefault.Count} total (default) spaces. Active space '{activeSpaceNameForTest}' was present. Archived space '{archivedSpaceNameForTest}' was not.");
        }

        [Fact]
        public async Task GetSpaceAsync_ExistingSpace_ReturnsSpaceDetails()
        {
            string spaceIdForTest = "space_single_456"; // Predefined for playback
            string spaceNameForTest = "My Single Playback Space";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GetSpace", "GetSpace_Success.json");
                _output.LogInformation($"[Playback] Using response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/space/{spaceIdForTest}")
                               .Respond("application/json", responseContent);
                _output.LogInformation($"[Playback] Mocking GET /space/{spaceIdForTest}");
            }
            else // Record or Passthrough mode
            {
                _output.LogInformation("[Record/Passthrough] Performing live space creation for GetSpaceAsync test.");
                spaceNameForTest = $"SingleSpaceTest_{Guid.NewGuid()}";
                var spaceFeatures = new Features(
                    DueDates: new DueDatesFeature(Enabled: true, StartDateEnabled: false, RemapDueDatesEnabled: true, DueDatesForSubtasksRollUpEnabled: null),
                    Sprints: null,
                    Points: null,
                    CustomTaskIds: null,
                    TimeTracking: new TimeTrackingFeature(Enabled: false, HarvestEnabled: null, RollUpEnabled: null),
                    Tags: new TagsFeature(Enabled: true),
                    TimeEstimates: new TimeEstimatesFeature(Enabled: true, RollUpEnabled: null, PerAssigneeEnabled: null),
                    Checklists: new ChecklistsFeature(Enabled: true),
                    CustomFields: new CustomFieldsFeature(Enabled: true),
                    RemapDependencies: new RemapDependenciesFeature(Enabled: true),
                    DependencyWarning: new DependencyWarningFeature(Enabled: false),
                    MultipleAssignees: null, // Handled by top-level CreateSpaceRequest.MultipleAssignees
                    Portfolios: new PortfoliosFeature(Enabled: false),
                    Emails: null
                );
                var newSpaceRequest = new CreateSpaceRequest(
                    Name: spaceNameForTest,
                    MultipleAssignees: true, // Top-level property for creation
                    Features: spaceFeatures
                );
                var liveSpace = await _spaceService.CreateSpaceAsync(_testWorkspaceId, newSpaceRequest);
                RegisterCreatedSpace(liveSpace.Id);
                spaceIdForTest = liveSpace.Id; // Use live ID for this mode
                _output.LogInformation($"Created space '{liveSpace.Name}' (ID: {liveSpace.Id}) for GetSpaceAsync test.");

                // Allow a brief moment for the space to be fully available via GET
                await Task.Delay(1000);
            }

            // Act
            _output.LogInformation($"Fetching space details for ID: {spaceIdForTest}");
            var space = await _spaceService.GetSpaceAsync(spaceIdForTest);

            // Assert
            Assert.NotNull(space);
            Assert.Equal(spaceIdForTest, space.Id);
            Assert.Equal(spaceNameForTest, space.Name); // Name might differ if live ID was different from playback's expected name
            Assert.False(space.Archived);

            if (CurrentTestMode == TestMode.Playback) // More detailed assertions for playback based on the static JSON
            {
                Assert.Equal("My Single Playback Space", space.Name); // Explicitly check name from JSON
                Assert.True(space.Features.Tags.Enabled);
                Assert.False(space.Features.TimeTracking.Enabled);
                Assert.NotEmpty(space.Statuses);
                Assert.Contains(space.Statuses, s => s.StatusValue == "in progress");
                Assert.NotEmpty(space.Members);
                Assert.Contains(space.Members, m => m.User.Username == "Playback User");
            }
            _output.LogInformation($"Successfully fetched and validated space '{space.Name}' (ID: {space.Id}).");
        }
    }
}
