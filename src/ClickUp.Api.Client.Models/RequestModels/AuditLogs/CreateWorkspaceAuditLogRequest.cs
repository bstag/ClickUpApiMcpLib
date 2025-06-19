using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.AuditLogs;

/// <summary>
/// Represents the request model for creating a workspace audit log.
/// </summary>
public record class CreateWorkspaceAuditLogRequest
(
    [property: JsonPropertyName("filter")]
    AuditLogFilter? Filter,

    [property: JsonPropertyName("applicability")]
    string Applicability,

    [property: JsonPropertyName("pagination")]
    AuditLogPagination? Pagination
);
