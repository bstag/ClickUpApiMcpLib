using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Templates;
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

    public TaskTemplatesFluentGetRequest GetTaskTemplates(string workspaceId)
    {
        return new TaskTemplatesFluentGetRequest(workspaceId, _templatesService);
    }
}
