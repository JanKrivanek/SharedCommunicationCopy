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
using System.IO.MemoryMappedFiles;

namespace SolarWinds.SharedCommunication.Tests
{
    public class SingleProcessDataCacheFactoryTest
    {
        private Mock<IDateTime> dateTime;
        private SingleProcessDataCacheFactory singleProcessFactory;

        [SetUp]
        public void SetUp()
        {
            dateTime = new Mock<IDateTime>();
            singleProcessFactory = new SingleProcessDataCacheFactory(dateTime.Object);
        }

        [Test]
        public void CreateCache_WithValidArguments_ReturnsDataCache()
        {
            var cacheName = "testCache1";
            var ttl = new TimeSpan(0, 3, 0);

            var result = singleProcessFactory.CreateCache<CacheEntryBase>(cacheName, ttl);

            result.Should().NotBeNull().And.BeOfType<SingleProcessDataCache<CacheEntryBase>>();
        }
    }
}
