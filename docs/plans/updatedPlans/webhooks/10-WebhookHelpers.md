# Detailed Plan: Webhook Helpers

This document outlines the plan for providing utility methods and guidance for consuming ClickUp webhooks with the SDK. While the SDK itself won't typically *receive* webhook calls (that's the job of a user's web application), it can provide helpers for processing webhook payloads.

**Source Documents:**
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 4, Step 13)
*   `docs/OpenApiSpec/ClickUp-6-17-25.json` (To understand webhook payload structures and signature mechanisms, if any)

**Location in Codebase:**
*   Helper classes/methods: `src/ClickUp.Api.Client/Webhooks/` (new folder) or within `src/ClickUp.Api.Client.Models/Webhooks/` if primarily DTOs.
*   Documentation: Conceptual documentation under `docs/docfx/articles/webhooks.md`.

## 1. Understanding ClickUp Webhook Mechanisms

*   **Action:** Review ClickUp's official API documentation regarding webhooks. Key areas to investigate:
    *   **Payload Structure:** What is the common structure of a webhook payload? Is there a base wrapper object? What do payloads for different event types (e.g., `taskCreated`, `taskUpdated`) look like?
    *   **Event Types:** Confirm the list of available event types (`taskCreated`, `listUpdated`, etc.). The OpenAPI spec lists many under the "Create Webhook" endpoint.
    *   **Signature Validation:** Does ClickUp sign its webhook requests (e.g., using a shared secret and an HMAC signature in a header like `X-ClickUp-Signature`)? This is critical for security. If so, what's the algorithm and how is the signature generated?
        *   The `CreateWebhookresponse` in the OpenAPI spec includes a `secret`. This strongly suggests signature validation is expected.
    *   **Idempotency:** Any guidance or best practices from ClickUp regarding handling duplicate webhook events.

## 2. Webhook Payload Deserialization

1.  **DTOs for Webhook Payloads:**
    *   Based on the findings from Step 1, define C# DTOs in `src/ClickUp.Api.Client.Models/Webhooks/` to represent the structure of incoming webhook payloads.
    *   Example:
        ```csharp
        // In ClickUp.Api.Client.Models/Webhooks/WebhookPayload.cs
        public class WebhookPayload<T> // Generic if event data varies significantly
        {
            [JsonPropertyName("webhook_id")]
            public string WebhookId { get; set; }

            [JsonPropertyName("event")]
            public string EventType { get; set; } // e.g., "taskCreated"

            [JsonPropertyName("history_items")] // Common for task-related events
            public List<HistoryItem> HistoryItems { get; set; } // Define HistoryItem DTO

            [JsonPropertyName("task_id")] // Often present
            public string TaskId { get; set; }

            // Add other common top-level fields if they exist
            // The actual data specific to the event might be in HistoryItems or a dedicated object
            // For T: This could be specific event data, e.g., a TaskDto for taskCreated if the full task is sent.
            // However, ClickUp webhooks often provide 'history_items' detailing changes rather than the full new entity.
        }

        // Example for taskCreated, if it sends the full task (needs verification)
        // public class TaskCreatedPayload : WebhookPayload<TaskDto> { }

        // More likely, specific DTOs for the content within 'history_items' or the primary subject.
        // For example, if 'taskCreated' just gives a task_id, the payload might not need a generic T.
        // The 'history_items' would be key to deserialize.
        ```
    *   Focus on creating DTOs for the most common event types initially (e.g., task events, list events).
    *   The `event` field in the payload will be crucial for determining how to interpret the rest of the data.

2.  **Deserialization Helper (Optional but Recommended):**
    *   A static helper class `WebhookParser` or similar.
    *   Method: `public static WebhookPayload<object> ParsePayload(string jsonPayload)` (or more specific types if a common wrapper exists).
        *   Uses `System.Text.Json.JsonSerializer` with the SDK's shared `JsonSerializerOptions`.
        *   This provides a consistent way for users to deserialize the raw JSON string they receive from ClickUp.
    *   If payloads are very diverse without a common wrapper, users might deserialize directly into their target DTOs based on the `event` type. The SDK's role would be to provide these DTOs.

## 3. Webhook Signature Validation (If Applicable)

*This is the most critical helper if ClickUp uses webhook signatures.*

1.  **Investigate Signature Mechanism:**
    *   From ClickUp's documentation, determine:
        *   The header name containing the signature (e.g., `X-ClickUp-Signature`).
        *   The hashing algorithm (e.g., HMAC-SHA256).
        *   How the signature is constructed (which parts of the request are signed: raw body, specific headers, timestamp?).
        *   The encoding of the signature (e.g., hex, base64).

