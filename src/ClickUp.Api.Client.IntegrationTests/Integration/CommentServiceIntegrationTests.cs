using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
// Removed incorrect using ClickUp.Api.Client.Abstractions.Services.Folders;
// Removed incorrect using ClickUp.Api.Client.Abstractions.Services.Spaces;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.RequestModels.Folders;
using ClickUp.Api.Client.Models.RequestModels.Lists;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Comments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;


namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class CommentServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private readonly ICommentsService _commentService; // Corrected interface name
        private readonly ITasksService _taskService;
        private readonly IListsService _listService;
        private readonly IFoldersService _folderService;
        private readonly ISpacesService _spaceService;

        private string _testWorkspaceId;
        private string _testSpaceId = null!;
        private string _testFolderId = null!;
        private string _testListId = null!;
        private string _testTaskId = null!;

        private List<long> _createdCommentIds = new List<long>(); // Comments have numeric IDs
        private TestHierarchyContext _hierarchyContext = null!;

        public CommentServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _commentService = ServiceProvider.GetRequiredService<ICommentsService>();
            _taskService = ServiceProvider.GetRequiredService<ITasksService>();
            _listService = ServiceProvider.GetRequiredService<IListsService>();
            _folderService = ServiceProvider.GetRequiredService<IFoldersService>();
            _spaceService = ServiceProvider.GetRequiredService<ISpacesService>();

            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"];

            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Test setup will fail.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for CommentServiceIntegrationTests.");
            }
        }

        public async Task InitializeAsync()
        {
            _output.LogInformation("Starting CommentServiceIntegrationTests class initialization using TestHierarchyHelper.");
            try
            {
                _hierarchyContext = await TestHierarchyHelper.CreateFullTestHierarchyAsync(
                    _spaceService, _folderService, _listService, _taskService,
                    _testWorkspaceId, "CommentsTest", _output);

                _testSpaceId = _hierarchyContext.SpaceId;
                _testFolderId = _hierarchyContext.FolderId;
                _testListId = _hierarchyContext.ListId;
                _testTaskId = _hierarchyContext.TaskId;

                _output.LogInformation($"Hierarchy created: SpaceId={_testSpaceId}, FolderId={_testFolderId}, ListId={_testListId}, TaskId={_testTaskId}");
            }
            catch (Exception ex)
            {
                _output.LogError($"Error during InitializeAsync using TestHierarchyHelper: {ex.Message}", ex);
                if (_hierarchyContext != null) // Attempt cleanup even if hierarchy creation was partial
                {
                    await TestHierarchyHelper.TeardownHierarchyAsync(_spaceService, _hierarchyContext, _output);
                }
                throw; // Re-throw to fail the test class setup
            }
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation("Starting CommentServiceIntegrationTests class disposal using TestHierarchyHelper.");
            _createdCommentIds.Clear();
            if (_hierarchyContext != null)
            {
                await TestHierarchyHelper.TeardownHierarchyAsync(_spaceService, _hierarchyContext, _output);
            }
            _output.LogInformation("CommentServiceIntegrationTests class disposal complete.");
        }

        // Removed CleanupLingeringResourcesAsync as TestHierarchyHelper.TeardownHierarchyAsync handles it.

        private void RegisterCreatedComment(long commentId)
        {
            _createdCommentIds.Add(commentId);
        }

        [Fact]
        public async Task CreateTaskCommentAsync_WithValidData_ShouldCreateComment()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            var commentText = $"This is a test comment - {Guid.NewGuid()}";
            // Corrected: CreateTaskCommentRequest(string CommentText, int? Assignee, string? GroupAssignee, bool NotifyAll)
            var createCommentRequest = new CreateTaskCommentRequest(commentText, null, null, false);

            _output.LogInformation($"Attempting to create comment on task '{_testTaskId}'.");
            CreateCommentResponse createdCommentInfo = null;
            try
            {
                createdCommentInfo = await _commentService.CreateTaskCommentAsync(_testTaskId, createCommentRequest);
                if (createdCommentInfo != null)
                {
                    // Assuming createdCommentInfo.Id is a string that can be parsed to long for RegisterCreatedComment
                    if (long.TryParse(createdCommentInfo.Id, out var commentIdLong))
                    {
                        RegisterCreatedComment(commentIdLong);
                    }
                    else
                    {
                        _output.LogWarning($"Could not parse comment ID '{createdCommentInfo.Id}' to long for registration.");
                    }
                    _output.LogInformation($"Comment created. ID: {createdCommentInfo.Id}, Date from nested comment: {createdCommentInfo.Comment?.Date}");
                }
            }
            catch (Exception ex)
            {
                _output.LogError($"Exception during CreateTaskCommentAsync: {ex.Message}", ex);
                Assert.Fail($"CreateTaskCommentAsync threw an exception: {ex.Message}");
            }

            Assert.NotNull(createdCommentInfo);
            Assert.False(string.IsNullOrWhiteSpace(createdCommentInfo.Id)); // Check if ID is not empty
            Assert.NotNull(createdCommentInfo.Comment); // Ensure nested comment object exists
            Assert.False(string.IsNullOrWhiteSpace(createdCommentInfo.Comment.Date)); // Check if Date string is not empty

            // Fetch the comment to verify its content (GetTaskCommentsAsync)
            var commentsResponse = await _commentService.GetTaskCommentsAsync(new GetTaskCommentsRequest(_testTaskId));
            // commentsResponse is IEnumerable<Comment>, no .Comments property
            var retrievedComment = commentsResponse.FirstOrDefault(c => c.Id == createdCommentInfo.Id);
            Assert.NotNull(retrievedComment);
            Assert.Equal(commentText, retrievedComment.CommentText);
        }

        [Fact]
        public async Task GetTaskCommentsAsync_WithExistingComments_ShouldReturnComments()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            var commentText1 = $"Comment 1 for get test - {Guid.NewGuid()}";
            var commentText2 = $"Comment 2 for get test - {Guid.NewGuid()}";

            var comment1Info = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest(commentText1, null, null, false));
            if (long.TryParse(comment1Info.Id, out var id1)) RegisterCreatedComment(id1);
            var comment2Info = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest(commentText2, null, null, false));
            if (long.TryParse(comment2Info.Id, out var id2)) RegisterCreatedComment(id2);
            _output.LogInformation($"Created comments {comment1Info.Id}, {comment2Info.Id} for GetTaskCommentsAsync test.");

            var commentsEnumerable = await _commentService.GetTaskCommentsAsync(new GetTaskCommentsRequest(_testTaskId));

            Assert.NotNull(commentsEnumerable);
            Assert.True(commentsEnumerable.Count() >= 2);

            var c1 = commentsEnumerable.FirstOrDefault(c => c.Id == comment1Info.Id);
            var c2 = commentsEnumerable.FirstOrDefault(c => c.Id == comment2Info.Id);

            Assert.NotNull(c1);
            Assert.Equal(commentText1, c1.CommentText);
            Assert.NotNull(c2);
            Assert.Equal(commentText2, c2.CommentText);
        }

        [Fact]
        public async Task UpdateCommentAsync_WithValidData_ShouldUpdateComment()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            var initialCommentText = $"Initial comment for update - {Guid.NewGuid()}";
            var createdCommentInfo = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest(initialCommentText, null, null, false));
            if (long.TryParse(createdCommentInfo.Id, out var id)) RegisterCreatedComment(id);
            else _output.LogWarning($"Could not parse ID for RegisterCreatedComment: {createdCommentInfo.Id}");
            _output.LogInformation($"Comment created for Update test. ID: {createdCommentInfo.Id}");

            var updatedCommentText = $"Updated comment text - {Guid.NewGuid()}";
            var updateCommentRequest = new UpdateCommentRequest(CommentText: updatedCommentText, Assignee: null, Resolved: false, NotifyAll: null);

            _output.LogInformation($"Attempting to update comment '{createdCommentInfo.Id}'.");
            await _commentService.UpdateCommentAsync(createdCommentInfo.Id, updateCommentRequest); // Pass ID as string
            _output.LogInformation($"Comment updated.");

            var commentsEnumerable = await _commentService.GetTaskCommentsAsync(new GetTaskCommentsRequest(_testTaskId));
            var updatedComment = commentsEnumerable.FirstOrDefault(c => c.Id == createdCommentInfo.Id);

            Assert.NotNull(updatedComment);
            Assert.Equal(updatedCommentText, updatedComment.CommentText);
        }

        [Fact]
        public async Task DeleteCommentAsync_WithExistingComment_ShouldDeleteComment()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            var commentText = $"Comment to delete - {Guid.NewGuid()}";
            var createdCommentInfo = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest(commentText, null, null, false));
            // Do NOT register for auto-cleanup here.
            _output.LogInformation($"Comment created for Delete test. ID: {createdCommentInfo.Id}");

            await _commentService.DeleteCommentAsync(createdCommentInfo.Id); // Pass ID as string
            _output.LogInformation($"DeleteCommentAsync called for comment ID: {createdCommentInfo.Id}.");

            var commentsEnumerable = await _commentService.GetTaskCommentsAsync(new GetTaskCommentsRequest(_testTaskId));
            var deletedComment = commentsEnumerable.FirstOrDefault(c => c.Id == createdCommentInfo.Id);

            Assert.Null(deletedComment); // The comment should no longer be in the list
            _output.LogInformation($"Verified comment {createdCommentInfo.Id} is deleted.");
        }

        [Fact]
        public async Task GetTaskCommentsStreamAsync_ShouldRetrieveAllComments()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");

            int commentsToCreate = 5; // More than a typical small page size to ensure pagination is triggered if applicable
            var createdCommentTexts = new List<string>();
            var createdCommentApiIds = new List<long>();

            _output.LogInformation($"Creating {commentsToCreate} comments for pagination stream test on task '{_testTaskId}'.");
            for (int i = 0; i < commentsToCreate; i++)
            {
                var commentText = $"Paginated Comment {i + 1} - {Guid.NewGuid()}";
                createdCommentTexts.Add(commentText);
                var createReq = new CreateTaskCommentRequest(commentText, null, null, false);
                var createdCommentInfo = await _commentService.CreateTaskCommentAsync(_testTaskId, createReq);
                // Don't register these for individual cleanup as the task itself will be cleaned.
                // But we need their API IDs if we want to verify them later.
                if (long.TryParse(createdCommentInfo.Id, out var commentIdLong))
                {
                    createdCommentApiIds.Add(commentIdLong);
                }
                else
                {
                    _output.LogWarning($"Could not parse comment ID '{createdCommentInfo.Id}' to long in stream test.");
                }
                _output.LogInformation($"Created comment {i+1}/{commentsToCreate}, API ID: {createdCommentInfo.Id}");
                await Task.Delay(200); // Small delay to avoid hitting rate limits rapidly if API is sensitive
            }

            var retrievedComments = new List<ClickUp.Api.Client.Models.Entities.Comments.Comment>();
            int count = 0;
            _output.LogInformation($"Starting to stream comments for task '{_testTaskId}'.");

            await foreach (var comment in _commentService.GetTaskCommentsStreamAsync(_testTaskId))
            {
                count++;
                retrievedComments.Add(comment);
                _output.LogInformation($"Streamed comment {count}: ID {comment.Id}, Text: '{comment.CommentText?.Substring(0, Math.Min(20, comment.CommentText.Length))}'...");
                // It's good practice to check for cancellation if the test might run long
                // cancellationToken.ThrowIfCancellationRequested();
            }

            _output.LogInformation($"Finished streaming. Total comments received: {count}");

            Assert.Equal(commentsToCreate, count);
            Assert.Equal(commentsToCreate, retrievedComments.Count);

            // Verify that all created comments were retrieved (order might vary depending on API default)
            foreach (var createdIdLong in createdCommentApiIds)
            {
                Assert.Contains(retrievedComments, rc => rc.Id == createdIdLong.ToString());
            }
             _output.LogInformation($"All {commentsToCreate} created comments were found in the streamed results.");
        }

        [Fact]
        public async Task UpdateCommentAsync_WithNonExistentCommentId_ShouldThrowNotFoundException()
        {
            var nonExistentCommentId = "0"; // ClickUp comment IDs are numeric, "0" is unlikely to exist
            var updateRequest = new UpdateCommentRequest(CommentText: "Attempt to update non-existent comment");
            _output.LogInformation($"Attempting to update non-existent comment with ID: {nonExistentCommentId}");

            // Assuming comment IDs are passed as strings to the service method, matching DeleteCommentAsync
            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _commentService.UpdateCommentAsync(nonExistentCommentId, updateRequest)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException for UpdateCommentAsync: {exception.Message}");
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task DeleteCommentAsync_WithNonExistentCommentId_ShouldThrowNotFoundException()
        {
            var nonExistentCommentId = "0";
            _output.LogInformation($"Attempting to delete non-existent comment with ID: {nonExistentCommentId}");

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _commentService.DeleteCommentAsync(nonExistentCommentId)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException for DeleteCommentAsync: {exception.Message}");
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetListCommentsStreamAsync_ShouldRetrieveAllListComments()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available for this test.");

            int commentsToCreate = 3; // Create a few comments directly on the list
            var createdCommentApiIds = new List<long>();

            _output.LogInformation($"Creating {commentsToCreate} comments for list comments stream test on list '{_testListId}'.");
            for (int i = 0; i < commentsToCreate; i++)
            {
                var commentText = $"List Comment {i + 1} - {Guid.NewGuid()}";
                var createReq = new CreateCommentRequest { CommentText = commentText, Assignee = null, NotifyAll = false };
                var createdCommentInfo = await _commentService.CreateListCommentAsync(_testListId, createReq);

                if (long.TryParse(createdCommentInfo.Id, out var commentIdLong))
                {
                    createdCommentApiIds.Add(commentIdLong);
                    // We don't need to register these for individual cleanup as the list will be cleaned up.
                }
                else
                {
                    _output.LogWarning($"Could not parse list comment ID '{createdCommentInfo.Id}' to long in stream test.");
                }
                _output.LogInformation($"Created list comment {i + 1}/{commentsToCreate}, API ID: {createdCommentInfo.Id}");
                await Task.Delay(250); // API niceness
            }

            var retrievedComments = new List<ClickUp.Api.Client.Models.Entities.Comments.Comment>();
            int count = 0;
            _output.LogInformation($"Starting to stream comments for list '{_testListId}'.");

            await foreach (var comment in _commentService.GetListCommentsStreamAsync(_testListId))
            {
                count++;
                retrievedComments.Add(comment);
                _output.LogInformation($"Streamed list comment {count}: ID {comment.Id}, Text: '{comment.CommentText?.Substring(0, Math.Min(20, comment.CommentText.Length))}'...");
            }

            _output.LogInformation($"Finished streaming list comments. Total comments received: {count}");

            Assert.Equal(commentsToCreate, count);
            Assert.Equal(commentsToCreate, retrievedComments.Count);

            foreach (var createdIdLong in createdCommentApiIds)
            {
                Assert.Contains(retrievedComments, rc => rc.Id == createdIdLong.ToString());
            }
            _output.LogInformation($"All {commentsToCreate} created list comments were found in the streamed results for list '{_testListId}'.");
        }

        // Test for GetChatViewCommentsStreamAsync would require creating a Chat View, which is more complex.
        // Placeholder for now, or implement if view creation is simple enough via API.
        // For now, this test is skipped as it requires a view_id that is hard to create programmatically without more services.
        // [Fact]
        // public async Task GetChatViewCommentsStreamAsync_ShouldRetrieveAllChatViewComments() { /* ... */ }
    }
}
