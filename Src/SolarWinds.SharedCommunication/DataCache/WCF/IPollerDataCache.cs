using System;
using System.ServiceModel;
using SolarWinds.SharedCommunication.Contracts.DataCache;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    /// <summary>
    /// An interface for poller data cache
    /// </summary>
    [ServiceContract(Name = "PollerDataCache", Namespace = "http://schemas.solarwinds.com/2020/09/jobengine")]
    internal interface IPollerDataCache
    {
        /// <summary>
        /// Fetches the data from remote endpoint based on entry key and TTL - if present and fresh. Otherwise returns null/default.
        /// </summary>
        /// <param name="entryKey">The key of the data (usually typename).</param>
        /// <param name="ttl">Time to live. If cache contains older item - it gets cleared. Zero/default ttl is used when we want to respect insertion time TTL.</param>
        [OperationContract]
        SerializedCacheEntry GetDataCacheEntry(string entryKey, TimeSpan ttl = default);

        /// <summary>
        /// Sets the data in cache based on entry key, time to live and cache entry. Null entry can be used to clear the data.
        /// </summary>
        /// <param name="entryKey">Entry key.</param>
        /// <param name="ttl">Time to live.</param>
        /// <param name="entry">Cache entry</param>
        [OperationContract]
        void SetDataCacheEntry(string entryKey, TimeSpan ttl, SerializedCacheEntry entry);
    }
}