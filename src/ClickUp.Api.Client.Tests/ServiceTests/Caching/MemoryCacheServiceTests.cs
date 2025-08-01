using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Abstractions.Strategies;
using ClickUp.Api.Client.Services.Caching;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Caching
{
    public class MemoryCacheServiceTests : IDisposable
    {
        private readonly Mock<ICachingStrategy> _mockCachingStrategy;
        private readonly Mock<ILogger<MemoryCacheService>> _mockLogger;
        private readonly ICacheConfiguration _configuration;
        private readonly MemoryCacheService _cacheService;

        public MemoryCacheServiceTests()
        {
            _mockCachingStrategy = new Mock<ICachingStrategy>();
            _mockLogger = new Mock<ILogger<MemoryCacheService>>();
            _configuration = new CacheConfiguration
            {
                DefaultExpiration = TimeSpan.FromMinutes(30),
                MaxCacheSize = 1000,
                CompressionThreshold = 1024,
                CleanupInterval = TimeSpan.FromMinutes(5)
            };
            
            _cacheService = new MemoryCacheService(_mockCachingStrategy.Object, _mockLogger.Object, _configuration);
        }

        [Fact]
        public void Constructor_WithValidParameters_InitializesCorrectly()
        {
            // Assert
            Assert.NotNull(_cacheService.Metrics);
            Assert.Equal(_configuration, _cacheService.Configuration);
        }

        [Fact]
        public void Constructor_WithNullCachingStrategy_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MemoryCacheService(null!, _mockLogger.Object, _configuration));
        }

        [Fact]
        public async Task GetAsync_WithValidKey_ReturnsValue()
        {
            // Arrange
            var key = "test-key";
            var expectedValue = new TestData { Id = 1, Name = "Test" };
            var wrapper = new CacheWrapper<TestData>
            {
                Data = System.Text.Json.JsonSerializer.Serialize(expectedValue),
                IsCompressed = false,
                Priority = CachePriority.Normal,
                Tags = new List<string>(),
                CreatedAt = DateTime.UtcNow
            };
            _mockCachingStrategy.Setup(x => x.GetAsync<CacheWrapper<TestData>>(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _cacheService.GetAsync<TestData>(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedValue.Id, result.Id);
            Assert.Equal(expectedValue.Name, result.Name);
            _mockCachingStrategy.Verify(x => x.GetAsync<CacheWrapper<TestData>>(key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WithNullOrEmptyKey_ReturnsNull()
        {
            // Act & Assert
            var result1 = await _cacheService.GetAsync<TestData>(null!);
            var result2 = await _cacheService.GetAsync<TestData>("");
            
            Assert.Null(result1);
            Assert.Null(result2);
        }

        [Fact]
        public async Task GetAsync_WhenCacheMiss_RecordsMetrics()
        {
            // Arrange
            var key = "missing-key";
            _mockCachingStrategy.Setup(x => x.GetAsync<TestData>(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TestData?)null);

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
            _mockCachingStrategy.Verify(x => x.SetAsync(key, It.IsAny<object>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WithNullKey_DoesNotThrow()
        {
            // Arrange
            var value = new TestData { Id = 1, Name = "Test" };

            // Act & Assert - Should not throw, just return without doing anything
            await _cacheService.SetAsync(null!, value);
            
            // Verify no interaction with caching strategy
            _mockCachingStrategy.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SetAsync_WithNullValue_DoesNotThrow()
        {
            // Arrange
            var key = "test-key";

            // Act & Assert - Should not throw, just return without doing anything
            await _cacheService.SetAsync<TestData>(key, null!);
            
            // Verify no interaction with caching strategy
            _mockCachingStrategy.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetOrCreateAsync_WhenCacheHit_ReturnsExistingValue()
        {
            // Arrange
            var key = "test-key";
            var cachedValue = new TestData { Id = 1, Name = "Cached" };
            var factoryValue = new TestData { Id = 2, Name = "Factory" };
            
            var wrapper = new CacheWrapper<TestData>
            {
                Data = System.Text.Json.JsonSerializer.Serialize(cachedValue),
                IsCompressed = false,
                Priority = CachePriority.Normal,
                Tags = new List<string>(),
                CreatedAt = DateTime.UtcNow
            };
            _mockCachingStrategy.Setup(x => x.GetAsync<CacheWrapper<TestData>>(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _cacheService.GetOrCreateAsync(key, () => Task.FromResult(factoryValue));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cachedValue.Id, result.Id);
            Assert.Equal(cachedValue.Name, result.Name);
            _mockCachingStrategy.Verify(x => x.GetAsync<CacheWrapper<TestData>>(key, It.IsAny<CancellationToken>()), Times.Once);
            _mockCachingStrategy.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetOrCreateAsync_WhenCacheMiss_CreatesAndStoresValue()
        {
            // Arrange
            var key = "test-key";
            var factoryValue = new TestData { Id = 1, Name = "Factory" };
            
            _mockCachingStrategy.Setup(x => x.GetAsync<CacheWrapper<TestData>>(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CacheWrapper<TestData>?)null);

            // Act
            var result = await _cacheService.GetOrCreateAsync(key, () => Task.FromResult(factoryValue));

            // Assert
            Assert.Equal(factoryValue, result);
            _mockCachingStrategy.Verify(x => x.GetAsync<CacheWrapper<TestData>>(key, It.IsAny<CancellationToken>()), Times.Once);
            _mockCachingStrategy.Verify(x => x.SetAsync(key, It.IsAny<object>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_WithValidKey_RemovesValue()
        {
            // Arrange
            var key = "test-key";

            // Act
            await _cacheService.RemoveAsync(key);

            // Assert
            _mockCachingStrategy.Verify(x => x.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveByPatternAsync_WithValidPattern_RemovesMatchingValues()
        {
            // Arrange
            var pattern = "test-*";

            // Act
            await _cacheService.RemoveByPatternAsync(pattern);

            // Assert
            _mockCachingStrategy.Verify(x => x.RemoveByPatternAsync(pattern, It.IsAny<CancellationToken>()), Times.Once);
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

            // Set up tagged cache entries
            await _cacheService.SetAsync(key1, value1, options);
            await _cacheService.SetAsync(key2, value2, options);

            // Act
            await _cacheService.RemoveByTagAsync(tag);

            // Assert - Verify that remove was called for tagged keys
            _mockCachingStrategy.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ClearAsync_RemovesAllValues()
        {
            // Act
            await _cacheService.ClearAsync();

            // Assert
            _mockCachingStrategy.Verify(x => x.ClearAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingKey_ReturnsTrue()
        {
            // Arrange
            var key = "test-key";
            _mockCachingStrategy.Setup(x => x.ExistsAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

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
            _mockCachingStrategy.Setup(x => x.ExistsAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _cacheService.ExistsAsync(key);

            // Assert
            Assert.False(result);
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

        public void Dispose()
        {
            _cacheService?.Dispose();
        }

        private class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}