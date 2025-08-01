using System;
using System.Threading;
using ClickUp.Api.Client.Services.Caching;
using Xunit;

namespace ClickUp.Api.Client.Tests.ServiceTests.Caching
{
    public class CacheMetricsTests
    {
        private readonly CacheMetrics _metrics;

        public CacheMetricsTests()
        {
            _metrics = new CacheMetrics();
        }

        [Fact]
        public void Constructor_InitializesWithZeroValues()
        {
            // Assert
            Assert.Equal(0, _metrics.HitCount);
            Assert.Equal(0, _metrics.MissCount);
            Assert.Equal(0, _metrics.SetCount);
            Assert.Equal(0, _metrics.EvictionCount);
            Assert.Equal(0, _metrics.ItemCount);
            Assert.Equal(0, _metrics.MemoryUsage);
            Assert.Equal(0, _metrics.AverageOperationTime);
            Assert.Equal(0, _metrics.HitRatio);
        }

        [Fact]
        public void RecordHit_IncrementsHitCount()
        {
            // Act
            _metrics.RecordHit();
            _metrics.RecordHit();
            _metrics.RecordHit();

            // Assert
            Assert.Equal(3, _metrics.HitCount);
        }

        [Fact]
        public void RecordMiss_IncrementsMissCount()
        {
            // Act
            _metrics.RecordMiss();
            _metrics.RecordMiss();

            // Assert
            Assert.Equal(2, _metrics.MissCount);
        }

        [Fact]
        public void RecordSet_IncrementsSetCount()
        {
            // Act
            _metrics.RecordSet();
            _metrics.RecordSet();
            _metrics.RecordSet();
            _metrics.RecordSet();

            // Assert
            Assert.Equal(4, _metrics.SetCount);
        }

        [Fact]
        public void RecordEviction_IncrementsEvictionCount()
        {
            // Act
            _metrics.RecordEviction();

            // Assert
            Assert.Equal(1, _metrics.EvictionCount);
        }

        [Fact]
        public void UpdateItemCount_SetsItemCount()
        {
            // Act
            _metrics.UpdateItemCount(100);

            // Assert
            Assert.Equal(100, _metrics.ItemCount);
        }

        [Fact]
        public void UpdateMemoryUsage_SetsMemoryUsage()
        {
            // Act
            _metrics.UpdateMemoryUsage(1024);

            // Assert
            Assert.Equal(1024, _metrics.MemoryUsage);
        }

        [Fact]
        public void RecordOperationTime_UpdatesAverageOperationTime()
        {
            // Act
            _metrics.RecordOperationTime(100.0);
            _metrics.RecordOperationTime(200.0);
            _metrics.RecordOperationTime(300.0);

            // Assert
            Assert.Equal(200.0, _metrics.AverageOperationTime, 1); // Allow for small precision differences
        }

        [Fact]
        public void HitRatio_CalculatesCorrectly_WithHitsAndMisses()
        {
            // Act
            _metrics.RecordHit();
            _metrics.RecordHit();
            _metrics.RecordHit();
            _metrics.RecordMiss();

            // Assert
            Assert.Equal(0.75, _metrics.HitRatio, 2); // 3 hits out of 4 total = 75%
        }

        [Fact]
        public void HitRatio_ReturnsZero_WithNoOperations()
        {
            // Assert
            Assert.Equal(0, _metrics.HitRatio);
        }

        [Fact]
        public void HitRatio_ReturnsZero_WithOnlyMisses()
        {
            // Act
            _metrics.RecordMiss();
            _metrics.RecordMiss();

            // Assert
            Assert.Equal(0, _metrics.HitRatio);
        }

        [Fact]
        public void HitRatio_ReturnsOne_WithOnlyHits()
        {
            // Act
            _metrics.RecordHit();
            _metrics.RecordHit();
            _metrics.RecordHit();

            // Assert
            Assert.Equal(1.0, _metrics.HitRatio);
        }

