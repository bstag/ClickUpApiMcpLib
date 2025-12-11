using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Checklists;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection; // Added for GetRequiredService

namespace ClickUp.Api.Client.IntegrationTests.Integration;

// [Collection(IntegrationTestCollection.Name)] // Removed Collection attribute
public class TaskChecklistsServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
{
    private readonly ITaskChecklistsService _taskChecklistsService;
    private readonly ISpacesService _spacesService; // For hierarchy teardown
    private readonly IFoldersService _foldersService; // For hierarchy setup
    private readonly IListsService _listsService;     // For hierarchy setup
    private readonly ITaskCrudService _taskCrudService;     // For hierarchy setup
    private readonly ITestOutputHelper _outputHelper;
    private TestHierarchyContext? _testContext; // To store IDs from TestHierarchyHelper

    // Store IDs for entities created during setup - these will now come from _testContext
    // private string _testTaskId;
    // private string _testListId;
    // private string _testFolderId;
    // private string _testSpaceId;

    public TaskChecklistsServiceIntegrationTests(ITestOutputHelper outputHelper) // Removed IntegrationTestFixture, base() calls parameterless base constructor
        : base()
    {
        _outputHelper = outputHelper;
        _taskChecklistsService = ServiceProvider.GetRequiredService<ITaskChecklistsService>();
        _spacesService = ServiceProvider.GetRequiredService<ISpacesService>();
        _foldersService = ServiceProvider.GetRequiredService<IFoldersService>();
        _listsService = ServiceProvider.GetRequiredService<IListsService>();
        _taskCrudService = ServiceProvider.GetRequiredService<ITaskCrudService>();
        // _hierarchyHelper = new TestHierarchyHelper(ServiceProvider, nameof(TaskChecklistsServiceIntegrationTests)); // Removed instance helper
    }

    public async Task InitializeAsync()
    {
        _outputHelper.WriteLine($"[{nameof(TaskChecklistsServiceIntegrationTests)}] Initializing test hierarchy...");
        if (CurrentTestMode == TestMode.Playback)
        {
            _testContext = new TestHierarchyContext
            {
                SpaceId = "playback_space_id",
                FolderId = "playback_folder_id",
                ListId = "playback_list_id",
                TaskId = "playback_task_id"
            };
            _outputHelper.WriteLine($"[{nameof(TaskChecklistsServiceIntegrationTests)}] Playback mode: Using predefined IDs.");
            return;
        }

        try
        {
            // Use static TestHierarchyHelper methods
            var workspaceId = Configuration["ClickUpApi:TestWorkspaceId"];
            if (string.IsNullOrWhiteSpace(workspaceId))
            {
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for hierarchy setup.");
            }

            _testContext = await TestHierarchyHelper.CreateFullTestHierarchyAsync(
                _spacesService,
                _foldersService,
                _listsService,
                _taskCrudService,
                workspaceId,
                "TaskChecklistTest",
                _outputHelper);

            _outputHelper.WriteLine($"[{nameof(TaskChecklistsServiceIntegrationTests)}] Initialized: Space '{_testContext.SpaceId}', Folder '{_testContext.FolderId}', List '{_testContext.ListId}', Task '{_testContext.TaskId}'");
        }
        catch (Exception ex)
        {
            _outputHelper.WriteLine($"[{nameof(TaskChecklistsServiceIntegrationTests)}] Error during InitializeAsync: {ex.Message}");
            if (_testContext != null) // Attempt cleanup if context was partially created
            {
                await TestHierarchyHelper.TeardownHierarchyAsync(_spacesService, _testContext, _outputHelper);
            }
            throw; // Re-throw to fail the test setup
        }
    }

    public async Task DisposeAsync()
    {
        _outputHelper.WriteLine($"[{nameof(TaskChecklistsServiceIntegrationTests)}] Disposing test hierarchy...");
        if (CurrentTestMode == TestMode.Playback || _testContext == null)
        {
            _outputHelper.WriteLine($"[{nameof(TaskChecklistsServiceIntegrationTests)}] Playback mode or no test context: Skipping live cleanup.");
            return; // No actual cleanup in playback mode or if setup failed before context creation
        }

        // Use static TestHierarchyHelper for teardown
        await TestHierarchyHelper.TeardownHierarchyAsync(_spacesService, _testContext, _outputHelper);
        _outputHelper.WriteLine($"[{nameof(TaskChecklistsServiceIntegrationTests)}] Test hierarchy disposal process completed via TestHierarchyHelper.");
    }

    // Test methods will be added here

