using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Workspaces;

using ClickUp.Api.Client.Models.ResponseModels.Attachments; // For CreateTaskAttachmentResponse
using System.IO; // For Stream
using ClickUp.Api.Client.Models.ResponseModels.Authorization; // For GetAccessTokenResponse
using ClickUp.Api.Client.Models.RequestModels.Authorization; // For GetAccessTokenRequest
using System; // For DateTimeOffset
using ClickUp.Api.Client.Models.ResponseModels.Chat; // For ChatChannelPaginatedResponse
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatChannel
using ClickUp.Api.Client.Models.Entities.Comments; // For Comment
using ClickUp.Api.Client.Models.ResponseModels.CustomFields; // For GetAccessibleCustomFieldsResponse
using ClickUp.Api.Client.Models.Entities.CustomFields; // For Field
using ClickUp.Api.Client.Models.ResponseModels.Docs; // For SearchDocsResponse
using ClickUp.Api.Client.Models.Entities.Docs; // For Doc
using ClickUp.Api.Client.Models.Entities.Folders; // For Folder
using ClickUp.Api.Client.Models.ResponseModels.Goals; // For GetGoalsResponse
using ClickUp.Api.Client.Models.Entities.Goals; // For Goal, GoalFolder
using ClickUp.Api.Client.Models.ResponseModels.Guests; // For GetGuestResponse
using ClickUp.Api.Client.Models.Entities.Users; // For Guest
using ClickUp.Api.Client.Models; // For ClickUpList
using ClickUp.Api.Client.Models.ResponseModels.Members; // For Member
using ClickUp.Api.Client.Models.ResponseModels.Roles; // For CustomRole
using ClickUp.Api.Client.Models.ResponseModels.Sharing; // For SharedHierarchyResponse
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask
using ClickUp.Api.Client.Models.ResponseModels.Templates; // For GetTaskTemplatesResponse
using ClickUp.Api.Client.Models.Entities.Templates; // For TaskTemplate
using ClickUp.Api.Client.Models.Entities.TimeTracking; // For TimeEntry, TimeEntryHistory, TaskTag
using ClickUp.Api.Client.Models.ResponseModels.TimeTracking; // For TaskTimeInStatusResponse, GetBulkTasksTimeInStatusResponse
using ClickUp.Api.Client.Models.Entities.UserGroups; // For UserGroup
using ClickUp.Api.Client.Models.ResponseModels.Views; // For GetViewsResponse, GetViewResponse, GetViewTasksResponse
using ClickUp.Api.Client.Models.Entities.Webhooks; // For Webhook
using ClickUp.Api.Client.Models.ResponseModels.Checklists; // For CreateChecklistResponse

namespace ClickUp.Api.Client.Tests.ServiceTests;

