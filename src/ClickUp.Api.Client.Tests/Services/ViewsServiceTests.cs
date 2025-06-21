using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Tests.Services
{
    public class ViewsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly ViewsService _viewsService;

        public ViewsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _viewsService = new ViewsService(_mockApiConnection.Object);
        }

        private View CreateSampleView(string id, string name)
        {
            var view = (View)Activator.CreateInstance(typeof(View), nonPublic: true)!;
            var props = typeof(View).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(view, id);
            props.FirstOrDefault(p => p.Name == "Name")?.SetValue(view, name);
            return view;
        }

        private GetViewResponse CreateSampleGetViewResponse(View view)
        {
            var responseType = typeof(GetViewResponse);
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(View));
            if (constructor != null)
            {
                return (GetViewResponse)constructor.Invoke(new object[] { view });
            }
            var instance = (GetViewResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("View")?.SetValue(instance, view);
            return instance;
        }

        [Fact]
        public async Task GetViewAsync_WhenViewExists_ReturnsView()
        {
            // Arrange
            var viewId = "view-id-123";
            var expectedView = CreateSampleView(viewId, "Test View");
            var expectedResponse = CreateSampleGetViewResponse(expectedView);

            _mockApiConnection.Setup(c => c.GetAsync<GetViewResponse>(
                $"view/{viewId}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _viewsService.GetViewAsync(viewId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedView);
        }

        [Fact]
        public async Task CreateListViewAsync_ValidRequest_ReturnsCreatedView()
        {
            // Arrange
            var listId = "list-id-for-view";
            var requestDto = new CreateViewRequest("New List View", "list"); // Assuming type is required
            var expectedView = CreateSampleView("new-view-id", "New List View");
            var expectedResponse = CreateSampleGetViewResponse(expectedView);

            _mockApiConnection.Setup(c => c.PostAsync<CreateViewRequest, GetViewResponse>(
                $"list/{listId}/view",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _viewsService.CreateListViewAsync(listId, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedView);
        }
    }
}
