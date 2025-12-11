## 2025-12-11 - Namespace Ambiguity in ApiConnection
**Learning:** `System.Net.Http.Json` extensions conflict with custom `ClickUp.Api.Client.Helpers.HttpContentExtensions.ReadFromJsonAsync`.
**Action:** When using `JsonContent` in `ApiConnection.cs`, alias it (`using JsonContent = System.Net.Http.Json.JsonContent;`) instead of importing the whole namespace to avoid breaking `ReadFromJsonAsync` calls.
