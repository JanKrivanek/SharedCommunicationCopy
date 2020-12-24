using System;
using System.ServiceModel;
using SolarWinds.SharedCommunication.Contracts.DataCache;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    /// <summary>
    /// A class that represents poller data cache client.
    /// </summary>
    internal class PollerDataCacheClient : ClientBase<IPollerDataCache>, IPollerDataCache
    {
        public PollerDataCacheClient()
        {
        }

        /// <summary>
        /// Sets data cache entry based on entry key, ttl and serialized cache entry.
        /// </summary>
        /// <param name="entryKey">Entry key.</param>
        /// <param name="ttl">Time to live.</param>
        /// <param name="entry">Serialized cache entry.</param>
        public void SetDataCacheEntry(string entryKey, TimeSpan ttl, SerializedCacheEntry entry)
        {
            Channel.SetDataCacheEntry(entryKey, ttl, entry);
        }

        /// <summary>
        /// Gets data cache entry based on entry key and ttl.
        /// </summary>
        /// <param name="entryKey">Entry key</param>
        /// <param name="ttl">Time to live.</param>
        public SerializedCacheEntry GetDataCacheEntry(string entryKey, TimeSpan ttl = default)
        {
            return Channel.GetDataCacheEntry(entryKey, ttl);
        }
    }
}