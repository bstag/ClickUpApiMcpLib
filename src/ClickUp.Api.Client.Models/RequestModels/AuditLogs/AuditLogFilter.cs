using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.AuditLogs;

/// <summary>
/// Represents the filter for querying audit logs.
/// </summary>
public record class AuditLogFilter
(
    [property: JsonPropertyName("workspaceId")]
    string WorkspaceId,

    [property: JsonPropertyName("userId")]
    List<string>? UserId,

    [property: JsonPropertyName("userEmail")]
    List<string>? UserEmail,

    [property: JsonPropertyName("eventType")]
    List<string>? EventType,

    [property: JsonPropertyName("eventStatus")]
    string? EventStatus,

    [property: JsonPropertyName("startTime")]
    long? StartTime,

    [property: JsonPropertyName("endTime")]
    long? EndTime
);
