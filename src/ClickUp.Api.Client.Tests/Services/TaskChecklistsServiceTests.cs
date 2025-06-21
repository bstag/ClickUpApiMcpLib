using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.TaskChecklists;
using ClickUp.Api.Client.Models.ResponseModels; // For GetChecklistResponse
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using ClickUp.Api.Client.Models.Entities.Checklists;
using ClickUp.Api.Client.Models.RequestModels.Checklists;

namespace ClickUp.Api.Client.Tests.Services
{
    public class TaskChecklistsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TaskChecklistsService _taskChecklistsService;

        public TaskChecklistsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _taskChecklistsService = new TaskChecklistsService(_mockApiConnection.Object);
        }

        private Checklist CreateSampleChecklist(string id, string name)
        {
            var checklist = (Checklist)Activator.CreateInstance(typeof(Checklist), nonPublic: true)!;
            var props = typeof(Checklist).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(checklist, id);
            props.FirstOrDefault(p => p.Name == "Name")?.SetValue(checklist, name);
            return checklist;
        }

        // Assuming GetChecklistResponse wraps a Checklist object
        private GetChecklistResponse CreateSampleGetChecklistResponse(Checklist checklist)
        {
            var responseType = typeof(GetChecklistResponse);
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(Checklist));
            if (constructor != null)
            {
                return (GetChecklistResponse)constructor.Invoke(new object[] { checklist });
            }
            var instance = (GetChecklistResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("Checklist")?.SetValue(instance, checklist);
            return instance;
        }


        [Fact]
        public async Task CreateChecklistAsync_ValidRequest_ReturnsChecklist()
        {
            // Arrange
            var taskId = "task-id-for-checklist";
            var requestDto = new CreateChecklistRequest("New Checklist");
            var sampleChecklist = CreateSampleChecklist("checklist-1", "New Checklist");
            var expectedResponse = CreateSampleGetChecklistResponse(sampleChecklist);

            _mockApiConnection.Setup(c => c.PostAsync<CreateChecklistRequest, GetChecklistResponse>(
                It.Is<string>(s => s.StartsWith($"task/{taskId}/checklist")),
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _taskChecklistsService.CreateChecklistAsync(taskId, requestDto, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(sampleChecklist);
        }

        [Fact]
        public async Task DeleteChecklistAsync_ValidId_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var checklistId = "checklist-to-delete";
            var expectedEndpoint = $"checklist/{checklistId}";

            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _taskChecklistsService.DeleteChecklistAsync(checklistId, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
