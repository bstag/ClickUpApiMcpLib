using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.CustomFields;
using ClickUp.Api.Client.Models.RequestModels.CustomFields;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class CustomFieldsFluentApi
{
    private readonly ICustomFieldsService _customFieldsService;

    public CustomFieldsFluentApi(ICustomFieldsService customFieldsService)
    {
        _customFieldsService = customFieldsService;
    }

    public async Task<IEnumerable<Field>> GetAccessibleCustomFieldsAsync(string listId, CancellationToken cancellationToken = default)
    {
        return await _customFieldsService.GetAccessibleCustomFieldsAsync(listId, cancellationToken);
    }

    public async Task<IEnumerable<Field>> GetFolderCustomFieldsAsync(string folderId, CancellationToken cancellationToken = default)
    {
        return await _customFieldsService.GetFolderCustomFieldsAsync(folderId, cancellationToken);
    }

    public async Task<IEnumerable<Field>> GetSpaceCustomFieldsAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        return await _customFieldsService.GetSpaceCustomFieldsAsync(spaceId, cancellationToken);
    }

    public async Task<IEnumerable<Field>> GetWorkspaceCustomFieldsAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _customFieldsService.GetWorkspaceCustomFieldsAsync(workspaceId, cancellationToken);
    }

    public CustomFieldValueFluentSetRequest SetCustomFieldValue(string taskId, string fieldId)
    {
        return new CustomFieldValueFluentSetRequest(taskId, fieldId, _customFieldsService);
    }

    

    public CustomFieldValueFluentRemoveRequest RemoveCustomFieldValue(string taskId, string fieldId)
    {
        return new CustomFieldValueFluentRemoveRequest(taskId, fieldId, _customFieldsService);
    }
}
