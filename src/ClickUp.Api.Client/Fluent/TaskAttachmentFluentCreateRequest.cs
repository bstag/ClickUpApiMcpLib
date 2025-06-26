using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Attachments;
using ClickUp.Api.Client.Models.ResponseModels.Attachments;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TaskAttachmentFluentCreateRequest
{
    private readonly CreateTaskAttachmentRequest _request = new();
    private readonly string _taskId;
    private readonly Stream _fileStream;
    private readonly string _fileName;
    private readonly IAttachmentsService _attachmentsService;

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

    public async Task<CreateTaskAttachmentResponse> CreateAsync(CancellationToken cancellationToken = default)
    {
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