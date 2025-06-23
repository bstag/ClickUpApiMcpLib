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
// Required for specific create view responses if we test those service methods directly
// using ClickUp.Api.Client.Models.ResponseModels.Views.CreateSpaceViewResponse; // Example
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

// Assuming these response DTOs exist or will be created, used by ViewsService
// If they don't exist, the Create...ViewAsync tests will need to be adjusted or deferred.
// For now, we'll assume a structure like: public record CreateXYZViewResponse { public View View { get; init; } }
// If not, PostAsync in service methods would return View directly, or another structure.
// Based on ViewsService.cs, it expects e.g. CreateSpaceViewResponse, CreateListViewResponse etc.
// These are not currently in the provided DTO list.
// For the purpose of this correction, I will assume that these response types (e.g. CreateSpaceViewResponse)
// wrap a 'View' property, similar to GetViewResponse or UpdateViewResponse.
// If this assumption is wrong, the PostAsync mocks will need to change.

// Placeholder for missing Create...Response DTOs.
// In a real scenario, these would be defined in their respective files.
namespace ClickUp.Api.Client.Models.ResponseModels.Views
{
    public record CreateTeamViewResponse { public View View { get; init; } = null!; }
    public record CreateSpaceViewResponse { public View View { get; init; } = null!; }
    public record CreateFolderViewResponse { public View View { get; init; } = null!; }
    public record CreateListViewResponse { public View View { get; init; } = null!; }
    // GetViewTasksResponse and its dependent CuTask would also be needed for GetViewTasksAsync tests
    public record GetViewTasksResponse { public List<object> Tasks { get; init; } = new List<object>(); public bool LastPage { get; init; } } // Placeholder Task DTO
}


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
                Settings = new ViewSettings { ShowTaskLocations = true } // Example of a complex property
            };
        }

        private CreateViewRequest CreateSampleCreateViewRequest(string name = "New View", string type = "list")
        {
            // CreateViewRequest is a class with settable properties.
            return new CreateViewRequest
            {
                Name = name,
                Type = type,
                Grouping = new ViewGrouping { Field = "status" }, // Example minimal valid sub-object
                Divide = new ViewDivide(),
                Sorting = new ViewSorting(),
                Filters = new ViewFilters { Operator = "AND" }, // Corrected from Op to Operator
                Columns = new ViewColumns(),
                TeamSidebar = new ViewTeamSidebar(),
                Settings = new ViewSettings { ShowAssignees = true }
            };
        }

        private UpdateViewRequest CreateSampleUpdateViewRequest(string name = "Updated View Name", string type = "list")
        {
            // UpdateViewRequest is also a class with settable properties.
            return new UpdateViewRequest
            {
                Name = name,
                Type = type, // Type is often required on update too
                Grouping = new ViewGrouping { Field = "priority" }, // Example minimal valid sub-object
                Divide = new ViewDivide(),
                Sorting = new ViewSorting(),
                Filters = new ViewFilters { Operator = "OR" }, // Corrected from Op to Operator
                Columns = new ViewColumns(),
                TeamSidebar = new ViewTeamSidebar(),
                Settings = new ViewSettings { ShowDueDate = true }
            };
        }

        // --- Tests for GetWorkspaceViewsAsync ---
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
        public async Task GetWorkspaceViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var workspaceId = "ws_null_resp";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetViewsResponse)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetWorkspaceViewsAsync(workspaceId));
        }


        // --- Tests for GetSpaceViewsAsync ---
        [Fact]
        public async Task GetSpaceViewsAsync_ValidSpaceId_ReturnsViews()
        {
            var spaceId = "space123";
            var expectedViews = new List<View> { CreateSampleView("v1", "View 1"), CreateSampleView("v2", "View 2") };
            // GetViewsResponse is a record, properties are init-only. Use object initializer.
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
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetViewsResponse)null);

            // Service method throws InvalidOperationException if API returns null
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetSpaceViewsAsync(spaceId));
        }

        [Fact]
        public async Task GetSpaceViewsAsync_ApiReturnsResponseWithNullViews_ServiceInitializesToEmptyList()
        {
            var spaceId = "space_null_views";
            // GetViewsResponse DTO initializes Views to new List<View>() if null is passed to constructor,
            // or if its init property is set to null, it will be an empty list due to default initializer.
            var apiResponse = new GetViewsResponse { Views = null! }; // Views property is init; { Views = null } is fine
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

            var result = await _viewsService.GetSpaceViewsAsync(spaceId); // Service gets GetViewsResponse with Views = new List()

            Assert.NotNull(result);
            Assert.NotNull(result.Views);
            Assert.Empty(result.Views); // Because GetViewsResponse initializes Views to an empty list
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
            // Ensure GetViewsResponse is correctly initialized for the mock return
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), token)).ReturnsAsync(new GetViewsResponse { Views = new List<View>() });
            await _viewsService.GetSpaceViewsAsync(spaceId, token);
            _mockApiConnection.Verify(x => x.GetAsync<GetViewsResponse>($"space/{spaceId}/view", token), Times.Once);
        }

        // --- Tests for GetFolderViewsAsync ---
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
        public async Task GetFolderViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var folderId = "folder_null_resp";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetViewsResponse)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetFolderViewsAsync(folderId));
        }


        // --- Tests for GetListViewsAsync ---
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
        public async Task GetListViewsAsync_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            var listId = "list_null_resp";
            _mockApiConnection.Setup(x => x.GetAsync<GetViewsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetViewsResponse)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetListViewsAsync(listId));
        }


        // --- Tests for CreateSpaceViewAsync (example specific create) ---
        // The old CreateViewAsync was too generic. Testing specific service methods.
        [Fact]
        public async Task CreateSpaceViewAsync_ValidRequest_ReturnsView()
        {
            var spaceId = "space_create_view";
            var request = CreateSampleCreateViewRequest("Space Specific View");
            var expectedView = CreateSampleView("new_space_view_id", request.Name, request.Type);
            // Assuming CreateSpaceViewResponse wraps a View object
            var apiResponse = new CreateSpaceViewResponse { View = expectedView };

            _mockApiConnection
                .Setup(x => x.PostAsync<CreateViewRequest, CreateSpaceViewResponse>(
                    $"space/{spaceId}/view",
                    request, // Match the request object
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
                .ReturnsAsync((CreateSpaceViewResponse)null); // API Connection returns null

            // Service method throws InvalidOperationException if API returns null
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.CreateSpaceViewAsync(spaceId, request));
        }

        // --- Tests for UpdateViewAsync ---
        [Fact]
        public async Task UpdateViewAsync_ValidRequest_ReturnsView()
        {
            var viewId = "view_to_update";
            var request = CreateSampleUpdateViewRequest("Updated View Name via Test", "board");
            var expectedView = CreateSampleView(viewId, request.Name, request.Type);
            // UpdateViewResponse wraps a View object
            var apiResponse = new UpdateViewResponse { View = expectedView };

            _mockApiConnection
                .Setup(x => x.PutAsync<UpdateViewRequest, UpdateViewResponse>(
                    $"view/{viewId}",
                    request, // Match the request object
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
                .ReturnsAsync((UpdateViewResponse)null); // API Connection returns null

            // Service method throws InvalidOperationException if API returns null
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.UpdateViewAsync(viewId, request));
        }

        // --- Tests for DeleteViewAsync ---
        [Fact]
        public async Task DeleteViewAsync_ValidViewId_CallsDeleteAndCompletes()
        {
            var viewId = "view_to_delete";
            _mockApiConnection
                .Setup(x => x.DeleteAsync($"view/{viewId}", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask); // DeleteAsync returns Task

            await _viewsService.DeleteViewAsync(viewId); // Should complete without exception

            _mockApiConnection.Verify(x => x.DeleteAsync($"view/{viewId}", It.IsAny<CancellationToken>()), Times.Once);
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
        public async Task GetViewAsync_ValidViewId_ReturnsView()
        {
            var viewId = "view_get_one";
            var expectedView = CreateSampleView(viewId);
            // GetViewResponse wraps a View object
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
                .ReturnsAsync((GetViewResponse)null); // API Connection returns null

            // Service method throws InvalidOperationException if API returns null
            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetViewAsync(viewId));
        }

        // --- Tests for GetViewTasksAsync ---
        [Fact]
        public async Task GetViewTasksAsync_ValidRequest_ReturnsTasks()
        {
            var viewId = "view_tasks_1";
            var page = 0;
            // Assuming GetViewTasksResponse has Tasks list and LastPage properties
            var apiResponse = new GetViewTasksResponse { Tasks = new List<object> { new object(), new object() }, LastPage = false };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetViewTasksResponse>(
                    $"view/{viewId}/task?page={page}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _viewsService.GetViewTasksAsync(viewId, page);

            Assert.NotNull(result);
            Assert.NotNull(result.Tasks);
            Assert.Equal(2, result.Tasks.Count);
            Assert.False(result.LastPage);
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
                .ReturnsAsync((GetViewTasksResponse)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _viewsService.GetViewTasksAsync(viewId, page));
        }

        [Fact]
        public async Task GetViewTasksAsync_PassesCancellationToken()
        {
            var viewId = "view_tasks_ct";
            var page = 0;
            var token = new CancellationTokenSource().Token;
            var apiResponse = new GetViewTasksResponse { Tasks = new List<object>(), LastPage = true };

            _mockApiConnection
                .Setup(x => x.GetAsync<GetViewTasksResponse>(It.IsAny<string>(), token))
                .ReturnsAsync(apiResponse);

            await _viewsService.GetViewTasksAsync(viewId, page, token);

            _mockApiConnection.Verify(x => x.GetAsync<GetViewTasksResponse>(
                $"view/{viewId}/task?page={page}", token), Times.Once);
        }

        // TODO: Add similar HttpRequestException tests for other methods like Create, Update, Delete, GetViewTasks
        // TODO: Add CancellationToken propagation tests for Create, Update, Delete methods
    }
}
