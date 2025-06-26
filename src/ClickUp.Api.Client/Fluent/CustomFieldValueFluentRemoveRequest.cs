using ClickUp.Api.Client.Abstractions.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class CustomFieldValueFluentRemoveRequest
{
    private readonly string _taskId;
    private readonly string _fieldId;
    private readonly ICustomFieldsService _customFieldsService;

    public CustomFieldValueFluentRemoveRequest(string taskId, string fieldId, ICustomFieldsService customFieldsService)
    {
        _taskId = taskId;
        _fieldId = fieldId;
        _customFieldsService = customFieldsService;
    }

    public CustomFieldValueFluentRemoveRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public CustomFieldValueFluentRemoveRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    private bool? _customTaskIds;
    private string? _teamId;

    public async Task RemoveAsync(CancellationToken cancellationToken = default)
    {
        await _customFieldsService.RemoveCustomFieldValueAsync(
            _taskId,
            _fieldId,
            _customTaskIds,
            _teamId,
            cancellationToken
        );
    }
}