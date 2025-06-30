using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common; // Changed from ResponseModels.Members to Common for Member model
using System.Collections.Generic;
using System.Runtime.CompilerServices; // Added for EnumeratorCancellation
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class MembersFluentApi
{
    private readonly IMembersService _membersService;

    public MembersFluentApi(IMembersService membersService)
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

    /// <summary>
    /// Retrieves all members of a specific Task asynchronously.
    /// While this method returns an IAsyncEnumerable, the underlying ClickUp API for task members
    /// does not appear to be paginated, so all members are typically fetched in a single call by the service.
    /// </summary>
    /// <param name="taskId">The ID of the task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Member"/>.</returns>
    public async IAsyncEnumerable<Member> GetTaskMembersAsyncEnumerableAsync(
        string taskId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var members = await _membersService.GetTaskMembersAsync(taskId, cancellationToken).ConfigureAwait(false);
        if (members != null)
        {
            foreach (var member in members)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return member;
            }
        }
    }

    /// <summary>
    /// Retrieves all members of a specific List asynchronously.
    /// While this method returns an IAsyncEnumerable, the underlying ClickUp API for list members
    /// does not appear to be paginated, so all members are typically fetched in a single call by the service.
    /// </summary>
    /// <param name="listId">The ID of the list.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Member"/>.</returns>
    public async IAsyncEnumerable<Member> GetListMembersAsyncEnumerableAsync(
        string listId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var members = await _membersService.GetListMembersAsync(listId, cancellationToken).ConfigureAwait(false);
        if (members != null)
        {
            foreach (var member in members)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return member;
            }
        }
    }
}