        [Fact]
        public void GetDetailedMetrics_ReturnsCompleteMetricsInfo()
        {
            // Arrange
            _metrics.RecordHit();
            _metrics.RecordHit();
            _metrics.RecordMiss();
            _metrics.RecordSet();
            _metrics.RecordEviction();
            _metrics.UpdateItemCount(50);
            _metrics.UpdateMemoryUsage(2048);
            _metrics.RecordOperationTime(150.0);

            // Act
            var detailedMetrics = _metrics.GetDetailedMetrics();

            // Assert
            Assert.Contains("HitCount", detailedMetrics.Keys);
            Assert.Contains("MissCount", detailedMetrics.Keys);
            Assert.Contains("SetCount", detailedMetrics.Keys);
            Assert.Contains("EvictionCount", detailedMetrics.Keys);
            Assert.Contains("ItemCount", detailedMetrics.Keys);
            Assert.Contains("MemoryUsage", detailedMetrics.Keys);
            Assert.Contains("HitRatio", detailedMetrics.Keys);
            Assert.Contains("AverageOperationTime", detailedMetrics.Keys);
            
            Assert.Equal(2L, detailedMetrics["HitCount"]);
            Assert.Equal(1L, detailedMetrics["MissCount"]);
            Assert.Equal(1L, detailedMetrics["SetCount"]);
            Assert.Equal(1L, detailedMetrics["EvictionCount"]);
            Assert.Equal(50L, detailedMetrics["ItemCount"]);
            Assert.Equal(2048L, detailedMetrics["MemoryUsage"]);
            Assert.Equal(150.0, (double)detailedMetrics["AverageOperationTime"], 1);
        }

        [Fact]
        public void Reset_ResetsAllMetricsToZero()
        {
            // Arrange
            _metrics.RecordHit();
            _metrics.RecordMiss();
            _metrics.RecordSet();
            _metrics.RecordEviction();
            _metrics.UpdateItemCount(100);
            _metrics.UpdateMemoryUsage(1024);
            _metrics.RecordOperationTime(200.0);

            // Act
            _metrics.Reset();

            // Assert
            Assert.Equal(0, _metrics.HitCount);
            Assert.Equal(0, _metrics.MissCount);
            Assert.Equal(0, _metrics.SetCount);
            Assert.Equal(0, _metrics.EvictionCount);
            Assert.Equal(0, _metrics.ItemCount);
            Assert.Equal(0, _metrics.MemoryUsage);
            Assert.Equal(0, _metrics.AverageOperationTime);
            Assert.Equal(0, _metrics.HitRatio);
        }

        [Fact]
        public void ThreadSafety_ConcurrentOperations_DoNotCorruptData()
        {
            // Arrange
            const int threadCount = 10;
            const int operationsPerThread = 100;
            var threads = new Thread[threadCount];
            var barrier = new Barrier(threadCount + 1);

            // Act
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    barrier.SignalAndWait(); // Wait for all threads to be ready
                    
                    for (int j = 0; j < operationsPerThread; j++)
                    {
                        _metrics.RecordHit();
                        _metrics.RecordMiss();
                        _metrics.RecordSet();
                        _metrics.RecordEviction();
                        _metrics.UpdateItemCount(j);
                        _metrics.UpdateMemoryUsage(j * 10);
                        _metrics.RecordOperationTime(j * 1.5);
                    }
                });
                threads[i].Start();
            }

            barrier.SignalAndWait(); // Release all threads
            
            foreach (var thread in threads)
            {
                thread.Join();
            }

            // Assert
            var expectedCount = threadCount * operationsPerThread;
            Assert.Equal(expectedCount, _metrics.HitCount);
            Assert.Equal(expectedCount, _metrics.MissCount);
            Assert.Equal(expectedCount, _metrics.SetCount);
            Assert.Equal(expectedCount, _metrics.EvictionCount);
            Assert.Equal(0.5, _metrics.HitRatio); // Equal hits and misses
        }

        [Fact]
        public void RecordOperationTime_WithZeroTime_DoesNotAffectAverage()
        {
            // Act
            _metrics.RecordOperationTime(100.0);
            _metrics.RecordOperationTime(0.0);
            _metrics.RecordOperationTime(200.0);

            // Assert
            Assert.Equal(100.0, _metrics.AverageOperationTime, 1);
        }

        [Fact]
        public void RecordOperationTime_WithNegativeTime_DoesNotAffectAverage()
        {
            // Act
            _metrics.RecordOperationTime(100.0);
            _metrics.RecordOperationTime(-50.0);
            _metrics.RecordOperationTime(200.0);

            // Assert
            Assert.Equal(150.0, _metrics.AverageOperationTime, 1);
        }

        [Fact]
        public void UpdateItemCount_WithNegativeValue_SetsToZero()
        {
            // Act
            _metrics.UpdateItemCount(-10);

            // Assert
            Assert.Equal(0, _metrics.ItemCount);
        }

        [Fact]
        public void UpdateMemoryUsage_WithNegativeValue_SetsToZero()
        {
            // Act
            _metrics.UpdateMemoryUsage(-1024);

            // Assert
            Assert.Equal(0, _metrics.MemoryUsage);
        }
    }
}