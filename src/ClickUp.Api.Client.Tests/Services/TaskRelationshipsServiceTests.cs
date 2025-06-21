using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.TaskRelationships;
using ClickUp.Api.Client.Models.ResponseModels; // For GetTaskResponse
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;

namespace ClickUp.Api.Client.Tests.Services
{
    public class TaskRelationshipsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TaskRelationshipsService _taskRelationshipsService;

        public TaskRelationshipsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _taskRelationshipsService = new TaskRelationshipsService(_mockApiConnection.Object);
        }

        private CuTask CreateSampleTask(string id, string name)
        {
            var task = (CuTask)Activator.CreateInstance(typeof(CuTask), nonPublic: true)!;
            var props = typeof(CuTask).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(task, id);
            props.FirstOrDefault(p => p.Name == "Name")?.SetValue(task, name);
            return task;
        }

        private GetTaskResponse CreateSampleGetTaskResponse(CuTask task)
        {
            var responseType = typeof(GetTaskResponse);
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(CuTask));
            if (constructor != null)
            {
                return (GetTaskResponse)constructor.Invoke(new object[] { task });
            }
            var instance = (GetTaskResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("CuTask")?.SetValue(instance, task); // Assuming GetTaskResponse has a "CuTask" property
            return instance;
        }

        [Fact]
        public async Task AddDependencyAsync_ValidRequest_CallsApiConnectionPostAsync()
        {
            // Arrange
            var taskId = "task-id";
            var dependsOnTaskId = "depends-on-task";
            var requestDto = new AddDependencyRequest(dependsOnTaskId, null); // Service creates this

            _mockApiConnection.Setup(c => c.PostAsync<AddDependencyRequest>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/dependency")),
                It.Is<AddDependencyRequest>(r => r.DependsOn == dependsOnTaskId && r.DependencyOf == null),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskRelationshipsService.AddDependencyAsync(taskId, dependsOnTaskId: dependsOnTaskId, cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.PostAsync<AddDependencyRequest>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/dependency")),
                It.Is<AddDependencyRequest>(r => r.DependsOn == dependsOnTaskId && r.DependencyOf == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTaskLinkAsync_ValidRequest_ReturnsLinkedTask()
        {
            // Arrange
            var taskId = "task-id-link-from";
            var linksToTaskId = "task-id-link-to";
            var expectedTask = CreateSampleTask(taskId, "CuTask with link");
            var expectedResponse = CreateSampleGetTaskResponse(expectedTask);

            _mockApiConnection.Setup(c => c.PostAsync<object, GetTaskResponse>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/link/{linksToTaskId}")),
                It.IsAny<object>(), // Service sends new {}
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _taskRelationshipsService.AddTaskLinkAsync(taskId, linksToTaskId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTask);
        }

        [Fact]
        public async Task DeleteTaskLinkAsync_ValidRequest_ReturnsTaskAfterDeletion()
        {
            // Arrange
            var taskId = "task-id-link-from";
            var linksToTaskId = "task-id-link-to";
            var expectedTask = CreateSampleTask(taskId, "CuTask after link deletion");
            var expectedResponse = CreateSampleGetTaskResponse(expectedTask);

            _mockApiConnection.Setup(c => c.DeleteAsync<GetTaskResponse>( // Using DeleteAsync<TResponse>
                It.Is<string>(s => s.StartsWith($"task/{taskId}/link/{linksToTaskId}")),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _taskRelationshipsService.DeleteTaskLinkAsync(taskId, linksToTaskId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTask);
            _mockApiConnection.Verify(c => c.DeleteAsync<GetTaskResponse>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/link/{linksToTaskId}")),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
