## 2025-12-11 - Namespace Ambiguity in ApiConnection
**Learning:** `System.Net.Http.Json` extensions conflict with custom `ClickUp.Api.Client.Helpers.HttpContentExtensions.ReadFromJsonAsync`.
**Action:** When using `JsonContent` in `ApiConnection.cs`, alias it (`using JsonContent = System.Net.Http.Json.JsonContent;`) instead of importing the whole namespace to avoid breaking `ReadFromJsonAsync` calls.

## 2025-05-23 - Frequent String Concatenation in Service Methods
**Learning:** Service methods extensively use `endpoint += UrlBuilderHelper.BuildQueryString(queryParams)`. This causes unnecessary string allocations for every request with query parameters.
**Action:** Future refactoring should prioritize using `StringBuilder` or passing a builder to helpers instead of concatenating strings.

## 2025-05-23 - Double Encoding in TaskQueryService
**Learning:** `TaskQueryService` was double-encoding parameters (like Statuses) because `GetTasksRequestParameters` encoded them, and `UrlBuilderHelper.BuildQueryString(Dictionary)` encoded them again. Also, converting to Dictionary caused unnecessary allocation.
**Action:** Move encoding responsibility to `ToQueryParametersList` for all fields, and use `UrlBuilderHelper.AppendQueryString(sb, List...)` which avoids re-encoding and Dictionary allocation.
