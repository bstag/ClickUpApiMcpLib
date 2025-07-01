using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Fluent;
using ClickUp.Api.Client.Models.Entities.Lists;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Fluent;

public class FluentListsApiTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly ClickUpClient _client;

    public FluentListsApiTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        _client = new ClickUpClient(_mockApiConnection.Object, _mockLoggerFactory.Object);
    }

    [Fact]
    public async Task FluentListsApi_GetListsInFolderAsync_ShouldCallService()
    {
        // Arrange
        var folderId = "testFolderId";
        var expectedLists = new List<ClickUpList>();

        var mockListsService = new Mock<IListsService>();
        mockListsService.Setup(x => x.GetListsInFolderAsync(
            It.IsAny<string>(),
            It.IsAny<bool?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLists);

        var fluentListsApi = new ListsFluentApi(mockListsService.Object);

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
    public async Task FluentListsApi_GetListsAsyncEnumerableAsync_ShouldCallServiceAndReturnItems()
    {
        // Arrange
        var folderId = "testFolderId";
        var expectedLists = new List<ClickUpList> { new ClickUpList { Id = "list1" }, new ClickUpList { Id = "list2" } };
        bool? archived = false;
        var cancellationToken = new CancellationTokenSource().Token;

        var mockListsService = new Mock<IListsService>();
        mockListsService.Setup(x => x.GetListsInFolderAsync(folderId, archived, cancellationToken))
            .ReturnsAsync(expectedLists);

        var fluentListsApi = new ListsFluentApi(mockListsService.Object);

        // Act
        var result = new List<ClickUpList>();
        await foreach (var item in fluentListsApi.GetListsAsyncEnumerableAsync(folderId, archived, cancellationToken))
        {
            result.Add(item);
        }

        // Assert
        Assert.Equal(expectedLists.Count, result.Count);
        for (int i = 0; i < expectedLists.Count; i++)
        {
            Assert.Equal(expectedLists[i].Id, result[i].Id);
        }
        mockListsService.Verify(x => x.GetListsInFolderAsync(folderId, archived, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task FluentListsApi_GetListsAsyncEnumerableAsync_WithNullArchived_ShouldCallServiceAndReturnItems()
    {
        // Arrange
        var folderId = "testFolderId";
        var expectedLists = new List<ClickUpList> { new ClickUpList { Id = "list1" } };
        var cancellationToken = new CancellationTokenSource().Token;

        var mockListsService = new Mock<IListsService>();
        mockListsService.Setup(x => x.GetListsInFolderAsync(folderId, null, cancellationToken))
            .ReturnsAsync(expectedLists);

        var fluentListsApi = new ListsFluentApi(mockListsService.Object);

        // Act
        var result = new List<ClickUpList>();
        await foreach (var item in fluentListsApi.GetListsAsyncEnumerableAsync(folderId, null, cancellationToken))
        {
            result.Add(item);
        }

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedLists[0].Id, result[0].Id);
        mockListsService.Verify(x => x.GetListsInFolderAsync(folderId, null, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task FluentListsApi_GetFolderlessListsAsyncEnumerableAsync_ShouldCallServiceAndReturnItems()
    {
        // Arrange
        var spaceId = "testSpaceId";
        var expectedLists = new List<ClickUpList> { new ClickUpList { Id = "list1" }, new ClickUpList { Id = "list2" } };
        bool? archived = true;
        var cancellationToken = new CancellationTokenSource().Token;

        // Mocking IAsyncEnumerable requires a bit more setup or a helper
        var mockListsService = new Mock<IListsService>();
        mockListsService.Setup(x => x.GetFolderlessListsAsyncEnumerableAsync(spaceId, archived, cancellationToken))
            .Returns(TestAsyncEnumerable(expectedLists, cancellationToken));

        var fluentListsApi = new ListsFluentApi(mockListsService.Object);

        // Act
        var result = new List<ClickUpList>();
        await foreach (var item in fluentListsApi.GetFolderlessListsAsyncEnumerableAsync(spaceId, archived, cancellationToken))
        {
            result.Add(item);
        }

        // Assert
        Assert.Equal(expectedLists.Count, result.Count);
        for (int i = 0; i < expectedLists.Count; i++)
        {
            Assert.Equal(expectedLists[i].Id, result[i].Id);
        }
        mockListsService.Verify(x => x.GetFolderlessListsAsyncEnumerableAsync(spaceId, archived, cancellationToken), Times.Once);
    }

    async IAsyncEnumerable<T> TestAsyncEnumerable<T>(IEnumerable<T> items, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var item in items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
            await Task.Yield(); // Ensure it's truly async for testing purposes
        }
    }
}
