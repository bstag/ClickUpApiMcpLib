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

            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"];

            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Test setup for creating spaces will fail.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for FolderServiceIntegrationTests.");
            }
        }

        public async Task InitializeAsync()
        {
            _output.LogInformation("Starting FolderServiceIntegrationTests class initialization: Creating shared test space.");
            try
            {
                var spaceName = $"TestSpace_Folders_{Guid.NewGuid()}";
                var createSpaceReq = new CreateSpaceRequest(spaceName, null, null);
                var space = await _spaceService.CreateSpaceAsync(_testWorkspaceId, createSpaceReq);
                _testSpaceId = space.Id;
                _output.LogInformation($"Test space created successfully. Space ID: {_testSpaceId}");
            }
            catch (Exception ex)
            {
                _output.LogError($"Error during InitializeAsync: {ex.Message}", ex);
                await CleanupLingeringResourcesAsync(); // Attempt cleanup
                throw;
            }
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation("Starting FolderServiceIntegrationTests class disposal: Cleaning up created folders and shared space.");
            // Folders are children of the space. Deleting the space should delete them.
            // However, if a test fails mid-way, explicit folder deletion might be desired if not relying on space deletion.
            // For now, rely on space deletion via CleanupLingeringResourcesAsync.
            _createdFolderIds.Clear();
            await CleanupLingeringResourcesAsync();
            _output.LogInformation("FolderServiceIntegrationTests class disposal complete.");
        }

        private async Task CleanupLingeringResourcesAsync()
        {
            if (!string.IsNullOrWhiteSpace(_testSpaceId))
            {
                try
                {
                    _output.LogInformation($"Deleting test space: {_testSpaceId}");
                    await _spaceService.DeleteSpaceAsync(_testSpaceId);
                    _testSpaceId = null; // Mark as deleted
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error deleting space {_testSpaceId}: {ex.Message}", ex);
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

        [Fact]
        public async Task GetFoldersAsync_FilterByArchived_ShouldReturnCorrectFolders()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testSpaceId), "TestSpaceId must be available.");

            // 1. Create an active folder
            var activeFolderName = $"ActiveFolder_{Guid.NewGuid()}";
            var activeFolder = await _folderService.CreateFolderAsync(_testSpaceId, new CreateFolderRequest(activeFolderName));
            RegisterCreatedFolder(activeFolder.Id);
            _output.LogInformation($"Created active folder '{activeFolder.Name}' (ID: {activeFolder.Id}).");

            // 2. Create another folder and archive it
            var folderToArchiveName = $"ArchivedFolder_{Guid.NewGuid()}";
            var folderToArchive = await _folderService.CreateFolderAsync(_testSpaceId, new CreateFolderRequest(folderToArchiveName));
            RegisterCreatedFolder(folderToArchive.Id); // Register for cleanup in case archive fails
            _output.LogInformation($"Created folder to archive '{folderToArchive.Name}' (ID: {folderToArchive.Id}).");

            await _folderService.UpdateFolderAsync(folderToArchive.Id, new UpdateFolderRequest(Name: folderToArchiveName, Archived: true));
            _output.LogInformation($"Archived folder '{folderToArchive.Name}' (ID: {folderToArchive.Id}).");

            // 3. Get non-archived folders
            _output.LogInformation($"Fetching non-archived folders from space '{_testSpaceId}'.");
            var nonArchivedFolders = (await _folderService.GetFoldersAsync(_testSpaceId, archived: false)).ToList();

            Assert.NotNull(nonArchivedFolders);
            Assert.Contains(nonArchivedFolders, f => f.Id == activeFolder.Id && f.Name == activeFolderName && f.Archived == false);
            Assert.DoesNotContain(nonArchivedFolders, f => f.Id == folderToArchive.Id);
            _output.LogInformation($"Found {nonArchivedFolders.Count} non-archived folders. Active folder was present. Archived folder was not.");

            // 4. Get archived folders
            _output.LogInformation($"Fetching archived folders from space '{_testSpaceId}'.");
            var archivedFolders = (await _folderService.GetFoldersAsync(_testSpaceId, archived: true)).ToList();

            Assert.NotNull(archivedFolders);
            Assert.Contains(archivedFolders, f => f.Id == folderToArchive.Id && f.Name == folderToArchiveName && f.Archived == true);
            Assert.DoesNotContain(archivedFolders, f => f.Id == activeFolder.Id);
            _output.LogInformation($"Found {archivedFolders.Count} archived folders. Archived folder was present. Active folder was not.");

            // 5. Get all folders (archived: null or not provided)
             _output.LogInformation($"Fetching all folders (archived: null) from space '{_testSpaceId}'.");
            var allFolders = (await _folderService.GetFoldersAsync(_testSpaceId, archived: null)).ToList();
            Assert.NotNull(allFolders);
            Assert.Contains(allFolders, f => f.Id == activeFolder.Id);
            Assert.Contains(allFolders, f => f.Id == folderToArchive.Id);
            _output.LogInformation($"Found {allFolders.Count} total folders. Both active and archived folders were present.");
        }
    }
}
