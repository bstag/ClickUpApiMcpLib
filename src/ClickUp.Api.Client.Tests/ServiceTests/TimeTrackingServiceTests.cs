using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.TimeTracking; // For TimeEntry, TaskTag, TimeEntryHistory
using ClickUp.Api.Client.Models.Entities.Users; // For User (if part of TimeEntry)
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask (if part of TimeEntry)
using ClickUp.Api.Client.Models.RequestModels.TimeTracking; // For request DTOs
using ClickUp.Api.Client.Models.ResponseModels.TimeTracking; // For response DTOs
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json; // Added for JsonDocument and JsonElement
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class TimeTrackingServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TimeTrackingService _timeTrackingService;
        private readonly Mock<ILogger<TimeTrackingService>> _mockLogger;

        public TimeTrackingServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<TimeTrackingService>>();
            _timeTrackingService = new TimeTrackingService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private ClickUp.Api.Client.Models.Entities.Users.User CreateSampleUser(long id = 1, string username = "Time User")
        {
            return new ClickUp.Api.Client.Models.Entities.Users.User((int)id, username, $"{username.Replace(" ", "")}@example.com", "#123", null, "TU");
        }

        private ClickUp.Api.Client.Models.Entities.TimeTracking.TimeEntryTaskReference CreateSampleTimeEntryTaskReference(string id = "task_tt_1", string name = "Timing Task")
        {
             return new ClickUp.Api.Client.Models.Entities.TimeTracking.TimeEntryTaskReference(
                Id: id,
                Name: name,
                CustomId: null,
                Status: null, // This would be Models.Common.Status
                Url: $"https://app.clickup.com/t/{id}"
            );
        }

        private TimeEntry CreateSampleTimeEntry(string id = "te_1", string description = "Dev work")
        {
            var user = CreateSampleUser();
            var taskRef = CreateSampleTimeEntryTaskReference(); // Changed to use TimeEntryTaskReference
            return new TimeEntry(
                Id: id,
                Task: taskRef, // Use TimeEntryTaskReference
                Wid: "ws_sample", // Added WorkspaceId (wid)
                User: user,
                Billable: false,
                Start: DateTimeOffset.UtcNow.AddHours(-2).ToUnixTimeMilliseconds().ToString(), // Changed to string
                End: DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeMilliseconds().ToString(), // Changed to string
                Duration: 3600000,
                Description: description,
                Tags: new List<TaskTag>(),
                Source: "clickup",
                At: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), // Changed to string
                TaskLocationInfo: new TaskLocation(ListId: "list_id", ListName: "list_name", FolderId: "folder_id", FolderName: "folder_name", SpaceId: "space_id", SpaceName: "space_name", ListHidden: null, FolderHidden: null, SpaceHidden: null),
                TaskTags: new List<TaskTag>(),
                TaskUrl: $"https://app.clickup.com/t/{taskRef.Id}",
                IsLocked: false, // Added IsLocked
                LockedDetails: null // Added LockedDetails
            );
        }

        // --- Tests for GetTimeEntriesAsync ---

        [Fact]
        public async Task GetTimeEntriesAsync_ValidRequest_ReturnsTimeEntries()
        {
            // Arrange
            var workspaceId = "ws123";
            var request = new GetTimeEntriesRequest(); // Empty request for default params
            var expectedEntries = new List<TimeEntry> { CreateSampleTimeEntry("te1", "Entry 1") };
            var apiResponse = new GetTimeEntriesResponse(expectedEntries, expectedEntries.Count, null);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntriesResponse>(
                    It.Is<string>(s => s.StartsWith($"team/{workspaceId}/time_entries")),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.GetTimeEntriesAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("te1", result.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntriesResponse>(
                $"team/{workspaceId}/time_entries", // Default URL with no query params from empty request
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTimeEntriesAsync_WithAllRequestParameters_BuildsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws_all_params";
            long startDateUnix = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds();
            long endDateUnix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var request = new GetTimeEntriesRequest
            {
                StartDate = DateTimeOffset.FromUnixTimeMilliseconds(startDateUnix),
                EndDate = DateTimeOffset.FromUnixTimeMilliseconds(endDateUnix),
                Assignee = "123,456",
                IncludeTaskTags = true,
                IncludeLocationNames = false,
                SpaceId = "space_x",
                FolderId = "folder_y",
                ListId = "list_z",
                TaskId = "task_abc"
            };
            var apiResponse = new GetTimeEntriesResponse(new List<TimeEntry>(), 0, null);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.GetTimeEntriesAsync(workspaceId, request);

            // Assert
            // Use the original long values for assertion as the service converts DateTimeOffset back to Unix ms for the URL
            var expectedUrl = $"team/{workspaceId}/time_entries?start_date={startDateUnix}&end_date={endDateUnix}&assignee=123%2c456&include_task_tags=true&include_location_names=false&space_id=space_x&folder_id=folder_y&list_id=list_z&task_id=task_abc";
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntriesResponse>(
                expectedUrl,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTimeEntriesAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_te_null_api_resp";
            var request = new GetTimeEntriesRequest();
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTimeEntriesResponse)null);

            // Act
            var result = await _timeTrackingService.GetTimeEntriesAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTimeEntriesAsync_ApiReturnsResponseWithNullData_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_te_null_data_in_resp";
            var request = new GetTimeEntriesRequest();
            // For a response where Data is null, the GetTimeEntriesResponse constructor still needs a List<TimeEntry>
            // So we pass null!, but the DTO itself might initialize Data to an empty list if it's non-nullable.
            // The service checks for response.Data == null.
            var apiResponse = new GetTimeEntriesResponse(null!, 0, null);
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.GetTimeEntriesAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTimeEntriesAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_te_http_ex";
            var request = new GetTimeEntriesRequest();
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.GetTimeEntriesAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task GetTimeEntriesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_te_cancel_ex";
            var request = new GetTimeEntriesRequest();
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.GetTimeEntriesAsync(workspaceId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetTimeEntriesAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_te_ct_pass";
            var request = new GetTimeEntriesRequest();
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetTimeEntriesResponse(new List<TimeEntry>(), 0, null));

            // Act
            await _timeTrackingService.GetTimeEntriesAsync(workspaceId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntriesResponse>(
                $"team/{workspaceId}/time_entries",
                expectedToken), Times.Once);
        }

        // --- Tests for CreateTimeEntryAsync ---

        [Fact]
        public async Task CreateTimeEntryAsync_ValidRequest_ReturnsTimeEntry()
        {
            // Arrange
            var workspaceId = "ws_create_te";
            var request = new CreateTimeEntryRequest(
                Description: "New Time Entry",
                Tags: null,
                Start: DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeMilliseconds(),
                Duration: 1800000, // 30 minutes
                Billable: null,
                Assignee: null,
                TaskId: "task_for_te",
                WorkspaceId: null
            );
            var expectedEntry = CreateSampleTimeEntry("te_new", "New Time Entry");
            var apiResponse = new GetTimeEntryResponse { Data = expectedEntry }; // Corrected GetTimeEntryResponse instantiation

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(
                    It.Is<string>(s => s.StartsWith($"team/{workspaceId}/time_entries")),
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.CreateTimeEntryAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntry.Id, result.Id);
            _mockApiConnection.Verify(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries", // Default URL without query params
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateTimeEntryAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws_create_te_custom";
            var request = new CreateTimeEntryRequest(null, null, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 900000, true, null, "custom_task", null);
            var customTaskIds = true;
            var teamIdForCustomTaskIds = "team_for_custom";
            var expectedEntry = CreateSampleTimeEntry("te_custom_id");
            var apiResponse = new GetTimeEntryResponse { Data = expectedEntry };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.CreateTimeEntryAsync(workspaceId, request, customTaskIds, teamIdForCustomTaskIds);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries?custom_task_ids=true&team_id={teamIdForCustomTaskIds}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateTimeEntryAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_create_te_null_api";
            var request = new CreateTimeEntryRequest(null, null, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 0, null, null, "t", null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTimeEntryResponse)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.CreateTimeEntryAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task CreateTimeEntryAsync_ApiReturnsResponseWithNullData_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_create_te_null_data";
            var request = new CreateTimeEntryRequest(null, null, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 0, null, null, "t", null);
            var apiResponse = new GetTimeEntryResponse { Data = null! }; // Data property is null
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.CreateTimeEntryAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task CreateTimeEntryAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_create_te_http_ex";
            var request = new CreateTimeEntryRequest(null, null, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 0, null, null, "t", null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.CreateTimeEntryAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task CreateTimeEntryAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_create_te_cancel_ex";
            var request = new CreateTimeEntryRequest(null, null, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 0, null, null, "t", null);
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.CreateTimeEntryAsync(workspaceId, request, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task CreateTimeEntryAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_create_te_ct_pass";
            var request = new CreateTimeEntryRequest(null, null, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 0, null, null, "t", null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedEntry = CreateSampleTimeEntry("te_ct_create");
            var apiResponse = new GetTimeEntryResponse { Data = expectedEntry };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.CreateTimeEntryAsync(workspaceId, request, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for GetTimeEntryAsync ---

        [Fact]
        public async Task GetTimeEntryAsync_ValidRequest_ReturnsTimeEntry()
        {
            // Arrange
            var workspaceId = "ws_get_te";
            var timerId = "te_get_1";
            var expectedEntry = CreateSampleTimeEntry(timerId);
            var apiResponse = new GetTimeEntryResponse { Data = expectedEntry };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(
                    $"team/{workspaceId}/time_entries/{timerId}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(timerId, result.Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/{timerId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTimeEntryAsync_WithAllIncludeParams_BuildsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws_get_te_params";
            var timerId = "te_get_params";
            var apiResponse = new GetTimeEntryResponse { Data = CreateSampleTimeEntry(timerId) };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId, includeTaskTags: true, includeLocationNames: false); // Only testing a subset of includes for brevity

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/{timerId}?include_task_tags=true&include_location_names=false",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTimeEntryAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_get_te_null_api";
            var timerId = "te_get_null_api";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTimeEntryResponse)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId)
            );
        }

        [Fact]
        public async Task GetTimeEntryAsync_ApiReturnsResponseWithNullData_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_get_te_null_data";
            var timerId = "te_get_null_data";
            var apiResponse = new GetTimeEntryResponse { Data = null! };
             _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId)
            );
        }

        [Fact]
        public async Task GetTimeEntryAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_get_te_http_ex";
            var timerId = "te_get_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId)
            );
        }

        [Fact]
        public async Task GetTimeEntryAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_get_te_cancel_ex";
            var timerId = "te_get_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetTimeEntryAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_get_te_ct_pass";
            var timerId = "te_get_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetTimeEntryResponse { Data = CreateSampleTimeEntry(timerId) };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/{timerId}",
                expectedToken), Times.Once);
        }

        // --- Tests for UpdateTimeEntryAsync ---

        [Fact]
        public async Task UpdateTimeEntryAsync_ValidRequest_ReturnsUpdatedTimeEntry()
        {
            // Arrange
            var workspaceId = "ws_update_te";
            var timerId = "te_update_1";
            var request = new UpdateTimeEntryRequest(
                Description: "Updated TE Description",
                Tags: null,
                TagAction: null,
                Start: null,
                End: null,
                TaskId: null,
                Billable: true,
                Duration: null,
                Assignee: null,
                IsLocked: null
            );
            var expectedEntry = CreateSampleTimeEntry(timerId, "Updated TE Description") with { Billable = true };
            var apiResponse = new GetTimeEntryResponse { Data = expectedEntry };

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(
                    It.Is<string>(s => s.StartsWith($"team/{workspaceId}/time_entries/{timerId}")),
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.UpdateTimeEntryAsync(workspaceId, timerId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntry.Description, result.Description);
            Assert.True(result.Billable);
            _mockApiConnection.Verify(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/{timerId}", // Default URL
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTimeEntryAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws_update_te_custom";
            var timerId = "te_update_custom";
            var request = new UpdateTimeEntryRequest(null, null, null, null, null, null, null, 60000, null, null);
            var customTaskIds = false;
            var teamIdForCustomTaskIds = "team_for_update";
            var expectedEntry = CreateSampleTimeEntry(timerId) with { Duration = 60000 };
            var apiResponse = new GetTimeEntryResponse { Data = expectedEntry };

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.UpdateTimeEntryAsync(workspaceId, timerId, request, customTaskIds, teamIdForCustomTaskIds);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/{timerId}?custom_task_ids=false&team_id={teamIdForCustomTaskIds}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTimeEntryAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_update_te_null_api";
            var timerId = "te_update_null_api";
            var request = new UpdateTimeEntryRequest(null, null, null, null, null, null, null, null, null, null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTimeEntryResponse)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.UpdateTimeEntryAsync(workspaceId, timerId, request)
            );
        }

        [Fact]
        public async Task UpdateTimeEntryAsync_ApiReturnsResponseWithNullData_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_update_te_null_data";
            var timerId = "te_update_null_data";
            var request = new UpdateTimeEntryRequest(null,null,null,null,null,null,null,null,null,null);
            var apiResponse = new GetTimeEntryResponse { Data = null! };
             _mockApiConnection
                .Setup(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.UpdateTimeEntryAsync(workspaceId, timerId, request)
            );
        }

        [Fact]
        public async Task UpdateTimeEntryAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_update_te_http_ex";
            var timerId = "te_update_http_ex";
            var request = new UpdateTimeEntryRequest(null,null,null,null,null,null,null,null,null,null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.UpdateTimeEntryAsync(workspaceId, timerId, request)
            );
        }

        [Fact]
        public async Task UpdateTimeEntryAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_update_te_cancel_ex";
            var timerId = "te_update_cancel_ex";
            var request = new UpdateTimeEntryRequest(null,null,null,null,null,null,null,null,null,null);
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.UpdateTimeEntryAsync(workspaceId, timerId, request, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task UpdateTimeEntryAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_update_te_ct_pass";
            var timerId = "te_update_ct_pass";
            var request = new UpdateTimeEntryRequest(null,null,null,null,null,null,null,null,null,null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetTimeEntryResponse { Data = CreateSampleTimeEntry(timerId) };

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.UpdateTimeEntryAsync(workspaceId, timerId, request, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/{timerId}",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteTimeEntryAsync ---

        [Fact]
        public async Task DeleteTimeEntryAsync_ValidRequest_CallsDeleteAsync()
        {
            // Arrange
            var workspaceId = "ws_delete_te";
            var timerId = "te_delete_1";

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    $"team/{workspaceId}/time_entries/{timerId}",
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _timeTrackingService.DeleteTimeEntryAsync(workspaceId, timerId);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"team/{workspaceId}/time_entries/{timerId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTimeEntryAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_delete_te_http_ex";
            var timerId = "te_delete_http_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.DeleteTimeEntryAsync(workspaceId, timerId)
            );
        }

        [Fact]
        public async Task DeleteTimeEntryAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_delete_te_cancel_ex";
            var timerId = "te_delete_cancel_ex";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.DeleteTimeEntryAsync(workspaceId, timerId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task DeleteTimeEntryAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_delete_te_ct_pass";
            var timerId = "te_delete_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _timeTrackingService.DeleteTimeEntryAsync(workspaceId, timerId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"team/{workspaceId}/time_entries/{timerId}",
                expectedToken), Times.Once);
        }

        // --- Tests for GetTimeEntryHistoryAsync ---
        private TimeEntryHistory CreateSampleTimeEntryHistory(string field = "status", long? beforeEpochMs = 0, long? afterEpochMs = 1)
        {
            // Simplified for testing; actual DTO might be more complex
            JsonElement? beforeJson = null;
            JsonElement? afterJson = null;

            if (beforeEpochMs.HasValue)
            {
                beforeJson = JsonDocument.Parse(beforeEpochMs.Value.ToString()).RootElement;
            }
            if (afterEpochMs.HasValue)
            {
                afterJson = JsonDocument.Parse(afterEpochMs.Value.ToString()).RootElement;
            }

            return new TimeEntryHistory
            {
                Field = field,
                Before = beforeJson,
                After = afterJson,
                User = CreateSampleUser(2, "History User"),
                Date = DateTimeOffset.UtcNow.AddMinutes(-30), // Corrected to DateTimeOffset
                Source = "api",
                Action = "update", // Example, ensure all relevant fields are set if needed by tests
                Note = null,
                Task = null,
                List = null,
                Space = null
            };
        }

        [Fact]
        public async Task GetTimeEntryHistoryAsync_ValidRequest_ReturnsHistory()
        {
            // Arrange
            var workspaceId = "ws_te_hist";
            var timerId = "te_hist_1";
            var expectedHistory = new List<TimeEntryHistory> { CreateSampleTimeEntryHistory() };
            var apiResponse = new GetTimeEntryHistoryResponse { History = expectedHistory };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryHistoryResponse>(
                    $"team/{workspaceId}/time_entries/{timerId}/history",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.GetTimeEntryHistoryAsync(workspaceId, timerId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("status", result.First().Field);
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntryHistoryResponse>(
                $"team/{workspaceId}/time_entries/{timerId}/history",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTimeEntryHistoryAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_te_hist_null_api";
            var timerId = "te_hist_null_api";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryHistoryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTimeEntryHistoryResponse)null);

            // Act
            var result = await _timeTrackingService.GetTimeEntryHistoryAsync(workspaceId, timerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTimeEntryHistoryAsync_ApiReturnsResponseWithNullHistory_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_te_hist_null_data";
            var timerId = "te_hist_null_data";
            var apiResponse = new GetTimeEntryHistoryResponse { History = null! }; // History property is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryHistoryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.GetTimeEntryHistoryAsync(workspaceId, timerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTimeEntryHistoryAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_te_hist_http_ex";
            var timerId = "te_hist_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryHistoryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.GetTimeEntryHistoryAsync(workspaceId, timerId)
            );
        }

        [Fact]
        public async Task GetTimeEntryHistoryAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_te_hist_cancel_ex";
            var timerId = "te_hist_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryHistoryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.GetTimeEntryHistoryAsync(workspaceId, timerId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetTimeEntryHistoryAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_te_hist_ct_pass";
            var timerId = "te_hist_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetTimeEntryHistoryResponse { History = new List<TimeEntryHistory>() };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryHistoryResponse>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.GetTimeEntryHistoryAsync(workspaceId, timerId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntryHistoryResponse>(
                $"team/{workspaceId}/time_entries/{timerId}/history",
                expectedToken), Times.Once);
        }

        // --- Tests for GetRunningTimeEntryAsync ---

        [Fact]
        public async Task GetRunningTimeEntryAsync_WhenTimerIsRunning_ReturnsTimeEntry()
        {
            // Arrange
            var workspaceId = "ws_running_te";
            var runningEntry = CreateSampleTimeEntry("te_running", "Currently Active Timer");
            var apiResponse = new GetTimeEntryResponse { Data = runningEntry }; // API returns {"data": TimeEntry} or {"data": null}

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(
                    $"team/{workspaceId}/time_entries/current", // Default URL without assignee
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.GetRunningTimeEntryAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(runningEntry.Id, result.Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/current",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRunningTimeEntryAsync_WithAssigneeUserId_BuildsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws_running_te_assignee";
            var assigneeUserId = "user_789";
            var runningEntry = CreateSampleTimeEntry("te_running_assignee");
            var apiResponse = new GetTimeEntryResponse { Data = runningEntry };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.GetRunningTimeEntryAsync(workspaceId, assigneeUserId);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/current?assignee_user_id={assigneeUserId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRunningTimeEntryAsync_WhenNoTimerIsRunning_ReturnsNull()
        {
            // Arrange
            var workspaceId = "ws_no_running_te";
            // API returns {"data": null} when no timer is running for the user
            var apiResponse = new GetTimeEntryResponse { Data = null! };
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.GetRunningTimeEntryAsync(workspaceId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetRunningTimeEntryAsync_ApiReturnsNullResponseObject_ReturnsNull()
        {
            // Arrange
            var workspaceId = "ws_running_te_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTimeEntryResponse)null); // Entire response object is null

            // Act
            var result = await _timeTrackingService.GetRunningTimeEntryAsync(workspaceId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetRunningTimeEntryAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_running_te_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.GetRunningTimeEntryAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetRunningTimeEntryAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_running_te_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.GetRunningTimeEntryAsync(workspaceId, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetRunningTimeEntryAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_running_te_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetTimeEntryResponse { Data = null! }; // Response can have null data

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTimeEntryResponse>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.GetRunningTimeEntryAsync(workspaceId, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/current",
                expectedToken), Times.Once);
        }

        // --- Tests for StartTimeEntryAsync ---

        [Fact]
        public async Task StartTimeEntryAsync_ValidRequest_ReturnsStartedTimeEntry()
        {
            // Arrange
            var workspaceId = "ws_start_te";
            var request = new StartTimeEntryRequest( // Corrected constructor
                Description: "Starting Timer",
                Tags: null,
                TaskId: "task_to_start_timer_on",
                Billable: null,
                WorkspaceId: workspaceId, // Often specified or can be null
                ProjectId_Legacy: null,
                CreatedWith: null
            );
            var startedEntry = CreateSampleTimeEntry("te_started", "Starting Timer");
            var apiResponse = new GetTimeEntryResponse { Data = startedEntry };

            _mockApiConnection
                .Setup(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(
                    It.Is<string>(s => s.StartsWith($"team/{workspaceId}/time_entries/start")),
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.StartTimeEntryAsync(workspaceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(startedEntry.Id, result.Id);
            _mockApiConnection.Verify(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/start", // Default URL
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StartTimeEntryAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectUrl()
        {
            // Arrange
            var workspaceId = "ws_start_te_custom";
            var request = new StartTimeEntryRequest(null, null, "custom_task_start", null, workspaceId, null, null);
            var customTaskIds = true;
            var teamIdForCustomTaskIds = "team_start_custom";
            var startedEntry = CreateSampleTimeEntry("te_start_custom_id");
            var apiResponse = new GetTimeEntryResponse { Data = startedEntry };

            _mockApiConnection
                .Setup(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.StartTimeEntryAsync(workspaceId, request, customTaskIds, teamIdForCustomTaskIds);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/start?custom_task_ids=true&team_id={teamIdForCustomTaskIds}",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StartTimeEntryAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_start_te_null_api";
            var request = new StartTimeEntryRequest(null, null, "t", null, workspaceId, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTimeEntryResponse)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.StartTimeEntryAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task StartTimeEntryAsync_ApiReturnsResponseWithNullData_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_start_te_null_data";
            var request = new StartTimeEntryRequest(null, null, "t", null, workspaceId, null, null);
            var apiResponse = new GetTimeEntryResponse { Data = null! };
             _mockApiConnection
                .Setup(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.StartTimeEntryAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task StartTimeEntryAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_start_te_http_ex";
            var request = new StartTimeEntryRequest(null, null, "t", null, workspaceId, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.StartTimeEntryAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task StartTimeEntryAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_start_te_cancel_ex";
            var request = new StartTimeEntryRequest(null, null, "t", null, workspaceId, null, null);
            _mockApiConnection
                .Setup(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.StartTimeEntryAsync(workspaceId, request, cancellationToken: new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task StartTimeEntryAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_start_te_ct_pass";
            var request = new StartTimeEntryRequest(null, null, "t", null, workspaceId, null, null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetTimeEntryResponse { Data = CreateSampleTimeEntry("te_start_ct") };

            _mockApiConnection
                .Setup(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.StartTimeEntryAsync(workspaceId, request, cancellationToken: expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/start",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for StopTimeEntryAsync ---

        [Fact]
        public async Task StopTimeEntryAsync_ValidRequest_ReturnsStoppedTimeEntry()
        {
            // Arrange
            var workspaceId = "ws_stop_te";
            var stoppedEntry = CreateSampleTimeEntry("te_stopped", "Timer Stopped");
            // API for stop time entry returns the entry that was active (now stopped)
            var apiResponse = new GetTimeEntryResponse { Data = stoppedEntry };

            _mockApiConnection
                .Setup(x => x.PostAsync<object, GetTimeEntryResponse>( // Takes object as TBody because no actual body is sent
                    $"team/{workspaceId}/time_entries/stop",
                    It.IsAny<object>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.StopTimeEntryAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(stoppedEntry.Id, result.Id);
            _mockApiConnection.Verify(x => x.PostAsync<object, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/stop",
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StopTimeEntryAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_stop_te_null_api";
            _mockApiConnection
                .Setup(x => x.PostAsync<object, GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTimeEntryResponse)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.StopTimeEntryAsync(workspaceId)
            );
        }

        [Fact]
        public async Task StopTimeEntryAsync_ApiReturnsResponseWithNullData_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_stop_te_null_data";
            var apiResponse = new GetTimeEntryResponse { Data = null! };
             _mockApiConnection
                .Setup(x => x.PostAsync<object, GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _timeTrackingService.StopTimeEntryAsync(workspaceId)
            );
        }

        [Fact]
        public async Task StopTimeEntryAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_stop_te_http_ex";
            _mockApiConnection
                .Setup(x => x.PostAsync<object, GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.StopTimeEntryAsync(workspaceId)
            );
        }

        [Fact]
        public async Task StopTimeEntryAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_stop_te_cancel_ex";
            _mockApiConnection
                .Setup(x => x.PostAsync<object, GetTimeEntryResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.StopTimeEntryAsync(workspaceId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task StopTimeEntryAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_stop_te_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetTimeEntryResponse { Data = CreateSampleTimeEntry("te_stop_ct") }; // Instantiation was correct here already

            _mockApiConnection
                .Setup(x => x.PostAsync<object, GetTimeEntryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.StopTimeEntryAsync(workspaceId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync<object, GetTimeEntryResponse>(
                $"team/{workspaceId}/time_entries/stop",
                It.IsAny<object>(),
                expectedToken), Times.Once);
        }

        // --- Tests for GetAllTimeEntryTagsAsync ---
        private TaskTag CreateSampleTaskTag(string name = "Billing", string tagFg = "#FF0000", string tagBg = "#0000FF", int? creator = 1) // Added creator
        {
            return new TaskTag(Name: name, TagFg: tagFg, TagBg: tagBg, Creator: creator); // Added creator
        }

        [Fact]
        public async Task GetAllTimeEntryTagsAsync_ValidRequest_ReturnsTags()
        {
            // Arrange
            var workspaceId = "ws_get_all_tags";
            var expectedTags = new List<TaskTag> { CreateSampleTaskTag("Urgent"), CreateSampleTaskTag("Internal") };
            var apiResponse = new GetAllTimeEntryTagsResponse { Data = expectedTags };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetAllTimeEntryTagsResponse>(
                    $"team/{workspaceId}/time_entries/tags",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.GetAllTimeEntryTagsAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Name == "Urgent");
            _mockApiConnection.Verify(x => x.GetAsync<GetAllTimeEntryTagsResponse>(
                $"team/{workspaceId}/time_entries/tags",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllTimeEntryTagsAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_get_tags_null_api";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAllTimeEntryTagsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAllTimeEntryTagsResponse)null);

            // Act
            var result = await _timeTrackingService.GetAllTimeEntryTagsAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllTimeEntryTagsAsync_ApiReturnsResponseWithNullData_ReturnsEmptyEnumerable()
        {
            // Arrange
            var workspaceId = "ws_get_tags_null_data";
            var apiResponse = new GetAllTimeEntryTagsResponse { Data = null! }; // Data is null
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAllTimeEntryTagsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _timeTrackingService.GetAllTimeEntryTagsAsync(workspaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllTimeEntryTagsAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_get_tags_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAllTimeEntryTagsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.GetAllTimeEntryTagsAsync(workspaceId)
            );
        }

        [Fact]
        public async Task GetAllTimeEntryTagsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_get_tags_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetAllTimeEntryTagsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.GetAllTimeEntryTagsAsync(workspaceId, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetAllTimeEntryTagsAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_get_tags_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new GetAllTimeEntryTagsResponse { Data = new List<TaskTag>() };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetAllTimeEntryTagsResponse>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _timeTrackingService.GetAllTimeEntryTagsAsync(workspaceId, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetAllTimeEntryTagsResponse>(
                $"team/{workspaceId}/time_entries/tags",
                expectedToken), Times.Once);
        }

        // --- Tests for AddTagsToTimeEntriesAsync ---

        [Fact]
        public async Task AddTagsToTimeEntriesAsync_ValidRequest_CallsPostAsync()
        {
            // Arrange
            var workspaceId = "ws_add_tags_te";
            var tagDefs = new List<TimeTrackingTagDefinition> { new TimeTrackingTagDefinition("Client Work", "#FF0000", "#FFFFFF") };
            var request = new AddTagsFromTimeEntriesRequest(
                TimeEntryIds: new List<string> { "te_1", "te_2" },
                Tags: tagDefs
            );

            _mockApiConnection
                .Setup(x => x.PostAsync( // Non-generic PostAsync as service method is void
                    $"team/{workspaceId}/time_entries/tags",
                    request,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _timeTrackingService.AddTagsToTimeEntriesAsync(workspaceId, request);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync(
                $"team/{workspaceId}/time_entries/tags",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTagsToTimeEntriesAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_add_tags_http_ex";
            var request = new AddTagsFromTimeEntriesRequest(new List<string>(), new List<TimeTrackingTagDefinition>()); // Already correct
            _mockApiConnection
                .Setup(x => x.PostAsync(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.AddTagsToTimeEntriesAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task AddTagsToTimeEntriesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_add_tags_cancel_ex";
            var request = new AddTagsFromTimeEntriesRequest(new List<string>(), new List<TimeTrackingTagDefinition>());
            _mockApiConnection
                .Setup(x => x.PostAsync(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.AddTagsToTimeEntriesAsync(workspaceId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task AddTagsToTimeEntriesAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_add_tags_ct_pass";
            var request = new AddTagsFromTimeEntriesRequest(new List<string>(), new List<TimeTrackingTagDefinition>());
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.PostAsync(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _timeTrackingService.AddTagsToTimeEntriesAsync(workspaceId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PostAsync(
                $"team/{workspaceId}/time_entries/tags",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for RemoveTagsFromTimeEntriesAsync ---

        [Fact]
        public async Task RemoveTagsFromTimeEntriesAsync_ValidRequest_CallsDeleteAsyncWithBody()
        {
            // Arrange
            var workspaceId = "ws_remove_tags_te";
            var request = new RemoveTagsFromTimeEntriesRequest(
                TimeEntryIds: new List<string> { "te_3", "te_4" },
                Tags: new List<TimeTrackingTagDefinition> { new TimeTrackingTagDefinition("Old Tag", null, null) }
            );

            // This setup assumes IApiConnection has an overload for DeleteAsync that takes a body.
            // If it doesn't, this test will need to be adjusted or the IApiConnection interface updated.
            _mockApiConnection
                .Setup(x => x.DeleteAsync( // Assuming DeleteAsync<TRequest> or similar that takes a body
                    $"team/{workspaceId}/time_entries/tags",
                    request,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _timeTrackingService.RemoveTagsFromTimeEntriesAsync(workspaceId, request);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"team/{workspaceId}/time_entries/tags",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveTagsFromTimeEntriesAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_remove_tags_http_ex";
            var request = new RemoveTagsFromTimeEntriesRequest(new List<string>(), new List<TimeTrackingTagDefinition>());
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.RemoveTagsFromTimeEntriesAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task RemoveTagsFromTimeEntriesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_remove_tags_cancel_ex";
            var request = new RemoveTagsFromTimeEntriesRequest(new List<string>(), new List<TimeTrackingTagDefinition>());
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.RemoveTagsFromTimeEntriesAsync(workspaceId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task RemoveTagsFromTimeEntriesAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_remove_tags_ct_pass";
            var request = new RemoveTagsFromTimeEntriesRequest(new List<string>(), new List<TimeTrackingTagDefinition>());
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.DeleteAsync(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _timeTrackingService.RemoveTagsFromTimeEntriesAsync(workspaceId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"team/{workspaceId}/time_entries/tags",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for ChangeTimeEntryTagNameAsync ---
        // Note: The service method uses UpdateTimeEntryRequest for this, which might be a naming mismatch
        // if the intention was a more specific "ChangeTagNameRequest". Tests will follow the current signature.

        [Fact]
        public async Task ChangeTimeEntryTagNameAsync_ValidRequest_CallsPutAsync()
        {
            // Arrange
            var workspaceId = "ws_change_tag_name";
            // UpdateTimeEntryRequest is used by the service method for this operation.
            // It doesn't have specific fields for "old name" and "new name" for a tag,
            // which is unusual for a "change tag name" operation.
            // The API endpoint PUT /team/{team_id}/time_entries/tags is for "Bulk update tags".
            // It expects a body like: { "name": "New Name", "new_name": "Old Name", "tag_bg": "#000000", "tag_fg": "#FFFFFF" }
            // This means UpdateTimeEntryRequest DTO is likely not the correct DTO for this specific service method's intent.
            // However, I will test based on the current implementation.
            // If UpdateTimeEntryRequest is indeed used, it would send its properties.
            var request = new UpdateTimeEntryRequest(
                Description: "This request is for changing tag names but uses UpdateTimeEntryRequest",
                Tags: null, TagAction: null, Start: null, End: null, TaskId: null, Billable: null, Duration: null, Assignee: null, IsLocked: null
            );

            _mockApiConnection
                .Setup(x => x.PutAsync( // Non-generic PutAsync as service method is void
                    $"team/{workspaceId}/time_entries/tags",
                    request, // Current DTO used by service
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _timeTrackingService.ChangeTimeEntryTagNameAsync(workspaceId, request);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync(
                $"team/{workspaceId}/time_entries/tags",
                request,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ChangeTimeEntryTagNameAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_change_tag_http_ex";
            var request = new UpdateTimeEntryRequest(null,null,null,null,null,null,null,null,null,null);
            _mockApiConnection
                .Setup(x => x.PutAsync(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _timeTrackingService.ChangeTimeEntryTagNameAsync(workspaceId, request)
            );
        }

        [Fact]
        public async Task ChangeTimeEntryTagNameAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_change_tag_cancel_ex";
            var request = new UpdateTimeEntryRequest(null,null,null,null,null,null,null,null,null,null);
            _mockApiConnection
                .Setup(x => x.PutAsync(It.IsAny<string>(), request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _timeTrackingService.ChangeTimeEntryTagNameAsync(workspaceId, request, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task ChangeTimeEntryTagNameAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_change_tag_ct_pass";
            var request = new UpdateTimeEntryRequest(null,null,null,null,null,null,null,null,null,null);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;

            _mockApiConnection
                .Setup(x => x.PutAsync(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .Returns(Task.CompletedTask);

            // Act
            await _timeTrackingService.ChangeTimeEntryTagNameAsync(workspaceId, request, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.PutAsync(
                $"team/{workspaceId}/time_entries/tags",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for GetTimeEntriesAsyncEnumerableAsync ---
        private List<TimeEntry> CreatePagedTimeEntriesInstance(int count, int pageNum) // Made instance method
        {
            var entries = new List<TimeEntry>();
            for (int i = 0; i < count; i++)
            {
                entries.Add(this.CreateSampleTimeEntry($"te_p{pageNum}_{i}", $"Paged Entry P{pageNum} I{i}")); // Called with this.
            }
            return entries;
        }

        [Fact]
        public async Task GetTimeEntriesAsyncEnumerableAsync_ReturnsAllEntries_WhenMultiplePages()
        {
            // Arrange
            var workspaceId = "ws_te_enum_multi";
            var request = new GetTimeEntriesRequest(); // Base request without page
            var firstPageEntries = CreatePagedTimeEntriesInstance(2, 0); // Using instance method
            var secondPageEntries = CreatePagedTimeEntriesInstance(1, 1); // Using instance method

            _mockApiConnection.SetupSequence(api => api.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTimeEntriesResponse(firstPageEntries, firstPageEntries.Count, null)) // Page 0
                .ReturnsAsync(new GetTimeEntriesResponse(secondPageEntries, secondPageEntries.Count, null)) // Page 1
                .ReturnsAsync(new GetTimeEntriesResponse(new List<TimeEntry>(), 0, null));  // Page 2 (empty, end)

            var allEntries = new List<TimeEntry>();

            // Act
            await foreach (var entry in _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(workspaceId, request, CancellationToken.None))
            {
                allEntries.Add(entry);
            }

            // Assert
            Assert.Equal(3, allEntries.Count);
            Assert.Contains(allEntries, e => e.Id == "te_p0_0");
            Assert.Contains(allEntries, e => e.Id == "te_p0_1");
            Assert.Contains(allEntries, e => e.Id == "te_p1_0");

            _mockApiConnection.Verify(api => api.GetAsync<GetTimeEntriesResponse>(
                It.Is<string>(s => s.Contains($"team/{workspaceId}/time_entries") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(api => api.GetAsync<GetTimeEntriesResponse>(
                It.Is<string>(s => s.Contains($"team/{workspaceId}/time_entries") && s.Contains("page=1")),
                It.IsAny<CancellationToken>()), Times.Once);
            _mockApiConnection.Verify(api => api.GetAsync<GetTimeEntriesResponse>(
                It.Is<string>(s => s.Contains($"team/{workspaceId}/time_entries") && s.Contains("page=2")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTimeEntriesAsyncEnumerableAsync_WithRequestParams_BuildsCorrectUrl()
        {
            var workspaceId = "ws_te_enum_params";
            long startDateUnix = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeMilliseconds();
            // Correctly assign DateTimeOffset to the request DTO
            var request = new GetTimeEntriesRequest { StartDate = DateTimeOffset.FromUnixTimeMilliseconds(startDateUnix), Assignee = "user1" };

            _mockApiConnection.Setup(api => api.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTimeEntriesResponse(new List<TimeEntry>(), 0, null)); // Empty to terminate

            await foreach (var _ in _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(workspaceId, request, CancellationToken.None)) { }

            // Use the original long value for assertion
            _mockApiConnection.Verify(api => api.GetAsync<GetTimeEntriesResponse>(
                It.Is<string>(s => s.Contains($"team/{workspaceId}/time_entries") && s.Contains($"start_date={startDateUnix}") && s.Contains("assignee=user1") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTimeEntriesAsyncEnumerableAsync_ReturnsEmpty_WhenNoEntries()
        {
            // Arrange
            var workspaceId = "ws_te_enum_empty";
            var request = new GetTimeEntriesRequest();
            _mockApiConnection.Setup(api => api.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTimeEntriesResponse(new List<TimeEntry>(), 0, null));

            var count = 0;

            // Act
            await foreach (var _ in _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(workspaceId, request, CancellationToken.None))
            {
                count++;
            }

            // Assert
            Assert.Equal(0, count);
            _mockApiConnection.Verify(api => api.GetAsync<GetTimeEntriesResponse>(
                It.Is<string>(s => s.Contains($"team/{workspaceId}/time_entries") && s.Contains("page=0")),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTimeEntriesAsyncEnumerableAsync_HandlesCancellation()
        {
            // Arrange
            var workspaceId = "ws_te_enum_cancel";
            var request = new GetTimeEntriesRequest();
            var firstPageEntries = this.CreatePagedTimeEntriesInstance(2, 0); // Corrected to use instance method
            var cts = new CancellationTokenSource();

            _mockApiConnection.Setup(api => api.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTimeEntriesResponse(firstPageEntries, firstPageEntries.Count, null));

            var entriesProcessed = 0;

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var entry in _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(workspaceId, request, cts.Token))
                {
                    entriesProcessed++;
                    if (entriesProcessed == 1) cts.Cancel();
                }
            });

            Assert.Equal(1, entriesProcessed);
        }

        [Fact]
        public async Task GetTimeEntriesAsyncEnumerableAsync_HandlesApiError()
        {
            // Arrange
            var workspaceId = "ws_te_enum_api_error";
            var request = new GetTimeEntriesRequest();
            _mockApiConnection.Setup(api => api.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await foreach (var _ in _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(workspaceId, request, CancellationToken.None)) { }
            });
        }

        [Fact]
        public async Task GetTimeEntriesAsyncEnumerableAsync_ApiReturnsNullResponse_StopsIteration()
        {
            var workspaceId = "ws_te_enum_null_resp";
            var request = new GetTimeEntriesRequest();
            _mockApiConnection.Setup(api => api.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTimeEntriesResponse)null);

            var count = 0;
            await foreach(var _ in _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(workspaceId, request)) { count++; }
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetTimeEntriesAsyncEnumerableAsync_ApiReturnsResponseWithNullData_StopsIteration()
        {
            var workspaceId = "ws_te_enum_null_data";
            var request = new GetTimeEntriesRequest();
            _mockApiConnection.Setup(api => api.GetAsync<GetTimeEntriesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTimeEntriesResponse(null!, null, null)); // Corrected constructor call

            var count = 0;
            await foreach(var _ in _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(workspaceId, request)) { count++; }
            Assert.Equal(0, count);
        }
    }
}
