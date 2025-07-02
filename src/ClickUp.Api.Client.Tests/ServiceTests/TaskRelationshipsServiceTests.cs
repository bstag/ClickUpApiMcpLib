using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.RequestModels.TaskRelationships; // For AddDependencyRequest
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class TaskRelationshipsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TaskRelationshipsService _taskRelationshipsService;
        private readonly Mock<ILogger<TaskRelationshipsService>> _mockLogger;

        public TaskRelationshipsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<TaskRelationshipsService>>();
            _taskRelationshipsService = new TaskRelationshipsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        // --- Tests for AddDependencyAsync ---

        [Fact]
        public async Task AddDependencyAsync_WithDependsOn_CallsPostAsyncWithCorrectPayloadAndUrl()
        {
            // Arrange
            var taskId = "task1";
            var dependsOnTaskId = "task2";

            _mockApiConnection
                .Setup(x => x.PostAsync<AddDependencyRequest>(
                    $"task/{taskId}/dependency",
                    It.Is<AddDependencyRequest>(p => p.DependsOn == dependsOnTaskId && p.DependencyOf == null),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.AddDependencyAsync(taskId, dependsOnTaskId: dependsOnTaskId);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<AddDependencyRequest>(
                $"task/{taskId}/dependency",
                It.Is<AddDependencyRequest>(p => p.DependsOn == dependsOnTaskId && p.DependencyOf == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddDependencyAsync_WithDependencyOf_CallsPostAsyncWithCorrectPayloadAndUrl()
        {
            // Arrange
            var taskId = "task1";
            var dependencyOfTaskId = "task0";

            _mockApiConnection
                .Setup(x => x.PostAsync<AddDependencyRequest>(
                    $"task/{taskId}/dependency",
                    It.Is<AddDependencyRequest>(p => p.DependencyOf == dependencyOfTaskId && p.DependsOn == null),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.AddDependencyAsync(taskId, dependencyOfTaskId: dependencyOfTaskId);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<AddDependencyRequest>(
                $"task/{taskId}/dependency",
                It.Is<AddDependencyRequest>(p => p.DependencyOf == dependencyOfTaskId && p.DependsOn == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddDependencyAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectUrl()
        {
            // Arrange
            var taskId = "custom_task_dep_1";
            var dependsOnTaskId = "custom_task_dep_2";
            var customTaskIds = true;
            var teamId = "team_dep";

            _mockApiConnection
                .Setup(x => x.PostAsync<AddDependencyRequest>(It.IsAny<string>(), It.IsAny<AddDependencyRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.AddDependencyAsync(taskId, dependsOnTaskId: dependsOnTaskId, customTaskIds: customTaskIds, teamId: teamId);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<AddDependencyRequest>(
                $"task/{taskId}/dependency?custom_task_ids=true&team_id={teamId}",
                It.Is<AddDependencyRequest>(p => p.DependsOn == dependsOnTaskId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddDependencyAsync_NeitherDependsOnNorDependencyOf_ThrowsArgumentException()
        {
            // Arrange
            var taskId = "task_no_dep";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _taskRelationshipsService.AddDependencyAsync(taskId)
            );
        }

        [Fact]
        public async Task AddDependencyAsync_BothDependsOnAndDependencyOf_ThrowsArgumentException()
        {
            // Arrange
            var taskId = "task_both_dep";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _taskRelationshipsService.AddDependencyAsync(taskId, dependsOnTaskId: "t2", dependencyOfTaskId: "t0")
            );
        }

        [Fact]
        public async Task AddDependencyAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var taskId = "task_dep_http_ex";
            var dependsOnTaskId = "task_dep_http_ex_target";
            _mockApiConnection
                .Setup(x => x.PostAsync<AddDependencyRequest>(It.IsAny<string>(), It.IsAny<AddDependencyRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskRelationshipsService.AddDependencyAsync(taskId, dependsOnTaskId: dependsOnTaskId)
            );
        }

        [Fact]
        public async Task AddDependencyAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var taskId = "task_dep_op_cancel";
            var dependsOnTaskId = "task_dep_op_cancel_target";
            var cancellationTokenSource = new CancellationTokenSource();

            _mockApiConnection.Setup(x => x.PostAsync<AddDependencyRequest>(
                    It.IsAny<string>(), It.IsAny<AddDependencyRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, AddDependencyRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .Returns(Task.CompletedTask);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskRelationshipsService.AddDependencyAsync(taskId, dependsOnTaskId: dependsOnTaskId, cancellationToken: cancellationTokenSource.Token));
        }

        [Fact]
        public async Task AddDependencyAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "task_dep_cancel_ex";
            var dependsOnTaskId = "task_dep_cancel_ex_target";
            _mockApiConnection
                .Setup(x => x.PostAsync<AddDependencyRequest>(It.IsAny<string>(), It.IsAny<AddDependencyRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskRelationshipsService.AddDependencyAsync(taskId, dependsOnTaskId: dependsOnTaskId, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task AddDependencyAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "task_dep_ct_pass";
            var dependsOnTaskId = "task_dep_ct_pass_target";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.PostAsync<AddDependencyRequest>(
                    It.IsAny<string>(),
                    It.IsAny<AddDependencyRequest>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.AddDependencyAsync(taskId, dependsOnTaskId: dependsOnTaskId, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<AddDependencyRequest>(
                $"task/{taskId}/dependency",
                It.Is<AddDependencyRequest>(p => p.DependsOn == dependsOnTaskId),
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteDependencyAsync ---

        [Fact]
        public async Task DeleteDependencyAsync_WithDependsOn_CallsDeleteAsyncWithCorrectUrl()
        {
            // Arrange
            var taskId = "task1_del_dep";
            var dependsOnTargetId = "task2_del_dep_on";

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"task/{taskId}/dependency?depends_on={dependsOnTargetId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.DeleteDependencyAsync(taskId, new DeleteDependencyRequest(dependsOnTaskId: dependsOnTargetId));

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"task/{taskId}/dependency?depends_on={dependsOnTargetId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDependencyAsync_WithDependencyOf_CallsDeleteAsyncWithCorrectUrl()
        {
            // Arrange
            var taskId = "task1_del_dep_of";
            var dependencyOfTargetId = "task0_del_dep_of";

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"task/{taskId}/dependency?dependency_of={dependencyOfTargetId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.DeleteDependencyAsync(taskId, new DeleteDependencyRequest(dependencyOfTaskId: dependencyOfTargetId));

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"task/{taskId}/dependency?dependency_of={dependencyOfTargetId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDependencyAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectUrl()
        {
            // Arrange
            var taskId = "custom_task_del_dep_1";
            var dependsOnTargetId = "custom_task_del_dep_2";
            var customTaskIds = false;
            var teamId = "team_del_dep";

            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.DeleteDependencyAsync(taskId, new DeleteDependencyRequest(dependsOnTaskId: dependsOnTargetId, customTaskIds: customTaskIds, teamId: teamId));

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"task/{taskId}/dependency?depends_on={dependsOnTargetId}&custom_task_ids=false&team_id={teamId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDependencyAsync_NeitherDependsOnNorDependencyOf_ThrowsArgumentException()
        {
            // Arrange
            var taskId = "task_del_no_specific_dep";

            // Act & Assert
            // Validation is now in DTO constructor
            Assert.Throws<ArgumentException>(() =>
                new DeleteDependencyRequest(dependsOnTaskId: "", dependencyOfTaskId: "") // This will throw
            );
        }

        [Fact]
        public async Task DeleteDependencyAsync_BothDependsOnAndDependencyOf_ThrowsArgumentException()
        {
            // Arrange
            // Validation is now in DTO constructor
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                 new DeleteDependencyRequest(dependsOnTaskId: "t2", dependencyOfTaskId: "t0") // This will throw
            );
        }

        [Fact]
        public async Task DeleteDependencyAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var taskId = "task_del_dep_http_ex";
            var dependsOnTargetId = "task_del_dep_http_ex_target";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskRelationshipsService.DeleteDependencyAsync(taskId, new DeleteDependencyRequest(dependsOnTaskId: dependsOnTargetId))
            );
        }

        [Fact]
        public async Task DeleteDependencyAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var taskId = "task_del_dep_op_cancel";
            var dependsOnTargetId = "task_del_dep_op_cancel_target";
            var requestModel = new DeleteDependencyRequest(dependsOnTaskId: dependsOnTargetId);
            var cancellationTokenSource = new CancellationTokenSource();

            _mockApiConnection.Setup(x => x.DeleteAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .Returns(Task.CompletedTask);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskRelationshipsService.DeleteDependencyAsync(taskId, requestModel, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task DeleteDependencyAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "task_del_dep_cancel_ex";
            var dependsOnTargetId = "task_del_dep_cancel_ex_target";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskRelationshipsService.DeleteDependencyAsync(taskId, new DeleteDependencyRequest(dependsOnTaskId: dependsOnTargetId), cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task DeleteDependencyAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "task_del_dep_ct_pass";
            var dependsOnTargetId = "task_del_dep_ct_pass_target";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.DeleteDependencyAsync(taskId, new DeleteDependencyRequest(dependsOnTaskId: dependsOnTargetId), cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"task/{taskId}/dependency?depends_on={dependsOnTargetId}",
                expectedToken), Times.Once);
        }

        // --- Tests for AddTaskLinkAsync ---
        // Helper to create a sample CuTask for responses, simplified
        private ClickUp.Api.Client.Models.Entities.Tasks.CuTask CreateSampleCuTask(string id, string name) =>
            new ClickUp.Api.Client.Models.Entities.Tasks.CuTask(
                Id: id, Name: name, CustomId: null, CustomItemId: null, TextContent: null, Description: null, MarkdownDescription: null,
                Status: null, OrderIndex: null, DateCreated: null, DateUpdated: null, DateClosed: null, Archived: null,
                Creator: null, Assignees: null, GroupAssignees: null, Watchers: null, Checklists: null, Tags: null,
                Parent: null, Priority: null, DueDate: null, StartDate: null, Points: null, TimeEstimate: null,
                TimeSpent: null, CustomFields: null, Dependencies: null, LinkedTasks: null, TeamId: null, Url: null,
                Sharing: null, PermissionLevel: null, List: null, Folder: null, Space: null, Project: null
            );


        [Fact]
        public async Task AddTaskLinkAsync_ValidRequest_ReturnsLinkedTask()
        {
            // Arrange
            var taskId = "task_link_from";
            var linksToTaskId = "task_link_to";
            var expectedResponseTask = CreateSampleCuTask(taskId, "Task with Link");
            var apiResponse = new ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse(expectedResponseTask);


            _mockApiConnection
                .Setup(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(
                    $"task/{taskId}/link/{linksToTaskId}",
                    It.IsAny<object>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _taskRelationshipsService.AddTaskLinkAsync(taskId, linksToTaskId, new AddTaskLinkRequest());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponseTask.Id, result.Id);
            _mockApiConnection.Verify(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(
                $"task/{taskId}/link/{linksToTaskId}",
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTaskLinkAsync_WithCustomIdsAndTeamId_BuildsCorrectUrl()
        {
            // Arrange
            var taskId = "custom_link_from";
            var linksToTaskId = "custom_link_to";
            var customTaskIds = true;
            var teamId = "team_link";
            var expectedResponseTask = CreateSampleCuTask(taskId, "Task with Link Custom");
            var apiResponse = new ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse(expectedResponseTask);


            _mockApiConnection
                .Setup(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _taskRelationshipsService.AddTaskLinkAsync(taskId, linksToTaskId, new AddTaskLinkRequest(customTaskIds, teamId));

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(
                $"task/{taskId}/link/{linksToTaskId}?custom_task_ids=true&team_id={teamId}",
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTaskLinkAsync_ApiReturnsNullResponse_ReturnsNull()
        {
            // Arrange
            var taskId = "task_link_null_resp";
            var linksToTaskId = "task_link_to_null";
            _mockApiConnection
                .Setup(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse?)null);

            // Act
            var result = await _taskRelationshipsService.AddTaskLinkAsync(taskId, linksToTaskId, new AddTaskLinkRequest());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddTaskLinkAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var taskId = "task_link_http_ex";
            var linksToTaskId = "task_link_to_http_ex";
            _mockApiConnection
                .Setup(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskRelationshipsService.AddTaskLinkAsync(taskId, linksToTaskId, new AddTaskLinkRequest())
            );
        }

        [Fact]
        public async Task AddTaskLinkAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var taskId = "task_link_op_cancel";
            var linksToTaskId = "task_link_to_op_cancel";
            var requestModel = new AddTaskLinkRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponseTask = CreateSampleCuTask(taskId, "Dummy Task");
            var dummyApiResponse = new ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse(dummyResponseTask);


            _mockApiConnection.Setup(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(
                    It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Callback<string, object, CancellationToken>((url, body, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyApiResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskRelationshipsService.AddTaskLinkAsync(taskId, linksToTaskId, requestModel, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task AddTaskLinkAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "task_link_cancel_ex";
            var linksToTaskId = "task_link_to_cancel_ex";
            _mockApiConnection
                .Setup(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskRelationshipsService.AddTaskLinkAsync(taskId, linksToTaskId, new AddTaskLinkRequest(), cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task AddTaskLinkAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "task_link_ct_pass";
            var linksToTaskId = "task_link_to_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedResponseTask = CreateSampleCuTask(taskId, "CT Pass Link Task");
            var apiResponse = new ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse(expectedResponseTask);

            _mockApiConnection
                .Setup(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _taskRelationshipsService.AddTaskLinkAsync(taskId, linksToTaskId, new AddTaskLinkRequest(), cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<object, ClickUp.Api.Client.Models.ResponseModels.Tasks.GetTaskResponse>(
                $"task/{taskId}/link/{linksToTaskId}",
                It.IsAny<object>(),
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteTaskLinkAsync ---
        [Fact]
        public async Task DeleteTaskLinkAsync_ValidRequest_ReturnsNullAsPerCurrentImpl()
        {
            // Arrange
            var taskId = "task_unlink_from";
            var linksToTaskId = "task_unlink_to";

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"task/{taskId}/link/{linksToTaskId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _taskRelationshipsService.DeleteTaskLinkAsync(taskId, linksToTaskId, new DeleteTaskLinkRequest());

            // Assert
            Assert.Null(result);
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"task/{taskId}/link/{linksToTaskId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskLinkAsync_WithCustomIdsAndTeamId_BuildsCorrectUrl()
        {
            // Arrange
            var taskId = "custom_unlink_from";
            var linksToTaskId = "custom_unlink_to";
            var customTaskIds = false;
            var teamId = "team_unlink";

            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.DeleteTaskLinkAsync(taskId, linksToTaskId, new DeleteTaskLinkRequest(customTaskIds, teamId));

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"task/{taskId}/link/{linksToTaskId}?custom_task_ids=false&team_id={teamId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskLinkAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var taskId = "task_unlink_http_ex";
            var linksToTaskId = "task_unlink_to_http_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _taskRelationshipsService.DeleteTaskLinkAsync(taskId, linksToTaskId, new DeleteTaskLinkRequest())
            );
        }

        [Fact]
        public async Task DeleteTaskLinkAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var taskId = "task_unlink_op_cancel";
            var linksToTaskId = "task_unlink_to_op_cancel";
            var requestModel = new DeleteTaskLinkRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            // DeleteAsync in this service currently returns null, so no dummy response needed for the ReturnsAsync part
            // if we were testing a DeleteAsync<T> that returns something.

            _mockApiConnection.Setup(x => x.DeleteAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .Returns(Task.CompletedTask); // DeleteAsync is void

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _taskRelationshipsService.DeleteTaskLinkAsync(taskId, linksToTaskId, requestModel, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task DeleteTaskLinkAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "task_unlink_cancel_ex";
            var linksToTaskId = "task_unlink_to_cancel_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _taskRelationshipsService.DeleteTaskLinkAsync(taskId, linksToTaskId, new DeleteTaskLinkRequest(), cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task DeleteTaskLinkAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "task_unlink_ct_pass";
            var linksToTaskId = "task_unlink_to_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.DeleteTaskLinkAsync(taskId, linksToTaskId, new DeleteTaskLinkRequest(), cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"task/{taskId}/link/{linksToTaskId}",
                expectedToken), Times.Once);
        }
    }
}
