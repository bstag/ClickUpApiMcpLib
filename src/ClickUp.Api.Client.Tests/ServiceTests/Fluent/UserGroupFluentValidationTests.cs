using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Exceptions;
using Moq;
using Xunit;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using ClickUp.Api.Client.Models.RequestModels.UserGroups;
using System.Collections.Generic; // Required for List
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class UserGroupFluentValidationTests
{
    // --- UserGroupFluentCreateRequest Tests ---

    [Fact]
    public void Create_Validate_MissingWorkspaceId_ThrowsException()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentCreateRequest(string.Empty, userGroupsServiceMock.Object) // Changed null to string.Empty
            .WithName("Test Group")
            .WithMembers(new List<int> { 1 });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingName_ThrowsException()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentCreateRequest("ws123", userGroupsServiceMock.Object)
            .WithMembers(new List<int> { 1 });
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("User group name is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_MissingMembers_ThrowsException()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentCreateRequest("ws123", userGroupsServiceMock.Object)
            .WithName("Test Group");
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("At least one member must be added to the user group.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_EmptyMembers_ThrowsException()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentCreateRequest("ws123", userGroupsServiceMock.Object)
            .WithName("Test Group")
            .WithMembers(new List<int>()); // Empty list
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("At least one member must be added to the user group.", ex.ValidationErrors);
    }

    [Fact]
    public void Create_Validate_ValidRequest_DoesNotThrow()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentCreateRequest("ws123", userGroupsServiceMock.Object)
            .WithName("Test Group")
            .WithMembers(new List<int> { 1 });
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsException()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentCreateRequest(string.Empty, userGroupsServiceMock.Object); // Invalid, Changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.CreateAsync());
        Assert.Contains("WorkspaceId (TeamId) is required.", ex.ValidationErrors);
        Assert.Contains("User group name is required.", ex.ValidationErrors);
        Assert.Contains("At least one member must be added to the user group.", ex.ValidationErrors);
    }

    // --- UserGroupFluentUpdateRequest Tests ---

    [Fact]
    public void Update_Validate_MissingGroupId_ThrowsException()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentUpdateRequest(string.Empty, userGroupsServiceMock.Object) // Changed null to string.Empty
            .WithName("New Name");
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("GroupId is required.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_NoFieldsSet_ThrowsException()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentUpdateRequest("group123", userGroupsServiceMock.Object); // No fields set
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("At least one property (Name, Handle, or Members) must be set for updating a User Group.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_MembersSetButEmpty_ThrowsException()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentUpdateRequest("group123", userGroupsServiceMock.Object)
            .WithMembers(new UpdateUserGroupMembersRequest(null, null)); // Both add and remove are null/empty
        var ex = Assert.Throws<ClickUpRequestValidationException>(() => request.Validate());
        Assert.Contains("If Members object is provided, it must specify members to add or remove.", ex.ValidationErrors);
    }

    [Fact]
    public void Update_Validate_ValidRequest_WithName_DoesNotThrow()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentUpdateRequest("group123", userGroupsServiceMock.Object)
            .WithName("New Name");
        request.Validate(); // Should not throw
    }

    [Fact]
    public void Update_Validate_ValidRequest_WithMembers_DoesNotThrow()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentUpdateRequest("group123", userGroupsServiceMock.Object)
            .WithMembers(new UpdateUserGroupMembersRequest(Add: new List<int> { 1 }, null));
        request.Validate(); // Should not throw
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_ThrowsException()
    {
        var userGroupsServiceMock = new Mock<IUserGroupsService>();
        var request = new UserGroupFluentUpdateRequest(string.Empty, userGroupsServiceMock.Object); // Invalid, changed null to string.Empty
        var ex = await Assert.ThrowsAsync<ClickUpRequestValidationException>(() => request.UpdateAsync());
        Assert.Contains("GroupId is required.", ex.ValidationErrors);
        Assert.Contains("At least one property (Name, Handle, or Members) must be set for updating a User Group.", ex.ValidationErrors);
    }
}
