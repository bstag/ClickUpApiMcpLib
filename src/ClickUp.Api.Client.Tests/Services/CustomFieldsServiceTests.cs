using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;
using ClickUp.Api.Client.Models.ResponseModels.CustomFields;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class CustomFieldsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly CustomFieldsService _customFieldsService;

        public CustomFieldsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _customFieldsService = new CustomFieldsService(_mockApiConnection.Object);
        }

        private CustomField CreateSampleCustomField(string id, string name)
        {
            // Simplified placeholder
            var field = (CustomField)Activator.CreateInstance(typeof(CustomField), nonPublic: true)!;
            typeof(CustomField).GetProperty("Id")?.SetValue(field, id);
            typeof(CustomField).GetProperty("Name")?.SetValue(field, name);
            return field;
        }

        [Fact]
        public async Task GetAccessibleCustomFieldsAsync_ForList_ReturnsFields()
        {
            // Arrange
            var listId = "test-list-id";
            var expectedFields = new List<CustomField> { CreateSampleCustomField("field1", "Field One") };
            var expectedResponse = new GetCustomFieldsResponse(expectedFields); // Assumes GetCustomFieldsResponse wraps List<CustomField>

            _mockApiConnection.Setup(c => c.GetAsync<GetCustomFieldsResponse>(
                $"list/{listId}/field",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _customFieldsService.GetAccessibleCustomFieldsAsync(listId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedFields);
            _mockApiConnection.Verify(c => c.GetAsync<GetCustomFieldsResponse>(
                $"list/{listId}/field",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // Note: ICustomFieldsService.GetAccessibleCustomFieldsAsync takes listId.
        // The prompt mentioned testing for CuTask, but the interface method is specific to List.
        // If there was a GetTaskCustomFieldsAsync, a similar test would be added for it.
        // For now, I will test GetFolderCustomFieldsAsync as another example of a 'Get * Collection' method.

        [Fact]
        public async Task GetFolderCustomFieldsAsync_ForFolder_ReturnsFields()
        {
            // Arrange
            var folderId = "test-folder-id";
            var expectedFields = new List<CustomField> { CreateSampleCustomField("field_folder_1", "Folder Field One") };
            var expectedResponse = new GetCustomFieldsResponse(expectedFields);

            _mockApiConnection.Setup(c => c.GetAsync<GetCustomFieldsResponse>(
                $"folder/{folderId}/field",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _customFieldsService.GetFolderCustomFieldsAsync(folderId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedFields);
            _mockApiConnection.Verify(c => c.GetAsync<GetCustomFieldsResponse>(
                $"folder/{folderId}/field",
                It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task SetCustomFieldValueAsync_ValidRequest_CallsApiConnectionPostAsync()
        {
            // Arrange
            var taskId = "test-task-id";
            var fieldId = "test-field-id";
            var requestDto = new SetCustomFieldValueRequest("some value"); // Assuming simple constructor
            var expectedEndpoint = $"task/{taskId}/field/{fieldId}";

            _mockApiConnection.Setup(c => c.PostAsync<SetCustomFieldValueRequest>( // Service method returns CuTask (void)
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _customFieldsService.SetCustomFieldValueAsync(taskId, fieldId, requestDto, cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.PostAsync<SetCustomFieldValueRequest>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetCustomFieldValueAsync_WithOptionalParams_ConstructsCorrectEndpoint()
        {
            // Arrange
            var taskId = "test-task-id";
            var fieldId = "test-field-id";
            var teamIdForQuery = "test-team-id";
            var requestDto = new SetCustomFieldValueRequest("some value");
            var expectedEndpoint = $"task/{taskId}/field/{fieldId}?custom_task_ids=true&team_id={teamIdForQuery}";

            _mockApiConnection.Setup(c => c.PostAsync<SetCustomFieldValueRequest>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _customFieldsService.SetCustomFieldValueAsync(taskId, fieldId, requestDto, customTaskIds: true, teamId: teamIdForQuery, cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.PostAsync<SetCustomFieldValueRequest>(
                expectedEndpoint,
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveCustomFieldValueAsync_ValidRequest_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var taskId = "test-task-id";
            var fieldId = "test-field-id";
            var expectedEndpoint = $"task/{taskId}/field/{fieldId}";

            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _customFieldsService.RemoveCustomFieldValueAsync(taskId, fieldId, cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
