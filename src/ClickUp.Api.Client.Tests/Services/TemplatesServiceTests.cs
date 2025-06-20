using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
// using ClickUp.Api.Client.Models.RequestModels.Templates; // No request DTOs for current methods
using ClickUp.Api.Client.Models.ResponseModels.Templates; // Assuming GetTemplatesResponse
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class TemplatesServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TemplatesService _templatesService;

        public TemplatesServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _templatesService = new TemplatesService(_mockApiConnection.Object);
        }

        private Template CreateSampleTemplate(string id, string name)
        {
            var template = (Template)Activator.CreateInstance(typeof(Template), nonPublic: true)!;
            var props = typeof(Template).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(template, id);
            props.FirstOrDefault(p => p.Name == "Name")?.SetValue(template, name);
            return template;
        }

        private GetTemplatesResponse CreateSampleGetTemplatesResponse(List<Template> templates)
        {
            // Assuming GetTemplatesResponse has a constructor or settable property "Templates"
            var responseType = typeof(GetTemplatesResponse);
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(List<Template>));
            if (constructor != null)
            {
                return (GetTemplatesResponse)constructor.Invoke(new object[] { templates });
            }
            var instance = (GetTemplatesResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("Templates")?.SetValue(instance, templates);
            return instance;
        }

        [Fact]
        public async Task GetTaskTemplatesAsync_WhenTemplatesExist_ReturnsTemplates()
        {
            // Arrange
            var workspaceId = "ws-id";
            var page = 0;
            var expectedTemplates = new List<Template> { CreateSampleTemplate("template-1", "My Task Template") };
            var expectedResponse = CreateSampleGetTemplatesResponse(expectedTemplates);

            _mockApiConnection.Setup(c => c.GetAsync<GetTemplatesResponse>(
                $"team/{workspaceId}/taskTemplate?page={page}",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _templatesService.GetTaskTemplatesAsync(workspaceId, page, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTemplates);
            _mockApiConnection.Verify(c => c.GetAsync<GetTemplatesResponse>(
                $"team/{workspaceId}/taskTemplate?page={page}",
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
