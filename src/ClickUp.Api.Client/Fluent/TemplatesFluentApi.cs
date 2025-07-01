using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Templates;
using ClickUp.Api.Client.Models.ResponseModels.Templates;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TemplatesFluentApi
{
    private readonly ITemplatesService _templatesService;

    public TemplatesFluentApi(ITemplatesService templatesService)
    {
        _templatesService = templatesService;
    }

    public TaskTemplatesFluentQueryRequest GetTaskTemplates(string workspaceId)
    {
        return new TaskTemplatesFluentQueryRequest(workspaceId, _templatesService);
    }

    /// <summary>
    /// Retrieves all task templates for a specific workspace asynchronously, handling pagination.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace (team).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="TaskTemplate"/>.</returns>
    public async IAsyncEnumerable<TaskTemplate> GetTemplatesAsyncEnumerableAsync(
        string workspaceId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int page = 0;
        bool morePages = true;

        while (morePages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GetTaskTemplatesResponse response = await _templatesService.GetTaskTemplatesAsync(workspaceId, page, cancellationToken).ConfigureAwait(false);

            if (response?.Templates != null && response.Templates.Count > 0)
            {
                foreach (var template in response.Templates)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return template;
                }
                page++;
            }
            else
            {
                morePages = false;
            }
        }
    }
}
