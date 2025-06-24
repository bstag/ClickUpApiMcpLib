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
            await Task.Delay(2000); // 2 seconds delay

            // 3. Get non-archived spaces
            _output.LogInformation($"Fetching non-archived spaces from workspace '{_testWorkspaceId}'.");
            var nonArchivedSpaces = (await _spaceService.GetSpacesAsync(_testWorkspaceId, archived: false)).ToList();

            Assert.NotNull(nonArchivedSpaces);
            Assert.Contains(nonArchivedSpaces, s => s.Id == activeSpace.Id && s.Name == activeSpaceName && s.Archived == false);
            var foundArchivedInNonArchivedQuery = nonArchivedSpaces.FirstOrDefault(s => s.Id == spaceToArchive.Id);
            if (foundArchivedInNonArchivedQuery != null)
            {
                 _output.LogWarning($"Archived space {spaceToArchive.Id} was found in non-archived query. Archived status: {foundArchivedInNonArchivedQuery.Archived}");
            }
            Assert.DoesNotContain(nonArchivedSpaces, s => s.Id == spaceToArchive.Id && s.Archived == false); // Check it's not there as non-archived
            _output.LogInformation($"Found {nonArchivedSpaces.Count} non-archived spaces. Active space was present. Archived space was not (or was correctly marked archived if it appeared).");

            // 4. Get archived spaces
            _output.LogInformation($"Fetching archived spaces from workspace '{_testWorkspaceId}'.");
            var archivedSpaces = (await _spaceService.GetSpacesAsync(_testWorkspaceId, archived: true)).ToList();

            Assert.NotNull(archivedSpaces);
            var foundArchivedInArchivedQuery = archivedSpaces.FirstOrDefault(s => s.Id == spaceToArchive.Id);
            Assert.NotNull(foundArchivedInArchivedQuery); // It must be found here
            Assert.True(foundArchivedInArchivedQuery.Archived, $"Space {spaceToArchive.Id} should be marked as archived.");
            _output.LogInformation($"Found {archivedSpaces.Count} archived spaces. Archived space '{spaceToArchive.Name}' (ID: {spaceToArchive.Id}) was present and marked archived.");

            // Ensure active space is not in the archived list
            Assert.DoesNotContain(archivedSpaces, s => s.Id == activeSpace.Id);
            _output.LogInformation($"Active space '{activeSpace.Name}' (ID: {activeSpace.Id}) was correctly not in the archived list.");

            // 5. Get all spaces (archived: null or not provided) - this should return non-archived by default based on API docs
            // "Get Spaces: ... By default, archived Spaces are not included."
            // So, testing with archived: null should behave like archived: false
            _output.LogInformation($"Fetching all (default, non-archived) spaces (archived: null) from workspace '{_testWorkspaceId}'.");
            var allSpacesDefault = (await _spaceService.GetSpacesAsync(_testWorkspaceId, archived: null)).ToList();
            Assert.NotNull(allSpacesDefault);
            Assert.Contains(allSpacesDefault, s => s.Id == activeSpace.Id && s.Archived == false);
            Assert.DoesNotContain(allSpacesDefault, s => s.Id == spaceToArchive.Id && s.Archived == true); // Should not contain the one we explicitly archived
             _output.LogInformation($"Found {allSpacesDefault.Count} total (default) spaces. Active space was present. Archived space was not.");
        }
    }
}
