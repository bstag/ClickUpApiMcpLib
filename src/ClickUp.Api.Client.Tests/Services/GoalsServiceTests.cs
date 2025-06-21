using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.ResponseModels.Goals;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using ClickUp.Api.Client.Models.Entities.Goals;

namespace ClickUp.Api.Client.Tests.Services
{
    public class GoalsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly GoalsService _goalsService;

        public GoalsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _goalsService = new GoalsService(_mockApiConnection.Object);
        }

        // Placeholder DTO creation helpers
        private Goal CreateSampleGoal(string id, string name)
        {
            var goal = (Goal)Activator.CreateInstance(typeof(Goal), nonPublic: true)!;
            var properties = typeof(Goal).GetProperties();
            properties.FirstOrDefault(p => p.Name == "Id")?.SetValue(goal, id);
            properties.FirstOrDefault(p => p.Name == "Name")?.SetValue(goal, name);
            // Set other required properties for Goal if any
            return goal;
        }

        private KeyResult CreateSampleKeyResult(string id, string name)
        {
             var kr = (KeyResult)Activator.CreateInstance(typeof(KeyResult), nonPublic: true)!;
            var properties = typeof(KeyResult).GetProperties();
            properties.FirstOrDefault(p => p.Name == "Id")?.SetValue(kr, id);
            properties.FirstOrDefault(p => p.Name == "Name")?.SetValue(kr, name);
            return kr;
        }
        private GetGoalResponse CreateSampleGetGoalResponse(Goal goal) => new GetGoalResponse(goal); // Assuming record constructor
        private GetKeyResultResponse CreateSampleGetKeyResultResponse(KeyResult kr) => new GetKeyResultResponse(kr);


        [Fact]
        public async Task GetGoalAsync_WhenGoalExists_ReturnsGoal()
        {
            // Arrange
            var goalId = "test-goal-id";
            var sampleGoal = CreateSampleGoal(goalId, "Test Goal");
            var expectedResponse = CreateSampleGetGoalResponse(sampleGoal);

            _mockApiConnection.Setup(c => c.GetAsync<GetGoalResponse>(
                $"goal/{goalId}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _goalsService.GetGoalAsync(goalId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleGoal);
            _mockApiConnection.Verify(c => c.GetAsync<GetGoalResponse>(
                $"goal/{goalId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateGoalAsync_ValidRequest_ReturnsCreatedGoal()
        {
            // Arrange
            var workspaceId = "test-workspace-id";
            var requestDto = new CreateGoalRequest("New Goal", DateTime.UtcNow.AddDays(30), "Owner", "Description", "Multiple Owners", false, new List<string>());
            var sampleGoal = CreateSampleGoal("new-goal-id", "New Goal");
            var expectedResponse = CreateSampleGetGoalResponse(sampleGoal);

            _mockApiConnection.Setup(c => c.PostAsync<CreateGoalRequest, GetGoalResponse>(
                $"team/{workspaceId}/goal",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _goalsService.CreateGoalAsync(workspaceId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleGoal);
            _mockApiConnection.Verify(c => c.PostAsync<CreateGoalRequest, GetGoalResponse>(
                $"team/{workspaceId}/goal",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateKeyResultAsync_ValidRequest_ReturnsCreatedKeyResult()
        {
            // Arrange
            var goalId = "test-goal-id";
            var requestDto = new CreateKeyResultRequest("New KR", new List<string>(), 0, 100, "unit", null, null, null, null, null);
            var sampleKeyResult = CreateSampleKeyResult("new-kr-id", "New KR");
            var expectedResponse = CreateSampleGetKeyResultResponse(sampleKeyResult);

            _mockApiConnection.Setup(c => c.PostAsync<CreateKeyResultRequest, GetKeyResultResponse>(
                $"goal/{goalId}/key_result",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _goalsService.CreateKeyResultAsync(goalId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleKeyResult);
            _mockApiConnection.Verify(c => c.PostAsync<CreateKeyResultRequest, GetKeyResultResponse>(
                $"goal/{goalId}/key_result",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteGoalAsync_ValidId_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var goalId = "goal-to-delete";
            var expectedEndpoint = $"goal/{goalId}";

            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _goalsService.DeleteGoalAsync(goalId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
