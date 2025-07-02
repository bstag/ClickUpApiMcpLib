using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.Entities.Users; // For User
using ClickUp.Api.Client.Models.RequestModels.Tags;
using ClickUp.Api.Client.Models.ResponseModels.Spaces;
using ClickUp.Api.Client.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests
{
    public class TagsServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TagsService _tagsService;
        private readonly Mock<ILogger<TagsService>> _mockLogger;

        public TagsServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _mockLogger = new Mock<ILogger<TagsService>>();
            _tagsService = new TagsService(_mockApiConnection.Object, _mockLogger.Object);
        }

        private User CreateSampleUserForTag(int id = 1)
        {
            return new User(
                Id: id,
                Username: "Test User " + id,
                Email: $"testuser{id}@example.com",
                Color: null,
                ProfilePicture: null,
                Initials: "TU",
                Role: null,
                CustomRole: null,
                LastActive: null,
                DateJoined: null,
                DateInvited: null,
                ProfileInfo: null
            );
        }
        private Tag CreateSampleTagEntity(string name = "Sample Tag", User? creator = null) // Renamed to avoid confusion with TagPayload
        {
            return new Tag(
                Name: name,
                TagFg: "#FFFFFF",
                TagBg: "#000000",
                Creator: creator
            );
        }

        // --- Tests for GetSpaceTagsAsync ---

        [Fact]
        public async Task GetSpaceTagsAsync_ValidSpaceId_ReturnsTags()
        {
            // Arrange
            var spaceId = "space123";
            var expectedTags = new List<Tag> {
                CreateSampleTagEntity("Tag1", CreateSampleUserForTag(1)),
                CreateSampleTagEntity("Tag2", CreateSampleUserForTag(2))
            };
            var apiResponse = new GetSpaceTagsResponse(expectedTags);

            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpaceTagsResponse>(
                    $"space/{spaceId}/tag",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _tagsService.GetSpaceTagsAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Tag1", result.First().Name);
            _mockApiConnection.Verify(x => x.GetAsync<GetSpaceTagsResponse>(
                $"space/{spaceId}/tag",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSpaceTagsAsync_ApiReturnsNullResponse_ReturnsEmptyEnumerable()
        {
            var spaceId = "space_null_api_resp";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpaceTagsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetSpaceTagsResponse?)null);
            var result = await _tagsService.GetSpaceTagsAsync(spaceId);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSpaceTagsAsync_ApiReturnsResponseWithNullTags_ReturnsEmptyEnumerable()
        {
            var spaceId = "space_null_tags_in_resp";
            var apiResponse = new GetSpaceTagsResponse(null!);
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpaceTagsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);
            var result = await _tagsService.GetSpaceTagsAsync(spaceId);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSpaceTagsAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            var spaceId = "space_http_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpaceTagsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _tagsService.GetSpaceTagsAsync(spaceId));
        }

        [Fact]
        public async Task GetSpaceTagsAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_cancel_ex";
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpaceTagsResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _tagsService.GetSpaceTagsAsync(spaceId, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task GetSpaceTagsAsync_PassesCancellationTokenToApiConnection()
        {
            var spaceId = "space_ct_pass";
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.GetAsync<GetSpaceTagsResponse>(It.IsAny<string>(), expectedToken))
                .ReturnsAsync(new GetSpaceTagsResponse(new List<Tag>()));
            await _tagsService.GetSpaceTagsAsync(spaceId, expectedToken);
            _mockApiConnection.Verify(x => x.GetAsync<GetSpaceTagsResponse>( $"space/{spaceId}/tag", expectedToken), Times.Once);
        }

        // --- Tests for CreateSpaceTagAsync ---

        [Fact]
        public async Task CreateSpaceTagAsync_ValidRequest_CallsPostAsync()
        {
            var spaceId = "space123_create_tag";
            var tagPayload = new TagAttributes("New Tag", "#FFFFFF", "#000000");
            var request = new SaveTagRequest(tagPayload);

            _mockApiConnection
                .Setup(x => x.PostAsync<SaveTagRequest>(
                    $"space/{spaceId}/tag",
                    It.IsAny<SaveTagRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            await _tagsService.CreateSpaceTagAsync(spaceId, request);
            _mockApiConnection.Verify(x => x.PostAsync<SaveTagRequest>(
                $"space/{spaceId}/tag",
                It.Is<SaveTagRequest>(r => r.Tag.Name == "New Tag"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateSpaceTagAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            var spaceId = "space_create_tag_http_ex";
            var tagPayload = new TagAttributes("Http Ex Tag", "#FF0000", "#00FF00");
            var request = new SaveTagRequest(tagPayload);
            _mockApiConnection
                .Setup(x => x.PostAsync<SaveTagRequest>(It.IsAny<string>(), It.IsAny<SaveTagRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _tagsService.CreateSpaceTagAsync(spaceId, request));
        }

        [Fact]
        public async Task CreateSpaceTagAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_create_tag_cancel_ex";
            var tagPayload = new TagAttributes("Cancel Ex Tag", "#0000FF", "#FFFF00");
            var request = new SaveTagRequest(tagPayload);
            _mockApiConnection
                .Setup(x => x.PostAsync<SaveTagRequest>(It.IsAny<string>(), It.IsAny<SaveTagRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _tagsService.CreateSpaceTagAsync(spaceId, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task CreateSpaceTagAsync_PassesCancellationTokenToApiConnection()
        {
            var spaceId = "space_create_tag_ct_pass";
            var tagPayload = new TagAttributes("CT Pass Tag", "#123456", "#654321");
            var request = new SaveTagRequest(tagPayload);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.PostAsync<SaveTagRequest>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .Returns(Task.CompletedTask);
            await _tagsService.CreateSpaceTagAsync(spaceId, request, expectedToken);
            _mockApiConnection.Verify(x => x.PostAsync<SaveTagRequest>(
                $"space/{spaceId}/tag",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for EditSpaceTagAsync ---

        [Fact]
        public async Task EditSpaceTagAsync_ValidRequest_ReturnsUpdatedTag()
        {
            var spaceId = "space123_edit_tag";
            var originalTagName = "Old Tag Name";
            var encodedOriginalTagName = Uri.EscapeDataString(originalTagName);
            var updatedPayload = new TagAttributes("Updated Tag Name", "#FFFFFF", "#112233");
            var request = new SaveTagRequest(updatedPayload);
            var returnedTagEntity = new Tag(updatedPayload.Name, updatedPayload.TagFg, updatedPayload.TagBg, CreateSampleUserForTag(1));

            _mockApiConnection
                .Setup(x => x.PutAsync<SaveTagRequest, Tag>(
                    $"space/{spaceId}/tag/{encodedOriginalTagName}",
                    It.Is<SaveTagRequest>(r => r.Tag.Name == updatedPayload.Name && r.Tag.TagFg == updatedPayload.TagFg && r.Tag.TagBg == updatedPayload.TagBg),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnedTagEntity);
            var result = await _tagsService.EditSpaceTagAsync(spaceId, originalTagName, request);
            Assert.NotNull(result);
            Assert.Equal(updatedPayload.Name, result.Name);
            Assert.Equal(updatedPayload.TagBg, result.TagBg);
            _mockApiConnection.Verify(x => x.PutAsync<SaveTagRequest, Tag>(
                $"space/{spaceId}/tag/{encodedOriginalTagName}",
                It.Is<SaveTagRequest>(r => r.Tag.Name == updatedPayload.Name && r.Tag.TagFg == updatedPayload.TagFg && r.Tag.TagBg == updatedPayload.TagBg),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EditSpaceTagAsync_ApiReturnsNullResponse_ThrowsInvalidOperationException()
        {
            var spaceId = "space_edit_tag_null_resp";
            var tagName = "SomeTag";
            var payload = new TagAttributes("Updated Tag", "#777777", "#888888");
            var request = new SaveTagRequest(payload);
            _mockApiConnection
                .Setup(x => x.PutAsync<SaveTagRequest, Tag>(It.IsAny<string>(), It.IsAny<SaveTagRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Tag?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _tagsService.EditSpaceTagAsync(spaceId, tagName, request));
        }

        [Fact]
        public async Task EditSpaceTagAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            var spaceId = "space_edit_tag_http_ex";
            var tagName = "ErrorTag";
            var payload = new TagAttributes("Http Ex Edit Tag", "#999999", "#AAAAAA");
            var request = new SaveTagRequest(payload);
            _mockApiConnection
                .Setup(x => x.PutAsync<SaveTagRequest, Tag>(It.IsAny<string>(), It.IsAny<SaveTagRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _tagsService.EditSpaceTagAsync(spaceId, tagName, request));
        }

        [Fact]
        public async Task EditSpaceTagAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_edit_tag_cancel_ex";
            var tagName = "CancelTag";
            var payload = new TagAttributes("Cancel Ex Edit Tag", "#BBBBBB", "#CCCCCC");
            var request = new SaveTagRequest(payload);
            _mockApiConnection
                .Setup(x => x.PutAsync<SaveTagRequest, Tag>(It.IsAny<string>(), It.IsAny<SaveTagRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _tagsService.EditSpaceTagAsync(spaceId, tagName, request, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task EditSpaceTagAsync_PassesCancellationTokenToApiConnection()
        {
            var spaceId = "space_edit_tag_ct_pass";
            var tagName = "CTTag";
            var encodedTagName = Uri.EscapeDataString(tagName);
            var payload = new TagAttributes("CT Pass Edit Tag", "#DDDDDD", "#EEEEEE");
            var request = new SaveTagRequest(payload);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            var expectedTag = CreateSampleTagEntity("CT Pass Edit Tag", CreateSampleUserForTag(1));

            _mockApiConnection
                .Setup(x => x.PutAsync<SaveTagRequest, Tag>(
                    It.IsAny<string>(),
                    request,
                    expectedToken))
                .ReturnsAsync(expectedTag);
            await _tagsService.EditSpaceTagAsync(spaceId, tagName, request, expectedToken);
            _mockApiConnection.Verify(x => x.PutAsync<SaveTagRequest, Tag>(
                $"space/{spaceId}/tag/{encodedTagName}",
                request,
                expectedToken), Times.Once);
        }

        // --- Tests for DeleteSpaceTagAsync ---

        [Fact]
        public async Task DeleteSpaceTagAsync_ValidRequest_CallsDeleteAsync()
        {
            var spaceId = "space123_delete_tag";
            var tagName = "TagToDelete";
            var encodedTagName = Uri.EscapeDataString(tagName);
            _mockApiConnection
                .Setup(x => x.DeleteAsync( $"space/{spaceId}/tag/{encodedTagName}", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            await _tagsService.DeleteSpaceTagAsync(spaceId, tagName);
            _mockApiConnection.Verify(x => x.DeleteAsync( $"space/{spaceId}/tag/{encodedTagName}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSpaceTagAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            var spaceId = "space_delete_tag_http_ex";
            var tagName = "ErrorDeleteTag";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _tagsService.DeleteSpaceTagAsync(spaceId, tagName));
        }

        [Fact]
        public async Task DeleteSpaceTagAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var spaceId = "space_delete_tag_cancel_ex";
            var tagName = "CancelDeleteTag";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _tagsService.DeleteSpaceTagAsync(spaceId, tagName, new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task DeleteSpaceTagAsync_PassesCancellationTokenToApiConnection()
        {
            var spaceId = "space_delete_tag_ct_pass";
            var tagName = "CTDeleteTag";
            var encodedTagName = Uri.EscapeDataString(tagName);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.DeleteAsync( It.IsAny<string>(), expectedToken))
                .Returns(Task.CompletedTask);
            await _tagsService.DeleteSpaceTagAsync(spaceId, tagName, expectedToken);
            _mockApiConnection.Verify(x => x.DeleteAsync( $"space/{spaceId}/tag/{encodedTagName}", expectedToken), Times.Once);
        }

        // --- Tests for AddTagToTaskAsync ---

        [Fact]
        public async Task AddTagToTaskAsync_ValidRequest_CallsPostAsyncWithCorrectUrl()
        {
            var taskId = "task_add_tag_1";
            var tagName = "Important Tag";
            var encodedTagName = Uri.EscapeDataString(tagName);
            _mockApiConnection
                .Setup(x => x.PostAsync<object>(
                    $"task/{taskId}/tag/{encodedTagName}",
                    It.IsAny<object>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            await _tagsService.AddTagToTaskAsync(taskId, tagName);
            _mockApiConnection.Verify(x => x.PostAsync<object>(
                $"task/{taskId}/tag/{encodedTagName}",
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTagToTaskAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectUrl()
        {
            var taskId = "custom_task_id_for_tag";
            var tagName = "CustomIDTag";
            var encodedTagName = Uri.EscapeDataString(tagName);
            var customTaskIds = true;
            var teamId = "team_abc";
            _mockApiConnection
                .Setup(x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            await _tagsService.AddTagToTaskAsync(taskId, tagName, customTaskIds, teamId);
            _mockApiConnection.Verify(x => x.PostAsync<object>(
                $"task/{taskId}/tag/{encodedTagName}?custom_task_ids=true&team_id={teamId}",
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTagToTaskAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            var taskId = "task_add_tag_http_ex";
            var tagName = "ErrorAddTag";
            _mockApiConnection
                .Setup(x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _tagsService.AddTagToTaskAsync(taskId, tagName));
        }

        [Fact]
        public async Task AddTagToTaskAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var taskId = "task_add_tag_cancel_ex";
            var tagName = "CancelAddTag";
            _mockApiConnection
                .Setup(x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _tagsService.AddTagToTaskAsync(taskId, tagName, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task AddTagToTaskAsync_PassesCancellationTokenToApiConnection()
        {
            var taskId = "task_add_tag_ct_pass";
            var tagName = "CTAddTag";
            var encodedTagName = Uri.EscapeDataString(tagName);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.PostAsync<object>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    expectedToken))
                .Returns(Task.CompletedTask);
            await _tagsService.AddTagToTaskAsync(taskId, tagName, cancellationToken: expectedToken);
            _mockApiConnection.Verify(x => x.PostAsync<object>(
                $"task/{taskId}/tag/{encodedTagName}",
                It.IsAny<object>(),
                expectedToken), Times.Once);
        }

        // --- Tests for RemoveTagFromTaskAsync ---

        [Fact]
        public async Task RemoveTagFromTaskAsync_ValidRequest_CallsDeleteAsyncWithCorrectUrl()
        {
            var taskId = "task_remove_tag_1";
            var tagName = "Obsolete Tag";
            var encodedTagName = Uri.EscapeDataString(tagName);
            _mockApiConnection
                .Setup(x => x.DeleteAsync( $"task/{taskId}/tag/{encodedTagName}", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            await _tagsService.RemoveTagFromTaskAsync(taskId, tagName);
            _mockApiConnection.Verify(x => x.DeleteAsync( $"task/{taskId}/tag/{encodedTagName}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveTagFromTaskAsync_WithCustomTaskIdsAndTeamId_BuildsCorrectUrl()
        {
            var taskId = "custom_task_id_for_remove_tag";
            var tagName = "CustomIDRemoveTag";
            var encodedTagName = Uri.EscapeDataString(tagName);
            var customTaskIds = false;
            var teamId = "team_xyz";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            await _tagsService.RemoveTagFromTaskAsync(taskId, tagName, customTaskIds, teamId);
            _mockApiConnection.Verify(x => x.DeleteAsync(
                $"task/{taskId}/tag/{encodedTagName}?custom_task_ids=false&team_id={teamId}",
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveTagFromTaskAsync_ApiConnectionThrowsHttpRequestException_PropagatesException()
        {
            var taskId = "task_remove_tag_http_ex";
            var tagName = "ErrorRemoveTag";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API call failed"));
            await Assert.ThrowsAsync<HttpRequestException>(() => _tagsService.RemoveTagFromTaskAsync(taskId, tagName));
        }

        [Fact]
        public async Task RemoveTagFromTaskAsync_ApiConnectionThrowsTaskCanceledException_PropagatesException()
        {
            var taskId = "task_remove_tag_cancel_ex";
            var tagName = "CancelRemoveTag";
            _mockApiConnection
                .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("API call timed out"));
            await Assert.ThrowsAsync<TaskCanceledException>(() => _tagsService.RemoveTagFromTaskAsync(taskId, tagName, cancellationToken: new CancellationTokenSource().Token));
        }

        [Fact]
        public async Task RemoveTagFromTaskAsync_PassesCancellationTokenToApiConnection()
        {
            var taskId = "task_remove_tag_ct_pass";
            var tagName = "CTRemoveTag";
            var encodedTagName = Uri.EscapeDataString(tagName);
            var cts = new CancellationTokenSource();
            var expectedToken = cts.Token;
            _mockApiConnection
                .Setup(x => x.DeleteAsync( It.IsAny<string>(), expectedToken))
                .Returns(Task.CompletedTask);
            await _tagsService.RemoveTagFromTaskAsync(taskId, tagName, cancellationToken: expectedToken);
            _mockApiConnection.Verify(x => x.DeleteAsync( $"task/{taskId}/tag/{encodedTagName}", expectedToken), Times.Once);
        }
    }
}
