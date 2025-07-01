using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.CustomFields;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;
using ClickUp.Api.Client.Models.ResponseModels.CustomFields;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class CustomFieldsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly CustomFieldsService _customFieldsService;
        private readonly Mock<ILogger<CustomFieldsService>> _mockLogger;

        public CustomFieldsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<CustomFieldsService>>();
            _customFieldsService = new CustomFieldsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        // Concrete implementation for testing SetCustomFieldValueRequest
        private class ConcreteSetCustomFieldValueRequest : SetCustomFieldValueRequest
        {
            public string? Value { get; set; }
            // Add other properties if needed by specific tests or field types
        }

        private CustomFieldDefinition CreateSampleField(string id = "field_1") => new CustomFieldDefinition
        {
            Id = id,
            Name = $"Sample Field {id}",
            Type = "text",
            TypeConfig = null, // TypeConfig
            DateCreated = DateTimeOffset.UtcNow,
            HideFromGuests = false, // HideFromGuests
            Required = false // Required
        };

        [Fact]
        public async Task GetAccessibleCustomFieldsAsync_ValidListId_BuildsCorrectUrlAndReturnsFields()
        {
            // Arrange
            var listId = "list_123";
            var expectedFields = new List<CustomFieldDefinition> { CreateSampleField("field_abc") };
            var apiResponse = new GetAccessibleCustomFieldsResponse(expectedFields); // Use constructor
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _customFieldsService.GetAccessibleCustomFieldsAsync(listId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("field_abc", result.First().Id);
            _mockApiConnection.Verify(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(
                $"list/{listId}/field",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetCustomFieldValueAsync_ValidRequest_CallsPostAsyncWithCorrectUrlAndBody()
        {
            // Arrange
            var taskId = "task_xyz";
            var fieldId = "field_123";
            var request = new ConcreteSetCustomFieldValueRequest { Value = "Test Value" }; // Use concrete type
            _mockApiConnection
                .Setup(c => c.PostAsync(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            await _customFieldsService.SetCustomFieldValueAsync(taskId, fieldId, request, true, "team_abc");

            // Assert
            _mockApiConnection.Verify(c => c.PostAsync<SetCustomFieldValueRequest>( // Expecting the base abstract type
                $"task/{taskId}/field/{fieldId}?custom_task_ids=true&team_id=team_abc",
                request, // The actual object can be the concrete type
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveCustomFieldValueAsync_ValidRequest_CallsDeleteAsyncWithCorrectUrl()
        {
            // Arrange
            var taskId = "task_xyz";
            var fieldId = "field_123";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            await _customFieldsService.RemoveCustomFieldValueAsync(taskId, fieldId, customTaskIds: false, teamId: "team_xyz");

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"task/{taskId}/field/{fieldId}?custom_task_ids=false&team_id=team_xyz",
                It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task GetAccessibleCustomFieldsAsync_ApiError_ThrowsHttpRequestException()
        {
            // Arrange
            var listId = "list_error";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _customFieldsService.GetAccessibleCustomFieldsAsync(listId));
        }

        [Fact]
        public async Task GetAccessibleCustomFieldsAsync_NullResponse_ReturnsEmptyList()
        {
            // Arrange
            var listId = "list_null_response";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAccessibleCustomFieldsResponse?)null);

            // Act
            var result = await _customFieldsService.GetAccessibleCustomFieldsAsync(listId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // --- Tests for API Error Cases (HttpRequestException) ---
        [Fact]
        public async Task SetCustomFieldValueAsync_ApiError_ThrowsHttpRequestException()
        {
            var taskId = "task_set_err";
            var fieldId = "field_set_err";
            var request = new ConcreteSetCustomFieldValueRequest { Value = "Error Value" };
            _mockApiConnection
                .Setup(c => c.PostAsync<SetCustomFieldValueRequest>(
                    It.IsAny<string>(),
                    It.IsAny<SetCustomFieldValueRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _customFieldsService.SetCustomFieldValueAsync(taskId, fieldId, request));
        }

        [Fact]
        public async Task RemoveCustomFieldValueAsync_ApiError_ThrowsHttpRequestException()
        {
            var taskId = "task_remove_err";
            var fieldId = "field_remove_err";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _customFieldsService.RemoveCustomFieldValueAsync(taskId, fieldId));
        }

        // --- Tests for TaskCanceledException and CancellationToken pass-through ---

        // GetAccessibleCustomFieldsAsync
        [Fact]
        public async Task GetAccessibleCustomFieldsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var listId = "list_cancel";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _customFieldsService.GetAccessibleCustomFieldsAsync(listId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetAccessibleCustomFieldsAsync_PassesCancellationTokenToApiConnection()
        {
            var listId = "list_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetAccessibleCustomFieldsResponse(new List<CustomFieldDefinition>());
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _customFieldsService.GetAccessibleCustomFieldsAsync(listId, expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(
                $"list/{listId}/field",
                expectedToken), Times.Once);
        }

        // SetCustomFieldValueAsync
        [Fact]
        public async Task SetCustomFieldValueAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var taskId = "task_set_cancel";
            var fieldId = "field_set_cancel";
            var request = new ConcreteSetCustomFieldValueRequest { Value = "Cancel Value" };
            _mockApiConnection
                .Setup(c => c.PostAsync<SetCustomFieldValueRequest>(
                    It.IsAny<string>(),
                    It.IsAny<SetCustomFieldValueRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _customFieldsService.SetCustomFieldValueAsync(taskId, fieldId, request, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task SetCustomFieldValueAsync_PassesCancellationTokenToApiConnection()
        {
            var taskId = "task_set_ct_pass";
            var fieldId = "field_set_ct_pass";
            var request = new ConcreteSetCustomFieldValueRequest { Value = "CT Pass Value" };
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(c => c.PostAsync(It.IsAny<string>(), request, expectedToken))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            await _customFieldsService.SetCustomFieldValueAsync(taskId, fieldId, request, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.PostAsync<SetCustomFieldValueRequest>(
                $"task/{taskId}/field/{fieldId}",
                request,
                expectedToken), Times.Once);
        }

        // RemoveCustomFieldValueAsync
        [Fact]
        public async Task RemoveCustomFieldValueAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var taskId = "task_remove_cancel";
            var fieldId = "field_remove_cancel";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _customFieldsService.RemoveCustomFieldValueAsync(taskId, fieldId, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task RemoveCustomFieldValueAsync_PassesCancellationTokenToApiConnection()
        {
            var taskId = "task_remove_ct_pass";
            var fieldId = "field_remove_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), expectedToken))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            await _customFieldsService.RemoveCustomFieldValueAsync(taskId, fieldId, cancellationToken: expectedToken);

            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"task/{taskId}/field/{fieldId}",
                expectedToken), Times.Once);
        }

        // --- GetFolderCustomFieldsAsync Tests ---
        [Fact]
        public async Task GetFolderCustomFieldsAsync_ValidFolderId_BuildsCorrectUrlAndReturnsFields()
        {
            // Arrange
            var folderId = "folder_456";
            var expectedFields = new List<CustomFieldDefinition> { CreateSampleField("field_folder_1") };
            var apiResponse = new GetAccessibleCustomFieldsResponse(expectedFields);
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>($"folder/{folderId}/field", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _customFieldsService.GetFolderCustomFieldsAsync(folderId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("field_folder_1", result.First().Id);
            _mockApiConnection.Verify(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(
                $"folder/{folderId}/field",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderCustomFieldsAsync_ApiError_ThrowsHttpRequestException()
        {
            var folderId = "folder_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _customFieldsService.GetFolderCustomFieldsAsync(folderId));
        }

        [Fact]
        public async Task GetFolderCustomFieldsAsync_NullResponse_ReturnsEmptyList()
        {
            var folderId = "folder_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAccessibleCustomFieldsResponse?)null);

            var result = await _customFieldsService.GetFolderCustomFieldsAsync(folderId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFolderCustomFieldsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_cancel_op";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _customFieldsService.GetFolderCustomFieldsAsync(folderId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetFolderCustomFieldsAsync_PassesCancellationTokenToApiConnection()
        {
            var folderId = "folder_ct_pass_fields";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetAccessibleCustomFieldsResponse(new List<CustomFieldDefinition>());
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _customFieldsService.GetFolderCustomFieldsAsync(folderId, expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(
                $"folder/{folderId}/field",
                expectedToken), Times.Once);
        }

        // --- GetSpaceCustomFieldsAsync Tests ---
        [Fact]
        public async Task GetSpaceCustomFieldsAsync_ValidSpaceId_BuildsCorrectUrlAndReturnsFields()
        {
            // Arrange
            var spaceId = "space_789";
            var expectedFields = new List<CustomFieldDefinition> { CreateSampleField("field_space_1") };
            var apiResponse = new GetAccessibleCustomFieldsResponse(expectedFields);
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>($"space/{spaceId}/field", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _customFieldsService.GetSpaceCustomFieldsAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("field_space_1", result.First().Id);
            _mockApiConnection.Verify(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(
                $"space/{spaceId}/field",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSpaceCustomFieldsAsync_ApiError_ThrowsHttpRequestException()
        {
            var spaceId = "space_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _customFieldsService.GetSpaceCustomFieldsAsync(spaceId));
        }

        [Fact]
        public async Task GetSpaceCustomFieldsAsync_NullResponse_ReturnsEmptyList()
        {
            var spaceId = "space_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAccessibleCustomFieldsResponse?)null);

            var result = await _customFieldsService.GetSpaceCustomFieldsAsync(spaceId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSpaceCustomFieldsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_cancel_op";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _customFieldsService.GetSpaceCustomFieldsAsync(spaceId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetSpaceCustomFieldsAsync_PassesCancellationTokenToApiConnection()
        {
            var spaceId = "space_ct_pass_fields";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetAccessibleCustomFieldsResponse(new List<CustomFieldDefinition>());
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _customFieldsService.GetSpaceCustomFieldsAsync(spaceId, expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(
                $"space/{spaceId}/field",
                expectedToken), Times.Once);
        }

        // --- GetWorkspaceCustomFieldsAsync Tests ---
        [Fact]
        public async Task GetWorkspaceCustomFieldsAsync_ValidWorkspaceId_BuildsCorrectUrlAndReturnsFields()
        {
            // Arrange
            var workspaceId = "ws_001"; // team_id is workspaceId
            var expectedFields = new List<CustomFieldDefinition> { CreateSampleField("field_ws_1") };
            var apiResponse = new GetAccessibleCustomFieldsResponse(expectedFields);
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>($"team/{workspaceId}/field", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _customFieldsService.GetWorkspaceCustomFieldsAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("field_ws_1", result.First().Id);
            _mockApiConnection.Verify(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(
                $"team/{workspaceId}/field",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetWorkspaceCustomFieldsAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Error"));

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _customFieldsService.GetWorkspaceCustomFieldsAsync(workspaceId));
        }

        [Fact]
        public async Task GetWorkspaceCustomFieldsAsync_NullResponse_ReturnsEmptyList()
        {
            var workspaceId = "ws_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAccessibleCustomFieldsResponse?)null);

            var result = await _customFieldsService.GetWorkspaceCustomFieldsAsync(workspaceId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetWorkspaceCustomFieldsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_cancel_op";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API timeout"));

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _customFieldsService.GetWorkspaceCustomFieldsAsync(workspaceId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetWorkspaceCustomFieldsAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_ct_pass_fields";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponse = new GetAccessibleCustomFieldsResponse(new List<CustomFieldDefinition>());
            _mockApiConnection
                .Setup(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(expectedResponse);

            await _customFieldsService.GetWorkspaceCustomFieldsAsync(workspaceId, expectedToken);

            _mockApiConnection.Verify(c => c.GetAsync<GetAccessibleCustomFieldsResponse>(
                $"team/{workspaceId}/field",
                expectedToken), Times.Once);
        }
    }
}
