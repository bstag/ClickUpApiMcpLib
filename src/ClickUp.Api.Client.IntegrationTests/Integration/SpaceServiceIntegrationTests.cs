using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using ClickUp.Api.Client.Models.RequestModels.Spaces; // For CreateSpaceRequest, UpdateSpaceRequest
// Remove direct using for ResponseModels.Spaces.Space as it does not exist.
// Individual response DTOs like GetSpacesResponse will be used directly or Space entity.
using ClickUp.Api.Client.Models.Entities.Spaces; // For Space entity and Features record (and its children like DueDatesFeature)
using ClickUp.Api.Client.Models.ResponseModels.Spaces; // For GetSpacesResponse
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp; // For MockHttpMessageHandler
using Xunit;
using Xunit.Abstractions;

namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class SpaceServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private readonly ISpacesService _spaceService;
        private readonly string _testWorkspaceId;
        // private TestHierarchyContext _hierarchyContext = null!; // Might not be needed if tests are workspace-level

        private List<string> _createdSpaceIdsForCleanup = new List<string>();

        public SpaceServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _spaceService = ServiceProvider.GetRequiredService<ISpacesService>();
            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"]!;

            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Test setup will fail for tests requiring a workspace ID.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for SpaceServiceIntegrationTests.");
            }
        }

        public async Task InitializeAsync()
        {
            _output.LogInformation($"[SpaceServiceIntegrationTests] Initializing for Workspace: {_testWorkspaceId}. Test Mode: {CurrentTestMode}");
            // For SpaceService, most operations are on existing spaces or creating new ones.
            // We don't necessarily need a full Space/Folder/List hierarchy for every test,
            // but some tests might create and delete spaces.
            // If a shared space is needed for multiple tests, it could be created here.
            // For now, individual tests will manage their own space creation if needed.

            if (CurrentTestMode == TestMode.Playback)
            {
                // If there are common setup calls that need mocking for ALL tests in Playback mode,
                // they would go here. For example, if GetWorkspaces was always called.
                // For GetSpaces, it's usually specific to a workspace.
            }
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation($"[SpaceServiceIntegrationTests] Disposing. Cleaning up {_createdSpaceIdsForCleanup.Count} spaces.");
            var cleanupTasks = _createdSpaceIdsForCleanup.Select(async spaceId =>
            {
                try
                {
                    if (CurrentTestMode == TestMode.Playback)
                    {
                        // In Playback mode, if a test created a resource (even a mocked one) and registered it for cleanup,
                        // it's responsible for ensuring that the Delete operation for that resource ID is also mocked.
                        // If no mock is found for the delete operation, MockHttpHandler will throw an error,
                        // which is acceptable as it indicates an incomplete playback setup for a test.
                        _output.LogInformation($"[Playback] Attempting dispose cleanup for space {spaceId}. Expecting DELETE mock if created by test.");
                        await _spaceService.DeleteSpaceAsync(spaceId); // This will use the mock if available
                        _output.LogInformation($"[Playback] Mocked DELETE for space {spaceId} completed.");
                        return; // Successfully used mock
                    }
                    // For Record or Passthrough mode, attempt live deletion.
                    _output.LogInformation($"[Record/Passthrough] Deleting space: {spaceId}");
                    await _spaceService.DeleteSpaceAsync(spaceId);
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error deleting space {spaceId}: {ex.Message}", ex);
                    // Don't rethrow, attempt to clean up other resources
                }
            }).ToList();

            if (cleanupTasks.Any())
            {
                await Task.WhenAll(cleanupTasks);
            }
            _createdSpaceIdsForCleanup.Clear();
            _output.LogInformation("[SpaceServiceIntegrationTests] Disposal complete.");
        }

        private void RegisterCreatedSpace(string spaceId)
        {
            if (!string.IsNullOrWhiteSpace(spaceId))
            {
                _createdSpaceIdsForCleanup.Add(spaceId);
                _output.LogInformation($"Registered space for cleanup: {spaceId}");
            }
        }

        [Fact]
        public async Task GetSpacesAsync_WhenCalled_ShouldReturnSpacesInWorkspace()
        {
            // Arrange
            Assert.False(string.IsNullOrWhiteSpace(_testWorkspaceId), "TestWorkspaceId must be configured.");
            _output.LogInformation($"Getting spaces for workspace: {_testWorkspaceId}");

            var expectedUrl = $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/space?archived=false";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // Path determined from handler logs: SpaceService/GETGetSpaces/Success_queryHASH.json
                // The hash 'a4b7f194' was seen in logs for ?archived=false
                var responsePath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GETGetSpaces", "Success_querya4b7f194.json");
                Assert.True(File.Exists(responsePath), $"Mock data file not found: {responsePath}");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(expectedUrl)
                               .Respond("application/json", responseContent);
                _output.LogInformation($"[Playback] Mocking GET spaces. Url: {expectedUrl}. Response from: {responsePath}");
            }

            // Act
            IEnumerable<Space>? result = null;
            try
            {
                result = await _spaceService.GetSpacesAsync(_testWorkspaceId, false);
            }
            catch (Exception ex)
            {
                _output.LogError($"GetSpacesAsync threw an exception: {ex.Message}. URL: {expectedUrl}", ex);
                if (CurrentTestMode == TestMode.Record) _output.LogWarning("Ensure you are in Record mode if you intend to capture this request.");
                Assert.Fail($"GetSpacesAsync threw an exception: {ex.Message}");
            }

            // Assert
            Assert.NotNull(result);
            // In playback, the number of spaces depends on the recorded JSON.
            // In record/passthrough, it depends on the actual workspace state.
            if (CurrentTestMode != TestMode.Playback)
            {
                 Assert.NotEmpty(result); // Assuming the test workspace has at least one space for live tests
            }
            else if (!result.Any()) // If playback JSON results in an empty enumerable
            {
                _output.LogInformation("[Playback] Retrieved 0 spaces as per mock data.");
            }

            _output.LogInformation($"Successfully retrieved {result.Count()} spaces.");
            foreach (var spaceEntity in result) // Iterate over Space entities
            {
                Assert.False(string.IsNullOrWhiteSpace(spaceEntity.Id));
                Assert.False(string.IsNullOrWhiteSpace(spaceEntity.Name));
                _output.LogInformation($"Space - ID: {spaceEntity.Id}, Name: {spaceEntity.Name}");
            }
        }

        [Fact]
        public async Task CreateSpaceAsync_WithValidData_ShouldCreateSpace()
        {
            // Arrange
            Assert.False(string.IsNullOrWhiteSpace(_testWorkspaceId), "TestWorkspaceId must be configured.");
            var spaceName = $"Test Space {Guid.NewGuid()}";
            var request = new CreateSpaceRequest(
                Name: spaceName,
                MultipleAssignees: true,
                Features: new Features( // Use the Features record from Entities.Spaces
                    DueDates: new DueDatesFeature(Enabled: true, StartDateEnabled: false, RemapDueDatesEnabled: false, DueDatesForSubtasksRollUpEnabled: null),
                    TimeTracking: new TimeTrackingFeature(Enabled: false, HarvestEnabled: null, RollUpEnabled: null),
                    Tags: new TagsFeature(Enabled: true),
                    TimeEstimates: new TimeEstimatesFeature(Enabled: true, RollUpEnabled: null, PerAssigneeEnabled: null),
                    Checklists: new ChecklistsFeature(Enabled: true),
                    CustomFields: new CustomFieldsFeature(Enabled: true),
                    RemapDependencies: new RemapDependenciesFeature(Enabled: true),
                    DependencyWarning: new DependencyWarningFeature(Enabled: true),
                    Portfolios: new PortfoliosFeature(Enabled: true),
                    Sprints: null, Points: null, CustomTaskIds: null, MultipleAssignees: null, Emails: null // Other features nullable
                )
            );
            // IMPORTANT: The body hash will depend on the exact 'spaceName' (due to Guid).
            // For robust playback, the 'request' object should be consistent or the body hash in the filename needs to be dynamic/matched.
            // For now, assume a specific hash, e.g., "Success_body32bcf349.json" (taken from one of the logs).
            // This means the CreateSpaceAsync test in playback will always "create" the space that corresponds to this recording.
            string playbackMockedSpaceId = "mocked-space-id-create"; // ID used in the JSON file for the created space.
            string playbackRecordedPostHash = "body32bcf349"; // Example hash

            _output.LogInformation($"Attempting to create space '{spaceName}' in workspace '{_testWorkspaceId}'.");

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "POSTCreateSpace", $"Success_{playbackRecordedPostHash}.json");
                // We need to ensure that Success_body32bcf349.json contains the 'playbackMockedSpaceId' and a name that matches 'spaceName' or a generic one.
                // For playback, it's better if the JSON has fixed values:
                // { "id": "mocked-space-id-create", "name": "Playback Created Space", ... }
                // And the test asserts against "Playback Created Space".
                // The request body sent during playback should also ideally match what was sent during recording to get this hash.
                // This is a complex part of request-body-dependent mocking.

                // For simplicity, let's assume the JSON at "Success_body32bcf349.json" returns a space with ID "mocked-space-id-create"
                // and the name asserted below will be based on this assumption.
                // The actual 'spaceName' with Guid will be different each run, so matching body hash is tricky unless request is static.

                Assert.True(File.Exists(responsePath), $"Mock data file not found: {responsePath}. Check hash and name.");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/space")
                               // .WithContent(...) // Ideally match the content that generated 'playbackRecordedPostHash'
                               .Respond("application/json", responseContent);
                _output.LogInformation($"[Playback] Mocking POST create space. Response from: {responsePath}");

                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/space/{playbackMockedSpaceId}")
                               .Respond(HttpStatusCode.NoContent);
                _output.LogInformation($"[Playback] Mocking DELETE for playback-created space {playbackMockedSpaceId}.");
            }

            // Act
            Space? result = null; // This should now correctly refer to Entities.Spaces.Space
            string? actualCreatedSpaceId = null;
            try
            {
                result = await _spaceService.CreateSpaceAsync(_testWorkspaceId, request); // CreateSpaceAsync returns Entities.Spaces.Space
                if (result != null)
                {
                    actualCreatedSpaceId = result.Id;
                    // In Playback mode, result.Id will be from the JSON (e.g., createdSpaceIdForMocking)
                    // In Record/Passthrough, it's a live ID.
                    RegisterCreatedSpace(actualCreatedSpaceId);
                    _output.LogInformation($"Space action completed. ID: {result.Id}, Name: {result.Name}");
                }
            }
            catch (Exception ex)
            {
                _output.LogError($"CreateSpaceAsync threw an exception: {ex.Message}", ex);
                if (CurrentTestMode == TestMode.Record) _output.LogWarning("Ensure you are in Record mode if you intend to capture this request.");
                Assert.Fail($"CreateSpaceAsync threw an exception: {ex.Message}");
            }

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.Id));
            Assert.True(result.MultipleAssignees);
            Assert.NotNull(result.Features);
            Assert.True(result.Features.DueDates.Enabled);

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(playbackMockedSpaceId, result.Id);
                // If using a static JSON for playback like 'Success_body32bcf349.json',
                // its 'name' field should be asserted here. For example, if it's "Playback Created Space":
                Assert.Equal("Test Space 4a6f3940-6beb-4ad2-8fe0-2fd26f438230", result.Name); // Assuming this name is in the specific JSON file
            }
            else
            {
                Assert.Equal(spaceName, result.Name);
            }
        }

        [Fact]
        public async Task GetSpaceAsync_WithExistingSpaceId_ShouldReturnSpace()
        {
            // Arrange
            string spaceIdForTest = "mocked-space-id-get"; // For Playback
            string spaceNameForTest = $"My Space To Get - {Guid.NewGuid()}"; // For Record
            string recordedCreateBodyHash_ForGetTest = "body1f17b88e"; // From logs, for the POST in this test's setup
            string expectedSpaceName = string.Empty; // Declare here

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // 1. Mock the POST for creating the space (setup for this test)
                var createResponsePath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "POSTCreateSpace", $"Success_{recordedCreateBodyHash_ForGetTest}.json");
                Assert.True(File.Exists(createResponsePath), $"Mock data file for Create (in GetTest) not found: {createResponsePath}");
                var createResponseContent = await File.ReadAllTextAsync(createResponsePath);
                // This JSON should return a space with ID 'spaceIdForTest' ("mocked-space-id-get")
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/space")
                               // .WithContent(...) // Match the body that created "body1f17b88e"
                               .Respond("application/json", createResponseContent);

                // 2. Mock the GET for the actual test action
                var getResponsePath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GETSpace", "Success.json");
                Assert.True(File.Exists(getResponsePath), $"Mock data file for GetSpace not found: {getResponsePath}");
                var getResponseContent = await File.ReadAllTextAsync(getResponsePath);
                // This JSON (GETSpace/Success.json) should contain the space with ID "mocked-space-id-get"
                // and its name should be used for expectedSpaceName.
                expectedSpaceName = "My Space To Get - 4e2b718a-f48a-48d2-8fd7-a25f088ad3ba"; // Name from the recorded GETSpace/Success.json

                MockHttpHandler.When($"https://api.clickup.com/api/v2/space/{spaceIdForTest}")
                               .Respond("application/json", getResponseContent);
                _output.LogInformation($"[Playback] Mocking GET space for ID {spaceIdForTest}. Response from: {getResponsePath}");

                // 3. Mock the DELETE for cleanup by DisposeAsync
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/space/{spaceIdForTest}")
                               .Respond(HttpStatusCode.NoContent);
            }
            else // Record or Passthrough
            {
                Assert.False(string.IsNullOrWhiteSpace(_testWorkspaceId));
                // In Record mode, spaceNameForTest (with Guid) is used for creation
                var createRequest = new CreateSpaceRequest(
                    spaceNameForTest,  // CORRECTED: Use spaceNameForTest for creation
                    true,
                    new Features(
                        DueDates: new DueDatesFeature(Enabled: true, StartDateEnabled: false, RemapDueDatesEnabled: false, DueDatesForSubtasksRollUpEnabled: null),
                        TimeTracking: new TimeTrackingFeature(Enabled: false, HarvestEnabled: null, RollUpEnabled: null),
                        Tags: new TagsFeature(Enabled: true),
                        TimeEstimates: new TimeEstimatesFeature(Enabled: true, RollUpEnabled: null, PerAssigneeEnabled: null),
                        Checklists: new ChecklistsFeature(Enabled: true),
                        CustomFields: new CustomFieldsFeature(Enabled: true),
                        RemapDependencies: new RemapDependenciesFeature(Enabled: true),
                        DependencyWarning: new DependencyWarningFeature(Enabled: true),
                        Portfolios: new PortfoliosFeature(Enabled: true),
                        Sprints: null, Points: null, CustomTaskIds: null, MultipleAssignees: null, Emails: null
                    )
                );
                var createdSpace = await _spaceService.CreateSpaceAsync(_testWorkspaceId, createRequest);
                Assert.NotNull(createdSpace);
                spaceIdForTest = createdSpace.Id;
                // expectedSpaceName should be spaceNameForTest in Record mode for assertion consistency
                expectedSpaceName = spaceNameForTest;
                RegisterCreatedSpace(spaceIdForTest);
                _output.LogInformation($"[Record/Passthrough] Created space '{spaceNameForTest}' with ID '{spaceIdForTest}' for Get test.");
            }

            // Act
            Space? result = null;
            try
            {
                result = await _spaceService.GetSpaceAsync(spaceIdForTest);
            }
            catch (Exception ex)
            {
                _output.LogError($"GetSpaceAsync threw an exception: {ex.Message}", ex);
                if (CurrentTestMode == TestMode.Record && CurrentTestMode != TestMode.Playback) _output.LogWarning("Ensure you are in Record mode if you intend to capture this request.");
                Assert.Fail($"GetSpaceAsync threw an exception: {ex.Message}");
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(spaceIdForTest, result.Id); // Use spaceIdForTest
            Assert.Equal(expectedSpaceName, result.Name); // expectedSpaceName is correctly set for both modes
            _output.LogInformation($"Successfully retrieved space ID: {result.Id}, Name: {result.Name}");
        }

        [Fact]
        public async Task UpdateSpaceAsync_WithValidData_ShouldUpdateSpace()
        {
            // Arrange
            string spaceIdForTest = "mocked-space-id-update"; // For Playback
            string initialNameForTest = $"Initial Space Name - {Guid.NewGuid()}"; // For Record setup
            string updatedNameForTest = $"Updated Space Name - {Guid.NewGuid()}"; // Used for actual update name

            // These hashes are identified from previous Record run logs for specific request bodies.
            // The request body for POST must match what generated recordedCreateBodyHash_ForUpdateTest.
            // The request body for PUT must match what generated recordedUpdateBodyHash_ForUpdateTest.
            string recordedCreateBodyHash_ForUpdateTest = "body11947460";
            string recordedUpdateBodyHash_ForUpdateTest = "body0e371cc7";

            string nameInPutResponseJson = "Updated Space Name - 8fd80712-012c-4956-8e7f-8c9c24236db0"; // Actual name in the recorded PUT response

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // 1. Mock POST for creating the initial space
                var createResponsePath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "POSTCreateSpace", $"Success_{recordedCreateBodyHash_ForUpdateTest}.json");
                Assert.True(File.Exists(createResponsePath), $"Mock data file for Create (in UpdateTest) not found: {createResponsePath}");
                var createResponseContent = await File.ReadAllTextAsync(createResponsePath);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/space")
                               .Respond("application/json", createResponseContent);

                // 2. Mock PUT for the update operation
                var updateResponsePath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "PUTSpace", $"Success_{recordedUpdateBodyHash_ForUpdateTest}.json");
                Assert.True(File.Exists(updateResponsePath), $"Mock data file for UpdateSpace not found: {updateResponsePath}");
                var updateResponseContent = await File.ReadAllTextAsync(updateResponsePath);

                MockHttpHandler.When(HttpMethod.Put, $"https://api.clickup.com/api/v2/space/{spaceIdForTest}")
                               .Respond("application/json", updateResponseContent);
                _output.LogInformation($"[Playback] Mocking PUT update space for ID {spaceIdForTest}. Response from: {updateResponsePath}");

                // 3. Mock DELETE for cleanup by DisposeAsync
                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/space/{spaceIdForTest}")
                               .Respond(HttpStatusCode.NoContent);

                // For playback, the 'updatedNameForTest' variable will be used to construct the UpdateSpaceRequest.
                // The assertion however, must be against the name that is *in the response JSON* ('nameInPutResponseJson').
                updatedNameForTest = nameInPutResponseJson; // Align for assertion consistency in playback
            }
            else // Record or Passthrough
            {
                Assert.False(string.IsNullOrWhiteSpace(_testWorkspaceId));
                var createRequest = new CreateSpaceRequest(
                    initialNameForTest, // Correct: Use initialNameForTest for creation
                    true,
                    new Features(
                        DueDates: new DueDatesFeature(Enabled: true, StartDateEnabled: false, RemapDueDatesEnabled: false, DueDatesForSubtasksRollUpEnabled: null),
                        TimeTracking: new TimeTrackingFeature(Enabled: false, HarvestEnabled: null, RollUpEnabled: null),
                        Tags: new TagsFeature(Enabled: true),
                        TimeEstimates: new TimeEstimatesFeature(Enabled: true, RollUpEnabled: null, PerAssigneeEnabled: null),
                        Checklists: new ChecklistsFeature(Enabled: true),
                        CustomFields: new CustomFieldsFeature(Enabled: true),
                        RemapDependencies: new RemapDependenciesFeature(Enabled: true),
                        DependencyWarning: new DependencyWarningFeature(Enabled: true),
                        Portfolios: new PortfoliosFeature(Enabled: true),
                        Sprints: null, Points: null, CustomTaskIds: null, MultipleAssignees: null, Emails: null
                    )
                );
                var createdSpace = await _spaceService.CreateSpaceAsync(_testWorkspaceId, createRequest);
                Assert.NotNull(createdSpace);
                spaceIdForTest = createdSpace.Id;
                RegisterCreatedSpace(spaceIdForTest);
                _output.LogInformation($"[Record/Passthrough] Created space '{initialNameForTest}' with ID '{spaceIdForTest}' for Update test.");
            }

            var updateRequest = new UpdateSpaceRequest(
                Name: updatedNameForTest,  // In Record, this Guid-based name is sent. In Playback, this is nameInPutResponseJson.
                Color: "#AB00FF",
                Private: null,
                AdminCanManage: null,
                MultipleAssignees: null,
                Features: null, // Not changing features in this test
                Archived: null
            );

            _output.LogInformation($"Attempting to update space '{spaceIdForTest}' to name '{updateRequest.Name}'.");

            // Act
            Space? result = null;
            try
            {
                result = await _spaceService.UpdateSpaceAsync(spaceIdForTest, updateRequest); // Recorded
            }
            catch (Exception ex)
            {
                _output.LogError($"UpdateSpaceAsync threw an exception: {ex.Message}", ex);
                if (CurrentTestMode == TestMode.Record && CurrentTestMode != TestMode.Playback) _output.LogWarning("Ensure you are in Record mode if you intend to capture this request.");
                Assert.Fail($"UpdateSpaceAsync threw an exception: {ex.Message}");
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(spaceIdForTest, result.Id); // Use spaceIdForTest
            Assert.Equal(updateRequest.Name, result.Name); // This asserts against updatedNameForTest (which is nameInPutResponseJson in playback)
            Assert.Equal("#ab00ff", result.Color?.ToLowerInvariant());
            _output.LogInformation($"Successfully updated space ID: {result.Id}, Name: {result.Name}, Color: {result.Color}");
        }

        [Fact]
        public async Task DeleteSpaceAsync_WithExistingSpaceId_ShouldDeleteSpace()
        {
            // Arrange
            string spaceIdForTest = "mocked-space-id-delete";
            string spaceNameForTest = $"Space To Delete - {Guid.NewGuid()}";
            string recordedCreateBodyHash_ForDeleteTest = "body106875c2";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var createResponsePath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "POSTCreateSpace", $"Success_{recordedCreateBodyHash_ForDeleteTest}.json");
                Assert.True(File.Exists(createResponsePath), $"Mock data for Create (DeleteTest) not found: {createResponsePath}");
                var createResponseContent = await File.ReadAllTextAsync(createResponsePath);
                MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/space")
                               .Respond("application/json", createResponseContent);

                MockHttpHandler.When(HttpMethod.Delete, $"https://api.clickup.com/api/v2/space/{spaceIdForTest}")
                               .Respond(HttpStatusCode.NoContent);
                _output.LogInformation($"[Playback] Mocking DELETE space for ID {spaceIdForTest}.");

                var getNotFoundResponsePath = Path.Combine(RecordedResponsesBasePath, "SpaceService", "GETSpace", "Error_404.json");
                Assert.True(File.Exists(getNotFoundResponsePath), $"Mock data for GetSpace (NotFound) not found: {getNotFoundResponsePath}");
                var getNotFoundResponseContent = await File.ReadAllTextAsync(getNotFoundResponsePath);
                MockHttpHandler.When(HttpMethod.Get, $"https://api.clickup.com/api/v2/space/{spaceIdForTest}")
                               .Respond(HttpStatusCode.NotFound, "application/json", getNotFoundResponseContent);
                _output.LogInformation($"[Playback] Mocking GET for deleted space {spaceIdForTest} to return NotFound.");
            }
            else
            {
                Assert.False(string.IsNullOrWhiteSpace(_testWorkspaceId));
                var createRequest = new CreateSpaceRequest(
                    spaceNameForTest, // CORRECTED: Use spaceNameForTest for creation, was mistakenly initialNameForTest
                    true,
                    new Features(
                        DueDates: new DueDatesFeature(Enabled: true, StartDateEnabled: false, RemapDueDatesEnabled: false, DueDatesForSubtasksRollUpEnabled: null),
                        TimeTracking: new TimeTrackingFeature(Enabled: false, HarvestEnabled: null, RollUpEnabled: null),
                        Tags: new TagsFeature(Enabled: true),
                        TimeEstimates: new TimeEstimatesFeature(Enabled: true, RollUpEnabled: null, PerAssigneeEnabled: null),
                        Checklists: new ChecklistsFeature(Enabled: true),
                        CustomFields: new CustomFieldsFeature(Enabled: true),
                        RemapDependencies: new RemapDependenciesFeature(Enabled: true),
                        DependencyWarning: new DependencyWarningFeature(Enabled: true),
                        Portfolios: new PortfoliosFeature(Enabled: true),
                        Sprints: null, Points: null, CustomTaskIds: null, MultipleAssignees: null, Emails: null
                    )
                );
                var createdSpace = await _spaceService.CreateSpaceAsync(_testWorkspaceId, createRequest);
                Assert.NotNull(createdSpace);
                spaceIdForTest = createdSpace.Id;
                _output.LogInformation($"[Record/Passthrough] Created space '{spaceNameForTest}' with ID '{spaceIdForTest}' for Delete test.");
            }

            // Act
            try
            {
                await _spaceService.DeleteSpaceAsync(spaceIdForTest);
            }
            catch (Exception ex)
            {
                _output.LogError($"DeleteSpaceAsync threw an exception: {ex.Message}", ex);
                if (CurrentTestMode == TestMode.Record && CurrentTestMode != TestMode.Playback) _output.LogWarning("Ensure you are in Record mode if you intend to capture this request.");
                Assert.Fail($"DeleteSpaceAsync threw an exception: {ex.Message}");
            }

            // Assert
            _output.LogInformation($"DeleteSpaceAsync called for space ID: {spaceIdForTest}. Verifying...");
            await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _spaceService.GetSpaceAsync(spaceIdForTest) // Recorded (should be 404)
            );
            _output.LogInformation($"Verified space {spaceIdForTest} is deleted (GetSpaceAsync threw NotFound).");
        }
    }
}
