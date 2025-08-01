using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Services.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Caching
{
    public class DistributedCacheServiceTests : IDisposable
    {
        private readonly Mock<IDistributedCache> _mockDistributedCache;
        private readonly Mock<ILogger<DistributedCacheService>> _mockLogger;
        private readonly ICacheConfiguration _configuration;
        private readonly DistributedCacheService _cacheService;

        public DistributedCacheServiceTests()
        {
            _mockDistributedCache = new Mock<IDistributedCache>();
            _mockLogger = new Mock<ILogger<DistributedCacheService>>();
            _configuration = new CacheConfiguration
            {
                DefaultExpiration = TimeSpan.FromMinutes(30),
                MaxCacheSize = 1000,
                CompressionThreshold = 1024,
                CleanupInterval = TimeSpan.FromMinutes(5)
            };
            
            _cacheService = new DistributedCacheService(_mockDistributedCache.Object, _mockLogger.Object, _configuration);
        }

        [Fact]
        public void Constructor_WithValidParameters_InitializesCorrectly()
        {
            // Assert
            Assert.NotNull(_cacheService.Metrics);
            Assert.Equal(_configuration, _cacheService.Configuration);
        }

        [Fact]
        public void Constructor_WithNullDistributedCache_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DistributedCacheService(null!, _mockLogger.Object, _configuration));
        }

        [Fact]
        public async Task GetAsync_WithValidKey_ReturnsValue()
        {
            // Arrange
            var key = "test-key";
            var expectedValue = new TestData { Id = 1, Name = "Test" };
            var wrapper = CreateCacheWrapper(expectedValue, false);
            var serializedWrapper = JsonSerializer.SerializeToUtf8Bytes(wrapper);
            
            _mockDistributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serializedWrapper);

            // Act
            var result = await _cacheService.GetAsync<TestData>(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedValue.Id, result.Id);
            Assert.Equal(expectedValue.Name, result.Name);
            _mockDistributedCache.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WithNullOrEmptyKey_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _cacheService.GetAsync<TestData>(null!));
            await Assert.ThrowsAsync<ArgumentException>(() => _cacheService.GetAsync<TestData>(""));
        }

        [Fact]
        public async Task GetAsync_WhenCacheMiss_ReturnsNullAndRecordsMetrics()
        {
            // Arrange
            var key = "missing-key";
            _mockDistributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            // Act
            var result = await _cacheService.GetAsync<TestData>(key);

            // Assert
            Assert.Null(result);
            Assert.True(_cacheService.Metrics.MissCount > 0);
        }

        [Fact]
        public async Task GetAsync_WithCorruptedData_ReturnsNullAndRecordsMetrics()
        {
            // Arrange
            var key = "corrupted-key";
            var corruptedData = Encoding.UTF8.GetBytes("invalid json data");
            
            _mockDistributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(corruptedData);

            // Act
            var result = await _cacheService.GetAsync<TestData>(key);

            // Assert
            Assert.Null(result);
            Assert.True(_cacheService.Metrics.MissCount > 0);
        }

        [Fact]
        public async Task SetAsync_WithValidData_StoresValue()
        {
            // Arrange
            var key = "test-key";
            var value = new TestData { Id = 1, Name = "Test" };
            var options = new CacheOptions { Expiration = TimeSpan.FromMinutes(10) };

            // Act
            await _cacheService.SetAsync(key, value, options);

            // Assert
            _mockDistributedCache.Verify(x => x.SetAsync(
                key, 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WithNullKey_ThrowsArgumentException()
        {
            // Arrange
            var value = new TestData { Id = 1, Name = "Test" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _cacheService.SetAsync(null!, value));
        }

        [Fact]
        public async Task SetAsync_WithNullValue_ThrowsArgumentNullException()
        {
            // Arrange
            var key = "test-key";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _cacheService.SetAsync<TestData>(key, null!));
        }

        [Fact]
        public async Task SetAsync_WithCompressionEnabled_CompressesLargeData()
        {
            // Arrange
            var key = "test-key";
            var largeValue = new TestData { Id = 1, Name = new string('x', 2000) }; // Exceeds compression threshold
            var options = new CacheOptions { EnableCompression = true };

            // Act
            await _cacheService.SetAsync(key, largeValue, options);

            // Assert
            _mockDistributedCache.Verify(x => x.SetAsync(
                key, 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrCreateAsync_WhenCacheHit_ReturnsExistingValue()
        {
            // Arrange
            var key = "test-key";
            var cachedValue = new TestData { Id = 1, Name = "Cached" };
            var factoryValue = new TestData { Id = 2, Name = "Factory" };
            var wrapper = CreateCacheWrapper(cachedValue, false);
            var serializedWrapper = JsonSerializer.SerializeToUtf8Bytes(wrapper);
            
            _mockDistributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(serializedWrapper);

            // Act
            var result = await _cacheService.GetOrCreateAsync(key, () => Task.FromResult(factoryValue));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cachedValue.Id, result.Id);
            Assert.Equal(cachedValue.Name, result.Name);
            _mockDistributedCache.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
            _mockDistributedCache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetOrCreateAsync_WhenCacheMiss_CreatesAndStoresValue()
        {
            // Arrange
            var key = "test-key";
            var factoryValue = new TestData { Id = 1, Name = "Factory" };
            
            _mockDistributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            // Act
            var result = await _cacheService.GetOrCreateAsync(key, () => Task.FromResult(factoryValue));

            // Assert
            Assert.Equal(factoryValue, result);
            _mockDistributedCache.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
            _mockDistributedCache.Verify(x => x.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_WithValidKey_RemovesValue()
        {
            // Arrange
            var key = "test-key";

            // Act
            await _cacheService.RemoveAsync(key);

            // Assert
            _mockDistributedCache.Verify(x => x.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(_cacheService.Metrics.EvictionCount > 0);
        }

        [Fact]
        public async Task RemoveByPatternAsync_LogsWarning()
        {
            // Arrange
            var pattern = "test-*";

            // Act
            await _cacheService.RemoveByPatternAsync(pattern);

            // Assert - Pattern-based removal is not supported in distributed cache
            // Verify that a warning was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Pattern-based removal not supported")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task RemoveByTagAsync_WithValidTag_RemovesTaggedValues()
        {
            // Arrange
            var tag = "test-tag";
            var key1 = "key1";
            var key2 = "key2";
            var value1 = new TestData { Id = 1, Name = "Test1" };
            var value2 = new TestData { Id = 2, Name = "Test2" };
            var options = new CacheOptions { Tags = new[] { tag } };

            // Set up tagged cache entries first
            await _cacheService.SetAsync(key1, value1, options);
            await _cacheService.SetAsync(key2, value2, options);

            // Act
            await _cacheService.RemoveByTagAsync(tag);

            // Assert - Verify that remove was called for tagged keys
            _mockDistributedCache.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ClearAsync_LogsWarning()
        {
            // Act
            await _cacheService.ClearAsync();

            // Assert - Clear operation is not fully supported in distributed cache
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Clear operation not fully supported")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingKey_ReturnsTrue()
        {
            // Arrange
            var key = "test-key";
            var dummyData = Encoding.UTF8.GetBytes("dummy");
            _mockDistributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dummyData);

            // Act
            var result = await _cacheService.ExistsAsync(key);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingKey_ReturnsFalse()
        {
            // Arrange
            var key = "missing-key";
            _mockDistributedCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            // Act
            var result = await _cacheService.ExistsAsync(key);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNullOrEmptyKey_ReturnsFalse()
        {
            // Act & Assert
            Assert.False(await _cacheService.ExistsAsync(null!));
            Assert.False(await _cacheService.ExistsAsync(""));
        }

        [Fact]
        public async Task WarmupAsync_WithStrategies_ExecutesAllStrategies()
        {
            // Arrange
            var mockStrategy1 = new Mock<ICacheWarmupStrategy>();
            var mockStrategy2 = new Mock<ICacheWarmupStrategy>();
            var strategies = new[] { mockStrategy1.Object, mockStrategy2.Object };

            mockStrategy1.Setup(x => x.ExecuteAsync(It.IsAny<ICacheService>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            mockStrategy2.Setup(x => x.ExecuteAsync(It.IsAny<ICacheService>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.WarmupAsync(strategies);

            // Assert
            mockStrategy1.Verify(x => x.ExecuteAsync(_cacheService, It.IsAny<CancellationToken>()), Times.Once);
            mockStrategy2.Verify(x => x.ExecuteAsync(_cacheService, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Metrics_ReturnsValidMetricsInstance()
        {
            // Act
            var metrics = _cacheService.Metrics;

            // Assert
            Assert.NotNull(metrics);
            Assert.IsAssignableFrom<ICacheMetrics>(metrics);
        }

        [Fact]
        public void Configuration_CanBeSetAndRetrieved()
        {
            // Arrange
            var newConfig = new CacheConfiguration
            {
                DefaultExpiration = TimeSpan.FromHours(1),
                MaxCacheSize = 2000
            };

            // Act
            _cacheService.Configuration = newConfig;

            // Assert
            Assert.Equal(newConfig, _cacheService.Configuration);
        }

        [Fact]
        public void Dispose_DisposesResourcesProperly()
        {
            // Act & Assert - Should not throw
            _cacheService.Dispose();
            _cacheService.Dispose(); // Should handle multiple dispose calls
        }

        public void Dispose()
        {
            _cacheService?.Dispose();
        }

        private static object CreateCacheWrapper<T>(T value, bool isCompressed)
        {
            var serializedData = JsonSerializer.SerializeToUtf8Bytes(value);
            return new
            {
                Data = serializedData,
                IsCompressed = isCompressed,
                CreatedAt = DateTimeOffset.UtcNow,
                Tags = new List<string>()
            };
        }

        private class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;

            public override bool Equals(object? obj)
            {
                return obj is TestData other && Id == other.Id && Name == other.Name;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id, Name);
            }
        }
    }
}