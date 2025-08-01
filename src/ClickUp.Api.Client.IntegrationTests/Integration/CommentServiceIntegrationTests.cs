using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.IntegrationTests.TestInfrastructure;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments;
using ClickUp.Api.Client.Models.Entities.Comments; // Required for Comment model
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp; // Required for When, Respond etc.
using Xunit;
using Xunit.Abstractions;


namespace ClickUp.Api.Client.IntegrationTests.Integration
{
    [Trait("Category", "Integration")]
    public class CommentServiceIntegrationTests : IntegrationTestBase, IAsyncLifetime
    {
        private readonly ITestOutputHelper _output;
        private readonly ICommentsService _commentService;
        private readonly ITaskCrudService _taskCrudService;
        private readonly IListsService _listService;
        private readonly IFoldersService _folderService;
        private readonly ISpacesService _spaceService;
        // private readonly IViewsService _viewsService; // Would be needed for Chat View tests

        private string _testWorkspaceId;
        private string _testSpaceId = null!;
        private string _testFolderId = null!;
        private string _testListId = null!;
        private string _testTaskId = null!;
#pragma warning disable CS0414 // Field is assigned but its value is never used in active code paths
        private string _testChatViewId = "mock_chat_view_id_tests"; // Default mock ID, used in skipped tests
#pragma warning restore CS0414

        private List<string> _createdCommentIds = new List<string>(); // Comment IDs are strings in API responses
        private TestHierarchyContext _hierarchyContext = null!;

        public CommentServiceIntegrationTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _commentService = ServiceProvider.GetRequiredService<ICommentsService>();
            _taskCrudService = ServiceProvider.GetRequiredService<ITaskCrudService>();
            _listService = ServiceProvider.GetRequiredService<IListsService>();
            _folderService = ServiceProvider.GetRequiredService<IFoldersService>();
            _spaceService = ServiceProvider.GetRequiredService<ISpacesService>();
            // _viewsService = ServiceProvider.GetRequiredService<IViewsService>();

            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"]!;

            if (string.IsNullOrWhiteSpace(_testWorkspaceId) && CurrentTestMode != TestMode.Playback)
            {
                _output.LogWarning("ClickUpApi:TestWorkspaceId is not configured. Test setup will fail if not in Playback mode.");
                throw new InvalidOperationException("ClickUpApi:TestWorkspaceId must be configured for CommentServiceIntegrationTests unless in Playback mode.");
            }
        }

