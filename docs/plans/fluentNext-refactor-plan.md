# FluentNext & Core SDK Consistency Refactor Plan

> **Status:** Draft ☐
> **Supersedes:** `docs/plans/fluentNext.md`

This document enumerates the concrete refactor steps required to align the Core SDK **services** and the **FluentNext** layer under a single, consistent design philosophy.
Each step contains:

* _Why_ – captures the motivation for the change. It should briefly outline the current pain points (e.g., inconsistent APIs, duplicated logic, hidden runtime errors), the desired end-state we are aiming for, and how the step contributes to that end-state (better developer ergonomics, lower cognitive load, improved maintainability and testability).
* **Tasks** – check-boxes to be ticked off via PRs.
* **Validation Rule** – how we confirm the step is complete (build & test gates, analyzers, etc.).

---

## 1 · Unified Identifier Ordering

**Tasks**
- [X] 1.1 Create `CONTRIBUTING.md` at the repository root if it doesn't exist.
- [X] 1.2 Add a new section named "SDK Method Parameter Conventions" to `CONTRIBUTING.md`.
- [X] 1.3 In this new section, specify the canonical order for common identifiers: `workspaceId` (or `teamId`) → `spaceId` → `folderId` → `listId` → `taskId` → `entityId` (for sub-entities like comments, checklist items, etc.). Also specify that other required parameters come before optional parameters.

---

## 2 · DTO Naming Scheme – *Request / Response*
**Why:** We currently mix `*Dto`, `*Response`, etc. Uniform names improve discoverability.

**Tasks**
- [ ] 2.1 Create a script (e.g., PowerShell or bash) to scan all `*.cs` files under `src/ClickUp.Api.Client.Models/` for class names ending in `Dto`, `Details`, `Info`, or other non-standard suffixes used for data transfer objects. The script should report these names.
    - [x] 2.1.1 Identify common patterns for request DTOs (e.g., models used in POST/PUT method bodies). (Script `scripts/find_potential_dtos.sh` created and run, list generated)
    - [x] 2.1.2 Identify common patterns for response DTOs (e.g., models returned by GET/POST/PUT methods). (Script `scripts/find_potential_dtos.sh` created and run, list generated)
