using ClickUp.Api.Client.Abstractions.Services;
using System.IO;

namespace ClickUp.Api.Client.Fluent;

public class AttachmentsFluentApi
{
    private readonly IAttachmentsService _attachmentsService;

    public AttachmentsFluentApi(IAttachmentsService attachmentsService)
    {
        _attachmentsService = attachmentsService;
    }

    public TaskAttachmentFluentCreateRequest Create(string taskId, Stream fileStream, string fileName)
    {
        return new TaskAttachmentFluentCreateRequest(taskId, fileStream, fileName, _attachmentsService);
    }
}
