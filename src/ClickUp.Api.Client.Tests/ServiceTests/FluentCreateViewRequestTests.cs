using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Views;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentCreateViewRequestTests
{
    private readonly Mock<IViewsService> _mockViewsService;

    public FluentCreateViewRequestTests()
    {
        _mockViewsService = new Mock<IViewsService>();
    }

    [Fact]
    public async Task CreateAsync_WorkspaceContainerType_ShouldCallCreateWorkspaceViewAsync()
    {
        // Arrange
        var containerId = "testWorkspaceId";
        var name = "testViewName";
        var type = "list";
        var isPrivate = true;
        var expectedView = new View("viewId", "viewName", "viewType");
        var expectedResponse = new CreateTeamViewResponse { View = expectedView };

        _mockViewsService.Setup(x => x.CreateWorkspaceViewAsync(
            It.IsAny<string>(),
            It.IsAny<CreateViewRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new FluentCreateViewRequest(containerId, _mockViewsService.Object, FluentCreateViewRequest.ViewContainerType.Workspace)
            .WithName(name)
            .WithType(type)
            .WithPrivate(isPrivate);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedView, result);
        _mockViewsService.Verify(x => x.CreateWorkspaceViewAsync(
            containerId,
            It.Is<CreateViewRequest>(req =>
                req.Name == name &&
                req.Type == type &&
                req.Settings!.Sharing == (isPrivate ? "private" : "public")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SpaceContainerType_ShouldCallCreateSpaceViewAsync()
    {
        // Arrange
        var containerId = "testSpaceId";
        var name = "testViewName";
        var type = "board";
        var expectedView = new View("viewId", "viewName", "viewType");
        var expectedResponse = new CreateSpaceViewResponse { View = expectedView };

        _mockViewsService.Setup(x => x.CreateSpaceViewAsync(
            It.IsAny<string>(),
            It.IsAny<CreateViewRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new FluentCreateViewRequest(containerId, _mockViewsService.Object, FluentCreateViewRequest.ViewContainerType.Space)
            .WithName(name)
            .WithType(type);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedView, result);
        _mockViewsService.Verify(x => x.CreateSpaceViewAsync(
            containerId,
            It.Is<CreateViewRequest>(req =>
                req.Name == name &&
                req.Type == type),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_FolderContainerType_ShouldCallCreateFolderViewAsync()
    {
        // Arrange
        var containerId = "testFolderId";
        var name = "testViewName";
        var type = "calendar";
        var expectedView = new View("viewId", "viewName", "viewType");
        var expectedResponse = new CreateFolderViewResponse { View = expectedView };

        _mockViewsService.Setup(x => x.CreateFolderViewAsync(
            It.IsAny<string>(),
            It.IsAny<CreateViewRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new FluentCreateViewRequest(containerId, _mockViewsService.Object, FluentCreateViewRequest.ViewContainerType.Folder)
            .WithName(name)
            .WithType(type);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedView, result);
        _mockViewsService.Verify(x => x.CreateFolderViewAsync(
            containerId,
            It.Is<CreateViewRequest>(req =>
                req.Name == name &&
                req.Type == type),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ListContainerType_ShouldCallCreateListViewAsync()
    {
        // Arrange
        var containerId = "testListId";
        var name = "testViewName";
        var type = "gantt";
        var expectedView = new View("viewId", "viewName", "viewType");
        var expectedResponse = new CreateListViewResponse { View = expectedView };

        _mockViewsService.Setup(x => x.CreateListViewAsync(
            It.IsAny<string>(),
            It.IsAny<CreateViewRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new FluentCreateViewRequest(containerId, _mockViewsService.Object, FluentCreateViewRequest.ViewContainerType.List)
            .WithName(name)
            .WithType(type);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedView, result);
        _mockViewsService.Verify(x => x.CreateListViewAsync(
            containerId,
            It.Is<CreateViewRequest>(req =>
                req.Name == name &&
                req.Type == type),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateViewAsyncWithAllOptionalParameters()
    {
        // Arrange
        var containerId = "testWorkspaceId";
        var name = "testViewName";
        var type = "list";
        var isPrivate = false;
        var grouping = new ViewGrouping();
        var divide = new ViewDivide();
        var sorting = new ViewSorting();
        var filters = new ViewFilters();
        var columns = new ViewColumns();
        var teamSidebar = new ViewTeamSidebar();
        var settings = new ViewSettings();
        var expectedView = new View("viewId", "viewName", "viewType");
        var expectedResponse = new CreateTeamViewResponse { View = expectedView };

        _mockViewsService.Setup(x => x.CreateWorkspaceViewAsync(
            It.IsAny<string>(),
            It.IsAny<CreateViewRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new FluentCreateViewRequest(containerId, _mockViewsService.Object, FluentCreateViewRequest.ViewContainerType.Workspace)
            .WithName(name)
            .WithType(type)
            .WithPrivate(isPrivate)
            .WithGrouping(grouping)
            .WithDivide(divide)
            .WithSorting(sorting)
            .WithFilters(filters)
            .WithColumns(columns)
            .WithTeamSidebar(teamSidebar)
            .WithSettings(settings);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedView, result);
        _mockViewsService.Verify(x => x.CreateWorkspaceViewAsync(
            containerId,
            It.IsAny<CreateViewRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateViewAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var containerId = "testWorkspaceId";
        var name = "testViewName";
        var type = "list";
        var expectedView = new View("viewId", "viewName", "viewType");
        var expectedResponse = new CreateTeamViewResponse { View = expectedView };

        _mockViewsService.Setup(x => x.CreateWorkspaceViewAsync(
            It.IsAny<string>(),
            It.IsAny<CreateViewRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentRequest = new FluentCreateViewRequest(containerId, _mockViewsService.Object, FluentCreateViewRequest.ViewContainerType.Workspace)
            .WithName(name)
            .WithType(type);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedView, result);
        _mockViewsService.Verify(x => x.CreateWorkspaceViewAsync(
            containerId,
            It.IsAny<CreateViewRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
