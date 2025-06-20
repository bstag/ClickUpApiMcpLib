using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Tags;
using ClickUp.Api.Client.Models.ResponseModels.Tags; // Assuming response wrappers
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClickUp.Api.Client.Tests.Services
{
    public class TagsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TagsService _tagsService;

        public TagsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _tagsService = new TagsService(_mockApiConnection.Object);
        }

        private Tag CreateSampleTag(string name, string tagFg, string tagBg)
        {
            // Simplified placeholder. Actual Tag DTO might be more complex.
            // For testing, we only need to ensure the properties we assert on are set.
            var tag = (Tag)Activator.CreateInstance(typeof(Tag), nonPublic: true)!;
            var props = typeof(Tag).GetProperties();
            props.FirstOrDefault(p => p.Name == "Name")?.SetValue(tag, name);
            props.FirstOrDefault(p => p.Name == "TagFg")?.SetValue(tag, tagFg);
            props.FirstOrDefault(p => p.Name == "TagBg")?.SetValue(tag, tagBg);
            return tag;
        }

        // Assuming GetTagsResponse wraps a list of tags
        private GetTagsResponse CreateSampleGetTagsResponse(List<Tag> tags) => new GetTagsResponse(tags);
        // Assuming EditTagResponse wraps a single tag
        private EditTagResponse CreateSampleEditTagResponse(Tag tag) => new EditTagResponse(tag);


        [Fact]
        public async Task GetSpaceTagsAsync_WhenTagsExist_ReturnsTags()
        {
            // Arrange
            var spaceId = "test-space-id";
            var expectedTags = new List<Tag> { CreateSampleTag("tag1", "#FF0000", "#00FF00") };
            var expectedResponse = CreateSampleGetTagsResponse(expectedTags);

            _mockApiConnection.Setup(c => c.GetAsync<GetTagsResponse>(
                $"space/{spaceId}/tag",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _tagsService.GetSpaceTagsAsync(spaceId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTags);
            _mockApiConnection.Verify(c => c.GetAsync<GetTagsResponse>(
                $"space/{spaceId}/tag",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateSpaceTagAsync_ValidRequest_CallsApiConnectionPostAsync()
        {
            // Arrange
            var spaceId = "test-space-id";
            var requestDto = new CreateTagRequest(CreateSampleTag("New Tag", "#FFFFFF", "#000000"));
            // CreateSpaceTagAsync in service is void, but API returns the tag.
            // We mock PostAsync<CreateTagRequest, Tag> as that's what the service calls.
            var createdTag = requestDto.Tag;

            _mockApiConnection.Setup(c => c.PostAsync<CreateTagRequest, Tag>(
                $"space/{spaceId}/tag",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdTag); // The service discards this, but the mock needs it.

            // Act
            await _tagsService.CreateSpaceTagAsync(spaceId, requestDto, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.PostAsync<CreateTagRequest, Tag>(
                $"space/{spaceId}/tag",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditSpaceTagAsync_ValidRequest_ReturnsUpdatedTag()
        {
            // Arrange
            var spaceId = "test-space-id";
            var tagName = "old-tag-name";
            var requestDto = new UpdateTagRequest(CreateSampleTag("Updated Tag", "#112233", "#AABBCC"));
            var expectedTag = requestDto.Tag; // The request DTO contains the tag to update to
            var expectedResponse = CreateSampleEditTagResponse(expectedTag);
            var encodedTagName = Uri.EscapeDataString(tagName);

            _mockApiConnection.Setup(c => c.PutAsync<UpdateTagRequest, EditTagResponse>(
                $"space/{spaceId}/tag/{encodedTagName}",
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _tagsService.EditSpaceTagAsync(spaceId, tagName, requestDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTag);
            _mockApiConnection.Verify(c => c.PutAsync<UpdateTagRequest, EditTagResponse>(
                $"space/{spaceId}/tag/{encodedTagName}",
                requestDto,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSpaceTagAsync_ValidId_CallsApiConnectionDeleteAsync()
        {
            // Arrange
            var spaceId = "test-space-id";
            var tagName = "tag-to-delete";
            var encodedTagName = Uri.EscapeDataString(tagName);
            var expectedEndpoint = $"space/{spaceId}/tag/{encodedTagName}";

            _mockApiConnection.Setup(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _tagsService.DeleteSpaceTagAsync(spaceId, tagName, CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.DeleteAsync(
                expectedEndpoint,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTagToTaskAsync_ValidRequest_CallsApiConnectionPostAsync()
        {
            // Arrange
            var taskId = "test-task-id";
            var tagName = "tag-to-add";
            var encodedTagName = Uri.EscapeDataString(tagName);
            var expectedEndpoint = $"task/{taskId}/tag/{encodedTagName}";

            _mockApiConnection.Setup(c => c.PostAsync<object>( // Service method returns Task (void), PostAsync<TRequest>
                expectedEndpoint,
                It.IsAny<object>(), // Service sends new object() {}
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _tagsService.AddTagToTaskAsync(taskId, tagName, cancellationToken: CancellationToken.None);

            // Assert
            _mockApiConnection.Verify(c => c.PostAsync<object>(
                expectedEndpoint,
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
