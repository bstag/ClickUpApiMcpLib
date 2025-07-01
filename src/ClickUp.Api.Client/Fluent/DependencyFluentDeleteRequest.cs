using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.RequestModels.TaskRelationships; // Added for DeleteDependencyRequest

public class DependencyFluentDeleteRequest
{
    private readonly DeleteDependencyRequest _requestDto; // Use the DTO
    private readonly string _taskId;
    private readonly ITaskRelationshipsService _taskRelationshipsService;

    // Temp store for mutually exclusive params until DeleteAsync is called
    private string? _tempDependsOnTaskId;
    private string? _tempDependencyOfTaskId;

    public DependencyFluentDeleteRequest(string taskId, ITaskRelationshipsService taskRelationshipsService)
    {
        _taskId = taskId;
        _taskRelationshipsService = taskRelationshipsService;
        _requestDto = new DeleteDependencyRequest(); // Initialize with default constructor which expects one of the task ids to be set later
    }

    public DependencyFluentDeleteRequest WithDependsOnTaskId(string dependsOnTaskId)
    {
        _tempDependsOnTaskId = dependsOnTaskId;
        _tempDependencyOfTaskId = null; // Ensure mutual exclusivity
        return this;
    }

    public DependencyFluentDeleteRequest WithDependencyOfTaskId(string dependencyOfTaskId)
    {
        _tempDependencyOfTaskId = dependencyOfTaskId;
        _tempDependsOnTaskId = null; // Ensure mutual exclusivity
        return this;
    }

    public DependencyFluentDeleteRequest WithCustomTaskIds(bool customTaskIds)
    {
        _requestDto.CustomTaskIds = customTaskIds;
        return this;
    }

    public DependencyFluentDeleteRequest WithTeamId(string teamId)
    {
        _requestDto.TeamId = teamId;
        return this;
    }

    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        // Finalize DTO construction before calling service
        if (!string.IsNullOrWhiteSpace(_tempDependsOnTaskId))
        {
            _requestDto.DependsOnTaskId = _tempDependsOnTaskId;
            _requestDto.DependencyOfTaskId = null;
        }
        else if (!string.IsNullOrWhiteSpace(_tempDependencyOfTaskId))
        {
            _requestDto.DependencyOfTaskId = _tempDependencyOfTaskId;
            _requestDto.DependsOnTaskId = null;
        }
        else
        {
            // This case should ideally be prevented by fluent design or handled if user calls DeleteAsync without specifying one.
            // The DTO constructor will throw if both are null.
            throw new InvalidOperationException("Either DependsOnTaskId or DependencyOfTaskId must be specified for deleting a dependency using the fluent builder.");
        }

        // The DTO constructor now handles validation for one of them being set.
        // We re-assign here to ensure the DTO is valid before use,
        // or rather, we'd rely on the DTO's constructor logic if we passed these values to it.
        // For simplicity with the current DTO, we ensure its internal state is correct.

        await _taskRelationshipsService.DeleteDependencyAsync(
            _taskId,
            _requestDto,
            cancellationToken
        );
    }
}