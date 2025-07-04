using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Attachments;
using ClickUp.Api.Client.Models.ResponseModels.Attachments;
using ClickUp.Api.Client.Models.Exceptions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Fluent;

public class TaskAttachmentFluentCreateRequest
{
    private readonly CreateTaskAttachmentRequest _request = new();
    private readonly string _taskId;
    private readonly Stream _fileStream;
    private readonly string _fileName;
    private readonly IAttachmentsService _attachmentsService;
    private readonly List<string> _validationErrors = new List<string>();

    public TaskAttachmentFluentCreateRequest(string taskId, Stream fileStream, string fileName, IAttachmentsService attachmentsService)
    {
        _taskId = taskId;
        _fileStream = fileStream;
        _fileName = fileName;
        _attachmentsService = attachmentsService;
    }

    public TaskAttachmentFluentCreateRequest WithCustomTaskIds(bool customTaskIds)
    {
        _request.CustomTaskIds = customTaskIds;
        return this;
    }

    public TaskAttachmentFluentCreateRequest WithTeamId(string teamId)
    {
        _request.TeamId = teamId;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_taskId))
        {
            _validationErrors.Add("TaskId is required.");
        }
        if (_fileStream == null || (_fileStream == Stream.Null && _fileStream.Length == 0)) // Consider Stream.Null as invalid if it's truly empty and not just a placeholder
        {
            _validationErrors.Add("FileStream is required.");
        }
        if (string.IsNullOrWhiteSpace(_fileName))
        {
            _validationErrors.Add("FileName is required.");
        }
        // Add other validation rules as needed

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<CreateTaskAttachmentResponse> CreateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
        return await _attachmentsService.CreateTaskAttachmentAsync(
            _taskId,
            _fileStream,
            _fileName,
            _request.CustomTaskIds,
            _request.TeamId,
            cancellationToken
        );
    }
}