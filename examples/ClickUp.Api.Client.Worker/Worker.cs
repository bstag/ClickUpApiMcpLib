using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks; // Ensure this is present
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using ClickUp.Api.Client.Models.Entities.Users;
using Microsoft.Extensions.Options;
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask

namespace ClickUp.Api.Client.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITasksService _tasksService;
    private readonly IAuthorizationService _authorizationService;
    private readonly WorkerExampleSettings _workerSettings;
    private DateTimeOffset _lastPollTime;

    public Worker(
        ILogger<Worker> logger,
        ITasksService tasksService,
        IAuthorizationService authorizationService,
        IOptions<WorkerExampleSettings> workerSettings)
    {
        _logger = logger;
        _tasksService = tasksService;
        _authorizationService = authorizationService;
        _workerSettings = workerSettings.Value;
        _lastPollTime = DateTimeOffset.UtcNow;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ClickUp Worker Service starting.");
        _logger.LogInformation("Polling List ID: {ListId}", string.IsNullOrWhiteSpace(_workerSettings.ListIdForPolling) || _workerSettings.ListIdForPolling.Contains("YOUR_") ? "NOT CONFIGURED" : _workerSettings.ListIdForPolling);
        _logger.LogInformation("Polling Interval: {Interval} seconds", _workerSettings.PollingIntervalSeconds);

        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker polling for tasks at: {time}", DateTimeOffset.Now);

            if (string.IsNullOrWhiteSpace(_workerSettings.ListIdForPolling) || _workerSettings.ListIdForPolling.Contains("YOUR_"))
            {
                _logger.LogWarning("ListIdForPolling is not configured in appsettings.json. Skipping task polling.");
                await Task.Delay(TimeSpan.FromSeconds(_workerSettings.PollingIntervalSeconds), stoppingToken);
                continue;
            }

            try
            {
                User? authorizedUser = await _authorizationService.GetAuthorizedUserAsync(stoppingToken);
                if (authorizedUser == null)
                {
                    _logger.LogError("Failed to authorize with ClickUp API: User object is null. Check API token.");
                    await Task.Delay(TimeSpan.FromSeconds(_workerSettings.PollingIntervalSeconds), stoppingToken);
                    continue;
                }
                _logger.LogDebug("Successfully authorized as {User}", authorizedUser.Username);

                var getTasksRequest = new GetTasksRequest // This should now be found with the using statement
                {
                    IncludeClosed = false,
                };

                _logger.LogDebug("Fetching tasks for List ID: {ListId}. Last poll time: {LastPollTime}", _workerSettings.ListIdForPolling, _lastPollTime);

                GetTasksResponse? tasksResponse = await _tasksService.GetTasksAsync(_workerSettings.ListIdForPolling, getTasksRequest, stoppingToken);

                if (tasksResponse != null && tasksResponse.Tasks != null)
                {
                    if (tasksResponse.Tasks.Any())
                    {
                        _logger.LogInformation("Found {TaskCount} tasks in list {ListId}.", tasksResponse.Tasks.Count, _workerSettings.ListIdForPolling);
                        foreach (var task in tasksResponse.Tasks)
                        {
                            // Use task.DateUpdated (DateTimeOffset?) directly
                            DateTimeOffset taskUpdateTime = task.DateUpdated ?? DateTimeOffset.MinValue;
                            if (taskUpdateTime > _lastPollTime)
                            {
                                _logger.LogInformation("New or Updated Task: ID={TaskId}, Name='{TaskName}', Status='{TaskStatus}', UpdatedAt='{TaskUpdatedAt}'",
                                    task.Id, task.Name, task.Status?.StatusValue, taskUpdateTime.ToString("o")); // Use Status?.StatusValue
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No tasks found in list {ListId}.", _workerSettings.ListIdForPolling);
                    }
                    _lastPollTime = DateTimeOffset.UtcNow;
                }
                else
                {
                    _logger.LogError("Failed to get tasks for list {ListId} or tasks response was null/empty.", _workerSettings.ListIdForPolling);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Worker polling was canceled.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during worker execution.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_workerSettings.PollingIntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("ClickUp Worker Service stopping.");
    }
}
