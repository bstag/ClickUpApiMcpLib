using ClickUp.Api.Client.Abstractions.Services;
using System.IO;

namespace ClickUp.Api.Client.Fluent;

public class FluentAttachmentsApi
{
    private readonly IAttachmentsService _attachmentsService;

    public FluentAttachmentsApi(IAttachmentsService attachmentsService)
    {
        _attachmentsService = attachmentsService;
    }

    public FluentCreateTaskAttachmentRequest Create(string taskId, Stream fileStream, string fileName)
    {
        return new FluentCreateTaskAttachmentRequest(taskId, fileStream, fileName, _attachmentsService);
    }
}
