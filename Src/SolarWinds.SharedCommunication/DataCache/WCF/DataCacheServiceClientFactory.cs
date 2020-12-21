using System;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    public class DataCacheServiceClientFactory<T> 
    {
        private readonly IAsyncSemaphoreFactory semaphoreFactory;

        public DataCacheServiceClientFactory(IAsyncSemaphoreFactory semaphoreFactory)
        {
            this.semaphoreFactory = semaphoreFactory;
        }

        //optionaly we can change ttl to be per cache call
        public IDataCache<T> CreateCache(string cacheName, TimeSpan ttl)
        {
            return DataCacheServiceClient<T>.Create(cacheName, ttl, semaphoreFactory);
        }
    }
}