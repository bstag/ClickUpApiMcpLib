using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Members;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentMembersApi
{
    private readonly IMembersService _membersService;

    public FluentMembersApi(IMembersService membersService)
    {
        _membersService = membersService;
    }

    public async Task<IEnumerable<Member>> GetTaskMembersAsync(string taskId, CancellationToken cancellationToken = default)
    {
        return await _membersService.GetTaskMembersAsync(taskId, cancellationToken);
    }

    public async Task<IEnumerable<Member>> GetListMembersAsync(string listId, CancellationToken cancellationToken = default)
    {
        return await _membersService.GetListMembersAsync(listId, cancellationToken);
    }
}