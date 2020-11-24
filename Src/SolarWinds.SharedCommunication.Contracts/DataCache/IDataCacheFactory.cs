using System;

namespace SolarWinds.SharedCommunication.Contracts.DataCache
{
    public interface IDataCacheFactory
    {
        IDataCache<T> CreateCache<T>(string cacheName, TimeSpan ttl);
    }
}