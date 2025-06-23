using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Templates;
using ClickUp.Api.Client.Models.Entities.Users; // For User
using ClickUp.Api.Client.Models.ResponseModels.Templates;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class TemplatesServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TemplatesService _templatesService;
        private readonly Mock<ILogger<TemplatesService>> _mockLogger;

        public TemplatesServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<TemplatesService>>();
            _templatesService = new TemplatesService(_mockApiConnection.Object, _mockLogger.Object);
        }

        // Minimal User for Creator property of TaskTemplate
        private User CreateSampleUserForTemplate(int id = 1, string username = "Template Creator")
        {
            return new User(
                Id: id,
                Username: username,
                Email: $"{username.Replace(" ", "").ToLower()}_template@example.com",
                Color: "#ABCDEF",
                ProfilePicture: null,
                Initials: "TC"
            );
        }

        private TaskTemplate CreateSampleTaskTemplate(string id = "tpl_1", string name = "Sample Template")
        {
            // TaskTemplate is a class with public setters, no primary constructor with all these fields.
            // It has a parameterless constructor implicitly or explicitly.
            return new TaskTemplate
            {
                Id = id,
                Name = name,
                Description = "A sample task template.",
                // Creator is not part of TaskTemplate DTO
                // DateCreated is not part of TaskTemplate DTO; it has DueDate and StartDate (both string?)
                DueDate = DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeMilliseconds().ToString(), // Example
                // TemplateContent DTO was not found
            };
        }

        // --- Tests for GetTaskTemplatesAsync ---

        [Fact]
        public async Task GetTaskTemplatesAsync_ValidRequest_ReturnsResponseWithTemplates()
        {
            // Arrange
            var workspaceId = "ws123";
            var page = 0;
            var expectedTemplates = new List<TaskTemplate> { CreateSampleTaskTemplate("tpl1", "Template One") };
            // GetTaskTemplatesResponse is a record with a primary constructor
            var apiResponse = new GetTaskTemplatesResponse(expectedTemplates);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetTaskTemplatesResponse>(
                    $"team/{workspaceId}/taskTemplate?page={page}",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _templatesService.GetTaskTemplatesAsync(workspaceId, page);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Templates);
            Assert.Single(result.Templates);
            Assert.Equal("tpl1", result.Templates.First().Id);
            _mockApiConnection.Verify(x => x.GetAsync<GetTaskTemplatesResponse>(
                $"team/{workspaceId}/taskTemplate?page={page}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTaskTemplatesAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var workspaceId = "ws_null_api_resp";
            var page = 0;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTaskTemplatesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetTaskTemplatesResponse?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _templatesService.GetTaskTemplatesAsync(workspaceId, page)
            );
        }

        [Fact]
        public async Task GetTaskTemplatesAsync_ApiReturnsResponseWithNullTemplates_ReturnsEmptyList()
        {
            var workspaceId = "ws_null_templates_in_resp";
            var page = 0;
            // Pass null to the constructor for the Templates list
            var apiResponse = new GetTaskTemplatesResponse(null!);
             _mockApiConnection
                .Setup(x => x.GetAsync<GetTaskTemplatesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var result = await _templatesService.GetTaskTemplatesAsync(workspaceId, page);

            Assert.NotNull(result);
            Assert.NotNull(result.Templates); // Service initializes Templates to empty list if null
            Assert.Empty(result.Templates);
        }


        [Fact]
        public async Task GetTaskTemplatesAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_http_ex";
            var page = 0;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTaskTemplatesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _templatesService.GetTaskTemplatesAsync(workspaceId, page)
            );
        }

        [Fact]
        public async Task GetTaskTemplatesAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            // Arrange
            var workspaceId = "ws_cancel_ex";
            var page = 0;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetTaskTemplatesResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                _templatesService.GetTaskTemplatesAsync(workspaceId, page, new CancellationTokenSource().Token)
            );
        }

        [Fact]
        public async Task GetTaskTemplatesAsync_PassesCancellationTokenToApiConnection()
        {
            // Arrange
            var workspaceId = "ws_ct_pass";
            var page = 0;
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            // Use constructor for GetTaskTemplatesResponse
            var apiResponse = new GetTaskTemplatesResponse(new List<TaskTemplate>());


            _mockApiConnection
                .Setup(x => x.GetAsync<GetTaskTemplatesResponse>(
                    It.IsAny<string>(),
                    expectedToken))
                .ReturnsAsync(apiResponse);

            // Act
            await _templatesService.GetTaskTemplatesAsync(workspaceId, page, expectedToken);

            // Assert
            _mockApiConnection.Verify(x => x.GetAsync<GetTaskTemplatesResponse>(
                $"team/{workspaceId}/taskTemplate?page={page}",
                expectedToken), Times.Once);
        }
    }
}
