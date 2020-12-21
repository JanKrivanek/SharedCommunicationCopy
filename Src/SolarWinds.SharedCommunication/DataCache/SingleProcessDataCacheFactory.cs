using System;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache
{
    public class SingleProcessDataCacheFactory : IDataCacheFactory
    {
        private readonly IDateTime dateTime;

        public SingleProcessDataCacheFactory(IDateTime dateTime)
        {
            this.dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        public IDataCache<T> CreateCache<T>(string cacheName, TimeSpan ttl)
        {
            return SingleProcessDataCache<T>.Create(cacheName, ttl, dateTime);
        }
    }
}