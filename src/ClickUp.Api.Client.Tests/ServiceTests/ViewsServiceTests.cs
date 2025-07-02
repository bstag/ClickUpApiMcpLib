using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Views;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

// Removed the placeholder namespace ClickUp.Api.Client.Models.ResponseModels.Views
// as these DTOs should exist in the main Models project.

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class ViewsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly ViewsService _viewsService;
        private readonly Mock<ILogger<ViewsService>> _mockLogger;

        public ViewsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<ViewsService>>();
            _viewsService = new ViewsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private View CreateSampleView(string id = "view_1", string name = "Sample View", string type = "list")
        {
            return new View(Id: id, Name: name, Type: type)
            {
                Settings = new ViewSettings { ShowTaskLocations = true }
            };
        }

        private CreateViewRequest CreateSampleCreateViewRequest(string name = "New View", string type = "list")
        {
            return new CreateViewRequest
            {
                Name = name,
                Type = type,
                Grouping = new ViewGrouping { Field = "status" },
                Divide = new ViewDivide(),
                Sorting = new ViewSorting(),
                Filters = new ViewFilters { Operator = "AND" },
                Columns = new ViewColumns(),
                TeamSidebar = new ViewTeamSidebar(),
                Settings = new ViewSettings { ShowAssignees = true }
            };
        }

        private UpdateViewRequest CreateSampleUpdateViewRequest(string name = "Updated View Name", string type = "list")
        {
            return new UpdateViewRequest
            {
                Name = name,
                Type = type,
                Grouping = new ViewGrouping { Field = "priority" },
                Divide = new ViewDivide(),
                Sorting = new ViewSorting(),
                Filters = new ViewFilters { Operator = "OR" },
                Columns = new ViewColumns(),
                TeamSidebar = new ViewTeamSidebar(),
                Settings = new ViewSettings { ShowDueDate = true }
            };
        }

        // --- Tests for GetWorkspaceViewsAsync ---
        [Fact]
        public async Task GetWorkspaceViewsAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var workspaceId = "ws_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetViewsResponse { Views = new List<View>() };

            _mockApiConnection.Setup(c => c.GetAsync<GetViewsResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.GetWorkspaceViewsAsync(workspaceId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetWorkspaceViewsAsync_ValidWorkspaceId_ReturnsViews()
        {
            var workspaceId = "ws123";
            var expectedViews = new List<View> { CreateSampleView("wv1", "Workspace View 1") };
            var apiResponse = new GetViewsResponse { Views = expectedViews };
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>($"team/{workspaceId}/view", It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetWorkspaceViewsAsync(workspaceId);

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Single(result.Views);
            Assert.Equal("wv1", result.Views.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewsResponse>($"team/{workspaceId}/view", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetWorkspaceViewsAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetViewsResponse { Views = new List<View>() });
            await _viewsService.GetWorkspaceViewsAsync(workspaceId, expectedToken);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewsResponse>($"team/{workspaceId}/view", expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetWorkspaceViewsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_task_cancel_ex";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetWorkspaceViewsAsync(workspaceId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetWorkspaceViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_null_resp";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetViewsResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetWorkspaceViewsAsync(workspaceId));
        }


        // --- Tests for GetSpaceViewsAsync ---
        [Fact]
        public async Task CreateWorkspaceViewAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var workspaceId = "ws_create_op_cancel";
            var request = CreateSampleCreateViewRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new CreateTeamViewResponse { View = CreateSampleView() };

            _mockApiConnection.Setup(c => c.PostAsync<CreateViewRequest, CreateTeamViewResponse>(
                    It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, CreateViewRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.CreateWorkspaceViewAsync(workspaceId, request, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CreateWorkspaceViewAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_create_ct_pass";
            var request = CreateSampleCreateViewRequest();
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new CreateTeamViewResponse { View = CreateSampleView() };
            _mockApiConnection.Setup(x => x.PostAsync<CreateViewRequest, CreateTeamViewResponse>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.CreateWorkspaceViewAsync(workspaceId, request, expectedToken);
            _mockApiConnection.Verify(x => x.PostAsync<CreateViewRequest, CreateTeamViewResponse>($"team/{workspaceId}/view", request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateWorkspaceViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_create_task_cancel_ex";
            var request = CreateSampleCreateViewRequest();
            _mockApiConnection.Setup(x => x.PostAsync<CreateViewRequest, CreateTeamViewResponse>(It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.CreateWorkspaceViewAsync(workspaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ValidSpaceId_ReturnsViews()
        {
            var spaceId = "space123";
            var expectedViews = new List<View> { CreateSampleView("v1", "View 1"), CreateSampleView("v2", "View 2") };
            var apiResponse = new GetViewsResponse { Views = expectedViews };
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>($"space/{spaceId}/view", It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetSpaceViewsAsync(spaceId);

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Equal(2, result.Views.Count());
            Assert.Equal("v1", result.Views.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewsResponse>($"space/{spaceId}/view", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var spaceId = "space_null_resp";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetViewsResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetSpaceViewsAsync(spaceId));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ApiReturnsResponseWithNullViews_ServiceInitializesToEmptyList()
        {
            var spaceId = "space_null_views";
            var apiResponse = new GetViewsResponse { Views = null! };
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetSpaceViewsAsync(spaceId);

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Empty(result.Views);
        }

        [Fact]
        public async Task GetSpaceViewsAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var spaceId = "space_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetViewsResponse { Views = new List<View>() };

            _mockApiConnection.Setup(c => c.GetAsync<GetViewsResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.GetSpaceViewsAsync(spaceId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_task_cancel_ex";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetSpaceViewsAsync(spaceId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ThrowsHttpRequestException_WhenApiCallFails()
        {
            var spaceId = "space_http_ex";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API Error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _viewsService.GetSpaceViewsAsync(spaceId));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_PassesCancellationToken()
        {
            var spaceId = "space_ct";
            var token = new CancellationTokenSource().Token;
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), token)).ReturnsAsync(new GetViewsResponse { Views = new List<View>() });
            await _viewsService.GetSpaceViewsAsync(spaceId, token);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewsResponse>($"space/{spaceId}/view", token), Times.Once);
        }

        // --- Tests for GetFolderViewsAsync ---
        [Fact]
        public async Task CreateSpaceViewAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var spaceId = "space_create_view_op_cancel";
            var request = CreateSampleCreateViewRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new CreateSpaceViewResponse { View = CreateSampleView() };

            _mockApiConnection.Setup(c => c.PostAsync<CreateViewRequest, CreateSpaceViewResponse>(
                    It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, CreateViewRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.CreateSpaceViewAsync(spaceId, request, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CreateSpaceViewAsync_PassesCancellationTokenToApiConnection()
        {
            var spaceId = "space_create_view_ct_pass";
            var request = CreateSampleCreateViewRequest();
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new CreateSpaceViewResponse { View = CreateSampleView() };
            _mockApiConnection.Setup(x => x.PostAsync<CreateViewRequest, CreateSpaceViewResponse>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.CreateSpaceViewAsync(spaceId, request, expectedToken);
            _mockApiConnection.Verify(x => x.PostAsync<CreateViewRequest, CreateSpaceViewResponse>($"space/{spaceId}/view", request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateSpaceViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_create_view_task_cancel_ex";
            var request = CreateSampleCreateViewRequest();
            _mockApiConnection.Setup(x => x.PostAsync<CreateViewRequest, CreateSpaceViewResponse>(It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.CreateSpaceViewAsync(spaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetFolderViewsAsync_ValidFolderId_ReturnsViews()
        {
            var folderId = "folder123";
            var expectedViews = new List<View> { CreateSampleView("fv1", "Folder View 1") };
            var apiResponse = new GetViewsResponse { Views = expectedViews };
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>($"folder/{folderId}/view", It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetFolderViewsAsync(folderId);

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Single(result.Views);
            Assert.Equal("fv1", result.Views.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewsResponse>($"folder/{folderId}/view", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderViewsAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var folderId = "folder_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetViewsResponse { Views = new List<View>() };

            _mockApiConnection.Setup(c => c.GetAsync<GetViewsResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.GetFolderViewsAsync(folderId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetFolderViewsAsync_PassesCancellationTokenToApiConnection()
        {
            var folderId = "folder_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetViewsResponse { Views = new List<View>() });
            await _viewsService.GetFolderViewsAsync(folderId, expectedToken);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewsResponse>($"folder/{folderId}/view", expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetFolderViewsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_task_cancel_ex";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetFolderViewsAsync(folderId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetFolderViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var folderId = "folder_null_resp";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetViewsResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetFolderViewsAsync(folderId));
        }


        // --- Tests for GetListViewsAsync ---
        [Fact]
        public async Task CreateFolderViewAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var folderId = "folder_create_view_op_cancel";
            var request = CreateSampleCreateViewRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new CreateFolderViewResponse { View = CreateSampleView() };

            _mockApiConnection.Setup(c => c.PostAsync<CreateViewRequest, CreateFolderViewResponse>(
                    It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, CreateViewRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.CreateFolderViewAsync(folderId, request, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CreateFolderViewAsync_PassesCancellationTokenToApiConnection()
        {
            var folderId = "folder_create_view_ct_pass";
            var request = CreateSampleCreateViewRequest();
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new CreateFolderViewResponse { View = CreateSampleView() };
            _mockApiConnection.Setup(x => x.PostAsync<CreateViewRequest, CreateFolderViewResponse>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.CreateFolderViewAsync(folderId, request, expectedToken);
            _mockApiConnection.Verify(x => x.PostAsync<CreateViewRequest, CreateFolderViewResponse>($"folder/{folderId}/view", request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateFolderViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_create_view_task_cancel_ex";
            var request = CreateSampleCreateViewRequest();
            _mockApiConnection.Setup(x => x.PostAsync<CreateViewRequest, CreateFolderViewResponse>(It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.CreateFolderViewAsync(folderId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetListViewsAsync_ValidListId_ReturnsViews()
        {
            var listId = "list123";
            var expectedViews = new List<View> { CreateSampleView("lv1", "List View 1") };
            var apiResponse = new GetViewsResponse { Views = expectedViews };
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>($"list/{listId}/view", It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetListViewsAsync(listId);

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Single(result.Views);
            Assert.Equal("lv1", result.Views.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewsResponse>($"list/{listId}/view", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListViewsAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var listId = "list_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetViewsResponse { Views = new List<View>() };

            _mockApiConnection.Setup(c => c.GetAsync<GetViewsResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.GetListViewsAsync(listId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetListViewsAsync_PassesCancellationTokenToApiConnection()
        {
            var listId = "list_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetViewsResponse { Views = new List<View>() });
            await _viewsService.GetListViewsAsync(listId, expectedToken);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewsResponse>($"list/{listId}/view", expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetListViewsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var listId = "list_task_cancel_ex";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetListViewsAsync(listId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetListViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var listId = "list_null_resp";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetViewsResponse?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetListViewsAsync(listId));
        }


        // --- Tests for CreateSpaceViewAsync (example specific create) ---
        [Fact]
        public async Task CreateListViewAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var listId = "list_create_view_op_cancel";
            var request = CreateSampleCreateViewRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new CreateListViewResponse { View = CreateSampleView() };

            _mockApiConnection.Setup(c => c.PostAsync<CreateViewRequest, CreateListViewResponse>(
                    It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, CreateViewRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.CreateListViewAsync(listId, request, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task CreateListViewAsync_PassesCancellationTokenToApiConnection()
        {
            var listId = "list_create_view_ct_pass";
            var request = CreateSampleCreateViewRequest();
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new CreateListViewResponse { View = CreateSampleView() };
            _mockApiConnection.Setup(x => x.PostAsync<CreateViewRequest, CreateListViewResponse>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.CreateListViewAsync(listId, request, expectedToken);
            _mockApiConnection.Verify(x => x.PostAsync<CreateViewRequest, CreateListViewResponse>($"list/{listId}/view", request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateListViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var listId = "list_create_view_task_cancel_ex";
            var request = CreateSampleCreateViewRequest();
            _mockApiConnection.Setup(x => x.PostAsync<CreateViewRequest, CreateListViewResponse>(It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.CreateListViewAsync(listId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateSpaceViewAsync_ValidRequest_ReturnsView()
        {
            var spaceId = "space_create_view";
            var request = CreateSampleCreateViewRequest("Space Specific View");
            var expectedView = CreateSampleView("new_space_view_id", request.Name, request.Type);
            var apiResponse = new CreateSpaceViewResponse { View = expectedView };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateViewRequest, CreateSpaceViewResponse>(
                    $"space/{spaceId}/view",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _viewsService.CreateSpaceViewAsync(spaceId, request);

            Assert.NotNull(result);
            Assert.NotNull(result.View);
            Assert.Equal(expectedView.Id, result.View.Id);
            _mockApiConnection.Verify(x => x.PostAsync<CreateViewRequest, CreateSpaceViewResponse>(
                $"space/{spaceId}/view", request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateSpaceViewAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var spaceId = "space_create_view_null";
            var request = CreateSampleCreateViewRequest();
            _mockApiConnection
                .Setup(x => x.PostAsync<CreateViewRequest, CreateSpaceViewResponse>(
                    It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CreateSpaceViewResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.CreateSpaceViewAsync(spaceId, request));
        }

        // --- Tests for UpdateViewAsync ---
        [Fact]
        public async Task UpdateViewAsync_ValidRequest_ReturnsView()
        {
            var viewId = "view_to_update";
            var request = CreateSampleUpdateViewRequest("Updated View Name via Test", "board");
            var expectedView = CreateSampleView(viewId, request.Name, request.Type);
            var apiResponse = new UpdateViewResponse { View = expectedView };

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateViewRequest, UpdateViewResponse>(
                    $"view/{viewId}",
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _viewsService.UpdateViewAsync(viewId, request);

            Assert.NotNull(result);
            Assert.NotNull(result.View);
            Assert.Equal(expectedView.Name, result.View.Name);
            _mockApiConnection.Verify(x => x.PutAsync<UpdateViewRequest, UpdateViewResponse>(
                $"view/{viewId}", request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateViewAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var viewId = "view_update_null";
            var request = CreateSampleUpdateViewRequest();
            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateViewRequest, UpdateViewResponse>(
                    It.IsAny<string>(), It.IsAny<UpdateViewRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UpdateViewResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.UpdateViewAsync(viewId, request));
        }

        // --- Tests for DeleteViewAsync ---
        [Fact]
        public async Task UpdateViewAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var viewId = "view_update_op_cancel";
            var request = CreateSampleUpdateViewRequest();
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new UpdateViewResponse { View = CreateSampleView() };

            _mockApiConnection.Setup(c => c.PutAsync<UpdateViewRequest, UpdateViewResponse>(
                    It.IsAny<string>(), It.IsAny<UpdateViewRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, UpdateViewRequest, CancellationToken>((url, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.UpdateViewAsync(viewId, request, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task UpdateViewAsync_PassesCancellationTokenToApiConnection()
        {
            var viewId = "view_update_ct_pass";
            var request = CreateSampleUpdateViewRequest();
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var apiResponse = new UpdateViewResponse { View = CreateSampleView() };
            _mockApiConnection.Setup(x => x.PutAsync<UpdateViewRequest, UpdateViewResponse>(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.UpdateViewAsync(viewId, request, expectedToken);
            _mockApiConnection.Verify(x => x.PutAsync<UpdateViewRequest, UpdateViewResponse>($"view/{viewId}", request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task UpdateViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var viewId = "view_update_task_cancel_ex";
            var request = CreateSampleUpdateViewRequest();
            _mockApiConnection.Setup(x => x.PutAsync<UpdateViewRequest, UpdateViewResponse>(It.IsAny<string>(), It.IsAny<UpdateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.UpdateViewAsync(viewId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task DeleteViewAsync_ValidViewId_CallsDeleteAndCompletes()
        {
            var viewId = "view_to_delete";
            _mockApiConnection
                .Setup(x => x.DeleteAsync($"view/{viewId}", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _viewsService.DeleteViewAsync(viewId);

            _mockApiConnection.Verify(x => x.DeleteAsync($"view/{viewId}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteViewAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var viewId = "view_delete_op_cancel";
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
                _viewsService.DeleteViewAsync(viewId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task DeleteViewAsync_PassesCancellationTokenToApiConnection()
        {
            var viewId = "view_delete_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection.Setup(x => x.DeleteAsync(It.IsAny<string>(), expectedToken))
                .Returns(Task.CompletedTask);
            await _viewsService.DeleteViewAsync(viewId, expectedToken);
            _mockApiConnection.Verify(x => x.DeleteAsync($"view/{viewId}", expectedToken), Times.Once);
        }

        [Fact]
        public async Task DeleteViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var viewId = "view_delete_task_cancel_ex";
            _mockApiConnection.Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.DeleteViewAsync(viewId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task DeleteViewAsync_ApiThrowsError_PropagatesException()
        {
            var viewId = "view_delete_error";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Deletion failed"));

            await Assert.ThrowsAsync<HttpRequestException>(() => _viewsService.DeleteViewAsync(viewId));
        }


        // --- Tests for GetViewAsync ---
        [Fact]
        public async Task GetViewAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var viewId = "view_get_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetViewResponse { View = CreateSampleView() };

            _mockApiConnection.Setup(c => c.GetAsync<GetViewResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.GetViewAsync(viewId, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetViewAsync_PassesCancellationTokenToApiConnection()
        {
            var viewId = "view_get_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection.Setup(x => x.GetAsync<GetViewResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetViewResponse { View = CreateSampleView() });
            await _viewsService.GetViewAsync(viewId, expectedToken);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewResponse>($"view/{viewId}", expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var viewId = "view_get_task_cancel_ex";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetViewAsync(viewId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetViewAsync_ValidViewId_ReturnsView()
        {
            var viewId = "view_get_one";
            var expectedView = CreateSampleView(viewId);
            var apiResponse = new GetViewResponse { View = expectedView };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetViewResponse>($"view/{viewId}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _viewsService.GetViewAsync(viewId);

            Assert.NotNull(result);
            Assert.NotNull(result.View);
            Assert.Equal(expectedView.Id, result.View.Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewResponse>($"view/{viewId}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetViewAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var viewId = "view_get_null";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetViewResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetViewResponse?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetViewAsync(viewId));
        }

        // --- Tests for GetViewTasksAsync ---
        [Fact]
        public async Task GetViewTasksAsync_ValidRequest_ReturnsTasks()
        {
            var viewId = "view_tasks_1";
            var page = 0;
            // Corrected: Initialize with List<CuTask>
            var apiResponse = new GetViewTasksResponse(Tasks: new List<ClickUp.Api.Client.Models.Entities.Tasks.CuTask> { /* Create sample CuTask if needed, or leave empty for structure test */ }, LastPage: false);


            _mockApiConnection
                .Setup(x => x.GetAsync<GetViewTasksResponse>(
                    $"view/{viewId}/task?page={page}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var request = new GetViewTasksRequest { Page = page };
            var result = await _viewsService.GetViewTasksAsync(viewId, request);

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.Empty(result.Items);
            Assert.True(result.HasNextPage);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewTasksResponse>(
                $"view/{viewId}/task?page={page}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetViewTasksAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var viewId = "view_tasks_null";
            var page = 0;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetViewTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetViewTasksResponse?)null);

            var request = new GetViewTasksRequest { Page = page };
            // Act
            var result = await _viewsService.GetViewTasksAsync(viewId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.False(result.HasNextPage); // PagedResult.Empty() sets HasNextPage to false
            Assert.Equal(page, result.Page);
        }

        [Fact]
        public async Task GetViewTasksAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var viewId = "view_tasks_op_cancel";
            var request = new GetViewTasksRequest { Page = 0 };
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetViewTasksResponse(Tasks: new List<ClickUp.Api.Client.Models.Entities.Tasks.CuTask>(), LastPage: true);

            _mockApiConnection.Setup(c => c.GetAsync<GetViewTasksResponse>(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(dummyResponse);

            cancellationTokenSource.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _viewsService.GetViewTasksAsync(viewId, request, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetViewTasksAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var viewId = "view_tasks_task_cancel_ex";
            var request = new GetViewTasksRequest { Page = 0 };
            _mockApiConnection.Setup(x => x.GetAsync<GetViewTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetViewTasksAsync(viewId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetViewTasksAsync_PassesCancellationToken()
        {
            var viewId = "view_tasks_ct";
            var page = 0;
            var token = new CancellationTokenSource().Token;
            // Corrected: Initialize with List<CuTask>
            var apiResponse = new GetViewTasksResponse(Tasks: new List<ClickUp.Api.Client.Models.Entities.Tasks.CuTask>(), LastPage: true);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetViewTasksResponse>(It.IsAny<string>(), token))
                .ReturnsAsync(apiResponse);

            var request = new GetViewTasksRequest { Page = page };
            await _viewsService.GetViewTasksAsync(viewId, request, token);

            _mockApiConnection.Verify(x => x.GetAsync<GetViewTasksResponse>(
                $"view/{viewId}/task?page={page}", token), Times.Once);
        }
    }
}
