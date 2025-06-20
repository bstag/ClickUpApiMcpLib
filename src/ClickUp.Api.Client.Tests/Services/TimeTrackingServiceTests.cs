using Xunit;
using Moq;
using FluentAssertions;
using ClickUp.Api.Client.Services;
using ClickUp.Api.Client.Abstractions.Http; // For IApiConnection
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using ClickUp.Api.Client.Models.ResponseModels.TimeTracking;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

namespace ClickUp.Api.Client.Tests.Services
{
    public class TimeTrackingServiceTests
    {
        private readonly Mock<IApiConnection> _mockApiConnection;
        private readonly TimeTrackingService _timeTrackingService;

        public TimeTrackingServiceTests()
        {
            _mockApiConnection = new Mock<IApiConnection>();
            _timeTrackingService = new TimeTrackingService(_mockApiConnection.Object);
        }

        // Placeholder DTO creation helpers
        private TimeEntry CreateSampleTimeEntry(string id, string description)
        {
            var entry = (TimeEntry)Activator.CreateInstance(typeof(TimeEntry), nonPublic: true)!;
            var props = typeof(TimeEntry).GetProperties();
            props.FirstOrDefault(p => p.Name == "Id")?.SetValue(entry, id);
            props.FirstOrDefault(p => p.Name == "Description")?.SetValue(entry, description); // Assuming 'Description' property
            // Set other relevant properties as needed for tests
            return entry;
        }

        // Assuming a wrapper for single time entry GET if API wraps it (e.g. in "data")
        private GetTimeEntryResponse CreateSampleGetTimeEntryResponse(TimeEntry entry)
        {
            // If GetTimeEntryResponse is a record: new GetTimeEntryResponse(entry);
            // If it's a class with a Data property: new GetTimeEntryResponse { Data = entry };
            // For this test, I'll assume it's a record with a constructor.
            // This might need adjustment based on actual DTO definition.
            var responseType = typeof(GetTimeEntryResponse);
            // Try to find a constructor that takes TimeEntry, or set Data property
            var constructor = responseType.GetConstructors().FirstOrDefault(c =>
                c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(TimeEntry));
            if (constructor != null)
            {
                return (GetTimeEntryResponse)constructor.Invoke(new object[] { entry });
            }
            // Fallback if no such constructor, assuming a property named Data
            var instance = (GetTimeEntryResponse)Activator.CreateInstance(responseType, nonPublic: true)!;
            responseType.GetProperty("Data")?.SetValue(instance, entry);
            return instance;
        }

        [Fact]
        public async Task GetTimeEntryAsync_WhenEntryExists_ReturnsTimeEntry()
        {
            // Arrange
            var workspaceId = "ws-id";
            var timerId = "timer-id-123";
            var expectedEntry = CreateSampleTimeEntry(timerId, "Test Time Entry");
            var expectedResponse = CreateSampleGetTimeEntryResponse(expectedEntry);

            _mockApiConnection.Setup(c => c.GetAsync<GetTimeEntryResponse>(
                It.Is<string>(s => s.StartsWith($"{TimeTrackingService.BaseEndpoint}/{workspaceId}/time_entries/{timerId}")),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _timeTrackingService.GetTimeEntryAsync(workspaceId, timerId, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedEntry);
        }

        [Fact]
        public async Task CreateTimeEntryAsync_ValidRequest_ReturnsCreatedTimeEntry()
        {
            // Arrange
            var workspaceId = "ws-id";
            var requestDto = new CreateTimeEntryRequest(duration: 3600, description: "New Entry"); // Simplified
            var expectedEntry = CreateSampleTimeEntry("new-timer-id", "New Entry");
            var expectedResponse = CreateSampleGetTimeEntryResponse(expectedEntry);

            _mockApiConnection.Setup(c => c.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(
                It.Is<string>(s => s.StartsWith($"{TimeTrackingService.BaseEndpoint}/{workspaceId}/time_entries")),
                requestDto,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _timeTrackingService.CreateTimeEntryAsync(workspaceId, requestDto, cancellationToken: CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedEntry);
        }
    }
}
