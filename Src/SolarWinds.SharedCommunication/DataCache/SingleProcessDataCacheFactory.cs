using System;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache
{
    /// <summary>
    /// A class for single process data cache factory.
    /// </summary>
    public class SingleProcessDataCacheFactory : IDataCacheFactory
    {
        private readonly IDateTime dateTime;

        public SingleProcessDataCacheFactory(IDateTime dateTime)
        {
            this.dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        /// <summary>
        /// A method for creating a single process data cache based on cache name and TTL.
        /// </summary>
        /// <param name="cacheName">Cache name.</param>
        /// <param name="ttl">Time to live.</param>
        /// <returns>Created data cache.</returns>
        public IDataCache<T> CreateCache<T>(string cacheName, TimeSpan ttl)
        {
            return SingleProcessDataCache<T>.Create(cacheName, ttl, dateTime);
        }
    }
}