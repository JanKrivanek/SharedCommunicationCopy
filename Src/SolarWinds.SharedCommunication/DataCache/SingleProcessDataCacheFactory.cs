using System;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache
{
    /// <summary>
    /// a class for single process data cache factory
    /// </summary>
    public class SingleProcessDataCacheFactory : IDataCacheFactory
    {
        private readonly IDateTime dateTime;

        public SingleProcessDataCacheFactory(IDateTime dateTime)
        {
            this.dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        /// <summary>
        /// creates a single process data cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheName"> cache name </param>
        /// <param name="ttl"> time to live </param>
        /// <returns></returns>
        public IDataCache<T> CreateCache<T>(string cacheName, TimeSpan ttl)
        {
            return SingleProcessDataCache<T>.Create(cacheName, ttl, dateTime);
        }
    }
}