public class FluentApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentTasksApi_Get_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var listId = "testListId";
        var expectedResponse = new GetTasksResponse(new List<ClickUp.Api.Client.Models.Entities.Tasks.CuTask>(), true); // Mock a response

        var mockTasksService = new Mock<ITasksService>();
        mockTasksService.Setup(x => x.GetTasksAsync(
            It.IsAny<string>(),
            It.IsAny<ClickUp.Api.Client.Models.RequestModels.Tasks.GetTasksRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Manually inject the mock service into the fluent API for testing
        var fluentTasksApi = new FluentTasksApi(mockTasksService.Object);

        // Act
        var result = await fluentTasksApi.Get(listId)
            .WithArchived(true)
            .WithPage(1)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockTasksService.Verify(x => x.GetTasksAsync(
            listId,
            It.Is<ClickUp.Api.Client.Models.RequestModels.Tasks.GetTasksRequest>(req =>
                req.Archived == true &&
                req.Page == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentWorkspacesApi_GetSeatsAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetWorkspaceSeatsResponse(
            new WorkspaceMemberSeatsInfo(1, 1, 0),
            new WorkspaceGuestSeatsInfo(1, 1, 0)
        ); // Mock a response

        var mockWorkspacesService = new Mock<IWorkspacesService>();
        mockWorkspacesService.Setup(x => x.GetWorkspaceSeatsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Manually inject the mock service into the fluent API for testing
        var fluentWorkspacesApi = new FluentWorkspacesApi(mockWorkspacesService.Object);

        // Act
        var result = await fluentWorkspacesApi.GetSeatsAsync(workspaceId);

        // Assert
        Assert.Equal(expectedResponse, result);
        mockWorkspacesService.Verify(x => x.GetWorkspaceSeatsAsync(
            workspaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentAttachmentsApi_Create_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var taskId = "testTaskId";
        var fileName = "testFile.txt";
        var fileStream = new MemoryStream();
        var expectedResponse = new CreateTaskAttachmentResponse(
            Id: "attachmentId",
            Version: "v1",
            Date: DateTimeOffset.UtcNow,
            Title: "test.txt",
            Extension: "txt",
            ThumbnailSmall: null,
            ThumbnailLarge: null,
            Url: "http://example.com/test.txt",
            UrlWQuery: "http://example.com/test.txt?query",
            UrlWHost: "http://example.com/test.txt",
            IsFolder: false,
            ParentId: "parentId",
            Size: 12345,
            TotalComments: 0,
            ResolvedComments: 0,
            User: new User(1, "testuser", "test@example.com", "#000000", null, "TU", null),
            Deleted: false,
            Orientation: null,
            Type: 0,
            Source: 0,
            EmailData: null,
            ResourceId: "resourceId"
        );

        var mockAttachmentsService = new Mock<IAttachmentsService>();
        mockAttachmentsService.Setup(x => x.CreateTaskAttachmentAsync(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentAttachmentsApi = new FluentAttachmentsApi(mockAttachmentsService.Object);

        // Act
        var result = await fluentAttachmentsApi.Create(taskId, fileStream, fileName)
            .WithCustomTaskIds(true)
            .WithTeamId("testTeamId")
            .CreateAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockAttachmentsService.Verify(x => x.CreateTaskAttachmentAsync(
            taskId,
            fileStream,
            fileName,
            true,
            "testTeamId",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentAuthorizationApi_GetAccessToken_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var clientId = "testClientId";
        var clientSecret = "testClientSecret";
        var code = "testCode";
        var expectedResponse = new GetAccessTokenResponse("testAccessToken");

        var mockAuthorizationService = new Mock<IAuthorizationService>();
        mockAuthorizationService.Setup(x => x.GetAccessTokenAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentAuthorizationApi = new FluentAuthorizationApi(mockAuthorizationService.Object);

        // Act
        var result = await fluentAuthorizationApi.GetAccessToken()
            .WithClientId(clientId)
            .WithClientSecret(clientSecret)
            .WithCode(code)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockAuthorizationService.Verify(x => x.GetAccessTokenAsync(
            clientId,
            clientSecret,
            code,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentAuthorizationApi_GetAuthorizedUserAsync_ShouldCallService()
    {
        // Arrange
        var expectedUser = new User(1, "testuser", "test@example.com", "#000000", null, "TU", null);
        var mockAuthorizationService = new Mock<IAuthorizationService>();
        mockAuthorizationService.Setup(x => x.GetAuthorizedUserAsync(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        var fluentAuthorizationApi = new FluentAuthorizationApi(mockAuthorizationService.Object);

        // Act
        var result = await fluentAuthorizationApi.GetAuthorizedUserAsync();

        // Assert
        Assert.Equal(expectedUser, result);
        mockAuthorizationService.Verify(x => x.GetAuthorizedUserAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentAuthorizationApi_GetAuthorizedWorkspacesAsync_ShouldCallService()
    {
        // Arrange
        var expectedWorkspaces = new List<ClickUp.Api.Client.Models.ClickUpWorkspace>();
        var mockAuthorizationService = new Mock<IAuthorizationService>();
        mockAuthorizationService.Setup(x => x.GetAuthorizedWorkspacesAsync(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWorkspaces);

        var fluentAuthorizationApi = new FluentAuthorizationApi(mockAuthorizationService.Object);

        // Act
        var result = await fluentAuthorizationApi.GetAuthorizedWorkspacesAsync();

        // Assert
        Assert.Equal(expectedWorkspaces, result);
        mockAuthorizationService.Verify(x => x.GetAuthorizedWorkspacesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentChatApi_GetChatChannels_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new ChatChannelPaginatedResponse(null, new List<ChatChannel>());

        var mockChatService = new Mock<IChatService>();
        mockChatService.Setup(x => x.GetChatChannelsAsync(
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>(),
            It.IsAny<bool?>(),
            It.IsAny<bool?>(),
            It.IsAny<long?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentChatApi = new FluentChatApi(mockChatService.Object);

        // Act
        var result = await fluentChatApi.GetChatChannels(workspaceId)
            .WithLimit(10)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockChatService.Verify(x => x.GetChatChannelsAsync(
            workspaceId,
            null,
            null,
            10,
            null,
            null,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentCommentApi_GetTaskComments_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var taskId = "testTaskId";
        var expectedComments = new List<Comment>();

        var mockCommentsService = new Mock<ICommentsService>();
        mockCommentsService.Setup(x => x.GetTaskCommentsStreamAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<long?>(),
            It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(expectedComments));

        var fluentCommentApi = new FluentCommentApi(mockCommentsService.Object);

        // Act
        var result = new List<Comment>();
        await foreach (var comment in fluentCommentApi.GetTaskComments(taskId).GetStreamAsync())
        {
            result.Add(comment);
        }

        // Assert
        Assert.Equal(expectedComments, result);
        mockCommentsService.Verify(x => x.GetTaskCommentsStreamAsync(
            taskId,
            null,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
        {
            await Task.Yield();
            yield return item;
        }
    }

    [Fact]
    public async Task FluentCustomFieldsApi_GetAccessibleCustomFieldsAsync_ShouldCallService()
    {
        // Arrange
        var listId = "testListId";
        var expectedFields = new List<Field>();

        var mockCustomFieldsService = new Mock<ICustomFieldsService>();
        mockCustomFieldsService.Setup(x => x.GetAccessibleCustomFieldsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFields);

        var fluentCustomFieldsApi = new FluentCustomFieldsApi(mockCustomFieldsService.Object);

        // Act
        var result = await fluentCustomFieldsApi.GetAccessibleCustomFieldsAsync(listId);

        // Assert
        Assert.Equal(expectedFields, result);
        mockCustomFieldsService.Verify(x => x.GetAccessibleCustomFieldsAsync(
            listId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentDocsApi_SearchDocs_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new SearchDocsResponse(new List<Doc>(), null, null, null);

        var mockDocsService = new Mock<IDocsService>();
        mockDocsService.Setup(x => x.SearchDocsAsync(
            It.IsAny<string>(),
            It.IsAny<ClickUp.Api.Client.Models.RequestModels.Docs.SearchDocsRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentDocsApi = new FluentDocsApi(mockDocsService.Object);

        // Act
        var result = await fluentDocsApi.SearchDocs(workspaceId)
            .WithQuery("testQuery")
            .WithLimit(5)
            .SearchAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockDocsService.Verify(x => x.SearchDocsAsync(
            workspaceId,
            It.Is<ClickUp.Api.Client.Models.RequestModels.Docs.SearchDocsRequest>(req =>
                req.Query == "testQuery" &&
                req.Limit == 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentFoldersApi_GetFoldersAsync_ShouldCallService()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var expectedFolders = new List<Folder>();

        var mockFoldersService = new Mock<IFoldersService>();
        mockFoldersService.Setup(x => x.GetFoldersAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFolders);

        var fluentFoldersApi = new FluentFoldersApi(mockFoldersService.Object);

        // Act
        var result = await fluentFoldersApi.GetFoldersAsync(spaceId);

        // Assert
        Assert.Equal(expectedFolders, result);
        mockFoldersService.Verify(x => x.GetFoldersAsync(
            spaceId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentGoalsApi_GetGoals_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetGoalsResponse(new List<Goal>(), new List<GoalFolder>());

        var mockGoalsService = new Mock<IGoalsService>();
        mockGoalsService.Setup(x => x.GetGoalsAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentGoalsApi = new FluentGoalsApi(mockGoalsService.Object);

        // Act
        var result = await fluentGoalsApi.GetGoals(workspaceId)
            .WithIncludeCompleted(true)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockGoalsService.Verify(x => x.GetGoalsAsync(
            workspaceId,
            true,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentGuestsApi_GetGuestAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var guestId = "testGuestId";
        var mockGuestUserInfo = new ClickUp.Api.Client.Models.Entities.Users.GuestUserInfo
        {
            Id = 1,
            Username = "testguest",
            Email = "test@example.com",
            Color = "#000000",
            ProfilePicture = null,
            Initials = "TG",
            Role = 0, // Assuming a default role
            CustomRole = null,
            LastActive = null,
            DateJoined = null,
            DateInvited = DateTimeOffset.UtcNow // Assuming a default date
        };
        var mockGuestSharingDetails = new ClickUp.Api.Client.Models.Entities.Users.GuestSharingDetails
        {
            Tasks = new List<string>(),
            Lists = new List<string>(),
            Folders = new List<string>()
        };
        var expectedGuest = new ClickUp.Api.Client.Models.Entities.Users.Guest
        {
            User = mockGuestUserInfo,
            Shared = mockGuestSharingDetails
        };
        var expectedResponse = new GetGuestResponse { Guest = expectedGuest };

        var mockGuestsService = new Mock<IGuestsService>();
        mockGuestsService.Setup(x => x.GetGuestAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentGuestsApi = new FluentGuestsApi(mockGuestsService.Object);

        // Act
        var result = await fluentGuestsApi.GetGuestAsync(workspaceId, guestId);

        // Assert
        Assert.Equal(expectedResponse, result);
        mockGuestsService.Verify(x => x.GetGuestAsync(
            workspaceId,
            guestId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentListsApi_GetListsInFolderAsync_ShouldCallService()
    {
        // Arrange
        var folderId = "testFolderId";
        var expectedLists = new List<ClickUp.Api.Client.Models.ClickUpList>();

        var mockListsService = new Mock<IListsService>();
        mockListsService.Setup(x => x.GetListsInFolderAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLists);

        var fluentListsApi = new FluentListsApi(mockListsService.Object);

        // Act
        var result = await fluentListsApi.GetListsInFolderAsync(folderId);

        // Assert
        Assert.Equal(expectedLists, result);
        mockListsService.Verify(x => x.GetListsInFolderAsync(
            folderId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentMembersApi_GetTaskMembersAsync_ShouldCallService()
    {
        // Arrange
        var taskId = "testTaskId";
        var expectedMembers = new List<ClickUp.Api.Client.Models.ResponseModels.Members.Member>();

        var mockMembersService = new Mock<IMembersService>();
        mockMembersService.Setup(x => x.GetTaskMembersAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMembers);

        var fluentMembersApi = new FluentMembersApi(mockMembersService.Object);

        // Act
        var result = await fluentMembersApi.GetTaskMembersAsync(taskId);

        // Assert
        Assert.Equal(expectedMembers, result);
        mockMembersService.Verify(x => x.GetTaskMembersAsync(
            taskId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentRolesApi_GetCustomRolesAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedRoles = new List<ClickUp.Api.Client.Models.ResponseModels.Roles.CustomRole>();

        var mockRolesService = new Mock<IRolesService>();
        mockRolesService.Setup(x => x.GetCustomRolesAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRoles);

        var fluentRolesApi = new FluentRolesApi(mockRolesService.Object);

        // Act
        var result = await fluentRolesApi.GetCustomRolesAsync(workspaceId);

        // Assert
        Assert.Equal(expectedRoles, result);
        mockRolesService.Verify(x => x.GetCustomRolesAsync(
            workspaceId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentSharedHierarchyApi_GetSharedHierarchyAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new SharedHierarchyResponse(new ClickUp.Api.Client.Models.ResponseModels.Sharing.SharedHierarchyDetails(
            new List<string>(),
            new List<ClickUp.Api.Client.Models.ResponseModels.Sharing.SharedHierarchyListItem>(),
            new List<ClickUp.Api.Client.Models.ResponseModels.Sharing.SharedHierarchyFolderItem>()));

        var mockSharedHierarchyService = new Mock<ISharedHierarchyService>();
        mockSharedHierarchyService.Setup(x => x.GetSharedHierarchyAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentSharedHierarchyApi = new FluentSharedHierarchyApi(mockSharedHierarchyService.Object);

        // Act
        var result = await fluentSharedHierarchyApi.GetSharedHierarchyAsync(workspaceId);

        // Assert
        Assert.Equal(expectedResponse, result);
        mockSharedHierarchyService.Verify(x => x.GetSharedHierarchyAsync(
            workspaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentSpacesApi_GetSpacesAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedSpaces = new List<ClickUp.Api.Client.Models.Entities.Spaces.Space>();

        var mockSpacesService = new Mock<ISpacesService>();
        mockSpacesService.Setup(x => x.GetSpacesAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSpaces);

        var fluentSpacesApi = new FluentSpacesApi(mockSpacesService.Object);

        // Act
        var result = await fluentSpacesApi.GetSpacesAsync(workspaceId);

        // Assert
        Assert.Equal(expectedSpaces, result);
        mockSpacesService.Verify(x => x.GetSpacesAsync(
            workspaceId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentTagsApi_GetSpaceTagsAsync_ShouldCallService()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var expectedTags = new List<ClickUp.Api.Client.Models.Entities.Tags.Tag>();

        var mockTagsService = new Mock<ITagsService>();
        mockTagsService.Setup(x => x.GetSpaceTagsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTags);

        var fluentTagsApi = new FluentTagsApi(mockTagsService.Object);

        // Act
        var result = await fluentTagsApi.GetSpaceTagsAsync(spaceId);

        // Assert
        Assert.Equal(expectedTags, result);
        mockTagsService.Verify(x => x.GetSpaceTagsAsync(
            spaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentTaskChecklistsApi_CreateChecklist_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var taskId = "testTaskId";
        var checklistName = "Test Checklist";
        var expectedResponse = new CreateChecklistResponse { Checklist = new ClickUp.Api.Client.Models.Entities.Checklists.Checklist("checklistId", "testTaskId", checklistName, 0, 0, 0, null) };

        var mockTaskChecklistsService = new Mock<ITaskChecklistsService>();
        mockTaskChecklistsService.Setup(x => x.CreateChecklistAsync(
            It.IsAny<string>(),
            It.IsAny<ClickUp.Api.Client.Models.RequestModels.Checklists.CreateChecklistRequest>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentTaskChecklistsApi = new FluentTaskChecklistsApi(mockTaskChecklistsService.Object);

        // Act
        var result = await fluentTaskChecklistsApi.CreateChecklist(taskId)
            .WithName(checklistName)
            .CreateAsync(true, "testTeamId");

        // Assert
        Assert.Equal(expectedResponse, result);
        mockTaskChecklistsService.Verify(x => x.CreateChecklistAsync(
            taskId,
            It.Is<ClickUp.Api.Client.Models.RequestModels.Checklists.CreateChecklistRequest>(req =>
                req.Name == checklistName),
            true,
            "testTeamId",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentTaskRelationshipsApi_AddDependency_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var taskId = "testTaskId";
        var dependsOnTaskId = "dependsOnTaskId";

        var mockTaskRelationshipsService = new Mock<ITaskRelationshipsService>();
        mockTaskRelationshipsService.Setup(x => x.AddDependencyAsync(
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var fluentTaskRelationshipsApi = new FluentTaskRelationshipsApi(mockTaskRelationshipsService.Object);

        // Act
        await fluentTaskRelationshipsApi.AddDependency(taskId)
            .WithDependsOnTaskId(dependsOnTaskId)
            .AddAsync();

        // Assert
        mockTaskRelationshipsService.Verify(x => x.AddDependencyAsync(
            taskId,
            dependsOnTaskId,
            null,
            null,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentTemplatesApi_GetTaskTemplates_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetTaskTemplatesResponse(new List<TaskTemplate>());

        var mockTemplatesService = new Mock<ITemplatesService>();
        mockTemplatesService.Setup(x => x.GetTaskTemplatesAsync(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentTemplatesApi = new FluentTemplatesApi(mockTemplatesService.Object);

        // Act
        var result = await fluentTemplatesApi.GetTaskTemplates(workspaceId)
            .WithPage(0)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockTemplatesService.Verify(x => x.GetTaskTemplatesAsync(
            workspaceId,
            0,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentTimeTrackingApi_GetTimeEntries_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedTimeEntries = new List<TimeEntry>();

        var mockTimeTrackingService = new Mock<ITimeTrackingService>();
        mockTimeTrackingService.Setup(x => x.GetTimeEntriesAsync(
            It.IsAny<string>(),
            It.IsAny<ClickUp.Api.Client.Models.RequestModels.TimeTracking.GetTimeEntriesRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTimeEntries);

        var fluentTimeTrackingApi = new FluentTimeTrackingApi(mockTimeTrackingService.Object);

        // Act
        var result = await fluentTimeTrackingApi.GetTimeEntries(workspaceId)
            .WithStartDate(1234567890L)
            .WithEndDate(9876543210L)
            .GetAsync();

        // Assert
        Assert.Equal(expectedTimeEntries, result);
        mockTimeTrackingService.Verify(x => x.GetTimeEntriesAsync(
            workspaceId,
            It.Is<ClickUp.Api.Client.Models.RequestModels.TimeTracking.GetTimeEntriesRequest>(req =>
                req.StartDate == DateTimeOffset.FromUnixTimeMilliseconds(1234567890L) &&
                req.EndDate == DateTimeOffset.FromUnixTimeMilliseconds(9876543210L)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentUserGroupsApi_GetUserGroupsAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedUserGroups = new List<UserGroup>();

        var mockUserGroupsService = new Mock<IUserGroupsService>();
        mockUserGroupsService.Setup(x => x.GetUserGroupsAsync(
            It.IsAny<string>(),
            It.IsAny<List<string>?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserGroups);

        var fluentUserGroupsApi = new FluentUserGroupsApi(mockUserGroupsService.Object);

        // Act
        var result = await fluentUserGroupsApi.GetUserGroupsAsync(workspaceId);

        // Assert
        Assert.Equal(expectedUserGroups, result);
        mockUserGroupsService.Verify(x => x.GetUserGroupsAsync(
            workspaceId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentUsersApi_GetUserFromWorkspaceAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var userId = "testUserId";
        var expectedUser = new User(1, "testuser", "test@example.com", "#000000", null, "TU", null);

        var mockUsersService = new Mock<IUsersService>();
        mockUsersService.Setup(x => x.GetUserFromWorkspaceAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        var fluentUsersApi = new FluentUsersApi(mockUsersService.Object);

        // Act
        var result = await fluentUsersApi.GetUserFromWorkspaceAsync(workspaceId, userId);

        // Assert
        Assert.Equal(expectedUser, result);
        mockUsersService.Verify(x => x.GetUserFromWorkspaceAsync(
            workspaceId,
            userId,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentViewsApi_GetWorkspaceViewsAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetViewsResponse { Views = new List<ClickUp.Api.Client.Models.Entities.Views.View>() };

        var mockViewsService = new Mock<IViewsService>();
        mockViewsService.Setup(x => x.GetWorkspaceViewsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentViewsApi = new FluentViewsApi(mockViewsService.Object);

        // Act
        var result = await fluentViewsApi.GetWorkspaceViewsAsync(workspaceId);

        // Assert
        Assert.Equal(expectedResponse, result);
        mockViewsService.Verify(x => x.GetWorkspaceViewsAsync(
            workspaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentWebhooksApi_GetWebhooksAsync_ShouldCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedWebhooks = new List<Webhook>();

        var mockWebhooksService = new Mock<IWebhooksService>();
        mockWebhooksService.Setup(x => x.GetWebhooksAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWebhooks);

        var fluentWebhooksApi = new FluentWebhooksApi(mockWebhooksService.Object);

        // Act
        var result = await fluentWebhooksApi.GetWebhooksAsync(workspaceId);

        // Assert
        Assert.Equal(expectedWebhooks, result);
        mockWebhooksService.Verify(x => x.GetWebhooksAsync(
            workspaceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FluentTasksApi_GetFilteredTeamTasks_ShouldBuildCorrectRequestAndCallService()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var expectedResponse = new GetTasksResponse(new List<CuTask>(), true);

        var mockTasksService = new Mock<ITasksService>();
        mockTasksService.Setup(x => x.GetFilteredTeamTasksAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<bool?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<bool?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<IEnumerable<long>?>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var fluentTasksApi = new FluentTasksApi(mockTasksService.Object);

        // Act
        var result = await fluentTasksApi.GetFilteredTeamTasks(workspaceId)
            .WithSubtasks(true)
            .WithIncludeClosed(false)
            .GetAsync();

        // Assert
        Assert.Equal(expectedResponse, result);
        mockTasksService.Verify(x => x.GetFilteredTeamTasksAsync(
            workspaceId,
            null, // page
            null, // orderBy
            null, // reverse
            true, // subtasks
            null, // spaceIds
            null, // projectIds
            null, // listIds
            null, // statuses
            false, // includeClosed
            null, // assignees
            null, // tags
            null, // dueDateGreaterThan
            null, // dueDateLessThan
            null, // dateCreatedGreaterThan
            null, // dateCreatedLessThan
            null, // dateUpdatedGreaterThan
            null, // dateUpdatedLessThan
            null, // customFields
            null, // customTaskIds
            null, // teamIdForCustomTaskIds
            null, // customItems
            null, // dateDoneGreaterThan
            null, // dateDoneLessThan
            null, // parentTaskId
            null, // includeMarkdownDescription
            It.IsAny<CancellationToken>()), Times.Once);
    }
}