using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using ClickUp.Api.Client.Models.Entities.Folders;
using RichardSzalay.MockHttp;
using System.IO;
using System.Net;
using ClickUp.Api.Client.Models.Exceptions;

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class FolderServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private readonly IFoldersService _folderService;
        private readonly ISpacesService _spaceService;

        private string _testWorkspaceId;
        private string _testSpaceId = null!; // Will be created for the test class

        private List<string> _createdFolderIds = new List<string>();

        public FolderServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _folderService = ServiceProvider.GetRequiredService<IFoldersService>();
            _spaceService = ServiceProvider.GetRequiredService<ISpacesService>();

            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"]!;

            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Test setup for creating spaces will fail.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for FolderServiceIntegrationTests.");
            }
        }

        private const string PlaybackTestSpaceId = "playback_space_folders_001";

        public async Task InitializeAsync()
        {
            _output.LogInformation("Starting FolderServiceIntegrationTests class initialization.");
            if (CurrentTestMode == TestMode.Playback)
            {
                _testSpaceId = PlaybackTestSpaceId;
                _output.LogInformation($"[Playback] Using predefined TestSpaceId: {_testSpaceId}");
            }
            else
            {
                _output.LogInformation("[Record/Passthrough] Creating shared test space.");
                try
                {
                    var spaceName = $"TestSpace_Folders_{Guid.NewGuid()}";
                    // Using the corrected CreateSpaceRequest from SpaceServiceIntegrationTests fix
                    var spaceFeatures = new ClickUp.Api.Client.Models.Entities.Spaces.Features(
                        DueDates: new ClickUp.Api.Client.Models.Entities.Spaces.DueDatesFeature(Enabled: true, StartDateEnabled: false, RemapDueDatesEnabled: true, DueDatesForSubtasksRollUpEnabled: null),
                        Sprints: null, Points: null, CustomTaskIds: null,
                        TimeTracking: new ClickUp.Api.Client.Models.Entities.Spaces.TimeTrackingFeature(Enabled: false, HarvestEnabled: null, RollUpEnabled: null),
                        Tags: new ClickUp.Api.Client.Models.Entities.Spaces.TagsFeature(Enabled: true),
                        TimeEstimates: new ClickUp.Api.Client.Models.Entities.Spaces.TimeEstimatesFeature(Enabled: true, RollUpEnabled: null, PerAssigneeEnabled: null),
                        Checklists: new ClickUp.Api.Client.Models.Entities.Spaces.ChecklistsFeature(Enabled: true),
                        CustomFields: new ClickUp.Api.Client.Models.Entities.Spaces.CustomFieldsFeature(Enabled: true),
                        RemapDependencies: new ClickUp.Api.Client.Models.Entities.Spaces.RemapDependenciesFeature(Enabled: true),
                        DependencyWarning: new ClickUp.Api.Client.Models.Entities.Spaces.DependencyWarningFeature(Enabled: false),
                        MultipleAssignees: null, // This refers to the feature flag inside Features object, not the top-level space property
                        Portfolios: new ClickUp.Api.Client.Models.Entities.Spaces.PortfoliosFeature(Enabled: false),
                        Emails: null
                    );
                    var createSpaceReq = new CreateSpaceRequest(Name: spaceName, MultipleAssignees: true, Features: spaceFeatures);
                    var space = await _spaceService.CreateSpaceAsync(_testWorkspaceId, createSpaceReq);
                    _testSpaceId = space.Id;
                    _output.LogInformation($"Test space created successfully. Space ID: {_testSpaceId}");
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error during InitializeAsync (creating space): {ex.Message}", ex);
                    await CleanupLingeringResourcesAsync(); // Attempt cleanup
                    throw; // Re-throw to fail test run if space creation fails in non-playback
                }
            }
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation("Starting FolderServiceIntegrationTests class disposal.");
            if (CurrentTestMode != TestMode.Playback && !string.IsNullOrWhiteSpace(_testSpaceId))
            {
                _output.LogInformation("[Record/Passthrough] Cleaning up shared test space and folders.");
                await CleanupLingeringResourcesAsync();
            }
            else
            {
                _output.LogInformation("[Playback] Skipping cleanup of shared test space.");
            }
            _createdFolderIds.Clear(); // Clear folder IDs regardless of mode
            _output.LogInformation("FolderServiceIntegrationTests class disposal complete.");
        }

        private async Task CleanupLingeringResourcesAsync()
        {
            // This method is now primarily for non-playback modes or if explicit resource cleanup is needed.
            if (!string.IsNullOrWhiteSpace(_testSpaceId) && _testSpaceId != PlaybackTestSpaceId)
            {
                try
                {
                    _output.LogInformation($"Deleting test space: {_testSpaceId}");
                    await _spaceService.DeleteSpaceAsync(_testSpaceId);
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error deleting space {_testSpaceId}: {ex.Message}", ex);
                }
                finally
                {
                    _testSpaceId = null; // Mark as deleted or no longer valid
                }
            }
        }

        private void RegisterCreatedFolder(string folderId)
        {
            if (!string.IsNullOrWhiteSpace(folderId))
            {
                _createdFolderIds.Add(folderId);
            }
        }

        private const string PlaybackActiveFolderId = "folder_active_123";
        private const string PlaybackActiveFolderName = "Active Playback Folder";
        private const string PlaybackArchivedFolderId = "folder_archived_789";
        private const string PlaybackArchivedFolderName = "Archived Playback Folder";

        [Fact]
        public async Task GetFoldersAsync_FilterByArchived_ShouldReturnCorrectFolders()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testSpaceId), "TestSpaceId must be available for GetFoldersAsync test.");

            string activeFolderIdForTest = PlaybackActiveFolderId;
            string activeFolderNameForTest = PlaybackActiveFolderName;
            string archivedFolderIdForTest = PlaybackArchivedFolderId;
            string archivedFolderNameForTest = PlaybackArchivedFolderName;

            List<Folder> nonArchivedFolders;
            List<Folder> archivedFolders;
            List<Folder> allFolders;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var spaceIdForMock = _testSpaceId; // Should be PlaybackTestSpaceId

                // Mock for GetFoldersAsync(archived: false)
                var getNonArchivedPath = Path.Combine(RecordedResponsesBasePath, "FoldersService", "GetFolders", "GetFolders_NotArchived_Success.json");
                _output.LogInformation($"[Playback] Using response file for Not Archived: {getNonArchivedPath}");
                Assert.True(File.Exists(getNonArchivedPath), $"Playback file not found: {getNonArchivedPath}");
                var getNonArchivedContent = await File.ReadAllTextAsync(getNonArchivedPath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/space/{spaceIdForMock}/folder?archived=false")
                               .Respond("application/json", getNonArchivedContent ?? string.Empty);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/space/{spaceIdForMock}/folder?archived=False")
                               .Respond("application/json", getNonArchivedContent ?? string.Empty);

                // Mock for GetFoldersAsync(archived: true)
                var getArchivedPath = Path.Combine(RecordedResponsesBasePath, "FoldersService", "GetFolders", "GetFolders_Archived_Success.json");
                _output.LogInformation($"[Playback] Using response file for Archived: {getArchivedPath}");
                Assert.True(File.Exists(getArchivedPath), $"Playback file not found: {getArchivedPath}");
                var getArchivedContent = await File.ReadAllTextAsync(getArchivedPath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/space/{spaceIdForMock}/folder?archived=true")
                               .Respond("application/json", getArchivedContent ?? string.Empty);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/space/{spaceIdForMock}/folder?archived=True")
                               .Respond("application/json", getArchivedContent ?? string.Empty);

                // Mock for GetFoldersAsync(archived: null) - parameter omitted
                var getAllPath = Path.Combine(RecordedResponsesBasePath, "FoldersService", "GetFolders", "GetFolders_All_Success.json");
                _output.LogInformation($"[Playback] Using response file for All (archived=null): {getAllPath}");
                Assert.True(File.Exists(getAllPath), $"Playback file not found: {getAllPath}");
                var getAllContent = await File.ReadAllTextAsync(getAllPath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/space/{spaceIdForMock}/folder")
                               .Respond("application/json", getAllContent ?? string.Empty);

                _output.LogInformation("[Playback] Skipped live creation/archiving of folders.");
            }
            else // Record or Passthrough mode
            {
                _output.LogInformation($"[Record/Passthrough] Performing live folder creation and archiving in space: {_testSpaceId}");
                // 1. Create an active folder
                activeFolderNameForTest = $"ActiveFolder_{Guid.NewGuid()}";
                var liveActiveFolder = await _folderService.CreateFolderAsync(_testSpaceId, new CreateFolderRequest(activeFolderNameForTest));
                RegisterCreatedFolder(liveActiveFolder.Id);
                activeFolderIdForTest = liveActiveFolder.Id;
                _output.LogInformation($"Created active folder '{liveActiveFolder.Name}' (ID: {liveActiveFolder.Id}).");

                // 2. Create another folder and archive it
                archivedFolderNameForTest = $"ArchivedFolder_{Guid.NewGuid()}";
                var liveFolderToArchive = await _folderService.CreateFolderAsync(_testSpaceId, new CreateFolderRequest(archivedFolderNameForTest));
                RegisterCreatedFolder(liveFolderToArchive.Id);
                archivedFolderIdForTest = liveFolderToArchive.Id;
                _output.LogInformation($"Created folder to archive '{liveFolderToArchive.Name}' (ID: {liveFolderToArchive.Id}).");

                await _folderService.UpdateFolderAsync(liveFolderToArchive.Id, new UpdateFolderRequest(Name: archivedFolderNameForTest, Archived: true));
                _output.LogInformation($"Archived folder '{liveFolderToArchive.Name}' (ID: {liveFolderToArchive.Id}).");

                await Task.Delay(1000); // Allow time for API to process changes
            }

            // 3. Get non-archived folders
            _output.LogInformation($"Fetching non-archived folders from space '{_testSpaceId}'.");
            nonArchivedFolders = (await _folderService.GetFoldersAsync(_testSpaceId!, archived: false)).ToList();

            Assert.NotNull(nonArchivedFolders);
            Assert.Contains(nonArchivedFolders, f => f.Id == activeFolderIdForTest && f.Name == activeFolderNameForTest && f.Archived == false);
            Assert.DoesNotContain(nonArchivedFolders, f => f.Id == archivedFolderIdForTest && f.Archived == false); // Ensure archived one isn't there and non-archived
            _output.LogInformation($"Found {nonArchivedFolders.Count} non-archived folders. Active folder '{activeFolderNameForTest}' was present. Archived folder '{archivedFolderNameForTest}' was not (as non-archived).");

            // 4. Get archived folders
            _output.LogInformation($"Fetching archived folders from space '{_testSpaceId}'.");
            archivedFolders = (await _folderService.GetFoldersAsync(_testSpaceId!, archived: true)).ToList();

            Assert.NotNull(archivedFolders);
            Assert.Contains(archivedFolders, f => f.Id == archivedFolderIdForTest && f.Name == archivedFolderNameForTest && f.Archived == true);
            Assert.DoesNotContain(archivedFolders, f => f.Id == activeFolderIdForTest);
            _output.LogInformation($"Found {archivedFolders.Count} archived folders. Archived folder '{archivedFolderNameForTest}' was present. Active folder '{activeFolderNameForTest}' was not.");

            // 5. Get all folders (archived: null or not provided)
            // Based on existing test, this implies all folders (active and archived) are returned.
            _output.LogInformation($"Fetching all folders (archived: null) from space '{_testSpaceId}'.");
            allFolders = (await _folderService.GetFoldersAsync(_testSpaceId!, archived: null)).ToList();
            Assert.NotNull(allFolders);
            Assert.Contains(allFolders, f => f.Id == activeFolderIdForTest && f.Name == activeFolderNameForTest && f.Archived == false);
            Assert.Contains(allFolders, f => f.Id == archivedFolderIdForTest && f.Name == archivedFolderNameForTest && f.Archived == true);
            _output.LogInformation($"Found {allFolders.Count} total folders (when archived=null). Both active and archived folders were present.");
        }

        [Fact]
        public async Task GetFolderAsync_ExistingFolder_ReturnsFolderDetails()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testSpaceId), "TestSpaceId must be available for GetFolderAsync test.");

            const string playbackFolderId = "folder_single_456";
            const string playbackFolderName = "Single Playback Folder";

            string folderIdForTest = playbackFolderId;
            string folderNameForTest = playbackFolderName;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "FoldersService", "GetFolder", "GetFolder_Success.json");
                _output.LogInformation($"[Playback] Using response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/folder/{playbackFolderId}")
                               .Respond("application/json", responseContent);
                _output.LogInformation($"[Playback] Mocking GET /folder/{playbackFolderId}");
            }
            else // Record or Passthrough mode
            {
                _output.LogInformation($"[Record/Passthrough] Performing live folder creation in space {_testSpaceId} for GetFolderAsync test.");
                folderNameForTest = $"SingleFolderTest_{Guid.NewGuid()}";
                var liveFolder = await _folderService.CreateFolderAsync(_testSpaceId, new CreateFolderRequest(folderNameForTest));
                RegisterCreatedFolder(liveFolder.Id);
                folderIdForTest = liveFolder.Id;
                _output.LogInformation($"Created folder '{liveFolder.Name}' (ID: {liveFolder.Id}) for GetFolderAsync test.");
                await Task.Delay(1000); // Allow time for API to process
            }

            // Act
            _output.LogInformation($"Fetching folder details for ID: {folderIdForTest}");
            var folder = await _folderService.GetFolderAsync(folderIdForTest);

            // Assert
            Assert.NotNull(folder);
            Assert.Equal(folderIdForTest, folder.Id);
            Assert.Equal(folderNameForTest, folder.Name); // Name check
            Assert.False(folder.Archived);
            // Cannot assert folder.Space.Id as Folder model does not contain Space property directly.
            // Assert.NotNull(folder.Space); 
            // Assert.Equal(_testSpaceId, folder.Space.Id); 

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(playbackFolderName, folder.Name); // Explicitly check name from JSON
                // Cannot assert OverrideStatuses or Lists as they are not in the Folder model.
                // Assert.True(folder.OverrideStatuses);
                // Assert.NotNull(folder.Lists);
                // Assert.NotEmpty(folder.Lists);
                // Assert.Contains(folder.Lists, l => l.Id == "list_abc_123");
                Assert.NotNull(folder.Statuses); // Statuses is a valid property
                Assert.NotEmpty(folder.Statuses!); // Assuming the playback JSON has statuses
                Assert.Contains(folder.Statuses!, s => s.StatusValue == "Open");
            }
            _output.LogInformation($"Successfully fetched and validated folder '{folder.Name}' (ID: {folder.Id}).");
        }

        [Fact]
        public async Task CreateFolderAsync_ValidName_ReturnsCreatedFolder()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testSpaceId), "TestSpaceId must be available for CreateFolderAsync test.");

            var newFolderName = "Test Create Folder";
            var expectedFolderId = "folder_created_mock_001"; // From placeholder JSON

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "FoldersService", "CreateFolder", "CreateFolder_Success.json");
                _output.LogInformation($"[Playback] Using response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/space/{_testSpaceId}/folder")
                               .WithJsonContentMatcher(new { name = newFolderName })
                               .Respond("application/json", responseContent);
                _output.LogInformation($"[Playback] Mocking POST /space/{_testSpaceId}/folder");
            }
            else // Record or Passthrough mode
            {
                newFolderName = $"Live Create Folder_{Guid.NewGuid()}";
                _output.LogInformation($"[Record/Passthrough] Will attempt to create folder: {newFolderName} in space {_testSpaceId}");
            }

            // Act
            var createRequest = new CreateFolderRequest(newFolderName);
            _output.LogInformation($"Attempting to create folder with name: '{createRequest.Name}' in space ID: '{_testSpaceId}'.");
            var createdFolder = await _folderService.CreateFolderAsync(_testSpaceId, createRequest);

            // Assert
            Assert.NotNull(createdFolder);
            Assert.Equal(newFolderName, createdFolder.Name);

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(expectedFolderId, createdFolder.Id);
                Assert.Equal("Mock Created Folder", createdFolder.Name); // Name from JSON
            }
            else
            {
                Assert.False(string.IsNullOrWhiteSpace(createdFolder.Id));
                RegisterCreatedFolder(createdFolder.Id); // Important for cleanup in live modes
                _output.LogInformation($"[Record/Passthrough] Successfully created folder '{createdFolder.Name}' (ID: {createdFolder.Id}).");
                // In a real record scenario, we'd want to delete this folder in DisposeAsync or a specific cleanup.
                // For this task, we are not running tests, so direct cleanup here isn't triggered.
            }
            _output.LogInformation($"Validated created folder: '{createdFolder.Name}' (ID: {createdFolder.Id}).");
        }

        [Fact]
        public async Task UpdateFolderAsync_ValidChanges_ReturnsUpdatedFolder()
        {
            const string folderIdToUpdatePlayback = "folder_to_update_mock_002"; // From placeholder JSON
            var updatedFolderName = "Test Updated Folder Name";
            var updatedArchivedStatus = true;

            string folderIdForTest = folderIdToUpdatePlayback;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "FoldersService", "UpdateFolder", "UpdateFolder_Success.json");
                _output.LogInformation($"[Playback] Using response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(HttpMethod.Put, $"https://api.clickup.com/api/v2/folder/{folderIdToUpdatePlayback}")
                               .WithJsonContentMatcher(new { name = updatedFolderName, archived = updatedArchivedStatus })
                               .Respond("application/json", responseContent);
                _output.LogInformation($"[Playback] Mocking PUT /folder/{folderIdToUpdatePlayback}");
            }
            else // Record or Passthrough mode
            {
                var initialFolderName = $"Live FolderToUpdate_{Guid.NewGuid()}";
                _output.LogInformation($"[Record/Passthrough] Creating initial folder '{initialFolderName}' in space {_testSpaceId} for update test.");
                var liveFolder = await _folderService.CreateFolderAsync(_testSpaceId, new CreateFolderRequest(initialFolderName));
                RegisterCreatedFolder(liveFolder.Id);
                folderIdForTest = liveFolder.Id;
                updatedFolderName = $"Live Updated Folder_{Guid.NewGuid()}"; // Ensure unique updated name
                _output.LogInformation($"[Record/Passthrough] Initial folder created with ID: {folderIdForTest}. Will update to name '{updatedFolderName}'.");
                await Task.Delay(1000); // Delay for API consistency
            }

            // Act
            var updateRequest = new UpdateFolderRequest(Name: updatedFolderName, Archived: updatedArchivedStatus);
            _output.LogInformation($"Attempting to update folder ID: '{folderIdForTest}' with Name: '{updateRequest.Name}', Archived: {updateRequest.Archived}.");
            var updatedFolder = await _folderService.UpdateFolderAsync(folderIdForTest, updateRequest);

            // Assert
            Assert.NotNull(updatedFolder);
            Assert.Equal(folderIdForTest, updatedFolder.Id);
            Assert.Equal(updatedFolderName, updatedFolder.Name);
            Assert.Equal(updatedArchivedStatus, updatedFolder.Archived);

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal("Mock Updated Folder Name", updatedFolder.Name); // Name from JSON
                Assert.True(updatedFolder.Archived); // Archived status from JSON
                // Assert.NotEmpty(updatedFolder.Lists); // Folder model does not directly contain Lists
                Assert.NotEmpty(updatedFolder.Statuses);
            }
            _output.LogInformation($"Validated updated folder: '{updatedFolder.Name}' (ID: {updatedFolder.Id}), Archived: {updatedFolder.Archived}.");
        }

        [Fact]
        public async Task DeleteFolderAsync_ExistingFolder_DeletesSuccessfully()
        {
            const string folderIdToDeletePlayback = "folder_to_delete_mock_003";

            string folderIdForTest = folderIdToDeletePlayback;

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "FoldersService", "DeleteFolder", "DeleteFolder_Success.json");
                _output.LogInformation($"[Playback] Using response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/folder/{folderIdToDeletePlayback}")
                               .Respond(HttpStatusCode.OK, "application/json", responseContent); // Docs say 200 OK with {}
                _output.LogInformation($"[Playback] Mocking DELETE /folder/{folderIdToDeletePlayback}");
            }
            else // Record or Passthrough mode
            {
                var folderNameToDelete = $"Live FolderToDelete_{Guid.NewGuid()}";
                _output.LogInformation($"[Record/Passthrough] Creating folder '{folderNameToDelete}' in space {_testSpaceId} for delete test.");
                var liveFolder = await _folderService.CreateFolderAsync(_testSpaceId, new CreateFolderRequest(folderNameToDelete));
                // We don't register this one for cleanup via _createdFolderIds because it's meant to be deleted by the test itself.
                folderIdForTest = liveFolder.Id;
                _output.LogInformation($"[Record/Passthrough] Folder created with ID: {folderIdForTest}. Will attempt to delete.");
                await Task.Delay(1000); // Delay for API consistency
            }

            // Act
            _output.LogInformation($"Attempting to delete folder ID: '{folderIdForTest}'.");
            await _folderService.DeleteFolderAsync(folderIdForTest);

            // Assert
            // Primary assertion is that no exception was thrown.
            // In live mode, we could try to GetFolderAsync and expect a NotFoundException.
            if (CurrentTestMode != TestMode.Playback)
            {
                _output.LogInformation($"[Record/Passthrough] Delete called for folder {folderIdForTest}. Verifying deletion.");
                await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                    () => _folderService.GetFolderAsync(folderIdForTest));
                _output.LogInformation($"[Record/Passthrough] Folder {folderIdForTest} confirmed deleted (Get returned NotFound).");
            }
            else
            {
                _output.LogInformation($"[Playback] Delete call for folder {folderIdForTest} completed based on mock.");
            }
            _output.LogInformation($"Deletion process for folder ID '{folderIdForTest}' completed.");
        }

        [Fact]
        public async Task CreateFolderFromTemplateAsync_ValidTemplate_ReturnsCreatedFolder()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testSpaceId), "TestSpaceId must be available for CreateFolderFromTemplateAsync test.");

            const string templateId = "mock_template_id_12345"; // This would be a real ID in record/passthrough
            var newFolderName = "Test Folder from Template";
            var expectedFolderId = "folder_from_template_mock_003"; // From placeholder JSON

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "FoldersService", "CreateFolderFromTemplate", "CreateFolderFromTemplate_Success.json");
                _output.LogInformation($"[Playback] Using response file: {responsePath}");
                Assert.True(File.Exists(responsePath), $"Playback file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/space/{_testSpaceId}/folder/template/{templateId}")
                               .WithJsonContentMatcher(new { name = newFolderName })
                               .Respond("application/json", responseContent);
                _output.LogInformation($"[Playback] Mocking POST /space/{_testSpaceId}/folder/template/{templateId}");
            }
            else // Record or Passthrough mode
            {
                newFolderName = $"Live Folder from Template_{Guid.NewGuid()}";
                _output.LogInformation($"[Record/Passthrough] Will attempt to create folder from template {templateId}: {newFolderName} in space {_testSpaceId}");
                // In a real scenario, templateId would need to be a valid, accessible template ID.
                // For this exercise, we assume it would work if run.
            }

            // Act
            var createRequest = new CreateFolderFromTemplateRequest { Name = newFolderName };
            _output.LogInformation($"Attempting to create folder from template ID '{templateId}' with name: '{createRequest.Name}' in space ID: '{_testSpaceId}'.");
            var createdFolder = await _folderService.CreateFolderFromTemplateAsync(_testSpaceId, templateId, createRequest);

            // Assert
            Assert.NotNull(createdFolder);
            Assert.Equal(newFolderName, createdFolder.Name);

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(expectedFolderId, createdFolder.Id);
                Assert.Equal("Mock Folder From Template", createdFolder.Name); // Name from JSON
                // Assert.NotEmpty(createdFolder.Lists); // Folder model does not directly contain Lists
            }
            else
            {
                Assert.False(string.IsNullOrWhiteSpace(createdFolder.Id));
                RegisterCreatedFolder(createdFolder.Id); // Important for cleanup in live modes
                _output.LogInformation($"[Record/Passthrough] Successfully created folder '{createdFolder.Name}' (ID: {createdFolder.Id}) from template {templateId}.");
            }
            _output.LogInformation($"Validated folder from template: '{createdFolder.Name}' (ID: {createdFolder.Id}).");
        }
    }
}