2.  **Create `WebhookSignatureValidator.cs`:**
    *   Location: `src/ClickUp.Api.Client/Webhooks/`
    *   Method: `public static bool IsValidSignature(string rawRequestBody, string signatureHeaderValue, string webhookSecret)`
        *   **Steps inside the method:**
            1.  Reconstruct the string that ClickUp would have signed, using `rawRequestBody` and any other required elements (like timestamps if they are part of the signature).
            2.  Compute the HMAC (e.g., HMACSHA256) of this reconstructed string using the `webhookSecret`.
            3.  Encode the computed hash (e.g., to a hex string).
            4.  Compare this computed signature with the `signatureHeaderValue` provided by ClickUp.
            5.  Return `true` if they match, `false` otherwise.
        *   Handle potential encoding differences carefully (e.g., UTF-8 for the body).
        *   Provide clear documentation on how to obtain `rawRequestBody`, `signatureHeaderValue`, and `webhookSecret` in common web frameworks (ASP.NET Core, Azure Functions).

    ```csharp
    // Conceptual example - actual implementation depends on ClickUp's specific mechanism
    // using System.Security.Cryptography;
    // using System.Text;

    public static class WebhookSignatureValidator
    {
        public static bool IsValidSignature(string rawRequestBody, string signatureHeaderValue, string webhookSecret)
        {
            if (string.IsNullOrEmpty(rawRequestBody) || string.IsNullOrEmpty(signatureHeaderValue) || string.IsNullOrEmpty(webhookSecret))
            {
                return false; // Or throw ArgumentNullException
            }

            // Example: Assuming HMAC-SHA256 and hex encoding, and only body is signed.
            // The actual signature scheme from ClickUp MUST be followed.
            // This might involve a prefix like "sha256=" in the signatureHeaderValue.

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawRequestBody));
                string computedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                // Compare computedSignature with signatureHeaderValue
                // This might need adjustment if the header includes a scheme like "sha256="
                // e.g., if (signatureHeaderValue.StartsWith("sha256=")) signatureHeaderValue = signatureHeaderValue.Substring("sha256=".Length);

                return string.Equals(computedSignature, signatureHeaderValue, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
    ```

## 4. Conceptual Documentation (`articles/webhooks.md`)

*   **Introduction to ClickUp Webhooks:** Briefly explain what they are and link to ClickUp's official documentation.
*   **Creating Webhooks:** Remind users that webhooks are created via the ClickUp API (using the `IWebhooksService` from the SDK) or the ClickUp UI.
*   **Receiving Webhooks:**
    *   Explain that the user's application needs an HTTP endpoint to receive POST requests from ClickUp.
    *   Provide conceptual examples for ASP.NET Core:
        ```csharp
        // [HttpPost("clickup-webhook")]
        // public async Task<IActionResult> HandleClickUpWebhook()
        // {
        //     string rawBody = await new StreamReader(Request.Body).ReadToEndAsync();
        //     string signature = Request.Headers["X-ClickUp-Signature"]; // Example header
        //     string secret = _configuration["ClickUpWebhookSecret"]; // From app config

        //     if (!WebhookSignatureValidator.IsValidSignature(rawBody, signature, secret))
        //     {
        //         return Unauthorized("Invalid signature.");
        //     }

        //     var payload = WebhookParser.ParsePayload(rawBody); // Or JsonSerializer.Deserialize
        //     // Process payload based on payload.EventType
        //     // ...

        //     return Ok();
        // }
        ```
*   **Validating Signatures:**
    *   Emphasize the importance of signature validation for security.
    *   Show how to use the `WebhookSignatureValidator.IsValidSignature` method.
*   **Deserializing Payloads:**
    *   Show how to use `JsonSerializer.Deserialize` with the DTOs provided by the SDK (e.g., `WebhookPayload<T>`, `TaskWebhookEventData`).
    *   Explain how to check the `EventType` to cast or handle the specific event data.
*   **Best Practices:**
    *   Respond quickly to webhook requests (e.g., with HTTP 200 OK) and process the payload asynchronously (e.g., queue it for background processing) to avoid timeouts.
    *   Handle potential duplicate events (idempotency).
    *   Securely store the webhook secret.

## 5. Testing

*   **Unit Tests for `WebhookSignatureValidator`:**
    *   Test with known valid signature/payload/secret combinations.
    *   Test with invalid signatures, incorrect secrets, or modified payloads.
*   **Unit Tests for Payload DTO Deserialization:**
    *   Use sample JSON payloads from ClickUp's documentation or captured live webhooks.
    *   Verify correct deserialization into the DTOs.

## Plan Output

*   This document `10-WebhookHelpers.md` will contain the finalized plan.
*   It will specify the DTOs for common webhook payloads.
*   It will detail the `WebhookSignatureValidator` class and its methods, including the assumed or confirmed signature algorithm.
*   It will outline the content for the conceptual documentation article on webhooks.
*   **Key Action Item:** The exact signature validation mechanism and comprehensive payload structures for various events need to be definitively confirmed from ClickUp's official documentation or by inspecting live webhook calls. The `secret` field in the `CreateWebhookresponse` strongly implies a signature mechanism exists.
```
