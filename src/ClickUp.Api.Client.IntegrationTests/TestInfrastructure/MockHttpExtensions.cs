using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes; // Required for JsonNode

namespace ClickUp.Api.Client.IntegrationTests.TestInfrastructure
{
    public static class MockHttpExtensions
    {
        public static MockedRequest WithJsonContentMatcher(this MockedRequest mockedRequest, object expectedContent)
        {
            string expectedJsonString = JsonSerializer.Serialize(expectedContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

            return mockedRequest.With(req =>
            {
                if (req.Content == null) return false;

                var requestContentString = req.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (string.IsNullOrEmpty(requestContentString)) return false;

                try
                {
                    // Normalize both JSON strings for a more robust comparison (optional, can be complex)
                    // For now, direct string comparison after serializing expected object might be too brittle.
                    // Let's parse both to JsonNode and compare.
                    JsonNode? requestNode = JsonNode.Parse(requestContentString);
                    JsonNode? expectedNode = JsonNode.Parse(expectedJsonString);

                    if (requestNode == null || expectedNode == null) return false;

                    // JsonNode.DeepEquals is not available directly.
                    // A simple way is to compare their string representations after parsing.
                    // This handles minor formatting differences but not necessarily property order.
                    // For more robust comparison, a custom recursive comparison or serializing both with sorted properties would be needed.
                    // However, for test recordings, the structure should be quite fixed.
                    return requestNode.ToJsonString() == expectedNode.ToJsonString();
                }
                catch (JsonException)
                {
                    // If parsing fails, content doesn't match
                    return false;
                }
            });
        }
    }
}
