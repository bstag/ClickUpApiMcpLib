using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.ResponseModels.Goals;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class GoalsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly GoalsService _goalsService;
        private readonly Mock<ILogger<GoalsService>> _mockLogger;

        public GoalsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<GoalsService>>();
            _goalsService = new GoalsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private User CreateSampleUser(int id = 1) => new User(id, $"user{id}", $"user{id}@example.com", "#FFFFFF", "http://example.com/pic.jpg", $"U{id}");

        private Goal CreateSampleGoal(string id = "goal_1", string name = "Sample Goal", string teamId = "team_1") => new Goal(
            Id: id,
            PrettyId: $"P-{id}",
            Name: name,
            TeamId: teamId,
            CreatorUser: CreateSampleUser(1),
            OwnerUser: CreateSampleUser(2),
            Color: "#123456",
            DateCreated: DateTimeOffset.UtcNow.AddDays(-10),
            StartDate: DateTimeOffset.UtcNow.AddDays(-9),
            DueDate: DateTimeOffset.UtcNow.AddDays(30),
            Description: "Sample goal description",
            Private: false,
            Archived: false,
            MultipleOwners: true,
            EditorToken: "token_abc",
            DateUpdated: DateTimeOffset.UtcNow.AddDays(-1),
            LastUpdate: DateTimeOffset.UtcNow,
            FolderId: "folder_xyz",
            Pinned: false,
            Owners: new List<User> { CreateSampleUser(2), CreateSampleUser(3) },
            KeyResultCount: 0,
            Members: new List<ClickUp.Api.Client.Models.Common.Member>(),
            GroupMembers: new List<ClickUp.Api.Client.Models.Common.Member>(),
            PercentCompleted: 25, // Was 0, corrected to match previous error line number context
            History: new List<object>(),
            PrettyUrl: $"https://app.clickup.com/t/{teamId}/g/{id}"
        );

        private LastAction CreateSampleLastAction(string keyResultId = "kr_1", int userId = 1) => new LastAction(
            Id: "action_1", // string
            KeyResultId: keyResultId, // string
            UserId: userId, // int
            User: CreateSampleUser(userId), // User?
            DateModified: DateTimeOffset.UtcNow.ToString(), // string
            StepsTaken: 1, // int?
            Note: "Sample action note", // string?
            NoteHtml: "<p>Sample action note</p>", // string?
            StepsBefore: 0, // object?
            StepsCurrent: 1, // object?
            StepsBeforeFloat: 0.0, // double?
            StepsCurrentFloat: 1.0, // double?
            StepsBeforeString: "0", // string?
            StepsCurrentString: "1", // string?
            Type: "manual" // string?
        );

        private KeyResult CreateSampleKeyResult(string id = "kr_1", string goalId = "goal_1") => new KeyResult(
            Id: id, // string
            GoalId: goalId, // string
            Name: "Sample Key Result", // string
            Type: "number", // string
            Unit: "items", // string?
            CreatorUser: CreateSampleUser(1), // User?
            DateCreated: DateTimeOffset.UtcNow.AddDays(-5), // DateTimeOffset
            GoalPrettyId: "P-goal_1", // string?
            PercentCompleted: 10, // int?
            Completed: false, // bool
            TaskIds: new List<string>(), // List<string>?
            ListIds: new List<string>(), // List<string>?
            SubcategoryIds: new List<string>(), // List<string>?
            Owners: new List<User> { CreateSampleUser(1) }, // List<User>?
            LastAction: CreateSampleLastAction(id), // LastAction?
            StepsCurrent: 10,    // object?
            StepsStart: 0,       // object?
            StepsEnd: 100,       // object?
            StepsTaken: 1,       // int?
            History: new List<LastAction>(), // List<LastAction>?
            LastActionDate: DateTimeOffset.UtcNow.ToString(), // string?
            Active: true         // bool?
        );

        [Fact]
        public async Task GetGoalsAsync_ValidRequest_BuildsCorrectUrlAndReturnsResponse()
        {
            // Arrange
            var workspaceId = "ws_test";
            var expectedGoal = CreateSampleGoal(teamId: workspaceId);
            var apiResponse = new GetGoalsResponse(new List<Goal> { expectedGoal }, new List<GoalFolder>());
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGoalsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _goalsService.GetGoalsAsync(workspaceId, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGoal.Id, result.Goals.First().Id);
            _mockApiConnection.Verify(c => c.GetAsync<GetGoalsResponse>(
                $"team/{workspaceId}/goal?include_completed=true",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateGoalAsync_ValidRequest_CallsPostAndReturnsGoal()
        {
            // Arrange
            var workspaceId = "ws_test";
            var request = new CreateGoalRequest(
                Name: "New Test Goal",
                DueDate: DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeMilliseconds(),
                Description: "A goal for testing",
                MultipleOwners: false,
                Owners: new List<int>{123},
                Color: "#FF0000",
                TeamId: workspaceId, // Corrected: Was WorkspaceId, DTO uses TeamId
                FolderId: null
            );
            var expectedGoal = CreateSampleGoal("new_goal_1", teamId: workspaceId);
            var apiResponse = new GetGoalResponse(expectedGoal);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateGoalRequest, GetGoalResponse>(
                    It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _goalsService.CreateGoalAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGoal.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<CreateGoalRequest, GetGoalResponse>(
                $"team/{workspaceId}/goal",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetGoalAsync_ApiError_ThrowsHttpRequestException()
        {
            // Arrange
            var goalId = "goal_error";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGoalResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API Down"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _goalsService.GetGoalAsync(goalId));
        }

        [Fact]
        public async Task CreateKeyResultAsync_NullResponseData_ThrowsInvalidOperationException()
        {
            // Arrange
            var goalId = "goal_1";
            var request = new CreateKeyResultRequest(
                Name: "New KR",
                Owners: new List<int>{123}, // CreateKeyResultRequest expects List<int> for Owners
                Type: "number",
                StepsStart: 0,
                StepsEnd: 100,
                Unit: "items",
                TaskIds: new List<string>(),
                ListIds: new List<string>(),
                GoalId: goalId // Add GoalId as it's a required parameter
            );
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateKeyResultRequest, CreateKeyResultResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateKeyResultResponse(null!)); // Simulate null KeyResult in response

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _goalsService.CreateKeyResultAsync(goalId, request));
        }
    }
}