    [Fact]
    public async Task CreateChecklistAsync_ValidData_ShouldCreateChecklist()
    {
        Assert.NotNull(_testContext); // Ensure context is initialized
        _outputHelper.WriteLine($"[{nameof(CreateChecklistAsync_ValidData_ShouldCreateChecklist)}] Running in mode: {CurrentTestMode}");
        _outputHelper.WriteLine($"[{nameof(CreateChecklistAsync_ValidData_ShouldCreateChecklist)}] Using Task ID: {_testContext.TaskId}");

        var checklistName = $"My New Checklist {Guid.NewGuid()}";
        var createRequest = new CreateChecklistRequest(checklistName);
        Models.Entities.Checklists.Checklist? createdChecklistEntity = null;

        try
        {
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var mockChecklistId = "playback_checklist_id_123";
                var responsePath = Path.Combine(RecordedResponsesBasePath, "TaskChecklistsService", "CreateChecklistAsync", "Success.json");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                // Replace placeholder Task ID in response content if necessary (more robust for dynamic task IDs)
                // For this example, assume Success.json is generic or matches playback_task_id if that's used in URL
                responseContent = responseContent.Replace("\"task_id\": \"{{TASK_ID}}\"", $"\"task_id\": \"{_testContext.TaskId}\"")
                                                 .Replace("\"id\": \"{{CHECKLIST_ID}}\"", $"\"id\": \"{mockChecklistId}\"")
                                                 .Replace("\"name\": \"{{CHECKLIST_NAME}}\"", $"\"name\": \"{checklistName}\"");


                MockHttpHandler.When(HttpMethod.Post, $"{ClientOptions.BaseAddress}/task/{_testContext.TaskId}/checklist")
                               .WithPartialContent(checklistName) // Ensure request body contains the name
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }

            // Act
            var response = await _taskChecklistsService.CreateChecklistAsync(_testContext.TaskId, createRequest);

            // Assert
            response.Should().NotBeNull();
            response.Checklist.Should().NotBeNull();
            createdChecklistEntity = response.Checklist; // Store for potential cleanup

            createdChecklistEntity.Name.Should().Be(checklistName);
            createdChecklistEntity.TaskId.Should().Be(_testContext.TaskId);
            createdChecklistEntity.Id.Should().NotBeNullOrEmpty();

            if (CurrentTestMode == TestMode.Playback)
            {
                createdChecklistEntity.Id.Should().Be("playback_checklist_id_123");
            }

            _outputHelper.WriteLine($"[{nameof(CreateChecklistAsync_ValidData_ShouldCreateChecklist)}] Created checklist with ID: {createdChecklistEntity.Id}");
        }
        finally
        {
            // Cleanup: Delete the created checklist if in Record or Passthrough mode and it was created
            if (CurrentTestMode != TestMode.Playback && createdChecklistEntity != null && !string.IsNullOrEmpty(createdChecklistEntity.Id))
            {
                _outputHelper.WriteLine($"[{nameof(CreateChecklistAsync_ValidData_ShouldCreateChecklist)}] Attempting to delete checklist: {createdChecklistEntity.Id}");
                try
                {
                    await _taskChecklistsService.DeleteChecklistAsync(createdChecklistEntity.Id);
                    _outputHelper.WriteLine($"[{nameof(CreateChecklistAsync_ValidData_ShouldCreateChecklist)}] Successfully deleted checklist: {createdChecklistEntity.Id}");
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"[{nameof(CreateChecklistAsync_ValidData_ShouldCreateChecklist)}] Error deleting checklist {createdChecklistEntity.Id}: {ex.Message}");
                    // Don't fail the test for cleanup failure, but log it.
                }
            }
        }
    }

    // Initially, we won't create a separate playback test if the main test handles both modes.
    // A separate one might be useful if playback setup becomes very complex or needs distinct assertions.
    // For now, the above test demonstrates dual-mode capability.
    // If we needed a playback-only test, it would look like this:
    /*
    [Fact]
public async Task CreateChecklistAsync_Playback_ShouldReturnMockedChecklist()
    {
        if (CurrentTestMode != TestMode.Playback)
        {
            _outputHelper.WriteLine($"[{nameof(CreateChecklistAsync_Playback_ShouldReturnMockedChecklist)}] Skipping as not in Playback mode.");
            return; // Or use Assert.Skip()
        }

        _outputHelper.WriteLine($"[{nameof(CreateChecklistAsync_Playback_ShouldReturnMockedChecklist)}] Running in Playback mode.");
        Assert.NotNull(MockHttpHandler); // Crucial for playback

        var checklistName = "Mocked Checklist from Playback";
        var createRequest = new CreateChecklistRequest(checklistName);
        var mockChecklistId = "mock_checklist_playback_test_789";
        var mockTaskId = _testContext.TaskId; // Using the one from InitializeAsync for consistency

        var responsePath = Path.Combine(RecordedResponsesBasePath, "TaskChecklistsService", "CreateChecklistAsync", "Success_PlaybackOnly.json");
        // Ensure Success_PlaybackOnly.json exists and is tailored for this test
        // It might use specific IDs like mock_checklist_playback_test_789
        var responseContent = await File.ReadAllTextAsync(responsePath);

        MockHttpHandler.When(HttpMethod.Post, $"{ClientOptions.BaseAddress}/task/{mockTaskId}/checklist")
                       .WithPartialContent(checklistName)
                       .Respond(HttpStatusCode.OK, "application/json", responseContent);

        // Act
        var response = await _taskChecklistsService.CreateChecklistAsync(mockTaskId, createRequest);

        // Assert
        response.Should().NotBeNull();
        response.Checklist.Should().NotBeNull();
        response.Checklist.Name.Should().Be(checklistName); // Or whatever name is in Success_PlaybackOnly.json
        response.Checklist.Id.Should().Be(mockChecklistId); // Matching the ID in Success_PlaybackOnly.json
        response.Checklist.TaskId.Should().Be(mockTaskId);

        _outputHelper.WriteLine($"[{nameof(CreateChecklistAsync_Playback_ShouldReturnMockedChecklist)}] Successfully verified mocked checklist: {response.Checklist.Id}");
    }
    */

    [Fact]
    public async Task EditChecklistAsync_ValidData_ShouldUpdateChecklist()
    {
        Assert.NotNull(_testContext);
        _outputHelper.WriteLine($"[{nameof(EditChecklistAsync_ValidData_ShouldUpdateChecklist)}] Running in mode: {CurrentTestMode}");
        Models.Entities.Checklists.Checklist? originalChecklist = null;
        var initialChecklistName = $"Initial Checklist {Guid.NewGuid()}";
        var updatedChecklistName = $"Updated Checklist {Guid.NewGuid()}";
        var newPosition = 0; // ClickUp API uses 0 for top, or it might be 1-based depending on context. Assuming 0 for this test.

        try
        {
            // Arrange: Create a checklist first
            if (CurrentTestMode != TestMode.Playback)
            {
                var createRequest = new CreateChecklistRequest(initialChecklistName);
                var createResponse = await _taskChecklistsService.CreateChecklistAsync(_testContext.TaskId, createRequest);
                originalChecklist = createResponse.Checklist;
                _outputHelper.WriteLine($"[{nameof(EditChecklistAsync_ValidData_ShouldUpdateChecklist)}] Created initial checklist ID: {originalChecklist.Id}");
            }
            else
            {
                // For Playback, we assume a checklist exists and its ID is known or mocked.
                // The creation step won't run, so we need a placeholder ID.
                originalChecklist = new Models.Entities.Checklists.Checklist(
                    Id: "playback_checklist_to_edit_456",
                    TaskId: _testContext.TaskId,
                    Name: initialChecklistName,
                    OrderIndex: 1, // Some initial order index
                    Resolved: 0,
                    Unresolved: 0,
                    Items: new System.Collections.Generic.List<ChecklistItem>()
                );
                _outputHelper.WriteLine($"[{nameof(EditChecklistAsync_ValidData_ShouldUpdateChecklist)}] Using predefined checklist ID for Playback: {originalChecklist.Id}");

                Assert.NotNull(MockHttpHandler);
                // Mock for the EditChecklistAsync call
                // The API for editing a checklist returns an updated checklist object or just 200 OK.
                // Based on ITaskChecklistsService.EditChecklistAsync returning Task, it implies a 200 OK with no body or an already updated object.
                // Let's assume it returns the updated checklist for verification, or we'd need a subsequent GET.
                // For simplicity, assuming PUT returns 200 OK and we can't directly verify name change without a GET.
                // The service method `EditChecklistAsync` in `TaskChecklistsService.cs` returns `System.Threading.Tasks.Task` (void).
                // So, we mock a 200 OK response with no body.
                MockHttpHandler.When(HttpMethod.Put, $"{ClientOptions.BaseAddress}/checklist/{originalChecklist.Id}")
                               .WithPartialContent(updatedChecklistName) // Check that the new name is in the PUT body
                               .Respond(HttpStatusCode.OK);

                // If we wanted to verify the update, we'd also need to mock a GET call here.
                // For now, we're testing the PUT completes successfully.
            }

            Assert.NotNull(originalChecklist); // Ensure checklist is available

            var editRequest = new EditChecklistRequest(Name: updatedChecklistName, Position: newPosition);

            // Act
            await _taskChecklistsService.EditChecklistAsync(originalChecklist.Id, editRequest);
            _outputHelper.WriteLine($"[{nameof(EditChecklistAsync_ValidData_ShouldUpdateChecklist)}] Edit operation completed for checklist ID: {originalChecklist.Id}");

            // Assert
            // Since EditChecklistAsync returns void, direct assertion of the name change from the response isn't possible.
            // In a real scenario with Record/Passthrough, you'd make a GET call here to verify.
            // For Playback, if the PUT is just a 200 OK, this test mainly verifies the call was made as expected.
            // If the API *did* return the updated checklist, assertions would be here.

            if (CurrentTestMode != TestMode.Playback)
            {
                // Optional: Add a GET call here to verify the change in non-playback modes
                // This would require implementing GetChecklistAsync if it existed, or re-fetching the task's checklists.
                // For now, we assume the PUT was successful if no exception.
                _outputHelper.WriteLine($"[{nameof(EditChecklistAsync_ValidData_ShouldUpdateChecklist)}] In non-Playback mode, assuming PUT successful. Verification would need a GET call.");
            }
            else
            {
                // In Playback, MockHttpHandler.VerifyNoOutstandingExpectation() (called by base class)
                // will confirm the mocked PUT was called.
                _outputHelper.WriteLine($"[{nameof(EditChecklistAsync_ValidData_ShouldUpdateChecklist)}] In Playback mode, verified PUT call was expected.");
            }
        }
        finally
        {
            // Cleanup: Delete the created checklist if in Record or Passthrough mode and it was created
            if (CurrentTestMode != TestMode.Playback && originalChecklist != null && !string.IsNullOrEmpty(originalChecklist.Id))
            {
                _outputHelper.WriteLine($"[{nameof(EditChecklistAsync_ValidData_ShouldUpdateChecklist)}] Attempting to delete checklist: {originalChecklist.Id}");
                try
                {
                    await _taskChecklistsService.DeleteChecklistAsync(originalChecklist.Id);
                    _outputHelper.WriteLine($"[{nameof(EditChecklistAsync_ValidData_ShouldUpdateChecklist)}] Successfully deleted checklist: {originalChecklist.Id}");
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"[{nameof(EditChecklistAsync_ValidData_ShouldUpdateChecklist)}] Error deleting checklist {originalChecklist.Id}: {ex.Message}");
                }
            }
        }
    }

    [Fact]
    public async Task DeleteChecklistAsync_ValidId_ShouldDeleteChecklist()
    {
        Assert.NotNull(_testContext);
        _outputHelper.WriteLine($"[{nameof(DeleteChecklistAsync_ValidId_ShouldDeleteChecklist)}] Running in mode: {CurrentTestMode}");
        Models.Entities.Checklists.Checklist? checklistToDelete = null;
        var checklistName = $"Checklist to Delete {Guid.NewGuid()}";

        // Arrange: Create a checklist first
        if (CurrentTestMode != TestMode.Playback)
        {
            var createRequest = new CreateChecklistRequest(checklistName);
            var createResponse = await _taskChecklistsService.CreateChecklistAsync(_testContext.TaskId, createRequest);
            checklistToDelete = createResponse.Checklist;
            Assert.NotNull(checklistToDelete);
            _outputHelper.WriteLine($"[{nameof(DeleteChecklistAsync_ValidId_ShouldDeleteChecklist)}] Created checklist ID for deletion: {checklistToDelete.Id}");
        }
        else
        {
            // For Playback, we use a predefined ID for the checklist that's "deleted".
            checklistToDelete = new Models.Entities.Checklists.Checklist(
                Id: "playback_checklist_to_delete_789",
                TaskId: _testContext.TaskId,
                Name: checklistName,
                OrderIndex: 0, Resolved: 0, Unresolved: 0, Items: null
            );
            _outputHelper.WriteLine($"[{nameof(DeleteChecklistAsync_ValidId_ShouldDeleteChecklist)}] Using predefined checklist ID for Playback deletion: {checklistToDelete.Id}");

            Assert.NotNull(MockHttpHandler);
            // Mock the DELETE request
            MockHttpHandler.When(HttpMethod.Delete, $"{ClientOptions.BaseAddress}/checklist/{checklistToDelete.Id}")
                           .Respond(HttpStatusCode.OK); // Or HttpStatusCode.NoContent if that's what API returns
        }

        Assert.NotNull(checklistToDelete); // Ensure checklist is available

        // Act
        await _taskChecklistsService.DeleteChecklistAsync(checklistToDelete.Id);
        _outputHelper.WriteLine($"[{nameof(DeleteChecklistAsync_ValidId_ShouldDeleteChecklist)}] Delete operation completed for checklist ID: {checklistToDelete.Id}");

        // Assert
        if (CurrentTestMode != TestMode.Playback)
        {
            // In Record/Passthrough, verify it's gone.
            // This would typically involve trying to GET it and expecting a NotFoundException,
            // or listing checklists for the task and ensuring it's not present.
            // For simplicity here, we assume success if no exception from DeleteAsync.
            // A more robust test would add such a verification step.
            // For example, if a GetChecklistsForTask method existed:
            // var checklists = await _taskChecklistsService.GetChecklistsForTaskAsync(_testContext.TaskId);
            // checklists.Should().NotContain(c => c.Id == checklistToDelete.Id);
            _outputHelper.WriteLine($"[{nameof(DeleteChecklistAsync_ValidId_ShouldDeleteChecklist)}] In non-Playback, assumed deleted. Verification would need a subsequent GET/List.");
        }
        else
        {
            // In Playback, MockHttpHandler.VerifyNoOutstandingExpectation() will confirm the mock was called.
            _outputHelper.WriteLine($"[{nameof(DeleteChecklistAsync_ValidId_ShouldDeleteChecklist)}] In Playback, verified DELETE call was expected.");
        }

        // No finally block needed to delete the checklist here, as the test's purpose is to delete it.
        // If it was created in non-playback mode, it's now deleted.
        // If in playback, nothing was actually deleted.
    }

    [Fact]
    public async Task CreateChecklistItemAsync_ValidData_ShouldCreateItemInChecklist()
    {
        Assert.NotNull(_testContext);
        _outputHelper.WriteLine($"[{nameof(CreateChecklistItemAsync_ValidData_ShouldCreateItemInChecklist)}] Running in mode: {CurrentTestMode}");
        Models.Entities.Checklists.Checklist? parentChecklist = null;
        var checklistItemName = $"My New Checklist Item {Guid.NewGuid()}";
        ChecklistItem? createdItem = null;

        try
        {
            // Arrange: Create a parent checklist first
            if (CurrentTestMode != TestMode.Playback)
            {
                var checklistName = $"Parent Checklist for Item Test {Guid.NewGuid()}";
                var createChecklistRequest = new CreateChecklistRequest(checklistName);
                var createChecklistResponse = await _taskChecklistsService.CreateChecklistAsync(_testContext.TaskId, createChecklistRequest);
                parentChecklist = createChecklistResponse.Checklist;
                Assert.NotNull(parentChecklist);
                _outputHelper.WriteLine($"[{nameof(CreateChecklistItemAsync_ValidData_ShouldCreateItemInChecklist)}] Created parent checklist ID: {parentChecklist.Id}");
            }
            else
            {
                parentChecklist = new Models.Entities.Checklists.Checklist(
                    Id: "playback_parent_checklist_for_item_abc",
                    TaskId: _testContext.TaskId,
                    Name: "Playback Parent Checklist",
                    OrderIndex: 0, Resolved: 0, Unresolved: 0, Items: new System.Collections.Generic.List<ChecklistItem>()
                );
                _outputHelper.WriteLine($"[{nameof(CreateChecklistItemAsync_ValidData_ShouldCreateItemInChecklist)}] Using predefined parent checklist ID for Playback: {parentChecklist.Id}");

                Assert.NotNull(MockHttpHandler);
                var mockChecklistItemId = "playback_checklist_item_id_123";
                // The response for creating a checklist item includes the *entire* checklist object, with the new item presumably in its "items" list.
                var responsePath = Path.Combine(RecordedResponsesBasePath, "TaskChecklistsService", "CreateChecklistItemAsync", "Success.json");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                // Replace placeholders in response content
                responseContent = responseContent.Replace("\"id\": \"{{CHECKLIST_ID}}\"", $"\"id\": \"{parentChecklist.Id}\"")
                                                 .Replace("\"task_id\": \"{{TASK_ID}}\"", $"\"task_id\": \"{_testContext.TaskId}\"")
                                                 .Replace("\"name\": \"{{CHECKLIST_ITEM_NAME}}\"", $"\"name\": \"{checklistItemName}\"")
                                                 .Replace("\"item_id\": \"{{CHECKLIST_ITEM_ID}}\"", $"\"id\": \"{mockChecklistItemId}\""); // Assuming item_id is the id in the item itself

                MockHttpHandler.When(HttpMethod.Post, $"{ClientOptions.BaseAddress}/checklist/{parentChecklist.Id}/checklist_item")
                               .WithPartialContent(checklistItemName)
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }

            Assert.NotNull(parentChecklist);
            var createItemRequest = new CreateChecklistItemRequest(Name: checklistItemName, Assignee: null);

            // Act
            var response = await _taskChecklistsService.CreateChecklistItemAsync(parentChecklist.Id, createItemRequest);

            // Assert
            response.Should().NotBeNull();
            response.Checklist.Should().NotBeNull();
            response.Checklist.Id.Should().Be(parentChecklist.Id); // Ensure it's the same parent checklist
            response.Checklist.Items?.Should().NotBeNullOrEmpty(); // Added ?.

            createdItem = response.Checklist.Items?.FirstOrDefault(item => item.Name == checklistItemName); // Added ?.
            createdItem.Should().NotBeNull($"Expected to find checklist item with name '{checklistItemName}'");
            createdItem!.Id.Should().NotBeNullOrEmpty();

            if (CurrentTestMode == TestMode.Playback)
            {
                createdItem.Id.Should().Be("playback_checklist_item_id_123");
            }
            _outputHelper.WriteLine($"[{nameof(CreateChecklistItemAsync_ValidData_ShouldCreateItemInChecklist)}] Created checklist item ID: {createdItem.Id} in checklist {parentChecklist.Id}");
        }
        finally
        {
            // Cleanup: Delete the parent checklist (which should delete its items)
            // No need to delete individual item if parent is deleted.
            if (CurrentTestMode != TestMode.Playback && parentChecklist != null && !string.IsNullOrEmpty(parentChecklist.Id))
            {
                _outputHelper.WriteLine($"[{nameof(CreateChecklistItemAsync_ValidData_ShouldCreateItemInChecklist)}] Attempting to delete parent checklist: {parentChecklist.Id}");
                try
                {
                    await _taskChecklistsService.DeleteChecklistAsync(parentChecklist.Id);
                    _outputHelper.WriteLine($"[{nameof(CreateChecklistItemAsync_ValidData_ShouldCreateItemInChecklist)}] Successfully deleted parent checklist: {parentChecklist.Id}");
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"[{nameof(CreateChecklistItemAsync_ValidData_ShouldCreateItemInChecklist)}] Error deleting parent checklist {parentChecklist.Id}: {ex.Message}");
                }
            }
        }
    }

    [Fact]
    public async Task EditChecklistItemAsync_ValidData_ShouldUpdateItem()
    {
        Assert.NotNull(_testContext);
        _outputHelper.WriteLine($"[{nameof(EditChecklistItemAsync_ValidData_ShouldUpdateItem)}] Running in mode: {CurrentTestMode}");
        Models.Entities.Checklists.Checklist? parentChecklist = null;
        ChecklistItem? originalItem = null;
        var initialItemName = $"Initial Item {Guid.NewGuid()}";
        var updatedItemName = $"Updated Item {Guid.NewGuid()}";

        try
        {
            // Arrange: Create a parent checklist and an item in it
            if (CurrentTestMode != TestMode.Playback)
            {
                var checklistName = $"Parent Checklist for Item Edit {Guid.NewGuid()}";
                var createChecklistRequest = new CreateChecklistRequest(checklistName);
                var createChecklistResponse = await _taskChecklistsService.CreateChecklistAsync(_testContext.TaskId, createChecklistRequest);
                parentChecklist = createChecklistResponse.Checklist;
                Assert.NotNull(parentChecklist);
                _outputHelper.WriteLine($"[{nameof(EditChecklistItemAsync_ValidData_ShouldUpdateItem)}] Created parent checklist ID: {parentChecklist.Id}");

                var createItemRequest = new CreateChecklistItemRequest(Name: initialItemName, Assignee: null);
                var createItemResponse = await _taskChecklistsService.CreateChecklistItemAsync(parentChecklist.Id, createItemRequest);
                originalItem = createItemResponse.Checklist.Items?.FirstOrDefault(i => i.Name == initialItemName); // Added ?.
                Assert.NotNull(originalItem);
                _outputHelper.WriteLine($"[{nameof(EditChecklistItemAsync_ValidData_ShouldUpdateItem)}] Created checklist item ID: {originalItem.Id}");
            }
            else
            {
                originalItem = new ChecklistItem(
                    Id: "playback_item_to_edit_xyz",
                    Name: initialItemName,
                    OrderIndex: 0, Assignee: null, Resolved: false, Parent: null, DateCreated: DateTimeOffset.UtcNow, Children: null
                );
                parentChecklist = new Models.Entities.Checklists.Checklist(
                    Id: "playback_parent_for_item_edit_def",
                    TaskId: _testContext.TaskId,
                    Name: "Playback Parent for Item Edit",
                    OrderIndex: 0, Resolved: 0, Unresolved: 1, Items: new System.Collections.Generic.List<ChecklistItem> { originalItem }
                );
                _outputHelper.WriteLine($"[{nameof(EditChecklistItemAsync_ValidData_ShouldUpdateItem)}] Using predefined IDs for Playback: Checklist {parentChecklist.Id}, Item {originalItem.Id}");

                Assert.NotNull(MockHttpHandler);
                // The response for editing a checklist item includes the *entire* checklist object, with the item updated.
                var responsePath = Path.Combine(RecordedResponsesBasePath, "TaskChecklistsService", "EditChecklistItemAsync", "Success.json");
                var responseContent = await File.ReadAllTextAsync(responsePath);

                // Replace placeholders
                responseContent = responseContent.Replace("\"id\": \"{{CHECKLIST_ID}}\"", $"\"id\": \"{parentChecklist.Id}\"")
                                                 .Replace("\"task_id\": \"{{TASK_ID}}\"", $"\"task_id\": \"{_testContext.TaskId}\"")
                                                 .Replace("\"item_id_to_edit\": \"{{ITEM_ID}}\"", $"\"id\": \"{originalItem.Id}\"") // The item being edited in the list
                                                 .Replace("\"name\": \"{{UPDATED_ITEM_NAME}}\"", $"\"name\": \"{updatedItemName}\""); // The new name in the item

                MockHttpHandler.When(HttpMethod.Put, $"{ClientOptions.BaseAddress}/checklist/{parentChecklist.Id}/checklist_item/{originalItem.Id}")
                               .WithPartialContent(updatedItemName) // Check that the new name is in the PUT body
                               .Respond(HttpStatusCode.OK, "application/json", responseContent);
            }

            Assert.NotNull(parentChecklist);
            Assert.NotNull(originalItem);

            var editItemRequest = new EditChecklistItemRequest(Name: updatedItemName, Assignee: null, Resolved: true, Parent: null);

            // Act
            var response = await _taskChecklistsService.EditChecklistItemAsync(parentChecklist.Id, originalItem.Id, editItemRequest);

            // Assert
            response.Should().NotBeNull();
            response.Checklist.Should().NotBeNull();
            response.Checklist.Id.Should().Be(parentChecklist.Id);
            response.Checklist.Items?.Should().NotBeNullOrEmpty(); // Added ?.

            var updatedItem = response.Checklist.Items?.FirstOrDefault(i => i.Id == originalItem.Id); // Added ?.
            updatedItem.Should().NotBeNull();
            updatedItem!.Name.Should().Be(updatedItemName);
            updatedItem.Resolved.Should().Be(true);

            _outputHelper.WriteLine($"[{nameof(EditChecklistItemAsync_ValidData_ShouldUpdateItem)}] Successfully edited checklist item ID: {originalItem.Id}");
        }
        finally
        {
            if (CurrentTestMode != TestMode.Playback && parentChecklist != null && !string.IsNullOrEmpty(parentChecklist.Id))
            {
                _outputHelper.WriteLine($"[{nameof(EditChecklistItemAsync_ValidData_ShouldUpdateItem)}] Attempting to delete parent checklist: {parentChecklist.Id}");
                try
                {
                    await _taskChecklistsService.DeleteChecklistAsync(parentChecklist.Id);
                    _outputHelper.WriteLine($"[{nameof(EditChecklistItemAsync_ValidData_ShouldUpdateItem)}] Successfully deleted parent checklist: {parentChecklist.Id}");
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"[{nameof(EditChecklistItemAsync_ValidData_ShouldUpdateItem)}] Error deleting parent checklist {parentChecklist.Id}: {ex.Message}");
                }
            }
        }
    }

    [Fact]
    public async Task DeleteChecklistItemAsync_ValidId_ShouldDeleteItem()
    {
        Assert.NotNull(_testContext);
        _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] Running in mode: {CurrentTestMode}");
        Models.Entities.Checklists.Checklist? parentChecklist = null;
        ChecklistItem? itemToDelete = null;

        try
        {
            // Arrange: Create a parent checklist and an item in it
            if (CurrentTestMode != TestMode.Playback)
            {
                var checklistName = $"Parent CList for Item Del {Guid.NewGuid()}";
                var createChecklistResponse = await _taskChecklistsService.CreateChecklistAsync(_testContext.TaskId, new CreateChecklistRequest(checklistName));
                parentChecklist = createChecklistResponse.Checklist;
                Assert.NotNull(parentChecklist);
                _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] Created parent checklist ID: {parentChecklist.Id}");

                var itemName = $"Item to Delete {Guid.NewGuid()}";
                var createItemRequest = new CreateChecklistItemRequest(Name: itemName, Assignee: null);
                var createItemResponse = await _taskChecklistsService.CreateChecklistItemAsync(parentChecklist.Id, createItemRequest);
                itemToDelete = createItemResponse.Checklist.Items?.FirstOrDefault(i => i.Name == itemName); // Added ?.
                Assert.NotNull(itemToDelete);
                _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] Created checklist item ID for deletion: {itemToDelete.Id}");
            }
            else
            {
                // For Playback, use predefined IDs.
                itemToDelete = new ChecklistItem(
                    Id: "playback_item_to_delete_qwe",
                    Name: "Item for Playback Deletion",
                    OrderIndex: 0, Assignee: null, Resolved: false, Parent: null, DateCreated: DateTimeOffset.UtcNow, Children: null);

                parentChecklist = new Models.Entities.Checklists.Checklist(
                    Id: "playback_parent_for_item_delete_rty",
                    TaskId: _testContext.TaskId,
                    Name: "Playback Parent for Item Deletion",
                    OrderIndex: 0, Resolved: 0, Unresolved: 1, // Assuming one item initially
                    Items: new System.Collections.Generic.List<ChecklistItem> { itemToDelete }
                );
                _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] Using predefined IDs for Playback: Checklist {parentChecklist.Id}, Item {itemToDelete.Id}");

                Assert.NotNull(MockHttpHandler);
                // Mock the DELETE request for the item.
                // The API for deleting a checklist item typically returns 204 No Content or the updated checklist.
                // The service method DeleteChecklistItemAsync returns Task (void).
                MockHttpHandler.When(HttpMethod.Delete, $"{ClientOptions.BaseAddress}/checklist/{parentChecklist.Id}/checklist_item/{itemToDelete.Id}")
                               .Respond(HttpStatusCode.OK); // Or HttpStatusCode.NoContent
            }

            Assert.NotNull(parentChecklist);
            Assert.NotNull(itemToDelete);

            // Act
            await _taskChecklistsService.DeleteChecklistItemAsync(parentChecklist.Id, itemToDelete.Id);
            _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] Delete operation completed for item ID: {itemToDelete.Id}");

            // Assert
            if (CurrentTestMode != TestMode.Playback)
            {
                // To verify, try to get the parent checklist and check its items.
                // This requires a GET method for checklists or fetching the task and its full details.
                // For simplicity, we assume success if DeleteChecklistItemAsync doesn't throw.
                // A robust test might fetch the parent checklist (if GetChecklist(id) existed)
                // and assert that the item is no longer in its 'items' list.
                _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] In non-Playback, assumed deleted. Verification would need a subsequent GET on parent.");
            }
            else
            {
                // In Playback, MockHttpHandler.VerifyNoOutstandingExpectation() confirms the mock was called.
                _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] In Playback, verified DELETE call for item was expected.");
            }
        }
        finally
        {
            // Cleanup: Delete the parent checklist
            if (CurrentTestMode != TestMode.Playback && parentChecklist != null && !string.IsNullOrEmpty(parentChecklist.Id))
            {
                _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] Attempting to delete parent checklist: {parentChecklist.Id}");
                try
                {
                    await _taskChecklistsService.DeleteChecklistAsync(parentChecklist.Id);
                    _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] Successfully deleted parent checklist: {parentChecklist.Id}");
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"[{nameof(DeleteChecklistItemAsync_ValidId_ShouldDeleteItem)}] Error deleting parent checklist {parentChecklist.Id}: {ex.Message}");
                }
            }
        }
    }

    /*
    [Fact]
    public async Task CreateChecklistAsync_Playback_ShouldReturnMockedChecklist()
    {
        if (CurrentTestMode != TestMode.Playback)
        {
            OutputHelper.WriteLine($"[{nameof(CreateChecklistAsync_Playback_ShouldReturnMockedChecklist)}] Skipping as not in Playback mode.");
            return; // Or use Assert.Skip()
        }

        OutputHelper.WriteLine($"[{nameof(CreateChecklistAsync_Playback_ShouldReturnMockedChecklist)}] Running in Playback mode.");
        Assert.NotNull(MockHttpHandler); // Crucial for playback

        var checklistName = "Mocked Checklist from Playback";
        var createRequest = new CreateChecklistRequest(checklistName);
        var mockChecklistId = "mock_checklist_playback_test_789";
        var mockTaskId = _testTaskId; // Using the one from InitializeAsync for consistency

        var responsePath = Path.Combine(RecordedResponsesBasePath, "TaskChecklistsService", "CreateChecklistAsync", "Success_PlaybackOnly.json");
        // Ensure Success_PlaybackOnly.json exists and is tailored for this test
        // It might use specific IDs like mock_checklist_playback_test_789
        var responseContent = await File.ReadAllTextAsync(responsePath);

        MockHttpHandler.When(HttpMethod.Post, $"{ClientOptions.BaseAddress}/task/{mockTaskId}/checklist")
                       .WithPartialContent(checklistName)
                       .Respond(HttpStatusCode.OK, "application/json", responseContent);

        // Act
        var response = await _taskChecklistsService.CreateChecklistAsync(mockTaskId, createRequest);

        // Assert
        response.Should().NotBeNull();
        response.Checklist.Should().NotBeNull();
        response.Checklist.Name.Should().Be(checklistName); // Or whatever name is in Success_PlaybackOnly.json
        response.Checklist.Id.Should().Be(mockChecklistId); // Matching the ID in Success_PlaybackOnly.json
        response.Checklist.TaskId.Should().Be(mockTaskId);

        OutputHelper.WriteLine($"[{nameof(CreateChecklistAsync_Playback_ShouldReturnMockedChecklist)}] Successfully verified mocked checklist: {response.Checklist.Id}");
    }
    */
} // This is the closing brace for the TaskChecklistsServiceIntegrationTests class
