using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.ResponseModels.Members;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class MembersServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly MembersService _membersService;
        private readonly Mock<ILogger<MembersService>> _mockLogger;

        public MembersServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<MembersService>>();
            _membersService = new MembersService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private User CreateSampleUser(int id = 1, string username = "Test User") // Changed id to int
        {
            return new User(
                Id: id,
                Username: username,
                Email: $"{username.Replace(" ", "").ToLower()}@example.com",
                Color: "#000000",
                ProfilePicture: null,
                Initials: username.Length >= 2 ? username.Substring(0, 2).ToUpper() : username.ToUpper(),
                Role: null,
                CustomRole: null,
                LastActive: null,
                DateJoined: null,
                DateInvited: null,
                ProfileInfo: null
            );
        }

        private Member CreateSampleMember(int userId = 1, string username = "Test Member") // Changed userId to int
        {
            var user = CreateSampleUser(userId, username);
            var member = new Member { User = user }; // Initialize using object initializer for required property
            // member.Role = "some_role_if_needed"; // Role is string? and can be set as a property
            return member;
        }

        // --- Tests for GetTaskMembersAsync ---

        [Fact]
        public async Task GetTaskMembersAsync_ValidTaskId_ReturnsMembers()
        {
            // Arrange
            var taskId = "task123";
            var expectedMembers = new List<Member> { CreateSampleMember(1, "TaskMember1") };
            var apiResponse = new GetMembersResponse(expectedMembers);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(
                    $"task/{taskId}/member",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _membersService.GetTaskMembersAsync(taskId);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(expectedMembers.First().User.Id, resultList.First().User.Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetMembersResponse>(
                $"task/{taskId}/member",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTaskMembersAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var taskId = "task_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetMembersResponse)null);

            // Act
            var result = await _membersService.GetTaskMembersAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTaskMembersAsync_ApiReturnsResponseWithNullMembers_ReturnsEmptyEnumerable()
        {
            // Arrange
            var taskId = "task_null_members_in_resp";
            var apiResponse = new GetMembersResponse(null!); // Members property is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _membersService.GetTaskMembersAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTaskMembersAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var taskId = "task_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _membersService.GetTaskMembersAsync(taskId)
            );
        }

        [Fact]
        public async Task GetTaskMembersAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var taskId = "task_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _membersService.GetTaskMembersAsync(taskId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetTaskMembersAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var taskId = "task_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetMembersResponse(new List<Member>()));

            // Act
            await _membersService.GetTaskMembersAsync(taskId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetMembersResponse>(
                $"task/{taskId}/member",
                expectedToken), Times.Once);
        }

        // --- Tests for GetListMembersAsync ---

        [Fact]
        public async Task GetListMembersAsync_ValidListId_ReturnsMembers()
        {
            // Arrange
            var listId = "list123";
            var expectedMembers = new List<Member> { CreateSampleMember(2, "ListMember1") };
            var apiResponse = new GetMembersResponse(expectedMembers);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(
                    $"list/{listId}/member",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _membersService.GetListMembersAsync(listId);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(expectedMembers.First().User.Id, resultList.First().User.Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetMembersResponse>(
                $"list/{listId}/member",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListMembersAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var listId = "list_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetMembersResponse)null);

            // Act
            var result = await _membersService.GetListMembersAsync(listId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetListMembersAsync_ApiReturnsResponseWithNullMembers_ReturnsEmptyEnumerable()
        {
            // Arrange
            var listId = "list_null_members_in_resp";
            var apiResponse = new GetMembersResponse(null!); // Members property is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _membersService.GetListMembersAsync(listId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetListMembersAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var listId = "list_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _membersService.GetListMembersAsync(listId)
            );
        }

        [Fact]
        public async Task GetListMembersAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var listId = "list_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _membersService.GetListMembersAsync(listId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetListMembersAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var listId = "list_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetMembersResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetMembersResponse(new List<Member>()));

            // Act
            await _membersService.GetListMembersAsync(listId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetMembersResponse>(
                $"list/{listId}/member",
                expectedToken), Times.Once);
        }
    }
}
