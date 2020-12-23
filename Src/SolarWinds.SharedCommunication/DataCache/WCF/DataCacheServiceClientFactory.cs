using System;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    /// <summary>
    /// factory for creating data cache service clients
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataCacheServiceClientFactory<T> 
    {
        private readonly IAsyncSemaphoreFactory semaphoreFactory;

        public DataCacheServiceClientFactory(IAsyncSemaphoreFactory semaphoreFactory)
        {
            this.semaphoreFactory = semaphoreFactory;
        }

        /// <summary>
        /// creates a cache based on name and ttl. Optionally we can change ttl to be per cache call
        /// </summary>
        /// <param name="cacheName"> cache name </param>
        /// <param name="ttl"> time to live </param>
        /// <returns></returns>
        public IDataCache<T> CreateCache(string cacheName, TimeSpan ttl)
        {
            return DataCacheServiceClient<T>.Create(cacheName, ttl, semaphoreFactory);
        }
    }
}