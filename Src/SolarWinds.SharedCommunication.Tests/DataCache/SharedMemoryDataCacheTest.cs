using FluentAssertions;
using Moq;
using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.DataCache;
using SolarWinds.SharedCommunication.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace SolarWinds.SharedCommunication.Tests
{
    public class SharedMemoryDataCacheTest
    {
        private IDataCache<CacheEntryBase> sharedMemoryDataCache;

        private Mock<IAsyncSemaphore> asyncSemaphore;
        private AsyncSemaphoreFactory asyncSemaphoreFactory;
        private Mock<ISharedMemorySegment> memorySegment;
        private TimeSpan ttl;
        private Mock<IDateTime> dateTime;
        private string cacheName;
        private Mock<IKernelObjectsPrivilegesChecker> privilegesChecker;
        private Mock<ILogger> logger;

        [SetUp]
        public void SetUp()
        {
            asyncSemaphore = new Mock<IAsyncSemaphore>();
            memorySegment = new Mock<ISharedMemorySegment>();
            ttl = new TimeSpan();
            dateTime = new Mock<IDateTime>();
            logger = new Mock<ILogger>();
            asyncSemaphoreFactory = new AsyncSemaphoreFactory(logger.Object);
            privilegesChecker = new Mock<IKernelObjectsPrivilegesChecker>();
            cacheName = "cache1";
            sharedMemoryDataCache = new SharedMemoryDataCache<CacheEntryBase>(cacheName, ttl, dateTime.Object, asyncSemaphoreFactory, privilegesChecker.Object);
        }

        //only checks if the method doesn't crash as we have new in constructor and it is impossible to mock that
        [Test]
        public async Task GetDataAsync_WithValidArguments_RunsSuccessfully()
        {
            var asyncDataFactory = Task.FromResult(new StringCacheEntry("cache1") as CacheEntryBase);
            var token = new CancellationToken();
            var result = await this.sharedMemoryDataCache.GetDataAsync(() => asyncDataFactory, token);
            result.Should().BeNull();
        }
    }
}
