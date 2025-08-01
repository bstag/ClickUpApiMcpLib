using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Services.Caching;
using ClickUp.Api.Client.Strategies.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Caching
{
    /// <summary>
    /// Integration tests for the complete caching system including both memory and distributed cache services.
    /// </summary>
    public class CacheIntegrationTests : IDisposable
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<IDistributedCache> _mockDistributedCache;
        private readonly Mock<ILogger<MemoryCacheService>> _mockMemoryLogger;
        private readonly Mock<ILogger<DistributedCacheService>> _mockDistributedLogger;
        private readonly ICacheConfiguration _configuration;
        private readonly MemoryCacheService _memoryCacheService;
        private readonly DistributedCacheService _distributedCacheService;

        public CacheIntegrationTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _mockDistributedCache = new Mock<IDistributedCache>();
            _mockMemoryLogger = new Mock<ILogger<MemoryCacheService>>();
            _mockDistributedLogger = new Mock<ILogger<DistributedCacheService>>();
            
            _configuration = new CacheConfiguration
            {
                DefaultExpiration = TimeSpan.FromMinutes(30),
                MaxCacheSize = 1000,
                CompressionThreshold = 1024,
                CleanupInterval = TimeSpan.FromMinutes(5)
            };

            var mockStrategyLogger = new Mock<ILogger<MemoryCachingStrategy>>();
            var memoryCachingStrategy = new MemoryCachingStrategy(mockStrategyLogger.Object, TimeSpan.FromMinutes(30), 1000);
            _memoryCacheService = new MemoryCacheService(memoryCachingStrategy, _mockMemoryLogger.Object, _configuration);
            _distributedCacheService = new DistributedCacheService(_mockDistributedCache.Object, _mockDistributedLogger.Object, _configuration);
        }

        [Fact]
        public async Task MemoryCacheService_CompleteWorkflow_WorksCorrectly()
        {
            // Arrange
            var key = "integration-test-key";
            var value = new TestData { Id = 1, Name = "Integration Test" };
            var options = new CacheOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                Tags = new[] { "integration", "test" }
            };

            // Act & Assert - Set
            await _memoryCacheService.SetAsync(key, value, options);
            Assert.True(_memoryCacheService.Metrics.SetCount > 0);

            // Act & Assert - Get (Hit)
            var retrievedValue = await _memoryCacheService.GetAsync<TestData>(key);
            Assert.NotNull(retrievedValue);
            Assert.Equal(value.Id, retrievedValue.Id);
            Assert.Equal(value.Name, retrievedValue.Name);
            Assert.True(_memoryCacheService.Metrics.HitCount > 0);

            // Act & Assert - Exists
            var exists = await _memoryCacheService.ExistsAsync(key);
            Assert.True(exists);

            // Act & Assert - GetOrCreate (should return existing)
            var getOrCreateResult = await _memoryCacheService.GetOrCreateAsync(key, 
                () => Task.FromResult(new TestData { Id = 999, Name = "Should not be used" }));
            Assert.Equal(value.Id, getOrCreateResult.Id);

            // Act & Assert - Remove by tag
            await _memoryCacheService.RemoveByTagAsync("integration");
            var afterTagRemoval = await _memoryCacheService.GetAsync<TestData>(key);
            Assert.Null(afterTagRemoval);
            Assert.True(_memoryCacheService.Metrics.EvictionCount > 0);
        }

        [Fact]
        public async Task MemoryCacheService_GetOrCreate_WithMiss_CreatesValue()
        {
            // Arrange
            var key = "missing-key";
            var factoryValue = new TestData { Id = 42, Name = "Factory Created" };

            // Act
            var result = await _memoryCacheService.GetOrCreateAsync(key, () => Task.FromResult(factoryValue));

            // Assert
            Assert.Equal(factoryValue.Id, result.Id);
            Assert.Equal(factoryValue.Name, result.Name);
            
            // Verify it was actually cached
            var cachedValue = await _memoryCacheService.GetAsync<TestData>(key);
            Assert.NotNull(cachedValue);
            Assert.Equal(factoryValue.Id, cachedValue.Id);
        }

        [Fact]
        public async Task MemoryCacheService_Compression_WorksWithLargeData()
        {
            // Arrange
            var key = "large-data-key";
            var largeValue = new TestData 
            { 
                Id = 1, 
                Name = new string('x', 2000) // Exceeds compression threshold
            };
            var options = new CacheOptions { EnableCompression = true };

            // Act
            await _memoryCacheService.SetAsync(key, largeValue, options);
            var retrievedValue = await _memoryCacheService.GetAsync<TestData>(key);

            // Assert
            Assert.NotNull(retrievedValue);
            Assert.Equal(largeValue.Id, retrievedValue.Id);
            Assert.Equal(largeValue.Name, retrievedValue.Name);
        }

        [Fact]
        public async Task MemoryCacheService_PatternRemoval_WorksCorrectly()
        {
            // Arrange
            var keys = new[] { "pattern:test1", "pattern:test2", "other:test3" };
            var value = new TestData { Id = 1, Name = "Pattern Test" };

            foreach (var key in keys)
            {
                await _memoryCacheService.SetAsync(key, value);
            }

            // Act
            await _memoryCacheService.RemoveByPatternAsync("pattern:*");

            // Assert
            var result1 = await _memoryCacheService.GetAsync<TestData>("pattern:test1");
            var result2 = await _memoryCacheService.GetAsync<TestData>("pattern:test2");
            var result3 = await _memoryCacheService.GetAsync<TestData>("other:test3");

            Assert.Null(result1);
            Assert.Null(result2);
            Assert.NotNull(result3); // Should not be affected by pattern removal
        }

        [Fact]
        public async Task CacheWarmup_ExecutesStrategiesCorrectly()
        {
            // Arrange
            var mockStrategy1 = new Mock<ICacheWarmupStrategy>();
            var mockStrategy2 = new Mock<ICacheWarmupStrategy>();
            
            mockStrategy1.Setup(x => x.Name).Returns("Strategy1");
            mockStrategy2.Setup(x => x.Name).Returns("Strategy2");
            
            mockStrategy1.Setup(x => x.ExecuteAsync(It.IsAny<ICacheService>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns(Task.CompletedTask);
            mockStrategy2.Setup(x => x.ExecuteAsync(It.IsAny<ICacheService>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns(Task.CompletedTask);

            var strategies = new[] { mockStrategy1.Object, mockStrategy2.Object };

            // Act
            await _memoryCacheService.WarmupAsync(strategies);

            // Assert
            mockStrategy1.Verify(x => x.ExecuteAsync(_memoryCacheService, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            mockStrategy2.Verify(x => x.ExecuteAsync(_memoryCacheService, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void CacheMetrics_TrackOperationsCorrectly()
        {
            // Arrange
            var metrics = _memoryCacheService.Metrics;
            var initialHitCount = metrics.HitCount;
            var initialMissCount = metrics.MissCount;
            var initialSetCount = metrics.SetCount;

            // Act - Perform cache operations
            var key = "metrics-test";
            var value = new TestData { Id = 1, Name = "Metrics" };
            
            // This will be a miss since key doesn't exist
            _ = _memoryCacheService.GetAsync<TestData>(key).Result;
            
            // This will be a set
            _memoryCacheService.SetAsync(key, value).Wait();
            
            // This will be a hit
            _ = _memoryCacheService.GetAsync<TestData>(key).Result;

            // Assert
            Assert.True(metrics.MissCount > initialMissCount);
            Assert.True(metrics.SetCount > initialSetCount);
            Assert.True(metrics.HitCount > initialHitCount);
            Assert.True(metrics.HitRatio > 0);
        }

        [Fact]
        public void CacheConfiguration_CanBeModified()
        {
            // Arrange
            var newConfig = new CacheConfiguration
            {
                DefaultExpiration = TimeSpan.FromHours(2),
                MaxCacheSize = 5000,
                CompressionThreshold = 2048
            };

            // Act
            _memoryCacheService.Configuration = newConfig;

            // Assert
            Assert.Equal(newConfig.DefaultExpiration, _memoryCacheService.Configuration.DefaultExpiration);
            Assert.Equal(newConfig.MaxCacheSize, _memoryCacheService.Configuration.MaxCacheSize);
            Assert.Equal(newConfig.CompressionThreshold, _memoryCacheService.Configuration.CompressionThreshold);
        }

        [Fact]
        public async Task DistributedCacheService_BasicOperations_WorkCorrectly()
        {
            // Arrange
            var key = "distributed-test";
            var value = new TestData { Id = 1, Name = "Distributed Test" };

            // Mock the distributed cache to return null initially (miss)
            _mockDistributedCache.Setup(x => x.GetAsync(key, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            // Act & Assert - Cache miss
            var missResult = await _distributedCacheService.GetAsync<TestData>(key);
            Assert.Null(missResult);
            Assert.True(_distributedCacheService.Metrics.MissCount > 0);

            // Act & Assert - Set operation
            await _distributedCacheService.SetAsync(key, value);
            Assert.True(_distributedCacheService.Metrics.SetCount > 0);

            // Verify SetAsync was called on the mock
            _mockDistributedCache.Verify(x => x.SetAsync(
                key, 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BothCacheServices_HaveSimilarInterface()
        {
            // Arrange
            var services = new ICacheService[] { _memoryCacheService, _distributedCacheService };
            var key = "interface-test";
            var value = new TestData { Id = 1, Name = "Interface Test" };

            // Act & Assert - Both services implement the same interface
            foreach (var service in services)
            {
                Assert.NotNull(service.Metrics);
                Assert.NotNull(service.Configuration);
                
                // These operations should not throw
                await service.SetAsync(key, value);
                await service.ExistsAsync(key);
                await service.RemoveAsync(key);
                await service.ClearAsync();
            }
        }

        public void Dispose()
        {
            _memoryCacheService?.Dispose();
            _distributedCacheService?.Dispose();
            _memoryCache?.Dispose();
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