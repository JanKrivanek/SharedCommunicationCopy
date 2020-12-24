using System;

namespace SolarWinds.SharedCommunication.Contracts.DataCache
{
    /// <summary>
    /// An interface for cache directory.
    /// </summary>
    public interface IDataCacheFactory
    {
        IDataCache<T> CreateCache<T>(string cacheName, TimeSpan ttl);
    }
}