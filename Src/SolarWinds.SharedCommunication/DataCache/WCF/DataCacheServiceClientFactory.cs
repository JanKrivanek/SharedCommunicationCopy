using System;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    /// <summary>
    /// Factory for creating data cache service clients.
    /// </summary>
    public class DataCacheServiceClientFactory<T> 
    {
        private readonly IAsyncSemaphoreFactory semaphoreFactory;

        public DataCacheServiceClientFactory(IAsyncSemaphoreFactory semaphoreFactory)
        {
            this.semaphoreFactory = semaphoreFactory;
        }

        /// <summary>
        /// Creates a cache based on name and ttl. Optionally we can change ttl to be per cache call.
        /// </summary>
        /// <param name="cacheName">Cache name.</param>
        /// <param name="ttl">Time to live.</param>
        /// <returns>Data cache service client.</returns>
        public IDataCache<T> CreateCache(string cacheName, TimeSpan ttl)
        {
            return DataCacheServiceClient<T>.Create(cacheName, ttl, semaphoreFactory);
        }
    }
}