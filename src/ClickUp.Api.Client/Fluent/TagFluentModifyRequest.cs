using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.RequestModels.Spaces;
using ClickUp.Api.Client.Models.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Fluent;

public class TagFluentModifyRequest
{
    private string? _name;
    private string? _tagForegroundColor;
    private string? _tagBackgroundColor;

    private readonly string _spaceId;
    private readonly ITagsService _tagsService;
    private readonly string? _originalTagName; // Used for EditSpaceTagAsync
    private readonly List<string> _validationErrors = new List<string>();

    public TagFluentModifyRequest(string spaceId, ITagsService tagsService, string? originalTagName = null)
    {
        _spaceId = spaceId;
        _tagsService = tagsService;
        _originalTagName = originalTagName;
    }

    public TagFluentModifyRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public TagFluentModifyRequest WithTagForegroundColor(string tagForegroundColor)
    {
        _tagForegroundColor = tagForegroundColor;
        return this;
    }

    public TagFluentModifyRequest WithTagBackgroundColor(string tagBackgroundColor)
    {
        _tagBackgroundColor = tagBackgroundColor;
        return this;
    }

    private void ValidateForCreate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_spaceId))
        {
            _validationErrors.Add("SpaceId is required for creating a tag.");
        }
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors.Add("Tag name is required for creating a tag.");
        }
        // Colors are optional according to API, but could be validated for format if needed.

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Tag creation request validation failed.", _validationErrors);
        }
    }

    private void ValidateForEdit()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_spaceId))
        {
            _validationErrors.Add("SpaceId is required for editing a tag.");
        }
        if (string.IsNullOrWhiteSpace(_originalTagName))
        {
            _validationErrors.Add("Original tag name must be provided for editing.");
        }
        if (string.IsNullOrWhiteSpace(_name))
        {
            _validationErrors.Add("New tag name is required for editing.");
        }
        // Colors are optional

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Tag edit request validation failed.", _validationErrors);
        }
    }

    public async Task CreateAsync(CancellationToken cancellationToken = default)
    {
        ValidateForCreate();
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
        ValidateForEdit();
        // Original check for _originalTagName is now part of ValidateForEdit
        // if (string.IsNullOrEmpty(_originalTagName))
        // {
        //     throw new System.InvalidOperationException("Original tag name must be provided for editing.");
        // }

        var modifyTagRequest = new ModifyTagRequest
        {
            Name = _name ?? string.Empty,
            TagForegroundColor = _tagForegroundColor ?? string.Empty,
            TagBackgroundColor = _tagBackgroundColor ?? string.Empty
        };

        return await _tagsService.EditSpaceTagAsync(
            _spaceId,
            _originalTagName!, // Validated not to be null by ValidateForEdit
            modifyTagRequest,
            cancellationToken
        );
    }
}