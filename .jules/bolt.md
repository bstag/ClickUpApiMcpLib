## 2025-12-11 - Namespace Ambiguity in ApiConnection
**Learning:** `System.Net.Http.Json` extensions conflict with custom `ClickUp.Api.Client.Helpers.HttpContentExtensions.ReadFromJsonAsync`.
**Action:** When using `JsonContent` in `ApiConnection.cs`, alias it (`using JsonContent = System.Net.Http.Json.JsonContent;`) instead of importing the whole namespace to avoid breaking `ReadFromJsonAsync` calls.

## 2025-05-23 - Frequent String Concatenation in Service Methods
**Learning:** Service methods extensively use `endpoint += UrlBuilderHelper.BuildQueryString(queryParams)`. This causes unnecessary string allocations for every request with query parameters.
**Action:** Future refactoring should prioritize using `StringBuilder` or passing a builder to helpers instead of concatenating strings.
