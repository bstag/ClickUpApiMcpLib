using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.Parameters;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using ClickUp.Api.Client.Services.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class TaskQueryServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TaskQueryService _service;

        public TaskQueryServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _service = new TaskQueryService(_mockApiConnection.Object, NullLogger<TaskQueryService>.Instance);
        }

        [Fact]
        public async Task GetTasksAsync_CorrectlyEncodes_Statuses()
        {
            // Arrange
            var listId = "123";
            string? capturedUrl = null;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) => capturedUrl = url)
                .ReturnsAsync(new GetTasksResponse(new List<CuTask>(), true));

            // Act
            await _service.GetTasksAsync(listId, p =>
            {
                p.Statuses = new[] { "In Progress" };
            });

            // Assert
            Assert.NotNull(capturedUrl);
            // Expect single encoding: "In%20Progress"
            // We want to verify we fixed the double encoding.
            Assert.Contains("statuses=In%20Progress", capturedUrl);
            Assert.DoesNotContain("In%2520Progress", capturedUrl);
        }

        [Fact]
        public async Task GetTasksAsync_CorrectlyEncodes_CustomFields()
        {
             // Arrange
            var listId = "123";
            string? capturedUrl = null;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTasksResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((url, token) => capturedUrl = url)
                .ReturnsAsync(new GetTasksResponse(new List<CuTask>(), true));

            // Act
            await _service.GetTasksAsync(listId, p =>
            {
                p.CustomFields = new List<CustomFieldFilter>
                {
                    new CustomFieldFilter { FieldId = "f1", Operator = "=", Value = "val" }
                };
            });

            // Assert
            Assert.NotNull(capturedUrl);
            // Custom fields should be encoded.
            // JSON: [{"field_id":"f1","operator":"=","value":"val"}]
            // Encoded start: %5B%7B
            Assert.Contains("custom_fields=%5B", capturedUrl);
        }
    }
}
