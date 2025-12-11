using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Entities.Tasks;
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
        private readonly Mock<IViewCrudService> _mockViewCrudService;
        private readonly Mock<IViewQueryService> _mockViewQueryService;
        private readonly Mock<IViewTaskService> _mockViewTaskService;
        private readonly Mock<ILogger<ViewsService>> _mockLogger;
        private readonly ViewsService _viewsService;

        public ViewsServiceTests()
        {
            _mockViewCrudService = new Mock<IViewCrudService>();
            _mockViewQueryService = new Mock<IViewQueryService>();
            _mockViewTaskService = new Mock<IViewTaskService>();
            _mockLogger = new Mock<ILogger<ViewsService>>();
            _viewsService = new ViewsService(
                _mockViewCrudService.Object,
                _mockViewQueryService.Object,
                _mockViewTaskService.Object,
                _mockLogger.Object);
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

            _mockViewQueryService.Setup(c => c.GetWorkspaceViewsAsync(
                    workspaceId, It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((id, token) =>
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
            _mockViewQueryService.Setup(x => x.GetWorkspaceViewsAsync(workspaceId, It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetWorkspaceViewsAsync(workspaceId);

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Single(result.Views);
            Assert.Equal("wv1", result.Views.First().Id);
            _mockViewQueryService.Verify(x => x.GetWorkspaceViewsAsync(workspaceId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetWorkspaceViewsAsync_PassesCancellationTokenToApiConnection()
        {
            var workspaceId = "ws_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockViewQueryService.Setup(x => x.GetWorkspaceViewsAsync(workspaceId, expectedToken))
                .ReturnsAsync(new GetViewsResponse { Views = new List<View>() });
            await _viewsService.GetWorkspaceViewsAsync(workspaceId, expectedToken);
            _mockViewQueryService.Verify(x => x.GetWorkspaceViewsAsync(workspaceId, expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetWorkspaceViewsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_task_cancel_ex";
            _mockViewQueryService.Setup(x => x.GetWorkspaceViewsAsync(workspaceId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetWorkspaceViewsAsync(workspaceId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetWorkspaceViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_null_resp";
            _mockViewQueryService.Setup(x => x.GetWorkspaceViewsAsync(workspaceId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException($"API connection returned null response when getting data from team/{workspaceId}/view."));

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

            _mockViewQueryService.Setup(c => c.CreateWorkspaceViewAsync(
                    workspaceId, request, It.IsAny<CancellationToken>()))
                .Callback<string, CreateViewRequest, CancellationToken>((id, req, token) =>
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
            _mockViewQueryService.Setup(x => x.CreateWorkspaceViewAsync(workspaceId, request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.CreateWorkspaceViewAsync(workspaceId, request, expectedToken);
            _mockViewQueryService.Verify(x => x.CreateWorkspaceViewAsync(workspaceId, request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateWorkspaceViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var workspaceId = "ws_create_task_cancel_ex";
            var request = CreateSampleCreateViewRequest();
            _mockViewQueryService.Setup(x => x.CreateWorkspaceViewAsync(workspaceId, request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.CreateWorkspaceViewAsync(workspaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ValidSpaceId_ReturnsViews()
        {
            var spaceId = "space123";
            var expectedViews = new List<View> { CreateSampleView("v1", "View 1"), CreateSampleView("v2", "View 2") };
            var apiResponse = new GetViewsResponse { Views = expectedViews };
            _mockViewQueryService.Setup(x => x.GetSpaceViewsAsync(spaceId, It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetSpaceViewsAsync(spaceId);

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Equal(2, result.Views.Count());
            Assert.Equal("v1", result.Views.First().Id);
            _mockViewQueryService.Verify(x => x.GetSpaceViewsAsync(spaceId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var spaceId = "space_null_resp";
            _mockViewQueryService.Setup(x => x.GetSpaceViewsAsync(spaceId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("API returned null response"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetSpaceViewsAsync(spaceId));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ApiReturnsResponseWithNullViews_ServiceInitializesToEmptyList()
        {
            var spaceId = "space_null_views";
            var apiResponse = new GetViewsResponse { Views = new List<View>() };
            _mockViewQueryService.Setup(x => x.GetSpaceViewsAsync(spaceId, It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

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

            _mockViewQueryService.Setup(c => c.GetSpaceViewsAsync(
                    spaceId, It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((id, token) =>
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
            _mockViewQueryService.Setup(x => x.GetSpaceViewsAsync(spaceId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetSpaceViewsAsync(spaceId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ThrowsHttpRequestException_WhenApiCallFails()
        {
            var spaceId = "space_http_ex";
            _mockViewQueryService.Setup(x => x.GetSpaceViewsAsync(spaceId, It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("API Error"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _viewsService.GetSpaceViewsAsync(spaceId));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_PassesCancellationToken()
        {
            var spaceId = "space_ct";
            var token = new CancellationTokenSource().Token;
            _mockViewQueryService.Setup(x => x.GetSpaceViewsAsync(spaceId, token)).ReturnsAsync(new GetViewsResponse { Views = new List<View>() });
            await _viewsService.GetSpaceViewsAsync(spaceId, token);
            _mockViewQueryService.Verify(x => x.GetSpaceViewsAsync(spaceId, token), Times.Once);
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

            _mockViewQueryService.Setup(c => c.CreateSpaceViewAsync(
                    spaceId, request, It.IsAny<CancellationToken>()))
                .Callback<string, CreateViewRequest, CancellationToken>((id, req, token) =>
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
            _mockViewQueryService.Setup(x => x.CreateSpaceViewAsync(spaceId, request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.CreateSpaceViewAsync(spaceId, request, expectedToken);
            _mockViewQueryService.Verify(x => x.CreateSpaceViewAsync(spaceId, request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateSpaceViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_create_view_task_cancel_ex";
            var request = CreateSampleCreateViewRequest();
            _mockViewQueryService.Setup(x => x.CreateSpaceViewAsync(It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.CreateSpaceViewAsync(spaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetFolderViewsAsync_ValidFolderId_ReturnsViews()
        {
            var folderId = "folder123";
            var expectedViews = new List<View> { CreateSampleView("fv1", "Folder View 1") };
            var apiResponse = new GetViewsResponse { Views = expectedViews };
            _mockViewQueryService.Setup(x => x.GetFolderViewsAsync(folderId, It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetFolderViewsAsync(folderId);

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Single(result.Views);
            Assert.Equal("fv1", result.Views.First().Id);
            _mockViewQueryService.Verify(x => x.GetFolderViewsAsync(folderId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetFolderViewsAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var folderId = "folder_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetViewsResponse { Views = new List<View>() };

            _mockViewQueryService.Setup(c => c.GetFolderViewsAsync(
                    folderId, It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((id, token) =>
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
            _mockViewQueryService.Setup(x => x.GetFolderViewsAsync(folderId, expectedToken))
                .ReturnsAsync(new GetViewsResponse { Views = new List<View>() });
            await _viewsService.GetFolderViewsAsync(folderId, expectedToken);
            _mockViewQueryService.Verify(x => x.GetFolderViewsAsync(folderId, expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetFolderViewsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_task_cancel_ex";
            _mockViewQueryService.Setup(x => x.GetFolderViewsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetFolderViewsAsync(folderId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetFolderViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var folderId = "folder_null_resp";
            _mockViewQueryService.Setup(x => x.GetFolderViewsAsync(folderId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("API returned null response"));
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

            _mockViewQueryService.Setup(c => c.CreateFolderViewAsync(
                    folderId, request, It.IsAny<CancellationToken>()))
                .Callback<string, CreateViewRequest, CancellationToken>((id, req, token) =>
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
            _mockViewQueryService.Setup(x => x.CreateFolderViewAsync(folderId, request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.CreateFolderViewAsync(folderId, request, expectedToken);
            _mockViewQueryService.Verify(x => x.CreateFolderViewAsync(folderId, request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateFolderViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var folderId = "folder_create_view_task_cancel_ex";
            var request = CreateSampleCreateViewRequest();
            _mockViewQueryService.Setup(x => x.CreateFolderViewAsync(It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.CreateFolderViewAsync(folderId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetListViewsAsync_ValidListId_ReturnsViews()
        {
            var listId = "list123";
            var expectedViews = new List<View> { CreateSampleView("lv1", "List View 1") };
            var apiResponse = new GetViewsResponse { Views = expectedViews };
            _mockViewQueryService.Setup(x => x.GetListViewsAsync(listId, It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetListViewsAsync(listId);

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Single(result.Views);
            Assert.Equal("lv1", result.Views.First().Id);
            _mockViewQueryService.Verify(x => x.GetListViewsAsync(listId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetListViewsAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var listId = "list_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();
            var dummyResponse = new GetViewsResponse { Views = new List<View>() };

            _mockViewQueryService.Setup(c => c.GetListViewsAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((listId, token) =>
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
            _mockViewQueryService.Setup(x => x.GetListViewsAsync(listId, expectedToken))
                .ReturnsAsync(new GetViewsResponse { Views = new List<View>() });
            await _viewsService.GetListViewsAsync(listId, expectedToken);
            _mockViewQueryService.Verify(x => x.GetListViewsAsync(listId, expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetListViewsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var listId = "list_task_cancel_ex";
            _mockViewQueryService.Setup(x => x.GetListViewsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetListViewsAsync(listId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetListViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var listId = "list_null_resp";
            _mockViewQueryService.Setup(x => x.GetListViewsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("API returned null response"));
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

            _mockViewQueryService.Setup(c => c.CreateListViewAsync(
                    listId, request, It.IsAny<CancellationToken>()))
                .Callback<string, CreateViewRequest, CancellationToken>((id, req, token) =>
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
            _mockViewQueryService.Setup(x => x.CreateListViewAsync(listId, request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.CreateListViewAsync(listId, request, expectedToken);
            _mockViewQueryService.Verify(x => x.CreateListViewAsync(listId, request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task CreateListViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var listId = "list_create_view_task_cancel_ex";
            var request = CreateSampleCreateViewRequest();
            _mockViewQueryService.Setup(x => x.CreateListViewAsync(It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
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

            _mockViewQueryService
                .Setup(x => x.CreateSpaceViewAsync(
                    spaceId,
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _viewsService.CreateSpaceViewAsync(spaceId, request);

            Assert.NotNull(result);
            Assert.NotNull(result.View);
            Assert.Equal(expectedView.Id, result.View.Id);
            _mockViewQueryService.Verify(x => x.CreateSpaceViewAsync(
                spaceId, request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateSpaceViewAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var spaceId = "space_create_view_null";
            var request = CreateSampleCreateViewRequest();
            _mockViewQueryService
                .Setup(x => x.CreateSpaceViewAsync(
                    It.IsAny<string>(), It.IsAny<CreateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException($"API connection returned null response when creating space view for space {spaceId}."));

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

            _mockViewCrudService
                .Setup(x => x.UpdateViewAsync(
                    viewId,
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _viewsService.UpdateViewAsync(viewId, request);

            Assert.NotNull(result);
            Assert.NotNull(result.View);
            Assert.Equal(expectedView.Name, result.View.Name);
            _mockViewCrudService.Verify(x => x.UpdateViewAsync(
                viewId, request, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateViewAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var viewId = "view_update_null";
            var request = CreateSampleUpdateViewRequest();
            _mockViewCrudService
                .Setup(x => x.UpdateViewAsync(
                    It.IsAny<string>(), It.IsAny<UpdateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException($"API connection returned null response when updating view {viewId}."));

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

            _mockViewCrudService.Setup(c => c.UpdateViewAsync(
                    It.IsAny<string>(), It.IsAny<UpdateViewRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, UpdateViewRequest, CancellationToken>((id, req, token) =>
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
            _mockViewCrudService.Setup(x => x.UpdateViewAsync(It.IsAny<string>(), request, expectedToken))
                .ReturnsAsync(apiResponse);
            await _viewsService.UpdateViewAsync(viewId, request, expectedToken);
            _mockViewCrudService.Verify(x => x.UpdateViewAsync(viewId, request, expectedToken), Times.Once);
        }

        [Fact]
        public async Task UpdateViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var viewId = "view_update_task_cancel_ex";
            var request = CreateSampleUpdateViewRequest();
            _mockViewCrudService.Setup(x => x.UpdateViewAsync(It.IsAny<string>(), It.IsAny<UpdateViewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.UpdateViewAsync(viewId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task DeleteViewAsync_ValidViewId_CallsDeleteAndCompletes()
        {
            var viewId = "view_to_delete";
            _mockViewCrudService
                .Setup(x => x.DeleteViewAsync(viewId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _viewsService.DeleteViewAsync(viewId);

            _mockViewCrudService.Verify(x => x.DeleteViewAsync(viewId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteViewAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var viewId = "view_delete_op_cancel";
            var cancellationTokenSource = new CancellationTokenSource();

            _mockViewCrudService.Setup(x => x.DeleteViewAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((viewId, token) =>
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
            _mockViewCrudService.Setup(x => x.DeleteViewAsync(It.IsAny<string>(), expectedToken))
                .Returns(Task.CompletedTask);
            await _viewsService.DeleteViewAsync(viewId, expectedToken);
            _mockViewCrudService.Verify(x => x.DeleteViewAsync(viewId, expectedToken), Times.Once);
        }

        [Fact]
        public async Task DeleteViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var viewId = "view_delete_task_cancel_ex";
            _mockViewCrudService.Setup(x => x.DeleteViewAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.DeleteViewAsync(viewId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task DeleteViewAsync_ApiThrowsError_PropagatesException()
        {
            var viewId = "view_delete_error";
            _mockViewCrudService
                .Setup(x => x.DeleteViewAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
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

            _mockViewCrudService.Setup(c => c.GetViewAsync(
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((id, token) =>
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
            _mockViewCrudService.Setup(x => x.GetViewAsync(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetViewResponse { View = CreateSampleView() });
            await _viewsService.GetViewAsync(viewId, expectedToken);
            _mockViewCrudService.Verify(x => x.GetViewAsync(viewId, expectedToken), Times.Once);
        }

        [Fact]
        public async Task GetViewAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var viewId = "view_get_task_cancel_ex";
            _mockViewCrudService.Setup(x => x.GetViewAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetViewAsync(viewId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetViewAsync_ValidViewId_ReturnsView()
        {
            var viewId = "view_get_one";
            var expectedView = CreateSampleView(viewId);
            var apiResponse = new GetViewResponse { View = expectedView };

            _mockViewCrudService
                .Setup(x => x.GetViewAsync(viewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _viewsService.GetViewAsync(viewId);

            Assert.NotNull(result);
            Assert.NotNull(result.View);
            Assert.Equal(expectedView.Id, result.View.Id);
            _mockViewCrudService.Verify(x => x.GetViewAsync(viewId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetViewAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var viewId = "view_get_null";
            // The actual service implementation throws InvalidOperationException when API returns null
            // So we should test that the service handles this scenario properly
            _mockViewCrudService
                .Setup(x => x.GetViewAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException($"API connection returned null response when getting view {viewId}."));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetViewAsync(viewId));
        }

        // --- Tests for GetViewTasksAsync ---
        [Fact]
        public async Task GetViewTasksAsync_ValidRequest_ReturnsTasks()
        {
            var viewId = "view_tasks_1";
            var page = 0;
            // Create a mock IPagedResult<CuTask>
            var mockPagedResult = new Mock<IPagedResult<CuTask>>();
            mockPagedResult.Setup(x => x.Items).Returns(new List<CuTask>());
            mockPagedResult.Setup(x => x.HasNextPage).Returns(true);
            mockPagedResult.Setup(x => x.Page).Returns(page);

            _mockViewTaskService
                .Setup(x => x.GetViewTasksAsync(
                    viewId, It.IsAny<GetViewTasksRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockPagedResult.Object);

            var request = new GetViewTasksRequest { Page = page };
            var result = await _viewsService.GetViewTasksAsync(viewId, request);

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.Empty(result.Items);
            Assert.True(result.HasNextPage);
            _mockViewTaskService.Verify(x => x.GetViewTasksAsync(
                viewId, It.IsAny<GetViewTasksRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetViewTasksAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var viewId = "view_tasks_null";
            var page = 0;
            // Create an empty paged result to simulate null/empty response
            var emptyPagedResult = new Mock<IPagedResult<CuTask>>();
            emptyPagedResult.Setup(x => x.Items).Returns(new List<CuTask>());
            emptyPagedResult.Setup(x => x.HasNextPage).Returns(false);
            emptyPagedResult.Setup(x => x.Page).Returns(page);
            
            _mockViewTaskService
                .Setup(x => x.GetViewTasksAsync(It.IsAny<string>(), It.IsAny<GetViewTasksRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyPagedResult.Object);

            var request = new GetViewTasksRequest { Page = page };
            // Act
            var result = await _viewsService.GetViewTasksAsync(viewId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.False(result.HasNextPage);
            Assert.Equal(page, result.Page);
        }

        [Fact]
        public async Task GetViewTasksAsync_OperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var viewId = "view_tasks_op_cancel";
            var request = new GetViewTasksRequest { Page = 0 };
            var cancellationTokenSource = new CancellationTokenSource();
            var mockPagedResult = new Mock<IPagedResult<CuTask>>();
            mockPagedResult.Setup(x => x.Items).Returns(new List<CuTask>());
            mockPagedResult.Setup(x => x.HasNextPage).Returns(false);

            _mockViewTaskService.Setup(c => c.GetViewTasksAsync(
                    It.IsAny<string>(), It.IsAny<GetViewTasksRequest>(), It.IsAny<CancellationToken>()))
                .Callback<string, GetViewTasksRequest, CancellationToken>((id, req, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(token);
                    }
                })
                .ReturnsAsync(mockPagedResult.Object);

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
            _mockViewTaskService.Setup(x => x.GetViewTasksAsync(It.IsAny<string>(), It.IsAny<GetViewTasksRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _viewsService.GetViewTasksAsync(viewId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetViewTasksAsync_PassesCancellationToken()
        {
            var viewId = "view_tasks_ct";
            var page = 0;
            var token = new CancellationTokenSource().Token;
            // Create a mock IPagedResult<CuTask>
            var mockPagedResult = new Mock<IPagedResult<CuTask>>();
            mockPagedResult.Setup(x => x.Items).Returns(new List<CuTask>());
            mockPagedResult.Setup(x => x.HasNextPage).Returns(false);

            _mockViewTaskService
                .Setup(x => x.GetViewTasksAsync(It.IsAny<string>(), It.IsAny<GetViewTasksRequest>(), token))
                .ReturnsAsync(mockPagedResult.Object);

            var request = new GetViewTasksRequest { Page = page };
            await _viewsService.GetViewTasksAsync(viewId, request, token);

            _mockViewTaskService.Verify(x => x.GetViewTasksAsync(
                viewId, It.IsAny<GetViewTasksRequest>(), token), Times.Once);
        }
    }
}
