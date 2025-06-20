using System.Text.Json.Serialization;
using System.Collections.Generic; // For List

namespace ClickUp.Api.Client.Models.RequestModels.Goals
{
    public record EditKeyResultRequest
    (
        // Fields from OpenAPI spec for EditKeyResultrequest
        [property: JsonPropertyName("steps_current")] object? StepsCurrent, // Can be int, bool, string depending on KR type
        [property: JsonPropertyName("note")] string? Note,

        // Additional potentially editable fields (referencing CreateKeyResultRequest for common ones)
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("owners")] List<int>? Owners, // List of user IDs
        [property: JsonPropertyName("add_owners")] List<int>? AddOwners, // To add new owners
        [property: JsonPropertyName("rem_owners")] List<int>? RemoveOwners, // To remove existing owners
        // Type, steps_start, steps_end, unit are often not editable after creation or have specific endpoints.
        // CuTask/List IDs might be editable or managed via separate endpoints.
        [property: JsonPropertyName("task_ids")] List<string>? TaskIds,
        [property: JsonPropertyName("list_ids")] List<string>? ListIds,
        [property: JsonPropertyName("archived")] bool? Archived // To archive/unarchive key result
    );
}
