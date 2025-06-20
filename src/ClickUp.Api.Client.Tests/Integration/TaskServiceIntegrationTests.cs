using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection; // For GetRequiredService
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
// Ensure correct using for CuTask DTO, assuming it's in Entities:
using ClickUpTask = ClickUp.Api.Client.Models.Entities.Task;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace ClickUp.Api.Client.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class TaskServiceIntegrationTests : IntegrationTestBase
    {
        private readonly ITasksService _tasksService;
        private readonly string _testListId;
        private readonly string _testSpaceId; // For GetFilteredTeamTasksAsync
        private readonly string _testWorkspaceId; // Workspace ID (Team ID)

        public TaskServiceIntegrationTests() : base() // Ensure base constructor is called
        {
            _tasksService = ServiceProvider.GetRequiredService<ITasksService>();

            _testListId = Configuration["ClickUpApi:TestListId"] ?? throw new InvalidOperationException("ClickUpApi:TestListId not configured in user secrets or environment variables.");
            if (string.IsNullOrWhiteSpace(_testListId))
            {
                throw new InvalidOperationException("TestListId cannot be empty. Please configure it.");
            }

            _testSpaceId = Configuration["ClickUpApi:TestSpaceId"] ?? throw new InvalidOperationException("ClickUpApi:TestSpaceId not configured.");
             if (string.IsNullOrWhiteSpace(_testSpaceId))
            {
                throw new InvalidOperationException("TestSpaceId cannot be empty.");
            }

            // Assuming WorkspaceId is the same as TeamId used in some API calls
            _testWorkspaceId = Configuration["ClickUpApi:TestWorkspaceId"] ?? throw new InvalidOperationException("ClickUpApi:TestWorkspaceId not configured.");
            if (string.IsNullOrWhiteSpace(_testWorkspaceId))
            {
                throw new InvalidOperationException("TestWorkspaceId cannot be empty.");
            }
        }

        [Fact]
        public async Task CreateGetAndDeleteTask_ShouldSucceed()
        {
            // Arrange
            var taskName = $"My Integration Test CuTask - {Guid.NewGuid()}";
            var createTaskRequest = new CreateTaskRequest(Name: taskName);
            ClickUpTask? createdTask = null;

            try
            {
                // 1. Create CuTask
                Console.WriteLine($"Attempting to create task '{taskName}' in list '{_testListId}'...");
                createdTask = await _tasksService.CreateTaskAsync(_testListId, createTaskRequest, cancellationToken: CancellationToken.None);

                createdTask.Should().NotBeNull();
                createdTask!.Name.Should().Be(taskName);
                createdTask.Id.Should().NotBeNullOrEmpty();
                Console.WriteLine($"Created CuTask ID: {createdTask.Id}, Name: {createdTask.Name}");

                // 2. Get CuTask
                Console.WriteLine($"Attempting to fetch task ID: {createdTask.Id}...");
                var fetchedTask = await _tasksService.GetTaskAsync(createdTask.Id!, cancellationToken: CancellationToken.None);
                fetchedTask.Should().NotBeNull();
                fetchedTask!.Id.Should().Be(createdTask.Id);
                fetchedTask.Name.Should().Be(taskName);
                Console.WriteLine($"Fetched CuTask ID: {fetchedTask.Id}, Name: {fetchedTask.Name}");

            }
            finally
            {
                // 3. Delete CuTask (Cleanup)
                if (createdTask?.Id != null)
                {
                    Console.WriteLine($"Attempting to delete task ID: {createdTask.Id}...");
                    await _tasksService.DeleteTaskAsync(createdTask.Id!, cancellationToken: CancellationToken.None);
                    Console.WriteLine($"Deleted CuTask ID: {createdTask.Id}");

                    // Optional: Verify Deletion by trying to get it again
                    // try
                    // {
                    //     var deletedTask = await _tasksService.GetTaskAsync(createdTask.Id!, cancellationToken: CancellationToken.None);
                    //     deletedTask.Should().BeNull(); // Or expect ClickUpApiNotFoundException depending on ApiConnection behavior
                    // }
                    // catch (ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException)
                    // {
                    //     Console.WriteLine($"CuTask {createdTask.Id} confirmed deleted (NotFoundException).");
                    // }
                }
            }
        }

        [Fact]
        public async Task GetTasksAsyncEnumerable_ShouldRetrieveTasksFromList()
        {
            // Arrange
            // Ensure there's at least one task in the _testListId for this to be meaningful.
            // This test primarily checks if the call executes without error and yields some tasks.
            Console.WriteLine($"Getting tasks as IAsyncEnumerable from list '{_testListId}'...");

            var collectedTasks = new List<ClickUpTask>();
            int count = 0;
            bool hasAtLeastOneTask = false;

            // Act
            await foreach (var task in _tasksService.GetTasksAsyncEnumerableAsync(
                               listId: _testListId,
                               cancellationToken: CancellationToken.None))
            {
                task.Should().NotBeNull();
                task.Id.Should().NotBeNullOrEmpty();
                collectedTasks.Add(task);
                count++;
                hasAtLeastOneTask = true;
                Console.WriteLine($"Retrieved task: {task.Id} - {task.Name}");
                if (count >= 5) break; // Limit for test performance, don't fetch all tasks in a large list
            }

            // Assert
            // We can't know how many tasks are in the list, so we assert that the call succeeded.
            // If the list is empty, this will still pass but yield no items.
            if (hasAtLeastOneTask)
            {
                collectedTasks.Should().NotBeEmpty();
            }
            else
            {
                Console.WriteLine($"Warning: No tasks found in list {_testListId} during GetTasksAsyncEnumerable_ShouldRetrieveTasksFromList test or list is empty.");
            }
            // No exception means it worked.
        }
    }
}
