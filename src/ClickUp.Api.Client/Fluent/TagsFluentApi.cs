using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TagsFluentApi
{
    private readonly ITagsService _tagsService;

    public TagsFluentApi(ITagsService tagsService)
    {
        _tagsService = tagsService;
    }

    public async Task<IEnumerable<Tag>> GetSpaceTagsAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        return await _tagsService.GetSpaceTagsAsync(spaceId, cancellationToken);
    }

    public TagFluentModifyRequest CreateSpaceTag(string spaceId)
    {
        return new TagFluentModifyRequest(spaceId, _tagsService);
    }

    public TagFluentModifyRequest EditSpaceTag(string spaceId, string tagName)
    {
        return new TagFluentModifyRequest(spaceId, _tagsService, tagName);
    }

    public async Task DeleteSpaceTagAsync(string spaceId, string tagName, CancellationToken cancellationToken = default)
    {
        await _tagsService.DeleteSpaceTagAsync(spaceId, tagName, cancellationToken);
    }

    public async Task AddTagToTaskAsync(string taskId, string tagName, bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        await _tagsService.AddTagToTaskAsync(taskId, tagName, customTaskIds, teamId, cancellationToken);
    }

    public async Task RemoveTagFromTaskAsync(string taskId, string tagName, bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        await _tagsService.RemoveTagFromTaskAsync(taskId, tagName, customTaskIds, teamId, cancellationToken);
    }
}
