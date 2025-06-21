using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.ResponseModels; // Namespace from service
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic; // For List in DTOs

namespace ClickUp.Api.Client.Tests.Services
{
    public class SharedHierarchyServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly SharedHierarchyService _sharedHierarchyService;

        public SharedHierarchyServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _sharedHierarchyService = new SharedHierarchyService(_mockApiConnection.Object);
        }

        // Placeholder DTO creation helper
        private SharedHierarchy CreateSampleSharedHierarchy()
        {
            // Assuming SharedHierarchy has some properties we can check
            // For now, just creating an instance is enough as BeEquivalentTo will check structure.
            // If SharedHierarchy contains lists of tasks, lists, folders, those would be initialized here.
            var hierarchy = (SharedHierarchy)Activator.CreateInstance(typeof(SharedHierarchy), nonPublic: true)!;
            // Example: typeof(SharedHierarchy).GetProperty("Tasks")?.SetValue(hierarchy, new List<Models.Entities.CuTask>());
            return hierarchy;
        }

        [Fact]
        public async Task GetSharedHierarchyAsync_WhenDataExists_ReturnsSharedHierarchy()
        {
            // Arrange
            var workspaceId = "ws-id-shared";
            var expectedHierarchy = CreateSampleSharedHierarchy();

            _mockApiConnection.Setup(c => c.GetAsync<SharedHierarchy>(
                $"team/{workspaceId}/shared",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedHierarchy);

            // Act
            var result = await _sharedHierarchyService.GetSharedHierarchyAsync(workspaceId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedHierarchy); // BeEquivalentTo performs a deep comparison
            _mockApiConnection.Verify(c => c.GetAsync<SharedHierarchy>(
                $"team/{workspaceId}/shared",
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