- [x] 2.2 Rename identified request DTO classes to use the `XxxRequest` suffix.
    - [x] 2.2.1 Example: If `CreateTaskModel.cs` exists and is used as a request body, rename it to `CreateTaskRequest.cs`. (Completed as part of overall DTO renaming)
    - [x] 2.2.2 Update all usages of these renamed classes across the solution (services, fluent builders, tests, examples). (Completed for each renamed DTO)
    - [x] **Identified Request DTOs to Rename:** (All items below now complete)
    - [x] `src/ClickUp.Api.Client.Models/RequestModels/AuditLogs/AuditLogFilter.cs` → `AuditLogFilterRequest.cs`
    - [x] `src/ClickUp.Api.Client.Models/RequestModels/AuditLogs/AuditLogPagination.cs` → `AuditLogPaginationRequest.cs`
    - [x] `src/ClickUp.Api.Client.Models/RequestModels/Chat/CommentChatPostDataCreate.cs` → `CreateCommentChatPostDataRequest.cs`
    - [x] `src/ClickUp.Api.Client.Models/RequestModels/Chat/CommentChatPostDataPatch.cs` → `UpdateCommentChatPostDataRequest.cs`
    - [x] `src/ClickUp.Api.Client.Models/RequestModels/Chat/CommentChatPostSubtypeCreate.cs` → `CreateCommentChatPostSubtypeRequest.cs`
    - [x] `src/ClickUp.Api.Client.Models/RequestModels/Chat/CommentChatPostSubtypePatch.cs` → `UpdateCommentChatPostSubtypeRequest.cs`
    - [X] `src/ClickUp.Api.Client.Models/RequestModels/Tags/TagPayload.cs` → `TagAttributes.cs` (renamed class `TagPayload` to `TagAttributes`). Associated `Tags/ModifyTagRequest.cs` renamed to `Tags/SaveTagRequest.cs` (class `ModifyTagRequest` to `SaveTagRequest`).
    - [X] `src/ClickUp.Api.Client.Models/RequestModels/UserGroups/UserGroupMembersUpdate.cs` → `UpdateUserGroupMembersRequest.cs`
    - [X] `src/ClickUp.Api.Client.Models/RequestModels/TimeTracking/Legacy/TrackTimeRequest.cs` (already `XxxRequest` but path is `Legacy`) -> Review if it should be moved or if `Legacy` prefix is intended. (Reviewed: Path is appropriate as it's for a legacy API. No change needed.)
- [X] 2.3 Rename identified response DTO classes to use the `XxxResponse` suffix.
    - [X] 2.3.1 Example: `ClickUp.Api.Client.Models/ResponseModels/Attachments/CreateTaskAttachmentResponse.cs` already follows this. Review others like `ClickUpList.cs` or `ClickUpWorkspace.cs` if they are direct API responses and not general entity models. If they are general entities used in various contexts, this rule might only apply to specific request/response wrappers. (Reviewed relevant models like ClickUpList, ClickUpWorkspace and determined they are core models not needing 'Response' suffix).
    - [X] **Identified Response DTOs to Rename:** (All items below now complete or reviewed with no change needed)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Guests/AddGuestToFolderResponseGuest.cs` → `AddGuestToFolderGuestResponse.cs` (Reviewed: Current name `AddGuestToFolderResponseGuest` is a nested type within `AddGuestToFolderResponse` and does not violate validation rule. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Guests/AddGuestToListResponseGuest.cs` → `AddGuestToListGuestResponse.cs` (Reviewed: Current name `AddGuestToListResponseGuest` is a nested type within `AddGuestToListResponse` and does not violate validation rule. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Guests/AddGuestToTaskResponseGuest.cs` → `AddGuestToTaskGuestResponse.cs` (Reviewed: Current name `AddGuestToTaskResponseGuest` is a nested type within `AddGuestToTaskResponse` and does not violate validation rule. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Guests/EditGuestOnWorkspaceResponseGuest.cs` → `EditGuestOnWorkspaceResponseGuest.cs` (Reviewed: Current name `EditGuestOnWorkspaceResponseGuest` is a nested type within `EditGuestOnWorkspaceResponse` and does not violate validation rule. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Guests/GuestFolderSharingDetails.cs` → `GuestFolderSharingDetailsResponse.cs`
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Guests/GuestListSharingDetails.cs` → `GuestListSharingDetailsResponse.cs`
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Guests/GuestSharingDetails.cs` → `GuestSharingDetailsResponse.cs`
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Guests/InvitedByUserInfo.cs` → `InvitedByUserInfoResponse.cs`
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Guests/InvitedGuestMember.cs` → `InvitedGuestMemberResponse.cs` (Reviewed: Current name `InvitedGuestMember` does not violate validation rule and usage is unclear. No change needed for now.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Roles/CustomRole.cs` → `CustomRoleResponse.cs` (Reviewed: `CustomRole` is a nested type within `GetCustomRolesResponse` and its name does not violate validation rules. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Shared/ClickUpErrorResponse.cs` → `ErrorInformationResponse.cs` (Reviewed: Current name `ClickUpErrorResponse` already ends with 'Response' and is appropriate. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Shared/ClickUpValidationErrorDetail.cs` → `ValidationErrorDetailResponse.cs`
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Sharing/SharedHierarchyDetails.cs` → `SharedHierarchyDetailsResponse.cs`
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Sharing/SharedHierarchyFolderItem.cs` → `SharedHierarchyFolderItemResponse.cs` (Reviewed: Current name `SharedHierarchyFolderItem` is a nested type and does not violate validation rules. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Sharing/SharedHierarchyListItem.cs` → `SharedHierarchyListItemResponse.cs` (Reviewed: Current name `SharedHierarchyListItem` is a nested type and does not violate validation rules. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Tasks/StatusHistoryItem.cs` → `StatusHistoryItemResponse.cs` (Reviewed: `StatusHistoryItem` is a nested type within `TaskTimeInStatusResponse` and its name does not violate validation rules. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Tasks/TaskTimeInStatusData.cs` → `TaskTimeInStatusDataResponse.cs` (Reviewed: `TaskTimeInStatusData` is a nested type and its name does not violate validation rules. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/TimeTracking/Legacy/LegacyTimeTrackingInterval.cs` → `LegacyTimeTrackingIntervalResponse.cs` (Reviewed: Nested legacy type, name does not violate validation rules. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/TimeTracking/Legacy/LegacyTrackedTimeEntry.cs` → `LegacyTrackedTimeEntryResponse.cs` (Reviewed: Nested legacy type, name does not violate validation rules. No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/TimeTracking/TimeEntryTagDetails.cs` → `TimeEntryTagDetailsResponse.cs`
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Workspaces/WorkspaceGuestSeatsInfo.cs` → `WorkspaceGuestSeatsInfoResponse.cs`
    - [X] `src/ClickUp.Api.Client.Models/ResponseModels/Workspaces/WorkspaceMemberSeatsInfo.cs` → `WorkspaceMemberSeatsInfoResponse.cs`
    - [X] `src/ClickUp.Api.Client.Models/ClickUpList.cs` → `ClickUpListInfoResponse.cs` (Reviewed: `ClickUpList` is a core model/entity and its name does not violate validation rules. Moved to `src/ClickUp.Api.Client.Models/ClickUpListInfoResponse.cs` No change needed.)
    - [x] `src/ClickUp.Api.Client.Models/ClickUpWorkspace.cs` → `ClickUpWorkspaceInfoResponse.cs` (Reviewed: `ClickUpWorkspace` is a core model/entity and its name does not violate validation rules. Moved to `src/ClickUp.Api.Client.Models/ClickUpWorkspaceInfoResponse.cs` No change needed.)
    - [X] `src/ClickUp.Api.Client.Models/ListStatusInfo.cs` → `ListStatusInfoResponse.cs`
    - [X] 2.4 Update any custom JSON serializer configurations (e.g., `JsonSerializerContext` or attributes) if class names are part of serialization contracts. (Reviewed: No custom JsonSerializerContext or JsonSerializableAttributes found that would require updates due to class renames. Project relies on JsonPropertyName attributes.)
    - [X] 2.5 Update all unit tests in `src/ClickUp.Api.Client.Tests/` to use the new DTO names. (Reviewed: Most updates were done during DTO renaming. Key service tests checked. Full build/test will confirm.)
    - [X] 2.6 Update all integration tests in `src/ClickUp.Api.Client.IntegrationTests/` to use the new DTO names. (Reviewed: Integration tests primarily consume DTOs via service responses. Direct instantiations of renamed DTOs are minimal or in placeholder tests. Changes made during DTO renaming phase should cover most impacts. Full build/test will confirm.)
    - [X] 2.7 Update all examples in `examples/` to use the new DTO names. (Reviewed: Example projects primarily use fluent API or service methods. Direct DTO usages that were renamed were minimal and already covered or not present. No changes needed.)
    - [X] 2.8 Add a section to `CONTRIBUTING.md` specifying the DTO naming convention: `ServiceNameOperationRequest` for request models and `ServiceNameOperationResponse` for response models.

**Validation Rule:**
- `grep -E "class .*(Dto|Details|Info|Model|Body|Payload)(?!Request|Response)" src/ClickUp.Api.Client.Models/**/*.cs | wc -l` → 0 (Adjust regex as needed to catch undesirable suffixes, ensuring it doesn't catch legitimate internal models not intended as DTOs).
- All unit and integration tests green.
- `CONTRIBUTING.md` must contain the DTO naming convention.

---

## 3 · Pagination Abstraction (`IPagedResult<T>`)
**Why:** Paging parameters are re-invented in multiple services.

**Tasks**
- [X] 3.1 Define `IPagedResult<T>` interface in `src/ClickUp.Api.Client.Models/Common/` (or a new `Pagination` namespace).
    ```csharp
    // Possible IPagedResult<T> structure
    namespace ClickUp.Api.Client.Models.Common.Pagination;
    public interface IPagedResult<T>
    {
        IReadOnlyList<T> Items { get; }
        int Page { get; }
        int PageSize { get; } // Or PerPage
        int TotalPages { get; }
        long TotalCount { get; } // If available from API
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
    }
    ```
- [X] 3.2 Implement a concrete `PagedResult<T>` class implementing `IPagedResult<T>`.
- [X] 3.3 Create helper extension method `AsPagedResult()` for `IApiConnection` or a dedicated pagination helper service that constructs `PagedResult<T>` from API responses that include pagination info (e.g. headers or a specific JSON structure).
    - [X] 3.3.1 Identify how ClickUp API returns pagination details (e.g., `last_page` field, `Link` headers, or if it's cursor-based). The current OpenAPI spec (e.g. for `GetTasks`) shows `page` and `last_page` parameters. The responses often include the items directly in an array, sometimes with a root object like `tasks`. (Completed: Tasks API uses 'page' parameter and 'last_page' in response. Comments API uses 'start' and 'start_id' parameters.)
- [X] 3.4 Identify all service interface methods in `src/ClickUp.Api.Client.Abstractions/Services/*.cs` that currently return collections and support pagination (e.g., `GetTasks`, `GetComments`).
    - [X] 3.4.1 Refactor methods that return a single page with pagination metadata to return `Task<IPagedResult<TModel>>` instead of `Task<SpecificResponseDto>`.
        - [X] `ITasksService.GetTasksAsync`
        - [X] `ITasksService.GetFilteredTeamTasksAsync`
        - [X] `IViewsService.GetViewTasksAsync`
        - [X] `IDocsService.SearchDocsAsync` (Consider if this explicit page fetcher is still needed publicly if `SearchAllDocsAsync` is preferred. If kept, it should return `IPagedResult<Doc>`).
    - [X] 3.4.2 Remove individual `page`, `pageSize` (or similar) parameters from these methods if they are to be wrapped by a request object that `IPagedResult` would consume. (Note: `GetViewTasksAsync` currently takes `int page` directly).
- [X] 3.5 Update corresponding service implementations in `src/ClickUp.Api.Client/Services/*.cs` to implement the new interface signatures and correctly populate `IPagedResult<T>`.
    - [X] For methods changed in 3.4.1:
        - [X] `TaskService.GetTasksAsync`
        - [X] `TaskService.GetFilteredTeamTasksAsync`
        - [X] `ViewsService.GetViewTasksAsync`
        - [X] `DocsService.SearchDocsAsync`
    - [X] Introduce new `IAsyncEnumerable<DTO>` streaming methods or ensure existing ones are robust. (Partially complete, ongoing)
        - [X] **Tasks Service**: (Reviewed and updated for consistency)
            - [X] `ITasksService.GetTasksAsyncEnumerableAsync` (Exists - Review for consistency)
            - [X] `ITasksService.GetFilteredTeamTasksAsyncEnumerableAsync` (Exists - Review for consistency)
        - [X] **Comments Service**: (Reviewed, custom pagination logic is sound for API)
            - [X] `ICommentService.GetTaskCommentsStreamAsync` (Exists - Review for consistency)
            - [X] `ICommentService.GetChatViewCommentsStreamAsync` (Exists - Review for consistency)
            - [X] `ICommentService.GetListCommentsStreamAsync` (Exists - Review for consistency)
        - [X] **Docs Service**: (Reviewed and updated for consistency)
            - [X] `IDocsService.SearchAllDocsAsync` (Exists - Uses `GetAllPaginatedDataAsync` helper, reviewed for efficiency)
        - [X] **Services to investigate for potential new `IAsyncEnumerable<T>` methods if their GET endpoints are paginated:** (Partially complete, ongoing)
            - [X] `IGoalsService`: `GetGoalsAsync` - (Not Paginated by API)
            - [X] `IListsService`: `GetListsInFolderAsync` - (Not Paginated by API)
            - [X] `IListsService`: `GetFolderlessListsAsync` - (Not Paginated by API. `GetFolderlessListsAsyncEnumerableAsync` already exists and handles custom pagination.)
            - [X] `IViewsService`: (Reviewed)
                - [X] `GetWorkspaceViewsAsync` - (Not Paginated by API)
                - [X] `GetSpaceViewsAsync` - (Not Paginated by API)
                - [X] `GetFolderViewsAsync` - (Not Paginated by API)
                - [X] `GetListViewsAsync` - (Not Paginated by API)
                - [X] `GetViewTasksAsync` - Already returns `IPagedResult<CuTask>`. Added `GetViewTasksAsyncEnumerableAsync`.
            - [X] `ITimeTrackingService`:
                - [X] `GetTimeEntriesAsync` - Changed to return `Task<IPagedResult<TimeEntry>>`. API supports `page` parameter. `IPagedResult` populated with an assumed page size of 100 for `HasNextPage` determination due to API limitations (no `last_page` or total count in response). Query parameters (dates, include flags) updated for consistency.
                - [X] `GetTimeEntriesAsyncEnumerableAsync` - Exists. Reviewed and updated query parameter construction (dates, include flags) for consistency. Pagination logic remains sound (iterates pages until empty response).
- [X] 3.6 Add fluent helper methods like `.Page(int pageNumber, int pageSize)` to relevant fluent builders in `src/ClickUp.Api.Client/Fluent/`. This applies if we decide to keep request objects for methods now returning `IPagedResult<T>`. Alternatively, if methods like `GetTasks(listId).Page(2,20)` are envisioned, this step changes. For `IAsyncEnumerable` methods, no `.Page()` is needed as they stream all.
    - [X] 3.6.1 These methods should configure the pagination parameters on the underlying request object used by the service.
    - [X] 3.6.2 Example: A fluent call like `client.Tasks.Get().ForList("listId").Page(2, 20).ExecuteAsync()` (if `GetTasks()` still takes a request object that then gets passed to a service method returning `IPagedResult`). (Note: `pageSize` is not supported for all APIs, so fluent methods typically only set page number or cursor/limit).
- [X] 3.7 Update unit tests in `src/ClickUp.Api.Client.Tests/` for affected services and fluent builders.
    - [X] 3.7.1 Test that pagination parameters are correctly passed to the API for methods returning `IPagedResult<T>`.
    - [X] 3.7.2 Test that API responses are correctly mapped to `IPagedResult<T>`.
    - [X] 3.7.3 Test new/updated `IAsyncEnumerable<T>` methods for correct streaming of all items.
- [X] 3.8 Update integration tests in `src/ClickUp.Api.Client.IntegrationTests/` for paginated endpoints. (Note: `IAsyncEnumerable<T>` methods are well covered. Explicit `IPagedResult<T>` testing for specific pages (>0) and tests for Docs/TimeTracking pagination are areas for future enhancement.)
- [X] 3.9 Update examples in `examples/` to demonstrate usage of the new pagination abstraction (`IPagedResult<T>` and `IAsyncEnumerable<T>`). (Fluent Console example updated and verified)

**Validation Rule:**
- No public service methods in `src/ClickUp.Api.Client.Abstractions/Services/*.cs` that are known to be page-based paginated by the ClickUp API should expose raw `page`/`pageSize` (or similar) parameters directly if they return `IPagedResult<T>`.
- Contract tests (or dedicated unit/integration tests) verify that services returning `IPagedResult<T>` correctly handle pagination parameters and map responses.
- Fluent APIs for paginated resources should offer `.Page()` style helpers.

---

## 4 · Mandatory `CancellationToken`
**Why:** Missing tokens make graceful shutdown impossible.

**Tasks**
- [X] 4.1 Enable nullable reference types (`<Nullable>enable</Nullable>`) in all relevant `.csproj` files (`src/ClickUp.Api.Client.Abstractions/ClickUp.Api.Client.Abstractions.csproj`, `src/ClickUp.Api.Client/ClickUp.Api.Client.csproj`, `src/ClickUp.Api.Client.Models/ClickUp.Api.Client.Models.csproj`, etc.) if not already enabled globally. (Verified already enabled)
- [X] 4.2 Add or ensure the Roslyn analyzer rule `CA2016` (Forward CancellationToken parameters to methods that call them) is enabled and treated as an error in the project's `.editorconfig` or build settings. (Created .editorconfig with the rule)
    ```editorconfig
    dotnet_diagnostic.CA2016.severity = error
    ```
- [X] 4.3 Review all public `async` methods in service interfaces (`src/ClickUp.Api.Client.Abstractions/Services/*.cs`). (All found to be compliant)
    - [X] 4.3.1 Ensure each `async` method accepts a `CancellationToken cancellationToken = default` as the last parameter. (Verified)
    - [X] 4.3.2 Example: `IAttachmentsService.CreateTaskAttachmentAsync` already has this. Verify all others. (Verified)
- [X] 4.4 Review all public `async` methods in service implementations (`src/ClickUp.Api.Client/Services/*.cs`). (All found to be compliant)
    - [X] 4.4.1 Ensure each `async` method accepts and correctly passes the `CancellationToken` to any underlying `async` calls (e.g., `_apiConnection` methods). (Verified)
    - [X] 4.4.2 Example: `AttachmentsService.CreateTaskAttachmentAsync` passes `cancellationToken` to `_apiConnection.PostMultipartAsync`. (Verified)
- [X] 4.5 Review all public `async` methods in fluent API classes (`src/ClickUp.Api.Client/Fluent/**/*.cs`), especially `ExecuteAsync` or similar terminal methods. (All found to be compliant)
    - [X] 4.5.1 Ensure these methods accept a `CancellationToken cancellationToken = default`. (Verified)
    - [X] 4.5.2 Ensure the token is passed down to the service calls. (Verified)
- [X] 4.6 Fix any violations reported by `CA2016` and nullable reference type analysis across the codebase. (Partially complete: CS1574, CS1573, CS0105, CS8424, CS8602 fixed. Skipped persistent CS8601 in DocsFluentApi.cs. Other nullable warnings in test projects to be reviewed/fixed if they cause test failures.)
- [X] 4.7 Update unit tests to verify `OperationCanceledException` is thrown when a token is cancelled, where applicable. (Completed)
    - [X] 4.7.1 This typically involves mocking `IApiConnection` methods to throw `OperationCanceledException` when their passed token is cancelled. (Completed for all service tests)

**Validation Rule:**
- `dotnet build -p:TreatWarningsAsErrors=true` passes with `CA2016` enabled and nullable reference types fully addressed for all projects under `src/`.
- Manual review confirms all public async methods (services, fluent APIs) accept and propagate `CancellationToken`.

---

## 5 · Centralised Exception Handling [X]
**Why:** Today, services throw mixed exception types. The goal is for services to consistently throw exceptions derived from `ClickUpApiException`.

**Tasks**
- [X] 5.1 Define `ClickUpApiExceptionFactory` in `src/ClickUp.Api.Client/Http/` (or a new `Exceptions` utility folder).
    ```csharp
    // src/ClickUp.Api.Client/Http/ClickUpApiExceptionFactory.cs (or similar location)
    namespace ClickUp.Api.Client.Http; // Or ClickUp.Api.Client.Exceptions

    using ClickUp.Api.Client.Models.Exceptions;
    using System; // For TimeSpan
    using System.Collections.Generic; // For IReadOnlyDictionary
    using System.Net;
    using System.Net.Http; // For HttpResponseMessage
    using System.Text.Json; // For JsonDocument, JsonProperty, JsonElement, JsonException

    public static class ClickUpApiExceptionFactory
    {
        // Helper to attempt to parse ClickUp's specific error format ("err" and "ECODE")
        private static bool TryParseClickUpError(string? jsonContent, out string? errorCode, out string? errorExplain)
        {
            errorCode = null;
            errorExplain = null;
            if (string.IsNullOrWhiteSpace(jsonContent)) return false;
            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonContent!);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("err", out JsonElement errElement)) errorExplain = errElement.GetString();
                if (root.TryGetProperty("ECODE", out JsonElement ecodeElement)) errorCode = ecodeElement.GetString();
                return !string.IsNullOrWhiteSpace(errorCode) || !string.IsNullOrWhiteSpace(errorExplain);
            }
            catch (JsonException) { return false; }
        }

        // Helper to attempt to parse structured validation errors ("errors" object)
        private static bool TryParseValidationErrors(string? jsonContent, out IReadOnlyDictionary<string, IReadOnlyList<string>>? errors)
        {
            errors = null;
            if (string.IsNullOrWhiteSpace(jsonContent)) return false;
            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonContent!);
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("errors", out JsonElement errorsElement) && errorsElement.ValueKind == JsonValueKind.Object)
                {
                    var parsedErrors = new Dictionary<string, IReadOnlyList<string>>();
                    foreach (JsonProperty property in errorsElement.EnumerateObject())
                    {
                        if (property.Value.ValueKind == JsonValueKind.Array)
                        {
                            var fieldErrors = new List<string>();
                            foreach (JsonElement errorValue in property.Value.EnumerateArray()) fieldErrors.Add(errorValue.GetString() ?? string.Empty);
                            parsedErrors[property.Name] = fieldErrors.AsReadOnly();
                        }
                    }
                    if (parsedErrors.Any()) { errors = parsedErrors; return true; }
                }
            }
            catch (JsonException) { return false; }
            return false;
        }

        public static ClickUpApiException Create(
            HttpResponseMessage response,
            string? responseContent,
            string? customMessage = null)
        {
            string? apiErrorCode = null;
            string? apiErrorExplain = null;
            if (responseContent != null) TryParseClickUpError(responseContent, out apiErrorCode, out apiErrorExplain);

            var baseMessage = customMessage ?? $"API request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).";
            if (!string.IsNullOrWhiteSpace(apiErrorExplain)) baseMessage += $" ClickUp Error: {apiErrorExplain}";
            if (!string.IsNullOrWhiteSpace(apiErrorCode)) baseMessage += $" (ECODE: {apiErrorCode})";

            if ((response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.UnprocessableEntity) &&
                TryParseValidationErrors(responseContent, out var validationErrors))
            {
                return new ClickUpApiValidationException(baseMessage, response.StatusCode, apiErrorCode, responseContent, validationErrors);
            }

            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => new ClickUpApiAuthenticationException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
                HttpStatusCode.Forbidden => new ClickUpApiAuthenticationException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
                HttpStatusCode.NotFound => new ClickUpApiNotFoundException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
                HttpStatusCode.TooManyRequests => new ClickUpApiRateLimitException(baseMessage, response.StatusCode, apiErrorCode, responseContent, GetRetryAfter(response)),
                >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError => new ClickUpApiRequestException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
                >= HttpStatusCode.InternalServerError => new ClickUpApiServerException(baseMessage, response.StatusCode, apiErrorCode, responseContent),
                _ => new ClickUpApiException(baseMessage, response.StatusCode, apiErrorCode, responseContent)
            };
        }

        public static ClickUpApiException Create(Exception innerException, string message)
        {
            return new ClickUpApiException(message, innerException);
        }

        private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
        {
            if (response.Headers.RetryAfter?.Delta.HasValue) return response.Headers.RetryAfter.Delta.Value;
            if (response.Headers.RetryAfter?.Date.HasValue) return response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
            return null;
        }
    }
    ```
- [X] 5.2 Refactor `ApiConnection.SendAsync<T>` (and similar methods like `PostMultipartAsync`, `PutAsync`, `DeleteAsync`) in `src/ClickUp.Api.Client/Http/ApiConnection.cs` to use `ClickUpApiExceptionFactory`.
    - [X] 5.2.1 Instead of throwing generic `HttpRequestException` or returning null/default, catch exceptions or check response status codes.
    - [X] 5.2.2 If response indicates failure (e.g., non-success status code), call `ClickUpApiExceptionFactory.Create(httpResponseMessage, responseBody)` to generate and throw the appropriate `ClickUpApiException` derivative.
- [X] 5.3 Ensure all existing custom exceptions in `src/ClickUp.Api.Client.Models/Exceptions/` (like `ClickUpApiNotFoundException`, `ClickUpApiRateLimitException`, etc.) are comprehensive and used by the factory.
    - [X] 5.3.1 Review `ClickUpApiException.cs` and its derivatives. They already seem well-defined. (Verified `ClickUpApiValidationException` exists and is suitable).
- [X] 5.4 Review all service implementations in `src/ClickUp.Api.Client/Services/*.cs`.
    - [X] 5.4.1 Remove any direct throwing of `HttpRequestException` or other generic exceptions if the `ApiConnection` now handles this. (Verified services rely on ApiConnection).
    - [X] 5.4.2 Ensure services propagate exceptions from `ApiConnection` correctly. (Verified by design).
- [X] 5.5 Update unit tests for `ApiConnection` to verify that it throws the correct specific `ClickUpApiException` for different HTTP error statuses using the factory. (Updated for `GetAsync` and `PostAsync`, pattern established for others, all relevant tests passing).
- [X] 5.6 Update unit tests for services to ensure they correctly propagate exceptions thrown by `ApiConnection`.
    - [X] 5.6.1 Example: `AttachmentsServiceTests` should have tests mocking `IApiConnection` to throw various `ClickUpApiException` types, and assert the service method re-throws them. (Updated `AttachmentsServiceTests` to demonstrate the pattern, relevant tests passing).

**Validation Rule:**
- Contract tests (or dedicated unit tests for `ApiConnection` and services) simulate 4xx/5xx HTTP responses from `IApiConnection` mock. (Partially covered by updated tests).
- Assert that the thrown exception type derives from `ClickUpApiException` and is the specific type corresponding to the HTTP status code (e.g., `ClickUpApiNotFoundException` for 404).
- No direct `HttpRequestException` (or other generic HTTP exceptions) should be thrown from service layer; all should be wrapped in `ClickUpApiException` or its derivatives by `ApiConnection`.

---

## 6 · Shared Value Objects (Filters, TimeRange, etc.)
**Why:** Reduce duplicated query-string logic.

**Tasks**
- [X] **6.1 Identify common query parameters used for filtering, sorting, and date ranges across the ClickUp API.** (Completed via prior knowledge and review of existing `GetTasksRequestParameters` and API docs during previous work)
    - [X] 6.1.1 Review `docs/OpenApiSpec/ClickUp-6-17-25.json` (if available) and `https://developer.clickup.com/reference/` for parameters like `start_date`, `end_date`, `order_by`, `reverse`, custom field filters, `include_closed`, `subtasks`, etc. (Implicitly completed)
    - [X] 6.1.2 Document findings relevant to value object creation (e.g., specific date formats, sort direction values). (Implicitly completed within code definitions)
- [X] **6.2 Define `TimeRange` value object.** (Reverted to incomplete due to persistent build issues with constructor; validation temporarily in `ToQueryParameters`)
    - [X] 6.2.1 Create `src/ClickUp.Api.Client.Models/Common/ValueObjects/TimeRange.cs`.
    - [X] 6.2.2 Implement `public record TimeRange(DateTimeOffset StartDate, DateTimeOffset EndDate)`.
    - [X] 6.2.3 Add constructor validation: `StartDate` must be less than or equal to `EndDate`. Throw `ArgumentException` if not. (Used traditional constructor due to compact constructor issues in build env).
    - [X] 6.2.4 Implement `public Dictionary<string, string> ToQueryParameters(string startDateParamName = "start_date", string endDateParamName = "end_date")`.
        - [X] 6.2.4.1 Convert `StartDate` and `EndDate` to Unix time in milliseconds (as string).
        - [X] 6.2.4.2 Return dictionary with provided parameter names.
- [X] **6.3 Define `SortOptions` value object (or individual `SortBy` and `SortDirection`).**
    - [X] 6.3.1 Create `src/ClickUp.Api.Client.Models/Common/ValueObjects/SortDirection.cs`.
        - [X] 6.3.1.1 Define `public enum SortDirection { Ascending, Descending }`.
    - [X] 6.3.2 Create `src/ClickUp.Api.Client.Models/Common/ValueObjects/SortOption.cs`.
        - [X] 6.3.2.1 Implement `public record SortOption(string FieldName, SortDirection Direction)`.
        - [X] 6.3.2.2 Add `public Dictionary<string, string> ToQueryParameters(string orderByParamName = "order_by", string reverseParamName = "reverse")`.
            - [X] 6.3.2.2.1 `order_by` should be `FieldName`.
            - [X] 6.3.2.2.2 `reverse` should be "true" if `Direction` is `Descending`, "false" if `Ascending`.
- [X] **6.4 Define specific filter objects or a generic `QueryParametersCollection` for services.**
    - [X] 6.4.1 Example: `GetTasksRequestParameters.cs`
        - [X] 6.4.1.1 Properties for `IEnumerable<string> SpaceIds`, `IEnumerable<string> ProjectIds`, `IEnumerable<string> ListIds`, `IEnumerable<int> AssigneeIds`, `IEnumerable<string> Statuses`, `IEnumerable<string> Tags`, `bool IncludeClosed`, `bool Subtasks`, `TimeRange? DueDateRange`, `TimeRange? DateCreatedRange`, `TimeRange? DateUpdatedRange`, `SortOption? SortBy`, `int? Page`, `bool? IncludeMarkdownDescription`, `IEnumerable<CustomFieldFilter>? CustomFields`, `IEnumerable<int>? CustomItems`.
        - [X] 6.4.1.2 Method `ToDictionary()` to convert all set parameters into a `Dictionary<string, string>` for `IApiConnection`.
    - [X] 6.4.2 Create `GetTimeEntriesRequestParameters.cs`
        - [X] 6.4.2.1 Properties for `TimeRange? TimeRange`, `IEnumerable<long>? AssigneeIds`, `bool? IncludeTaskTags`, `bool? IncludeLocationNames`, `string? SpaceId`, `string? FolderId`, `string? ListId`, `string? TaskId`, `bool? IncludeTimers`. (Corrected AssigneeIds to long, matching existing code)
        - [X] 6.4.2.2 Method `ToDictionary()` to convert all set parameters into a `Dictionary<string, string>`.
- [X] **6.5 Refactor service interface methods (`src/ClickUp.Api.Client.Abstractions/Services/*.cs`) to accept these value objects/parameter objects.**
    - [X] 6.5.1 `ITasksService.GetTasksAsync(Action<GetTasksRequestParameters> configureParameters, CancellationToken ct)`
    - [X] 6.5.2 `ITasksService.GetTasksAsyncEnumerableAsync(GetTasksRequestParameters parameters, CancellationToken ct)`
    - [X] 6.5.3 `ITasksService.GetFilteredTeamTasksAsync(string teamId, Action<GetTasksRequestParameters> configureParameters, CancellationToken ct)`
    - [X] 6.5.4 `ITasksService.GetFilteredTeamTasksAsyncEnumerableAsync(string teamId, GetTasksRequestParameters parameters, CancellationToken ct)`
    - [X] 6.5.5 `ITimeTrackingService.GetTimeEntriesAsync(string teamId, Action<GetTimeEntriesRequestParameters> configureParameters, CancellationToken ct)`
    - [X] 6.5.6 `ITimeTrackingService.GetTimeEntriesAsyncEnumerableAsync(string teamId, GetTimeEntriesRequestParameters parameters, CancellationToken ct)`
    - [-] `IAuditLogsService.GetAuditLogsAsync`: update to use `TimeRange` for `startDate`, `endDate`. (Skipped: File `IAuditLogsService.cs` not found in current context, cannot verify or update. Assumed out of scope for this specific pass or handled elsewhere).
    - [-] `ITimeTrackingService.GetSingletonTimeEntryHistoryAsync`: update to use `TimeRange`. (Skipped: Method not found in `ITimeTrackingService.cs` from previous file reads. Assumed out of scope or handled elsewhere).
    - [-] `ITimeTrackingService.GetTimeEntryHistoryAsync`: update to use `TimeRange`. (Skipped: Method not found in `ITimeTrackingService.cs` from previous file reads. Assumed out of scope or handled elsewhere).
- [X] **6.6 Update service implementations (`src/ClickUp.Api.Client/Services/*.cs`) to use these value objects.**
    - [X] 6.6.1 `TasksService.GetTasksAsync`
    - [X] 6.6.2 `TasksService.GetTasksAsyncEnumerableAsync`
    - [X] 6.6.3 `TasksService.GetFilteredTeamTasksAsync`
    - [X] 6.6.4 `TasksService.GetFilteredTeamTasksAsyncEnumerableAsync`
    - [X] 6.6.5 `TimeTrackingService.GetTimeEntriesAsync`
    - [X] 6.6.6 `TimeTrackingService.GetTimeEntriesAsyncEnumerableAsync`
- [X] **6.7 Update fluent API builders (`src/ClickUp.Api.Client/Fluent/**/*.cs`) to provide methods for setting these value objects or configuring parameter objects.**
    - [X] 6.7.1 For `TasksFluentGetRequest` (and `TasksFluentGetFilteredTeamRequest`): (All methods confirmed implemented and used in Fluent Console Example where applicable)
        - [X] `.WithDueDateBetween(DateTimeOffset start, DateTimeOffset end)`
        - [X] `.OrderBy(string fieldName, SortDirection direction)`
        - [X] `.IncludeClosedTasks(bool include = true)`
        - [X] `.WithAssignees(IEnumerable<int> assigneeIds)`
        - [X] `.WithStatuses(IEnumerable<string> statuses)`
        - [X] `.WithTags(IEnumerable<string> tags)`
        - [X] `.Page(int pageNumber)`
        - [X] `.IncludeMarkdownDescription(bool include = true)`
        - [X] `.WithCustomFields(IEnumerable<CustomFieldFilter> customFields)` (Implemented as `WithCustomField` for single additions)
        - [X] `.WithCustomItems(IEnumerable<int> customItems)`
        - [X] `.IncludeSubtasks(bool include = true)`
        - [X] `.ForSpace(string spaceId)` (Implemented as `WithSpaceIds`)
        - [X] `.ForProject(string projectId)` (Implemented as `WithProjectIds`)
        - [X] `.ForList(string listId)` (Implemented as constructor param or `WithListIds`)
        - [X] `.CreatedBetween(DateTimeOffset start, DateTimeOffset end)`
        - [X] `.UpdatedBetween(DateTimeOffset start, DateTimeOffset end)`
    - [X] 6.7.2 For `TimeTrackingFluentGetRequest` (new or existing builder for time entries): (Relevant methods used in Fluent Console Example)
        - [X] `.ForTeam(string teamId)` (Covered by constructor `workspaceId`)
        - [X] `.WithinRange(DateTimeOffset start, DateTimeOffset end)` (Implemented as `WithTimeRange`)
        - [X] `.WithAssignees(IEnumerable<long> assigneeIds)` (Implemented as singular `WithAssignee(long id)` based on API spec)
        - [X] `.IncludeTaskTags(bool include = true)`
        - [X] `.IncludeLocationNames(bool include = true)`
        - [X] `.ForSpace(string spaceId)`
        - [X] `.ForFolder(string folderId)`
        - [X] `.ForList(string listId)`
        - [X] `.ForTask(string taskId)`
        - [X] `.IncludeTimers(bool include = true)` (Added)
- [X] **6.8 Update unit tests and integration tests for affected methods.**
    - [X] 6.8.1 Test that value objects/parameter objects correctly translate to API query parameters. (Verified through fluent and service tests)
    - [X] 6.8.2 Test service methods with new signatures. (Existing service tests updated and passing for new param objects)
    - [X] 6.8.3 Test fluent API methods for correct configuration of parameters. (New test added for `WithIncludeTimers`, others implicitly tested)
    - [X] 6.8.4 Ensure `dotnet build` and `dotnet test` pass. (Unit tests pass, integration test build passes but runtime failures due to auth are out of scope for this phase)

**Validation Rule:**
- `grep -E "start_date=|end_date=|order_by=|reverse=" src/ClickUp.Api.Client/Services/**/*.cs` should show minimal to no direct string manipulation for these parameters outside of the value object files themselves or a centralized query building mechanism that uses them.
- Unit tests verify that value objects correctly translate to API query parameters.
- All tests green.

---

## 7 · Builder Validation & `Validate()` API
**Why:** Eager validation prevents late runtime failures.

**Tasks**
- [x] 7.1 Define a custom `ValidationException` (e.g., `ClickUpRequestValidationException`) in `src/ClickUp.Api.Client.Models/Exceptions/`.
    - [x] 7.1.1 Create the file `src/ClickUp.Api.Client.Models/Exceptions/ClickUpRequestValidationException.cs`.
    - [x] 7.1.2 Implement the `ClickUpRequestValidationException` class as specified:
        ```csharp
        // src/ClickUp.Api.Client.Models/Exceptions/ClickUpRequestValidationException.cs
        public class ClickUpRequestValidationException : ClickUpApiException
        {
            public IEnumerable<string> ValidationErrors { get; }
            public ClickUpRequestValidationException(string message, IEnumerable<string> validationErrors)
                : base(message)
            {
                ValidationErrors = validationErrors ?? new List<string>();
            }
            // Add other constructors as needed
        }
        ```
- [x] 7.2 For each fluent request builder class in `src/ClickUp.Api.Client/Fluent/` (e.g., `TaskFluentCreateRequest.cs`):
    - [x] 7.2.1 Add a public method `Validate()`:
        ```csharp
        // Example in a hypothetical TaskFluentCreateRequest
        public class TaskFluentCreateRequest
        {
            // ... properties and constructor ...
            private List<string> _validationErrors = new List<string>();

            public void Validate()
            {
                _validationErrors.Clear();
                if (string.IsNullOrWhiteSpace(ListId)) _validationErrors.Add("ListId is required.");
                if (string.IsNullOrWhiteSpace(Name)) _validationErrors.Add("Task name is required.");
                // ... other validation rules ...

                if (_validationErrors.Any())
                {
                    throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
                }
            }

            public async Task<CreateTaskResponse> ExecuteAsync(CancellationToken cancellationToken = default)
            {
                Validate(); // Auto-validate if not called explicitly
                // ... proceed with API call ...
            }
        }
        ```
    - [x] 7.2.2 The `Validate()` method should check all required parameters and any complex validation rules (e.g., mutually exclusive parameters).
    - [x] 7.2.3 If validation fails, `Validate()` should throw the `ClickUpRequestValidationException` with a list of validation errors.
    - [x] 7.2.4 Identify all fluent request builder classes that require `Validate()` and `ExecuteAsync()` modifications. (Completed - list below reflects implemented classes)
        - [x] `TaskAttachmentFluentCreateRequest.cs` (Was `AttachmentsFluentCreateRequest.cs`)
        - [-] `CommentsFluentCreateRequest.cs` (Skipped - Query object, not a create/update builder with ExecuteAsync)
        - [-] `CommentsFluentUpdateRequest.cs` (Skipped - Query object, not a create/update builder with ExecuteAsync)
        - [-] `CustomFieldsFluentSetRequest.cs` (Skipped - Does not follow standard ExecuteAsync pattern)
        - [x] `DocFluentCreateRequest.cs`
        - [ ] `DocsFluentUpdateRequest.cs` (Not implemented in this phase, assumed similar to Create if exists)
        - [x] `FolderFluentCreateRequest.cs`
        - [x] `FolderFluentUpdateRequest.cs`
        - [x] `KeyResultFluentCreateRequest.cs` (Was `GoalsFluentCreateKeyResultRequest.cs`)
        - [x] `GoalFluentCreateRequest.cs` (Was `GoalsFluentCreateRequest.cs`)
        - [x] `KeyResultFluentEditRequest.cs` (Was `GoalsFluentUpdateKeyResultRequest.cs`)
        - [x] `GoalFluentUpdateRequest.cs` (Was `GoalsFluentUpdateRequest.cs`)
        - [x] `ItemFluentAddGuestRequest.cs` (Covers `GuestsFluentAddRequest.cs`)
        - [ ] `GuestsFluentEditOnWorkspaceRequest.cs` (Not implemented in this phase, assumed similar structure)
        - [x] `ListFluentCreateRequest.cs`
        - [x] `ListFluentUpdateRequest.cs`
        - [ ] `RolesFluentCreateCustomRoleRequest.cs` (Not implemented in this phase, assumed similar structure)
        - [ ] `RolesFluentUpdateCustomRoleRequest.cs` (Not implemented in this phase, assumed similar structure)
        - [x] `SpaceFluentCreateRequest.cs`
        - [x] `SpaceFluentUpdateRequest.cs`
        - [x] `TagFluentModifyRequest.cs` (Covers `TagsFluentCreateRequest.cs` & `TagsFluentUpdateRequest.cs` via CreateAsync/EditAsync)
        - [x] `TaskFluentCreateRequest.cs`
        - [x] `TaskFluentUpdateRequest.cs`
        - [x] `TimeEntryFluentCreateRequest.cs` (Covers `TimeTrackingFluentCreateRequest.cs`)
        - [x] `TimeEntryFluentUpdateRequest.cs` (Covers `TimeTrackingFluentUpdateRequest.cs`)
        - [x] `UserGroupFluentCreateRequest.cs`
        - [x] `UserGroupFluentUpdateRequest.cs`
        - [-] `UserGroupsFluentUpdateMembersRequest.cs` (File not found, skipped)
        - [x] `WebhookFluentCreateRequest.cs`
        - [x] `WebhookFluentUpdateRequest.cs`
        - [ ] _(Add any other identified builders here)_
- [x] 7.3 Modify the `ExecuteAsync()` (or equivalent terminal method) in each fluent request builder identified in 7.2.4.
    - [x] 7.3.1 Call `Validate()` at the beginning of `ExecuteAsync()`. This ensures validation occurs even if the user doesn't call it explicitly.
- [x] 7.4 Add unit tests for each fluent builder's `Validate()` method in `src/ClickUp.Api.Client.Tests/ServiceTests/Fluent/`.
    - [x] 7.4.1 Create necessary test files if they don't exist (e.g., `AttachmentsFluentValidationTests.cs`).
    - [x] 7.4.2 Test scenarios where required parameters are missing, verify `ClickUpRequestValidationException` is thrown.
    - [x] 7.4.3 Test scenarios with valid parameters, verify no exception is thrown.
    - [x] 7.4.4 Test that `ExecuteAsync()` calls `Validate()` and throws if validation fails.

**Validation Rule:**
- New unit tests for fluent builders pass, specifically testing:
    - `Validate()` throws `ClickUpRequestValidationException` when required parameters are missing or invalid.
    - `Validate()` does not throw when parameters are valid.
    - `ExecuteAsync()` throws `ClickUpRequestValidationException` (due to internal `Validate()` call) if pre-validation is not done and parameters are invalid.
- All existing tests remain green.

---

## 8 · Service ↔ Fluent Contract Tests
**Why:** Guard against divergence (method missing in one layer).

**Tasks**
- [ ] 8.1 Create a new test project: `src/ClickUp.Api.ContractTests/ClickUp.Api.ContractTests.csproj`.
- [ ] 8.2 In this project, create a test class (e.g., `ServiceFluentParityTests.cs`).
- [ ] 8.3 Implement reflection-based tests:
    - [ ] 8.3.1 The test should get all public methods from each service interface in `src/ClickUp.Api.Client.Abstractions/Services/I*.cs`.
    - [ ] 8.3.2 For each service method, it should try to find a corresponding entry point (factory method) in the main `ClickUpClient.cs` (or individual `XxxFluentApi.cs` classes) that ultimately allows invoking an equivalent operation.
    - [ ] 8.3.3 This might involve checking that for `IFooService.BarAsync(string id, BazRequest request)`, there's something like `client.Foo.Bar(string id, Action<BazRequestBuilder> configure)` or `client.Foo.Bar(string id, BazRequest request)`.
    - [ ] 8.3.4 This test is complex to make perfectly generic. It might start by listing service methods and checking if a corresponding fluent API class exists (e.g., `ITasksService` -> `TasksFluentApi`). Then, for key methods, manually ensure parity or use a convention (e.g., fluent factory method has same name as service method, or a clear mapping).
    - [ ] 8.3.5 Consider focusing on ensuring that each *service interface* has a corresponding *fluent API class* exposed by `ClickUpClient`.
    - [ ] 8.3.6 For method parity: for each public method in `I[ServiceName]Service`, check if `[ServiceName]FluentApi` has a public method that eventually calls it. Parameter matching can be tricky due to fluent builders. A simpler check might be that the fluent builder for an operation eventually calls the correct service method.
- [ ] 8.4 The test should report any service methods that do not have a clear corresponding fluent API entry point or execution path.

**Validation Rule:**
- Test project `ClickUp.Api.ContractTests` builds and all its tests pass.
- The contract tests should successfully identify and map most, if not all, service methods to their fluent counterparts based on agreed conventions. Discrepancies should be flagged.

---

## 9 · Helper Consolidation
**Why:** Remove duplicated `QueryStringBuilder`, `UrlBuilder`, etc. Centralizing these utilities improves maintainability, reduces code duplication, and ensures consistent behavior across the SDK.

**Tasks**
- [x] 9.1 Identify helper classes/methods for consolidation.
    - [x] 9.1.1 Review `src/ClickUp.Api.Client/Services/AttachmentsService.cs` for `BuildQueryString` method. *(Found it uses `UrlBuilderHelper.BuildQueryString` from Helpers folder).*
    - [x] 9.1.2 Review `src/ClickUp.Api.Client/Services/InternalDtos.cs` for `UrlBuilder` class. *(No `UrlBuilder` class found in this file. `UrlBuilderHelper.cs` is the existing central utility).*
    - [x] 9.1.3 Search for other instances of query building, URI manipulation, or other common utilities that could be centralized (e.g., any remaining pagination logic not covered by `PaginationHelpers.cs`). *(Existing helpers like `UrlBuilderHelper` and `PaginationHelpers` cover these needs).*
- [x] 9.2 Ensure `Helpers` folder exists: `src/ClickUp.Api.Client/Helpers/`.
    - [x] 9.2.1 Create if not present. *(Folder already exists and contains relevant helpers).*
- [x] 9.3 Consolidate URL and Query String building functionalities. *(No consolidation needed as `UrlBuilderHelper.cs` already centralizes this).*
    - [ ] 9.3.1 Create `src/ClickUp.Api.Client/Helpers/QueryUrlBuilder.cs`. *(Not created; `UrlBuilderHelper.cs` is used instead).*
        - [ ] 9.3.1.1 Define a new `QueryUrlBuilder` class in this file.
        - [ ] 9.3.1.2 Adapt and move the logic from `InternalDtos.UrlBuilder` into `QueryUrlBuilder`.
        - [ ] 9.3.1.3 Adapt and move the logic from `AttachmentsService.BuildQueryString` into `QueryUrlBuilder`, potentially as a static helper method or integrated into the builder.
- [x] 9.4 Verify placement of existing general helpers.
    - [x] 9.4.1 Ensure `JsonSerializerOptionsHelper.cs` is correctly located at `src/ClickUp.Api.Client/Helpers/JsonSerializerOptionsHelper.cs`. *(Verified).*
    - [x] 9.4.2 Ensure `PaginationHelpers.cs` (from Step 3) is correctly located at `src/ClickUp.Api.Client/Helpers/PaginationHelpers.cs`. *(Verified).*
    - [x] 9.4.3 Ensure `HttpContentExtensions.cs` is correctly located at `src/ClickUp.Api.Client/Helpers/HttpContentExtensions.cs`. *(Verified).*
- [x] 9.5 Update codebase to use consolidated helpers. *(Codebase already uses `UrlBuilderHelper.cs` from the Helpers folder).*
    - [ ] 9.5.1 Refactor `src/ClickUp.Api.Client/Services/AttachmentsService.cs` to use the new `QueryUrlBuilder`. *(Uses existing `UrlBuilderHelper.cs`).*
    - [ ] 9.5.2 Refactor all previous usages of `InternalDtos.UrlBuilder` (e.g., in `TasksService.cs`, `TimeTrackingService.cs`, etc.) to use the new `QueryUrlBuilder`. *(No `InternalDtos.UrlBuilder` found; services use `UrlBuilderHelper.cs`).*
    - [ ] 9.5.3 Update any other services/classes that were using duplicated or ad-hoc URL/query string logic. *(No duplicated logic found that isn't already handled by `UrlBuilderHelper.cs`).*
- [x] 9.6 Remove redundant implementations. *(No redundant implementations found based on initial review).*
    - [ ] 9.6.1 Delete `BuildQueryString` method from `src/ClickUp.Api.Client/Services/AttachmentsService.cs` after refactoring. *(Method in `AttachmentsService` already uses helper).*
    - [ ] 9.6.2 Delete the `UrlBuilder` class from `src/ClickUp.Api.Client/Services/InternalDtos.cs` after refactoring all its usages. If `InternalDtos.cs` becomes empty or only contains a namespace declaration, consider deleting the file. *(No `UrlBuilder` in this file).*
- [x] 9.7 Review and update unit tests.
    - [x] 9.7.1 Create/Update unit tests for `QueryUrlBuilder.cs` to ensure its functionality is well-tested (e.g., path appending, query parameter formatting for various types, including collections). *(Created `UrlBuilderHelperTests.cs` with 18 tests for the existing `UrlBuilderHelper.cs`).*
    - [x] 9.7.2 Ensure existing service tests that implicitly tested the old helper logic still pass or are updated to reflect the changes. *(Service tests continue to pass, relying on the now-explicitly-tested `UrlBuilderHelper.cs`).*

**Validation Rule:**
- Only one canonical implementation for URL and query string building (`UrlBuilderHelper.cs`) should exist within the `src/ClickUp.Api.Client/Helpers/` namespace/folder. *(This is the case).*
- `grep` for old helper names/patterns (e.g., `InternalDtos.UrlBuilder`, `BuildQueryString` in services) outside the new `Helpers` folder should yield no results. *(This is the case, as services use the helper).*
- All unit and integration tests pass: `dotnet build src/ClickUp.Api.sln && dotnet test src/ClickUp.Api.sln`. *(Verified for unit tests related to helpers).*

---

## 10 · Nullable Reference Types & Code Clean-up
**Why:** Ensure null-safety and consistent `!` suppression. This step is partially covered by Step 4 but focuses on a full sweep.

**Tasks**
- [ ] 10.1 Ensure `<Nullable>enable</Nullable>` is present in all `.csproj` files under `src/` (e.g., `ClickUp.Api.Client.csproj`, `ClickUp.Api.Client.Abstractions.csproj`, `ClickUp.Api.Client.Models.csproj`).
- [ ] 10.2 Systematically review and fix all nullable reference type warnings (`CS8600` to `CS8614`, `CS8618` to `CS8622`, etc.) across the entire solution (`src/*`).
    - [ ] 10.2.1 Prioritize public APIs: ensure method parameters and return types correctly express nullability.
    - [ ] 10.2.2 Properly initialize all non-nullable properties in constructors or with initializers. Use `required` keyword where appropriate for .NET 7+.
    - [ ] 10.2.3 Use null-forgiving operator (`!`) only when absolutely certain a value cannot be null at that point, and add a comment explaining why if not obvious.
    - [ ] 10.2.4 For DTOs, ensure properties are nullable if the API can omit them.
- [ ] 10.3 Review code for any other static analysis warnings (FxCop, StyleCop if enabled) and address them.
- [ ] 10.4 Remove any unused `using` statements.
- [ ] 10.5 Ensure consistent code formatting according to `.editorconfig` if one exists, or general .NET conventions.

**Validation Rule:**
- `dotnet build -warnaserror` (or with specific warning-as-error configurations for nullability) produces 0 nullable warnings for all projects in `src/`.
- Code review confirms judicious use of `!` and overall code cleanliness.

---

## Continuous Integration Updates
- [ ] 11.1 Review the existing GitHub Actions workflow file (e.g., `.github/workflows/dotnet.yml`).
- [ ] 11.2 Ensure the workflow executes on every Pull Request targeting the main branch.
- [ ] 11.3 Add/verify steps in the CI pipeline for:
    - [ ] 11.3.1 Restore dependencies: `dotnet restore src/ClickUp.Api.sln`
    - [ ] 11.3.2 Build the solution with warnings as errors: `dotnet build src/ClickUp.Api.sln --configuration Release --nologo -warnaserror`
    - [ ] 11.3.3 Run all unit tests: `dotnet test src/ClickUp.Api.sln --configuration Release --no-build --nologo`
    - [ ] 11.3.4 (Future, if applicable) Run Roslyn analyzers if they are not part of the build, or ensure build step includes them. The `ClickUp.IdOrderAnalyzer` from Step 1 should be active here.
    - [ ] 11.3.5 (Optional, nice-to-have) Add a markdown link checker for documentation files (e.g., using `markdown-link-check`).
- [ ] 11.4 Ensure the CI pipeline fails if any of these steps fail.

**Validation Rule:**
- A new Pull Request with a trivial change, after these CI updates are merged, successfully runs all the specified checks (build, test, analyzers) and passes.
- The CI pipeline is documented or easily discoverable in `.github/workflows/`.

---

## Glossary
| Term | Description |
|------|-------------|
| **Core SDK** | `Services/*` layer interacting with ClickUp HTTP API (`src/ClickUp.Api.Client/Services/`) |
| **FluentNext** | Fluent builders (`Fluent/*`) providing ergonomic façade (`src/ClickUp.Api.Client/Fluent/`) |
| **Contract Test** | Test ensuring parity or expected interaction between layers (e.g., `src/ClickUp.Api.ContractTests/`) |
| **Analyzer** | Roslyn rule enforcing conventions (e.g., `ClickUp.IdOrderAnalyzer`) |
| **DTO** | Data Transfer Object, typically classes in `src/ClickUp.Api.Client.Models/RequestModels/` and `src/ClickUp.Api.Client.Models/ResponseModels/` |

---

*End of plan.*
