using System.IO;

namespace ClickUp.Api.Client.Models.RequestModels.Attachments;

public class CreateTaskAttachmentRequest
{
    public bool? CustomTaskIds { get; set; }
    public string? TeamId { get; set; }
}