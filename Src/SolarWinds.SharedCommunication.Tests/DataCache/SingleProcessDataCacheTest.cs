using FluentAssertions;
using Moq;
using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.DataCache;
using System;
using System.Threading.Tasks;

namespace SolarWinds.SharedCommunication.Tests
{
    public class SingleProcessDataCacheTest
    {
        private Mock<IDateTime> dateTime;
        private SingleProcessDataCache<CacheEntryBase> singleProcessDataCache;

        [SetUp]
        public void SetUp()
        {
            dateTime = new Mock<IDateTime>();
            var ttl = new TimeSpan(0, 3, 0);
            singleProcessDataCache = SingleProcessDataCache<CacheEntryBase>.Create(ttl, dateTime.Object) 
                as SingleProcessDataCache<CacheEntryBase>;
        }

        [Test]
        public async Task GetDataAsync_WithValidArguments_ReturnsCacheEntryBase()
        {
            var asyncDataFactory = new Mock<Func<Task<CacheEntryBase>>>();
            dateTime.Setup(d => d.UtcNow).Returns(DateTime.Now);
            asyncDataFactory.Setup(a => a()).Returns(Task.FromResult(new StringCacheEntry("entry1") as CacheEntryBase));

            var result = await singleProcessDataCache.GetDataAsync(asyncDataFactory.Object);

            result.Should().NotBeNull().And.BeOfType<StringCacheEntry>();
        }
    }
}
