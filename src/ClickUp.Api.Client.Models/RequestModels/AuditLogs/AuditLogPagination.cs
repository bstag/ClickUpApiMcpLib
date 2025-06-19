using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.AuditLogs;

/// <summary>
/// Represents the pagination settings for querying audit logs.
/// </summary>
public record class AuditLogPagination
(
    [property: JsonPropertyName("pageRows")]
    int? PageRows,

    [property: JsonPropertyName("pageTimestamp")]
    long? PageTimestamp,

    [property: JsonPropertyName("pageDirection")]
    string? PageDirection
);
