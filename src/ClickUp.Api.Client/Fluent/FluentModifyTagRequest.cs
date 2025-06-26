using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentModifyTagRequest
{
    private string? _name;
    private string? _tagForegroundColor;
    private string? _tagBackgroundColor;

    private readonly string _spaceId;
    private readonly ITagsService _tagsService;
    private readonly string? _originalTagName; // Used for EditSpaceTagAsync

    public FluentModifyTagRequest(string spaceId, ITagsService tagsService, string? originalTagName = null)
    {
        _spaceId = spaceId;
        _tagsService = tagsService;
        _originalTagName = originalTagName;
    }

    public FluentModifyTagRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FluentModifyTagRequest WithTagForegroundColor(string tagForegroundColor)
    {
        _tagForegroundColor = tagForegroundColor;
        return this;
    }

    public FluentModifyTagRequest WithTagBackgroundColor(string tagBackgroundColor)
    {
        _tagBackgroundColor = tagBackgroundColor;
        return this;
    }

    public async Task CreateAsync(CancellationToken cancellationToken = default)
    {
        var modifyTagRequest = new ModifyTagRequest
        {
            Name = _name ?? string.Empty,
            TagForegroundColor = _tagForegroundColor ?? string.Empty,
            TagBackgroundColor = _tagBackgroundColor ?? string.Empty
        };

        await _tagsService.CreateSpaceTagAsync(
            _spaceId,
            modifyTagRequest,
            cancellationToken
        );
    }

    public async Task<Tag> EditAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_originalTagName))
        {
            throw new System.InvalidOperationException("Original tag name must be provided for editing.");
        }

        var modifyTagRequest = new ModifyTagRequest
        {
            Name = _name ?? string.Empty,
            TagForegroundColor = _tagForegroundColor ?? string.Empty,
            TagBackgroundColor = _tagBackgroundColor ?? string.Empty
        };

        return await _tagsService.EditSpaceTagAsync(
            _spaceId,
            _originalTagName,
            modifyTagRequest,
            cancellationToken
        );
    }
}