        public async Task InitializeAsync()
        {
            _output.LogInformation($"Initializing CommentServiceIntegrationTests in {CurrentTestMode} mode.");
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                // In Playback mode, we might need to mock hierarchy creation if tests depend on it.
                // For now, TestHierarchyHelper is designed for live calls.
                // We'll assume _testSpaceId etc. are placeholders or taken from recorded context if needed.
                _testSpaceId = "mock_space_id_comments";
                _testFolderId = "mock_folder_id_comments";
                _testListId = "mock_list_id_comments";
                _testTaskId = "mock_task_id_comments";
                _output.LogInformation($"Playback mode: Using mock IDs: SpaceId={_testSpaceId}, FolderId={_testFolderId}, ListId={_testListId}, TaskId={_testTaskId}");

                // If TestHierarchyHelper's calls were to be mocked, it would happen here.
                // Example: MockHttpHandler.When(HttpMethod.Post, $"https://api.clickup.com/api/v2/team/{_testWorkspaceId}/space")
                // .Respond("application/json", await File.ReadAllTextAsync(Path.Combine(RecordedResponsesBasePath, "SpaceService", "CreateSpace", "HierarchySetup_Space.json")));
                // For simplicity, current tests will mock direct service calls, not the helper's internals.
            }
            else // Record or Passthrough
            {
                _output.LogInformation("Starting CommentServiceIntegrationTests class initialization using TestHierarchyHelper.");
                try
                {
                    _hierarchyContext = await TestHierarchyHelper.CreateFullTestHierarchyAsync(
                        _spaceService, _folderService, _listService, _taskCrudService,
                        _testWorkspaceId, "CommentsIntTest", _output); // Shortened name

                    _testSpaceId = _hierarchyContext.SpaceId;
                    _testFolderId = _hierarchyContext.FolderId;
                    _testListId = _hierarchyContext.ListId;
                    _testTaskId = _hierarchyContext.TaskId;

                    _output.LogInformation($"Hierarchy created: SpaceId={_testSpaceId}, FolderId={_testFolderId}, ListId={_testListId}, TaskId={_testTaskId}");
                }
                catch (Exception ex)
                {
                    _output.LogError($"Error during InitializeAsync using TestHierarchyHelper: {ex.Message}", ex);
                    if (_hierarchyContext != null)
                    {
                        await TestHierarchyHelper.TeardownHierarchyAsync(_spaceService, _hierarchyContext, _output);
                    }
                    throw;
                }
            }
        }

        public async Task DisposeAsync()
        {
            _output.LogInformation($"Disposing CommentServiceIntegrationTests in {CurrentTestMode} mode.");
            _createdCommentIds.Clear();
            if (CurrentTestMode != TestMode.Playback && _hierarchyContext != null)
            {
                _output.LogInformation("Starting CommentServiceIntegrationTests class disposal using TestHierarchyHelper.");
                await TestHierarchyHelper.TeardownHierarchyAsync(_spaceService, _hierarchyContext, _output);
                _output.LogInformation("CommentServiceIntegrationTests class disposal complete.");
            }
            else
            {
                _output.LogInformation("Playback mode: Skipping real hierarchy teardown.");
            }
        }

        private void RegisterCreatedComment(string commentId) // Comment IDs are strings
        {
            if (!string.IsNullOrEmpty(commentId))
            {
                _createdCommentIds.Add(commentId);
            }
        }

        [Fact]
        public async Task CreateTaskCommentAsync_WithValidData_ShouldCreateComment()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            var commentText = $"This is a task comment - {Guid.NewGuid()}";
            var createCommentRequest = new CreateTaskCommentRequest(commentText, null, null, false);
            var mockCommentId = "mock_task_comment_id_123";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "CreateTaskCommentAsync_Success.json");
                MockHttpHandler.When(HttpMethod.Post, $"task/{_testTaskId}/comment") // Relative URL
                               .WithPartialContent("\"comment_text\":\"" + commentText + "\"") // Optional: match body
                               .Respond("application/json", await File.ReadAllTextAsync(responsePath));
            }

            _output.LogInformation($"Attempting to create comment on task '{_testTaskId}'.");
            CreateCommentResponse? createdCommentInfo = null;
            try
            {
                createdCommentInfo = await _commentService.CreateTaskCommentAsync(_testTaskId, createCommentRequest);
                if (createdCommentInfo != null) RegisterCreatedComment(createdCommentInfo.Id);
            }
            catch (Exception ex)
            {
                _output.LogError($"Exception during CreateTaskCommentAsync: {ex.Message}", ex);
                Assert.Fail($"CreateTaskCommentAsync threw an exception: {ex.Message}");
            }

            Assert.NotNull(createdCommentInfo);
            Assert.False(string.IsNullOrWhiteSpace(createdCommentInfo.Id));
            Assert.NotNull(createdCommentInfo.Comment);
            Assert.False(string.IsNullOrWhiteSpace(createdCommentInfo.Comment.Date));

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(mockCommentId, createdCommentInfo.Id); // Assuming mock JSON has this ID
                Assert.Contains(commentText, createdCommentInfo.Comment.CommentText); // Check if text from request is in response
            }
            else // Verify by fetching if not in playback
            {
                var commentsResponse = await _commentService.GetTaskCommentsAsync(new GetTaskCommentsRequest(_testTaskId));
                var retrievedComment = commentsResponse.FirstOrDefault(c => c.Id == createdCommentInfo.Id);
                Assert.NotNull(retrievedComment);
                Assert.Equal(commentText, retrievedComment.CommentText);
            }
        }

        [Fact]
        public async Task GetTaskCommentsAsync_WithExistingComments_ShouldReturnComments()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            var mockCommentId1 = "mock_task_comment_id_get1";
            var mockCommentId2 = "mock_task_comment_id_get2";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "GetTaskCommentsAsync_Success.json");
                MockHttpHandler.When(HttpMethod.Get, $"task/{_testTaskId}/comment") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(responsePath));
            }
            else // Create actual comments for Record/Passthrough
            {
                var commentText1 = $"Comment 1 for get test - {Guid.NewGuid()}";
                var commentText2 = $"Comment 2 for get test - {Guid.NewGuid()}";
                var comment1Info = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest(commentText1, null, null, false));
                RegisterCreatedComment(comment1Info.Id);
                var comment2Info = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest(commentText2, null, null, false));
                RegisterCreatedComment(comment2Info.Id);
                _output.LogInformation($"Created comments {comment1Info.Id}, {comment2Info.Id} for GetTaskCommentsAsync test.");
            }

            var commentsEnumerable = await _commentService.GetTaskCommentsAsync(new GetTaskCommentsRequest(_testTaskId));

            Assert.NotNull(commentsEnumerable);
            var commentsList = commentsEnumerable.ToList();
            Assert.True(commentsList.Count >= (CurrentTestMode == TestMode.Playback ? 2 : 2)); // Assuming mock JSON has 2 comments

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Contains(commentsList, c => c.Id == mockCommentId1);
                Assert.Contains(commentsList, c => c.Id == mockCommentId2);
            }
            // In non-playback, the previously created comments should be found (covered by CreateTaskComment's verification)
        }

        [Fact]
        public async Task UpdateCommentAsync_WithValidData_ShouldUpdateComment()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            var initialCommentText = $"Initial comment for update - {Guid.NewGuid()}";
            var updatedCommentText = $"Updated comment text - {Guid.NewGuid()}";
            var updateCommentRequest = new UpdateCommentRequest(CommentText: updatedCommentText, Assignee: null, Resolved: false, NotifyAll: null);
            string commentIdToUpdate = "mock_comment_to_update_id";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "UpdateCommentAsync_Success.json");
                MockHttpHandler.When(HttpMethod.Put, $"comment/{commentIdToUpdate}") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(responsePath));
            }
            else
            {
                var createdCommentInfo = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest(initialCommentText, null, null, false));
                Assert.NotNull(createdCommentInfo);
                commentIdToUpdate = createdCommentInfo.Id; // Use the real ID
                RegisterCreatedComment(commentIdToUpdate);
                _output.LogInformation($"Comment created for Update test. ID: {commentIdToUpdate}");
            }

            _output.LogInformation($"Attempting to update comment '{commentIdToUpdate}'.");
            // The service UpdateCommentAsync returns Task, but internally it's Task<Comment>.
            // The interface ICommentsService.UpdateCommentAsync returns Task.
            // Let's assume the method completes without error for now.
            // If the return type was Comment, we'd assert its properties.
            await _commentService.UpdateCommentAsync(commentIdToUpdate, updateCommentRequest);
            _output.LogInformation($"Comment update called for {commentIdToUpdate}.");

            // Verify
            if (CurrentTestMode == TestMode.Playback)
            {
                // Verification in playback relies on the mock being called.
                // If UpdateCommentAsync returned the updated comment, we'd check its text here.
                // Since it returns Task, we assume success if no exception.
                // To properly verify, GetCommentById would be needed, or check a GetTaskComments list.
            }
            else
            {
                var commentsEnumerable = await _commentService.GetTaskCommentsAsync(new GetTaskCommentsRequest(_testTaskId));
                var updatedComment = commentsEnumerable.FirstOrDefault(c => c.Id == commentIdToUpdate);
                Assert.NotNull(updatedComment);
                Assert.Equal(updatedCommentText, updatedComment.CommentText);
            }
        }

        [Fact]
        public async Task DeleteCommentAsync_WithExistingComment_ShouldDeleteComment()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            var commentText = $"Comment to delete - {Guid.NewGuid()}";
            string commentIdToDelete = "mock_comment_to_delete_id";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Delete, $"comment/{commentIdToDelete}") // Relative URL
                               .Respond(HttpStatusCode.NoContent); // Or OK with empty body
            }
            else
            {
                var createdCommentInfo = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest(commentText, null, null, false));
                Assert.NotNull(createdCommentInfo);
                commentIdToDelete = createdCommentInfo.Id;
                // DO NOT RegisterCreatedComment here as we are about to delete it.
                _output.LogInformation($"Comment created for Delete test. ID: {commentIdToDelete}");
            }

            await _commentService.DeleteCommentAsync(commentIdToDelete);
            _output.LogInformation($"DeleteCommentAsync called for comment ID: {commentIdToDelete}.");

            if (CurrentTestMode != TestMode.Playback)
            {
                var commentsEnumerable = await _commentService.GetTaskCommentsAsync(new GetTaskCommentsRequest(_testTaskId));
                var deletedComment = commentsEnumerable.FirstOrDefault(c => c.Id == commentIdToDelete);
                Assert.Null(deletedComment);
                _output.LogInformation($"Verified comment {commentIdToDelete} is deleted via GetTaskCommentsAsync.");
            }
            // In Playback, verification is that the mock was called and no error occurred.
        }

        [Fact]
        public async Task GetTaskCommentsStreamAsync_ShouldRetrieveAllComments()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            var mockCommentIdPage1 = "mock_stream_comment_id_p1";
            var mockCommentIdPage2 = "mock_stream_comment_id_p2";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePathPage1 = Path.Combine(RecordedResponsesBasePath, "CommentService", "GetTaskCommentsStreamAsync_Page1_Success.json");
                var responsePathPage2 = Path.Combine(RecordedResponsesBasePath, "CommentService", "GetTaskCommentsStreamAsync_Page2_Success.json");
                var emptyResponsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "GetTaskCommentsStreamAsync_Empty.json");


                MockHttpHandler.When(HttpMethod.Get, $"task/{_testTaskId}/comment") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(responsePathPage1));
                MockHttpHandler.When(HttpMethod.Get, $"task/{_testTaskId}/comment?start_id={mockCommentIdPage1}") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(responsePathPage2));
                MockHttpHandler.When(HttpMethod.Get, $"task/{_testTaskId}/comment?start_id={mockCommentIdPage2}") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(emptyResponsePath));
            }
            else
            {
                int commentsToCreate = 2; // Service implementation paginates internally after first call if needed
                for (int i = 0; i < commentsToCreate; i++)
                {
                    await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest($"Paginated Comment {i + 1} - {Guid.NewGuid()}", null, null, false));
                    await Task.Delay(100); // Be nice to API
                }
            }

            var retrievedComments = new List<Comment>();
            await foreach (var comment in _commentService.GetTaskCommentsStreamAsync(_testTaskId, new GetTaskCommentsRequest(_testTaskId)))
            {
                retrievedComments.Add(comment);
            }

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(2, retrievedComments.Count); // Assuming 2 comments across mocked pages
                Assert.Contains(retrievedComments, c => c.Id == mockCommentIdPage1 || c.Id == mockCommentIdPage2); // Simplified check
            }
            else
            {
                Assert.True(retrievedComments.Count >= 2);
            }
        }

        [Fact]
        public async Task CreateListCommentAsync_WithValidData_ShouldCreateComment()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            var commentText = $"This is a list comment - {Guid.NewGuid()}";
            var createCommentRequest = new CreateCommentRequest { CommentText = commentText };
            var mockCommentId = "mock_list_comment_id_456";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "CreateListCommentAsync_Success.json");
                MockHttpHandler.When(HttpMethod.Post, $"list/{_testListId}/comment") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(responsePath));
            }

            var createdCommentInfo = await _commentService.CreateListCommentAsync(_testListId, createCommentRequest);
            if (createdCommentInfo != null) RegisterCreatedComment(createdCommentInfo.Id);


            Assert.NotNull(createdCommentInfo);
            Assert.False(string.IsNullOrWhiteSpace(createdCommentInfo.Id));
            Assert.NotNull(createdCommentInfo.Comment);

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(mockCommentId, createdCommentInfo.Id);
                Assert.Contains(commentText, createdCommentInfo.Comment.CommentText);
            }
            else
            {
                var comments = await _commentService.GetListCommentsAsync(_testListId);
                Assert.Contains(comments, c => c.Id == createdCommentInfo.Id && c.CommentText == commentText);
            }
        }

        [Fact]
        public async Task GetListCommentsAsync_WithExistingComments_ShouldReturnComments()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            var mockCommentId = "mock_list_comment_id_get";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "GetListCommentsAsync_Success.json");
                MockHttpHandler.When(HttpMethod.Get, $"list/{_testListId}/comment") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(responsePath));
            }
            else
            {
                await _commentService.CreateListCommentAsync(_testListId, new CreateCommentRequest { CommentText = "Test List Comment for Get" });
            }

            var comments = await _commentService.GetListCommentsAsync(_testListId);
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Contains(comments, c => c.Id == mockCommentId);
            }
        }


        [Fact]
        public async Task GetListCommentsStreamAsync_ShouldRetrieveAllListComments()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testListId), "TestListId must be available.");
            var mockCommentIdPage1 = "mock_list_stream_p1";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePathPage1 = Path.Combine(RecordedResponsesBasePath, "CommentService", "GetListCommentsStreamAsync_Page1_Success.json");
                var emptyResponsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "GetListCommentsStreamAsync_Empty.json");

                MockHttpHandler.When(HttpMethod.Get, $"list/{_testListId}/comment") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(responsePathPage1));
                MockHttpHandler.When(HttpMethod.Get, $"list/{_testListId}/comment?start_id={mockCommentIdPage1}") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(emptyResponsePath));
            }
            else
            {
                 await _commentService.CreateListCommentAsync(_testListId, new CreateCommentRequest { CommentText = "List Stream Comment 1" });
            }

            var retrievedComments = new List<Comment>();
            await foreach (var comment in _commentService.GetListCommentsStreamAsync(_testListId))
            {
                retrievedComments.Add(comment);
            }

            Assert.NotEmpty(retrievedComments);
            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Contains(retrievedComments, c => c.Id == mockCommentIdPage1);
            }
        }

        [Fact]
        public async Task CreateThreadedCommentAsync_WithValidData_ShouldCreateReply()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            string parentCommentId = "mock_parent_comment_id_for_reply";
            var replyText = $"This is a threaded reply - {Guid.NewGuid()}";
            var createReplyRequest = new CreateCommentRequest { CommentText = replyText };
            var mockReplyId = "mock_reply_comment_id_789";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "CreateThreadedCommentAsync_Success.json");
                MockHttpHandler.When(HttpMethod.Post, $"comment/{parentCommentId}/reply") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(responsePath));
            }
            else
            {
                var parentCommentInfo = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest($"Parent comment for reply - {Guid.NewGuid()}", null, null, false));
                Assert.NotNull(parentCommentInfo);
                parentCommentId = parentCommentInfo.Id;
                RegisterCreatedComment(parentCommentId); // Register parent
            }

            var createdReplyInfo = await _commentService.CreateThreadedCommentAsync(parentCommentId, createReplyRequest);
            if (createdReplyInfo != null) RegisterCreatedComment(createdReplyInfo.Id); // Register reply

            Assert.NotNull(createdReplyInfo);
            Assert.False(string.IsNullOrWhiteSpace(createdReplyInfo.Id));

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Equal(mockReplyId, createdReplyInfo.Id);
                Assert.Contains(replyText, createdReplyInfo.Comment.CommentText);
            }
            else
            {
                // Verification for non-playback: GetThreadedComments and check
                var replies = await _commentService.GetThreadedCommentsAsync(parentCommentId);
                Assert.Contains(replies, r => r.Id == createdReplyInfo.Id && r.CommentText == replyText);
            }
        }

        [Fact]
        public async Task GetThreadedCommentsAsync_WithExistingReplies_ShouldReturnReplies()
        {
            Assert.False(string.IsNullOrWhiteSpace(_testTaskId), "TestTaskId must be available.");
            string parentCommentId = "mock_parent_comment_id_for_get_replies";
            var mockReplyId = "mock_get_reply_comment_id";

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                var responsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "GetThreadedCommentsAsync_Success.json");
                MockHttpHandler.When(HttpMethod.Get, $"comment/{parentCommentId}/reply") // Relative URL
                               .Respond("application/json", await File.ReadAllTextAsync(responsePath));
            }
            else
            {
                var parentCommentInfo = await _commentService.CreateTaskCommentAsync(_testTaskId, new CreateTaskCommentRequest($"Parent for GetThreaded - {Guid.NewGuid()}", null, null, false));
                Assert.NotNull(parentCommentInfo);
                parentCommentId = parentCommentInfo.Id;
                RegisterCreatedComment(parentCommentId);
                await _commentService.CreateThreadedCommentAsync(parentCommentId, new CreateCommentRequest { CommentText = "Test Reply for GetThreaded" });
                // The reply ID is not easily captured here without another Get or assuming CreateThreadedCommentResponse has it directly.
            }

            var replies = await _commentService.GetThreadedCommentsAsync(parentCommentId);
            Assert.NotNull(replies);
            Assert.NotEmpty(replies);

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.Contains(replies, r => r.Id == mockReplyId);
            }
        }


        // --- Negative Tests ---
        [Fact]
        public async Task UpdateCommentAsync_WithNonExistentCommentId_ShouldThrowNotFoundException()
        {
            var nonExistentCommentId = "000000000"; // A more ClickUp like non-existent ID
            var updateRequest = new UpdateCommentRequest(CommentText: "Attempt to update non-existent comment");

            if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Put, $"comment/{nonExistentCommentId}") // Relative URL
                               .Respond(HttpStatusCode.NotFound, "application/json", "{\"err\":\"Comment not found\",\"ECODE\":\"COMMENT_001\"}");
            }
            _output.LogInformation($"Attempting to update non-existent comment with ID: {nonExistentCommentId}");

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _commentService.UpdateCommentAsync(nonExistentCommentId, updateRequest)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException for UpdateCommentAsync: {exception.Message}");
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task DeleteCommentAsync_WithNonExistentCommentId_ShouldThrowNotFoundException()
        {
            var nonExistentCommentId = "000000000";
             if (CurrentTestMode == TestMode.Playback)
            {
                Assert.NotNull(MockHttpHandler);
                MockHttpHandler.When(HttpMethod.Delete, $"comment/{nonExistentCommentId}") // Relative URL
                               .Respond(HttpStatusCode.NotFound, "application/json", "{\"err\":\"Comment not found\",\"ECODE\":\"COMMENT_001\"}");
            }
            _output.LogInformation($"Attempting to delete non-existent comment with ID: {nonExistentCommentId}");

            var exception = await Assert.ThrowsAsync<ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException>(
                () => _commentService.DeleteCommentAsync(nonExistentCommentId)
            );

            _output.LogInformation($"Received expected ClickUpApiNotFoundException for DeleteCommentAsync: {exception.Message}");
            Assert.NotNull(exception);
        }

        // --- Chat View Comments - Marked as Skip or requiring manual setup for view_id ---
        // These tests require a valid _testChatViewId.
        // In a real scenario, this ID might come from another service (ViewsService) or be configured.
        // For now, they will likely fail if run in Record/Passthrough without a valid ID.
        // In Playback, they can be made to work with mock_chat_view_id and recorded JSONs.

        private const string SkipChatViewTestsReason = "Chat View ID (_testChatViewId) not dynamically provisioned. Run manually if a view ID is available or after recording.";
        // private string _testChatViewId = "mock_chat_view_id_tests"; // Default mock ID // This is the duplicate

        [Fact(Skip = SkipChatViewTestsReason)]
        public async Task CreateChatViewCommentAsync_WithValidData_ShouldCreateComment()
        {
            // Test logic similar to CreateTaskCommentAsync, but using _testChatViewId
            // and POST /view/{view_id}/comment
            // Remember to set up MockHttpHandler for Playback mode.
            // Example:
            // if (CurrentTestMode == TestMode.Playback)
            // {
            //     var responsePath = Path.Combine(RecordedResponsesBasePath, "CommentService", "CreateChatViewCommentAsync_Success.json");
            //     MockHttpHandler.When(HttpMethod.Post, $"view/{_testChatViewId}/comment") // Relative URL
            //                    .Respond("application/json", await File.ReadAllTextAsync(responsePath));
            // }
            // var result = await _commentService.CreateChatViewCommentAsync(_testChatViewId, new CreateCommentRequest { CommentText = "Test Chat Comment" });
            // Assert.NotNull(result);
            await Task.CompletedTask; // Placeholder
        }

        [Fact(Skip = SkipChatViewTestsReason)]
        public async Task GetChatViewCommentsAsync_WithExistingComments_ShouldReturnComments()
        {
            // Test logic similar to GetTaskCommentsAsync, but using _testChatViewId
            // and GET /view/{view_id}/comment
            // Remember to set up MockHttpHandler for Playback mode.
            await Task.CompletedTask; // Placeholder
        }

        [Fact(Skip = SkipChatViewTestsReason)]
        public async Task GetChatViewCommentsStreamAsync_ShouldRetrieveAllChatViewComments()
        {
            // Test logic similar to GetTaskCommentsStreamAsync, but using _testChatViewId
            // Remember to set up MockHttpHandler for Playback mode.
            await Task.CompletedTask; // Placeholder
        }
    }
}
