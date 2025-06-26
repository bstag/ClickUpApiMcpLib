using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Templates;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentTemplatesApi
{
    private readonly ITemplatesService _templatesService;

    public FluentTemplatesApi(ITemplatesService templatesService)
    {
        _templatesService = templatesService;
    }

    public FluentGetTaskTemplatesRequest GetTaskTemplates(string workspaceId)
    {
        return new FluentGetTaskTemplatesRequest(workspaceId, _templatesService);
    }
}
