using System;
using Xunit.Abstractions;

namespace ClickUp.Api.Client.IntegrationTests.TestInfrastructure // Changed namespace
{
    public static class TestOutputHelperExtensions
    {
        public static void LogInformation(this ITestOutputHelper output, string message)
        {
            output.WriteLine($"[INFO] {DateTime.UtcNow:O} | {message}");
        }

        public static void LogError(this ITestOutputHelper output, string message, Exception? ex = null)
        {
            output.WriteLine($"[ERROR] {DateTime.UtcNow:O} | {message}" + (ex != null ? $"\nException: {ex}" : ""));
        }

        public static void LogWarning(this ITestOutputHelper output, string message) // Added LogWarning
        {
            output.WriteLine($"[WARN] {DateTime.UtcNow:O} | {message}");
        }
    }
}
