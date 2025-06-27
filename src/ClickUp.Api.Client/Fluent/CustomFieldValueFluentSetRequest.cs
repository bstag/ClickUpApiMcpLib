using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class CustomFieldValueFluentSetRequest
{
    private readonly string _taskId;
    private readonly string _fieldId;
    private readonly ICustomFieldsService _customFieldsService;

    public CustomFieldValueFluentSetRequest(string taskId, string fieldId, ICustomFieldsService customFieldsService)
    {
        _taskId = taskId;
        _fieldId = fieldId;
        _customFieldsService = customFieldsService;
    }

    public async Task SetAsync(SetCustomFieldValueRequest setFieldValueRequest, bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        await _customFieldsService.SetCustomFieldValueAsync(
            _taskId,
            _fieldId,
            setFieldValueRequest,
            customTaskIds,
            teamId,
            cancellationToken
        );
    }
}