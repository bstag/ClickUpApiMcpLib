using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Templates;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentGetTaskTemplatesRequest
{
    private int? _page;

    private readonly string _workspaceId;
    private readonly ITemplatesService _templatesService;

    public FluentGetTaskTemplatesRequest(string workspaceId, ITemplatesService templatesService)
    {
        _workspaceId = workspaceId;
        _templatesService = templatesService;
    }

    public FluentGetTaskTemplatesRequest WithPage(int page)
    {
        _page = page;
        return this;
    }

    public async Task<GetTaskTemplatesResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _templatesService.GetTaskTemplatesAsync(
            _workspaceId,
            _page ?? 0, // Assuming 0 is a valid default page number
            cancellationToken
        );
    }
}