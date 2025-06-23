using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Checklists; // For Checklist, ChecklistItem
using ClickUp.Api.Client.Models.RequestModels.Checklists; // For request DTOs
using ClickUp.Api.Client.Models.ResponseModels.Checklists; // For response DTOs
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class TaskChecklistsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TaskChecklistsService _taskChecklistsService;
        private readonly Mock<ILogger<TaskChecklistsService>> _mockLogger;

        public TaskChecklistsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<TaskChecklistsService>>();
            _taskChecklistsService = new TaskChecklistsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private Checklist CreateSampleChecklist(string id = "cl_1", string name = "Sample Checklist", string taskId = "task_abc")
        {
            return new Checklist(
                Id: id,
                TaskId: taskId,
                Name: name,
                OrderIndex: 0, // Corrected from Orderindex
                Resolved: 0,
                Unresolved: 1,
                Items: new List<ChecklistItem>()
            );
        }

        // --- Tests for CreateChecklistAsync ---

        [Fact]
        public async Task CreateChecklistAsync_ValidRequest_ReturnsChecklistResponse()
        {
            // Arrange
            var taskId = "task123";
            var request = new CreateChecklistRequest(Name: "New Checklist"); // Corrected constructor
            var expectedChecklist = CreateSampleChecklist("cl_new", "New Checklist", taskId);
            var apiResponse = new CreateChecklistResponse { Checklist = expectedChecklist }; // Corrected instantiation

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(
                    It.Is<string>(s => s.StartsWith($"task/{taskId}/checklist")),
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _taskChecklistsService.CreateChecklistAsync(taskId, request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Checklist);
            Assert.Equal(expectedChecklist.Id, result.Checklist.Id);
            _mockApiConnection.Verify(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(
                $"task/{taskId}/checklist", // No query params by default
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateChecklistAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectUrl()
        {
            // Arrange
            var taskId = "custom_task_id_for_cl";
            var request = new CreateChecklistRequest(Name: "Custom ID Checklist"); // Corrected
            var customTaskIds = true;
            var teamId = "team_xyz";
            var expectedChecklist = CreateSampleChecklist("cl_custom", "Custom ID Checklist", taskId);
            var apiResponse = new CreateChecklistResponse { Checklist = expectedChecklist }; // Corrected

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _taskChecklistsService.CreateChecklistAsync(taskId, request, customTaskIds, teamId);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(
                $"task/{taskId}/checklist?custom_task_ids=true&team_id={teamId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateChecklistAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var taskId = "task_cl_null_api_resp";
            var request = new CreateChecklistRequest(Name: "Null API Resp Checklist"); // Corrected
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(It.IsAny<string>(), It.IsAny<CreateChecklistRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreateChecklistResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskChecklistsService.CreateChecklistAsync(taskId, request)
            );
        }

        [Fact]
        public async Task CreateChecklistAsync_ApiReturnsResponseWithNullChecklist_ThrowsInvalidOperationException()
        {
            // Arrange
            var taskId = "task_cl_null_checklist_in_resp";
            var request = new CreateChecklistRequest(Name: "Null Checklist Data"); // Corrected
            var apiResponse = new CreateChecklistResponse { Checklist = null! }; // Checklist property is null
             _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(It.IsAny<string>(), It.IsAny<CreateChecklistRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskChecklistsService.CreateChecklistAsync(taskId, request)
            );
        }


        [Fact]
        public async Task CreateChecklistAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var taskId = "task_cl_http_ex";
            var request = new CreateChecklistRequest(Name: "HTTP Ex Checklist"); // Corrected
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(It.IsAny<string>(), It.IsAny<CreateChecklistRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskChecklistsService.CreateChecklistAsync(taskId, request)
            );
        }

        [Fact]
        public async Task CreateChecklistAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "task_cl_cancel_ex";
            var request = new CreateChecklistRequest(Name: "Cancel Ex Checklist"); // Corrected
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(It.IsAny<string>(), It.IsAny<CreateChecklistRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskChecklistsService.CreateChecklistAsync(taskId, request, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateChecklistAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "task_cl_ct_pass";
            var request = new CreateChecklistRequest(Name: "CT Pass Checklist"); // Corrected
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedChecklist = CreateSampleChecklist("cl_ct_new", "CT Pass Checklist", taskId);
            var apiResponse = new CreateChecklistResponse { Checklist = expectedChecklist }; // Corrected

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _taskChecklistsService.CreateChecklistAsync(taskId, request, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(
                $"task/{taskId}/checklist",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for EditChecklistAsync ---

        [Fact]
        public async Task EditChecklistAsync_ValidRequest_CallsPutAsync()
        {
            // Arrange
            var checklistId = "cl_edit_123";
            var request = new EditChecklistRequest(Name: "Updated Checklist Name", Position: 1); // Corrected

            _mockApiConnection
                .Setup(x => x.PutAsync( // Non-generic PutAsync as service method is void
                    $"checklist/{checklistId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskChecklistsService.EditChecklistAsync(checklistId, request);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync(
                $"checklist/{checklistId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditChecklistAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_edit_http_ex";
            var request = new EditChecklistRequest(Name: "Edit HTTP Ex", Position: null); // Corrected
            _mockApiConnection
                .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<EditChecklistRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskChecklistsService.EditChecklistAsync(checklistId, request)
            );
        }

        [Fact]
        public async Task EditChecklistAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_edit_cancel_ex";
            var request = new EditChecklistRequest(Name: "Edit Cancel Ex", Position: null); // Corrected
            _mockApiConnection
                .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<EditChecklistRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskChecklistsService.EditChecklistAsync(checklistId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task EditChecklistAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var checklistId = "cl_edit_ct_pass";
            var request = new EditChecklistRequest(Name: "Edit CT Pass", Position: null); // Corrected
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.PutAsync(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _taskChecklistsService.EditChecklistAsync(checklistId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync(
                $"checklist/{checklistId}",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteChecklistAsync ---

        [Fact]
        public async Task DeleteChecklistAsync_ValidChecklistId_CallsDeleteAsync()
        {
            // Arrange
            var checklistId = "cl_delete_123";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"checklist/{checklistId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskChecklistsService.DeleteChecklistAsync(checklistId);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"checklist/{checklistId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteChecklistAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_delete_http_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskChecklistsService.DeleteChecklistAsync(checklistId)
            );
        }

        [Fact]
        public async Task DeleteChecklistAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_delete_cancel_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskChecklistsService.DeleteChecklistAsync(checklistId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task DeleteChecklistAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var checklistId = "cl_delete_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _taskChecklistsService.DeleteChecklistAsync(checklistId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"checklist/{checklistId}",
                expectedToken), Times.Once);
        }

        // --- Tests for CreateChecklistItemAsync ---

        private ChecklistItem CreateSampleChecklistItem(string id = "cli_1", string name = "Sample Item", int? assigneeId = null) // Added assigneeId
        {
            var sampleUser = assigneeId.HasValue ? new ClickUp.Api.Client.Models.Entities.Users.User(assigneeId.Value, "Assigned User", "assignee@example.com", "#CCC", null, "AU") : null;
            return new ChecklistItem(
                Id: id,
                Name: name,
                OrderIndex: 0, // Corrected from Orderindex
                Assignee: sampleUser, // Corrected
                Resolved: false,
                Parent: null,
                DateCreated: DateTimeOffset.UtcNow.AddMinutes(-5),
                Children: new List<ChecklistItem>()
            );
        }

        [Fact]
        public async Task CreateChecklistItemAsync_ValidRequest_ReturnsChecklistItemResponse()
        {
            // Arrange
            var checklistId = "cl_123_item";
            var request = new CreateChecklistItemRequest(Name: "New Item", Assignee: null); // Corrected
            // The response for CreateChecklistItem is CreateChecklistItemResponse, which wraps a Checklist.
            // The new item should be inside that Checklist's Items collection.
            var newItem = CreateSampleChecklistItem("cli_new", "New Item");
            var parentChecklist = CreateSampleChecklist(checklistId, "Parent Checklist");
            parentChecklist = parentChecklist with { Items = new List<ChecklistItem> { newItem } };
            var apiResponse = new CreateChecklistItemResponse { Checklist = parentChecklist }; // Corrected


            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistItemRequest, CreateChecklistItemResponse>(
                    $"checklist/{checklistId}/checklist_item",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _taskChecklistsService.CreateChecklistItemAsync(checklistId, request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Checklist);
            Assert.NotNull(result.Checklist.Items);
            Assert.Contains(result.Checklist.Items, item => item.Id == "cli_new" && item.Name == "New Item");
            _mockApiConnection.Verify(x => x.PostAsync<CreateChecklistItemRequest, CreateChecklistItemResponse>(
                $"checklist/{checklistId}/checklist_item",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateChecklistItemAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var checklistId = "cl_item_null_api_resp";
            var request = new CreateChecklistItemRequest(Name: "Null API Resp Item", Assignee: null); // Corrected
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistItemRequest, CreateChecklistItemResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreateChecklistItemResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskChecklistsService.CreateChecklistItemAsync(checklistId, request)
            );
        }

        [Fact]
        public async Task CreateChecklistItemAsync_ApiReturnsResponseWithNullChecklist_ThrowsInvalidOperationException()
        {
            // Arrange
            var checklistId = "cl_item_null_cl_in_resp";
            var request = new CreateChecklistItemRequest(Name: "Null Checklist Data Item", Assignee: null); // Corrected
            var apiResponse = new CreateChecklistItemResponse { Checklist = null! }; // Checklist property is null
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistItemRequest, CreateChecklistItemResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskChecklistsService.CreateChecklistItemAsync(checklistId, request)
            );
        }

        [Fact]
        public async Task CreateChecklistItemAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_item_http_ex";
            var request = new CreateChecklistItemRequest(Name: "HTTP Ex Item", Assignee: null); // Corrected
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistItemRequest, CreateChecklistItemResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskChecklistsService.CreateChecklistItemAsync(checklistId, request)
            );
        }

        [Fact]
        public async Task CreateChecklistItemAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_item_cancel_ex";
            var request = new CreateChecklistItemRequest(Name: "Cancel Ex Item", Assignee: null); // Corrected
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistItemRequest, CreateChecklistItemResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskChecklistsService.CreateChecklistItemAsync(checklistId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateChecklistItemAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var checklistId = "cl_item_ct_pass";
            var request = new CreateChecklistItemRequest(Name: "CT Pass Item", Assignee: null); // Corrected
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var parentChecklist = CreateSampleChecklist(checklistId, "Parent For CT Item");
            parentChecklist = parentChecklist with { Items = new List<ChecklistItem> { CreateSampleChecklistItem("cli_ct_new", "CT Pass Item") } };
            var apiResponse = new CreateChecklistItemResponse { Checklist = parentChecklist }; // Corrected

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateChecklistItemRequest, CreateChecklistItemResponse>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _taskChecklistsService.CreateChecklistItemAsync(checklistId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateChecklistItemRequest, CreateChecklistItemResponse>(
                $"checklist/{checklistId}/checklist_item",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for EditChecklistItemAsync ---

        [Fact]
        public async Task EditChecklistItemAsync_ValidRequest_ReturnsChecklistItemResponse()
        {
            // Arrange
            var checklistId = "cl_123_edit_item";
            var checklistItemId = "cli_456_edit";
            var request = new EditChecklistItemRequest(Name: "Updated Item Name", Assignee: null, Resolved: true, Parent: null); // Corrected
            var updatedItem = CreateSampleChecklistItem(checklistItemId, "Updated Item Name") with { Resolved = true };
            var parentChecklist = CreateSampleChecklist(checklistId, "Parent For Edit Item");
            parentChecklist = parentChecklist with { Items = new List<ChecklistItem> { updatedItem } }; // Assume it's the only item for simplicity
            var apiResponse = new EditChecklistItemResponse { Checklist = parentChecklist }; // Corrected


            _mockApiConnection
                .Setup(x => x.PutAsync<EditChecklistItemRequest, EditChecklistItemResponse>(
                    $"checklist/{checklistId}/checklist_item/{checklistItemId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _taskChecklistsService.EditChecklistItemAsync(checklistId, checklistItemId, request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Checklist);
            Assert.NotNull(result.Checklist.Items);
            var editedItemInResponse = result.Checklist.Items.FirstOrDefault(item => item.Id == checklistItemId);
            Assert.NotNull(editedItemInResponse);
            Assert.Equal("Updated Item Name", editedItemInResponse.Name);
            Assert.True(editedItemInResponse.Resolved);
            _mockApiConnection.Verify(x => x.PutAsync<EditChecklistItemRequest, EditChecklistItemResponse>(
                $"checklist/{checklistId}/checklist_item/{checklistItemId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditChecklistItemAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var checklistId = "cl_edit_item_null_api";
            var checklistItemId = "cli_edit_null_api";
            var request = new EditChecklistItemRequest(Name: "Edit Null API Item", Assignee: null, Resolved: null, Parent: null); // Corrected
            _mockApiConnection
                .Setup(x => x.PutAsync<EditChecklistItemRequest, EditChecklistItemResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EditChecklistItemResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskChecklistsService.EditChecklistItemAsync(checklistId, checklistItemId, request)
            );
        }

        [Fact]
        public async Task EditChecklistItemAsync_ApiReturnsResponseWithNullChecklist_ThrowsInvalidOperationException()
        {
            // Arrange
            var checklistId = "cl_edit_item_null_cl";
            var checklistItemId = "cli_edit_null_cl";
            var request = new EditChecklistItemRequest(Name: "Edit Null Checklist Item", Assignee: null, Resolved: null, Parent: null); // Corrected
            var apiResponse = new EditChecklistItemResponse { Checklist = null! }; // Checklist property is null
            _mockApiConnection
                .Setup(x => x.PutAsync<EditChecklistItemRequest, EditChecklistItemResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskChecklistsService.EditChecklistItemAsync(checklistId, checklistItemId, request)
            );
        }

        [Fact]
        public async Task EditChecklistItemAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_edit_item_http_ex";
            var checklistItemId = "cli_edit_http_ex";
            var request = new EditChecklistItemRequest(Name: "Edit HTTP Ex Item", Assignee: null, Resolved: null, Parent: null); // Corrected
            _mockApiConnection
                .Setup(x => x.PutAsync<EditChecklistItemRequest, EditChecklistItemResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskChecklistsService.EditChecklistItemAsync(checklistId, checklistItemId, request)
            );
        }

        [Fact]
        public async Task EditChecklistItemAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_edit_item_cancel_ex";
            var checklistItemId = "cli_edit_cancel_ex";
            var request = new EditChecklistItemRequest(Name: "Edit Cancel Ex Item", Assignee: null, Resolved: null, Parent: null); // Corrected
            _mockApiConnection
                .Setup(x => x.PutAsync<EditChecklistItemRequest, EditChecklistItemResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskChecklistsService.EditChecklistItemAsync(checklistId, checklistItemId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task EditChecklistItemAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var checklistId = "cl_edit_item_ct_pass";
            var checklistItemId = "cli_edit_ct_pass";
            var request = new EditChecklistItemRequest(Name: "Edit CT Pass Item", Assignee: null, Resolved: null, Parent: null); // Corrected
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var parentChecklist = CreateSampleChecklist(checklistId, "Parent For Edit CT Item");
            parentChecklist = parentChecklist with { Items = new List<ChecklistItem> { CreateSampleChecklistItem(checklistItemId, "Edit CT Pass Item") } };
            var apiResponse = new EditChecklistItemResponse { Checklist = parentChecklist }; // Corrected

            _mockApiConnection
                .Setup(x => x.PutAsync<EditChecklistItemRequest, EditChecklistItemResponse>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _taskChecklistsService.EditChecklistItemAsync(checklistId, checklistItemId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync<EditChecklistItemRequest, EditChecklistItemResponse>(
                $"checklist/{checklistId}/checklist_item/{checklistItemId}",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteChecklistItemAsync ---

        [Fact]
        public async Task DeleteChecklistItemAsync_ValidIds_CallsDeleteAsync()
        {
            // Arrange
            var checklistId = "cl_del_item_parent";
            var checklistItemId = "cli_del_item_child";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"checklist/{checklistId}/checklist_item/{checklistItemId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskChecklistsService.DeleteChecklistItemAsync(checklistId, checklistItemId);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"checklist/{checklistId}/checklist_item/{checklistItemId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteChecklistItemAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_del_item_http_ex";
            var checklistItemId = "cli_del_item_http_ex_child";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskChecklistsService.DeleteChecklistItemAsync(checklistId, checklistItemId)
            );
        }

        [Fact]
        public async Task DeleteChecklistItemAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var checklistId = "cl_del_item_cancel_ex";
            var checklistItemId = "cli_del_item_cancel_ex_child";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskChecklistsService.DeleteChecklistItemAsync(checklistId, checklistItemId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task DeleteChecklistItemAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var checklistId = "cl_del_item_ct_pass";
            var checklistItemId = "cli_del_item_ct_pass_child";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _taskChecklistsService.DeleteChecklistItemAsync(checklistId, checklistItemId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"checklist/{checklistId}/checklist_item/{checklistItemId}",
                expectedToken), Times.Once);
        }
    }
}
