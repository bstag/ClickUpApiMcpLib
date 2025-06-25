# Generating Integration-Test Recordings

This document describes the **one-time steps** required to (re)create the JSON response files consumed by the integration-test suite.

---

## 1. Prerequisites

1. .NET SDK 8.0 (or later) installed and on the PATH.
2. A valid **ClickUp Personal Access Token** with the permissions your tests need.
3. _Optional_: a ClickUp workspace populated with test data that the scenarios expect (spaces, lists, etc.).

---

## 2. Required environment variables

| Variable | Purpose | Example |
|----------|---------|---------|
| `CLICKUP_SDK_TEST_MODE` | Controls the test harness mode. Use `Record` to hit the live API and persist responses, or `Playback` to run offline. | `Record` |
| `ClickUpApi__PersonalAccessToken` | **Required when recording.** PAT that will be placed in the `Authorization` header for each request. Can also be supplied via User Secrets. | `pk_1234…` |
| `ClickUpApi__BaseAddress` | Override the default `https://api.clickup.com`. Rarely needed (e.g. proxy / local stub). | `https://proxy.acme.local` |

You can set them **temporarily** in a PowerShell session:
```powershell
$env:CLICKUP_SDK_TEST_MODE = "Record"
$env:ClickUpApi__PersonalAccessToken = "pk_1234567890ABCDEFGHIJ"
```

Or persist them in your user (or CI) environment variables dialog.

---

## 3. Generating recordings

1. **Clean out old recordings** (optional but recommended):
   ```powershell
   Remove-Item -Recurse -Force src/ClickUp.Api.Client.IntegrationTests/test-data/recorded-responses/*
   ```
2. Build the solution:
   ```powershell
   dotnet build ClickUpApiMcpLib.sln
   ```
3. Run only the integration-test project in **Record** mode:
   ```powershell
   dotnet test src/ClickUp.Api.Client.IntegrationTests/ClickUp.Api.Client.IntegrationTests.csproj --configuration Debug
   ```

   During the run the `RecordingDelegatingHandler` will:
   * Forward every request to the live ClickUp API.
   * Persist the full JSON body of each response under:
     ```
     src/ClickUp.Api.Client.IntegrationTests/test-data/recorded-responses/{Service}/{VerbMethod}/Success|Error_xxx[_query|_bodyHASH].json
     ```

4. Inspect the created files **immediately**. Remove or redact any sensitive user data before committing to source-control.

---

## 4. Running tests in **Playback** mode (offline / CI)

```powershell
$env:CLICKUP_SDK_TEST_MODE = "Playback"
dotnet test src/ClickUp.Api.Client.IntegrationTests/ClickUp.Api.Client.IntegrationTests.csproj
```

The test harness will use `MockHttpMessageHandler` and the JSON files instead of making network calls.

---

## 5. Troubleshooting

| Symptom | Cause / Fix |
|---------|-------------|
| `InvalidOperationException` about missing PAT | Ensure `ClickUpApi__PersonalAccessToken` is set **or** switch to `Playback`. |
| A new endpoint returns 404 in playback | Run again in `Record` mode to capture the new response. |
| Two threads try to write the same file | Rerun tests with `--no-parallel` or integrate a lock/unique path strategy (TODO). |

---

## 6. Useful one-liners

Remove all recordings:
```powershell
Remove-Item -Recurse -Force src/ClickUp.Api.Client.IntegrationTests/test-data/recorded-responses/*
```

Run only a single test class in Record mode:
```powershell
$env:CLICKUP_SDK_TEST_MODE = "Record"
dotnet test src/ClickUp.Api.Client.IntegrationTests -f net8.0 --filter FullyQualifiedName~SpaceServiceIntegrationTests
```

---

Happy testing! Feel free to extend this guide when new scenarios are covered.
