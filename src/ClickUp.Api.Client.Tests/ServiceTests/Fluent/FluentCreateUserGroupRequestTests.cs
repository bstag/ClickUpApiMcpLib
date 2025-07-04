using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using ClickUp.Api.Client.Models.RequestModels.UserGroups;

using Moq;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentCreateUserGroupRequestTests
{
    private readonly Mock<IUserGroupsService> _mockUserGroupsService;

    public FluentCreateUserGroupRequestTests()
    {
        _mockUserGroupsService = new Mock<IUserGroupsService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateUserGroupAsyncWithCorrectParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var name = "testGroupName";
        var handle = "testHandle";
        var members = new List<int> { 1, 2 };
        var expectedUserGroup = new UserGroup("groupId", workspaceId, 1, name, handle , "dateCreated", null, new List<Client.Models.Common.Member>(), null); // Mock a UserGroup object

        _mockUserGroupsService.Setup(x => x.CreateUserGroupAsync(
            It.IsAny<string>(),
            It.IsAny<CreateUserGroupRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserGroup);

        var fluentRequest = new UserGroupFluentCreateRequest(workspaceId, _mockUserGroupsService.Object)
            .WithName(name)
            .WithHandle(handle)
            .WithMembers(members);

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedUserGroup, result);
        _mockUserGroupsService.Verify(x => x.CreateUserGroupAsync(
            workspaceId,
            It.Is<CreateUserGroupRequest>(req =>
                req.Name == name &&
                req.Handle == handle &&
                req.Members.SequenceEqual(members)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreateUserGroupAsyncWithOnlyRequiredParameters()
    {
        // Arrange
        var workspaceId = "testWorkspaceId";
        var name = "testGroupName";
        var expectedUserGroup = new UserGroup("groupId", workspaceId, 1, name, string.Empty, "dateCreated", null, new List<Client.Models.Common.Member>(), null); // Mock a UserGroup object

        _mockUserGroupsService.Setup(x => x.CreateUserGroupAsync(
            It.IsAny<string>(),
            It.IsAny<CreateUserGroupRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserGroup);

        var fluentRequest = new UserGroupFluentCreateRequest(workspaceId, _mockUserGroupsService.Object)
            .WithName(name)
            .WithMembers(new List<int> { 123 }); // Required

        // Act
        var result = await fluentRequest.CreateAsync();

        // Assert
        Assert.Equal(expectedUserGroup, result);
        _mockUserGroupsService.Verify(x => x.CreateUserGroupAsync(
            workspaceId,
            It.Is<CreateUserGroupRequest>(req =>
                req.Name == name &&
                req.Handle == null &&
                req.Members.SequenceEqual(new List<int> { 123 })),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
