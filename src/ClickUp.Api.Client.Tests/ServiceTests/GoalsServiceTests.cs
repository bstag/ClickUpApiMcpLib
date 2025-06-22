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

        // --- GetGoalsAsync Additional Tests ---

        [Fact]
        public async Task GetGoalsAsync_IncludeCompletedFalse_BuildsCorrectUrl()
        {
            var workspaceId = "ws_completed_false";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGoalsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetGoalsResponse(new List<Goal>(), new List<GoalFolder>()));

            await _goalsService.GetGoalsAsync(workspaceId, false);

            _mockApiConnection.Verify(c => c.GetAsync<GetGoalsResponse>(
                $"team/{workspaceId}/goal?include_completed=false",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetGoalsAsync_IncludeCompletedNull_BuildsCorrectUrl()
        {
            var workspaceId = "ws_completed_null";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGoalsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetGoalsResponse(new List<Goal>(), new List<GoalFolder>()));

            await _goalsService.GetGoalsAsync(workspaceId, null);

            _mockApiConnection.Verify(c => c.GetAsync<GetGoalsResponse>(
                $"team/{workspaceId}/goal", // No include_completed query param
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetGoalsAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_null_resp";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGoalsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetGoalsResponse)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.GetGoalsAsync(workspaceId));
        }

        [Fact]
        public async Task GetGoalsAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_err";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGoalsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _goalsService.GetGoalsAsync(workspaceId));
        }

        // --- CreateGoalAsync Additional Tests ---

        [Fact]
        public async Task CreateGoalAsync_ApiError_ThrowsHttpRequestException()
        {
            var workspaceId = "ws_create_err";
            var request = new CreateGoalRequest("Error Goal", DateTimeOffset.UtcNow.AddDays(10).ToUnixTimeMilliseconds(), "Desc", false, new List<int> { 1 }, "#FFFFFF", workspaceId, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateGoalRequest, GetGoalResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _goalsService.CreateGoalAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateGoalAsync_ApiReturnsNullGoalInResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_create_null_goal";
            var request = new CreateGoalRequest("Null Goal", DateTimeOffset.UtcNow.AddDays(10).ToUnixTimeMilliseconds(), "Desc", false, new List<int> { 1 }, "#FFFFFF", workspaceId, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateGoalRequest, GetGoalResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetGoalResponse(null!)); // Null Goal

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.CreateGoalAsync(workspaceId, request));
        }

        [Fact]
        public async Task CreateGoalAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_create_null_resp";
            var request = new CreateGoalRequest("Null Resp Goal", DateTimeOffset.UtcNow.AddDays(10).ToUnixTimeMilliseconds(), "Desc", false, new List<int> { 1 }, "#FFFFFF", workspaceId, null);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateGoalRequest, GetGoalResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetGoalResponse)null); // Null response

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.CreateGoalAsync(workspaceId, request));
        }

        // --- GetGoalAsync Additional Tests ---
        [Fact]
        public async Task GetGoalAsync_ValidId_ReturnsGoal()
        {
            var goalId = "goal_xyz";
            var expectedGoal = CreateSampleGoal(goalId);
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGoalResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetGoalResponse(expectedGoal));

            var result = await _goalsService.GetGoalAsync(goalId);

            Assert.NotNull(result);
            Assert.Equal(expectedGoal.Id, result.Id);
            _mockApiConnection.Verify(c => c.GetAsync<GetGoalResponse>(
                $"goal/{goalId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetGoalAsync_ApiReturnsNullGoalInResponse_ThrowsInvalidOperationException()
        {
            var goalId = "goal_get_null_goal";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGoalResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetGoalResponse(null!)); // Null Goal

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.GetGoalAsync(goalId));
        }

        [Fact]
        public async Task GetGoalAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            var goalId = "goal_get_null_resp";
            _mockApiConnection
                .Setup(c => c.GetAsync<GetGoalResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetGoalResponse)null); // Null response

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.GetGoalAsync(goalId));
        }


        // --- UpdateGoalAsync Tests ---
        [Fact]
        public async Task UpdateGoalAsync_ValidRequest_CallsPutAndReturnsGoal()
        {
            var goalId = "goal_update_1";
            var request = new UpdateGoalRequest(Name: "Updated Goal Name", DueDate: null, Description: null, RemoveOwners: null, AddOwners: null, Color: null, Archived: null);
            var expectedGoal = CreateSampleGoal(goalId, "Updated Goal Name");
            _mockApiConnection
                .Setup(c => c.PutAsync<UpdateGoalRequest, GetGoalResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetGoalResponse(expectedGoal));

            var result = await _goalsService.UpdateGoalAsync(goalId, request);

            Assert.NotNull(result);
            Assert.Equal(expectedGoal.Name, result.Name);
            _mockApiConnection.Verify(c => c.PutAsync<UpdateGoalRequest, GetGoalResponse>(
                $"goal/{goalId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateGoalAsync_ApiError_ThrowsHttpRequestException()
        {
            var goalId = "goal_update_err";
            var request = new UpdateGoalRequest(Name: "Error Update Goal", DueDate: null, Description: null, RemoveOwners: null, AddOwners: null, Color: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<UpdateGoalRequest, GetGoalResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _goalsService.UpdateGoalAsync(goalId, request));
        }

        [Fact]
        public async Task UpdateGoalAsync_ApiReturnsNullGoalInResponse_ThrowsInvalidOperationException()
        {
            var goalId = "goal_update_null_goal";
            var request = new UpdateGoalRequest(Name: "Null Goal Update", DueDate: null, Description: null, RemoveOwners: null, AddOwners: null, Color: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<UpdateGoalRequest, GetGoalResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetGoalResponse(null!)); // Null Goal

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.UpdateGoalAsync(goalId, request));
        }

        [Fact]
        public async Task UpdateGoalAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            var goalId = "goal_update_null_resp";
            var request = new UpdateGoalRequest(Name: "Null Resp Update Goal", DueDate: null, Description: null, RemoveOwners: null, AddOwners: null, Color: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<UpdateGoalRequest, GetGoalResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetGoalResponse)null); // Null Response

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.UpdateGoalAsync(goalId, request));
        }

        // --- DeleteGoalAsync Tests ---
        [Fact]
        public async Task DeleteGoalAsync_ValidId_CallsDelete()
        {
            var goalId = "goal_delete_1";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            await _goalsService.DeleteGoalAsync(goalId);

            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"goal/{goalId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteGoalAsync_ApiError_ThrowsHttpRequestException()
        {
            var goalId = "goal_delete_err";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _goalsService.DeleteGoalAsync(goalId));
        }

        // --- CreateKeyResultAsync Additional Tests ---
        [Fact]
        public async Task CreateKeyResultAsync_ValidRequest_CallsPostAndReturnsKeyResult()
        {
            var goalId = "goal_kr_1";
            var request = new CreateKeyResultRequest("New KR", new List<int> { 1 }, "number", 0, 100, "units", new List<string>(), new List<string>(), goalId);
            var expectedKeyResult = CreateSampleKeyResult("kr_new", goalId);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateKeyResultRequest, CreateKeyResultResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateKeyResultResponse(expectedKeyResult));

            var result = await _goalsService.CreateKeyResultAsync(goalId, request);

            Assert.NotNull(result);
            Assert.Equal(expectedKeyResult.Id, result.Id);
            _mockApiConnection.Verify(c => c.PostAsync<CreateKeyResultRequest, CreateKeyResultResponse>(
                $"goal/{goalId}/key_result",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateKeyResultAsync_ApiError_ThrowsHttpRequestException()
        {
            var goalId = "goal_kr_err";
            var request = new CreateKeyResultRequest("Error KR", new List<int> { 1 }, "number", 0, 100, "units", new List<string>(), new List<string>(), goalId);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateKeyResultRequest, CreateKeyResultResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _goalsService.CreateKeyResultAsync(goalId, request));
        }

        [Fact]
        public async Task CreateKeyResultAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            var goalId = "goal_kr_null_resp";
            var request = new CreateKeyResultRequest("Null Resp KR", new List<int> { 1 }, "number", 0, 100, "units", new List<string>(), new List<string>(), goalId);
            _mockApiConnection
                .Setup(c => c.PostAsync<CreateKeyResultRequest, CreateKeyResultResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreateKeyResultResponse)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.CreateKeyResultAsync(goalId, request));
        }


        // --- EditKeyResultAsync Tests ---
        [Fact]
        public async Task EditKeyResultAsync_ValidRequest_CallsPutAndReturnsKeyResult()
        {
            var keyResultId = "kr_edit_1";
            var request = new EditKeyResultRequest(StepsCurrent: null, Note: "Updated KR Note", Name: null, Owners: null, AddOwners: null, RemoveOwners: null, TaskIds: null, ListIds: null, Archived: null);
            var expectedKeyResult = CreateSampleKeyResult(keyResultId);
            // Ensure the expectedKeyResult's note matches the request for assertion, assuming LastAction is not null
            if (expectedKeyResult.LastAction != null)
            {
                expectedKeyResult = expectedKeyResult with { LastAction = expectedKeyResult.LastAction with { Note = "Updated KR Note" } };
            }
            else // If LastAction can be null, we might need a different approach or ensure it's initialized for this test
            {
                // For this test, let's assume LastAction should exist if we are updating its note.
                // Or, the assertion logic needs to be more robust if LastAction can truly be null on a KR we are editing.
                // For now, this highlights a potential area for DTO design or test setup refinement.
                // To make the test pass with current structure, we'd need to ensure CreateSampleKeyResult returns a non-null LastAction.
                // The existing CreateSampleKeyResult already does this by calling CreateSampleLastAction.
            }


            _mockApiConnection
                .Setup(c => c.PutAsync<EditKeyResultRequest, EditKeyResultResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EditKeyResultResponse(expectedKeyResult));

            var result = await _goalsService.EditKeyResultAsync(keyResultId, request);

            Assert.NotNull(result);
            Assert.Equal(expectedKeyResult.LastAction.Note, result.LastAction.Note);
            _mockApiConnection.Verify(c => c.PutAsync<EditKeyResultRequest, EditKeyResultResponse>(
                $"key_result/{keyResultId}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditKeyResultAsync_ApiError_ThrowsHttpRequestException()
        {
            var keyResultId = "kr_edit_err";
            var request = new EditKeyResultRequest(StepsCurrent: null, Note: "Error KR Update", Name: null, Owners: null, AddOwners: null, RemoveOwners: null, TaskIds: null, ListIds: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<EditKeyResultRequest, EditKeyResultResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _goalsService.EditKeyResultAsync(keyResultId, request));
        }

        [Fact]
        public async Task EditKeyResultAsync_ApiReturnsNullKeyResultInResponse_ThrowsInvalidOperationException()
        {
            var keyResultId = "kr_edit_null_kr";
            var request = new EditKeyResultRequest(StepsCurrent: null, Note: "Null KR Update", Name: null, Owners: null, AddOwners: null, RemoveOwners: null, TaskIds: null, ListIds: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<EditKeyResultRequest, EditKeyResultResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EditKeyResultResponse(null!)); // Null KeyResult

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.EditKeyResultAsync(keyResultId, request));
        }

        [Fact]
        public async Task EditKeyResultAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            var keyResultId = "kr_edit_null_resp";
            var request = new EditKeyResultRequest(StepsCurrent: null, Note: "Null Resp KR Update", Name: null, Owners: null, AddOwners: null, RemoveOwners: null, TaskIds: null, ListIds: null, Archived: null);
            _mockApiConnection
                .Setup(c => c.PutAsync<EditKeyResultRequest, EditKeyResultResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EditKeyResultResponse)null); // Null Response

            await Assert.ThrowsAsync<InvalidOperationException>(() => _goalsService.EditKeyResultAsync(keyResultId, request));
        }


        // --- DeleteKeyResultAsync Tests ---
        [Fact]
        public async Task DeleteKeyResultAsync_ValidId_CallsDelete()
        {
            var keyResultId = "kr_delete_1";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            await _goalsService.DeleteKeyResultAsync(keyResultId);

            _mockApiConnection.Verify(c => c.DeleteAsync(
                $"key_result/{keyResultId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteKeyResultAsync_ApiError_ThrowsHttpRequestException()
        {
            var keyResultId = "kr_delete_err";
            _mockApiConnection
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _goalsService.DeleteKeyResultAsync(keyResultId));
        }
    }
}
