using System;
using System.ServiceModel;
using SolarWinds.SharedCommunication.Contracts.DataCache;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    /// <summary>
    /// class that represents poller data cache client
    /// </summary>
    internal class PollerDataCacheClient : ClientBase<IPollerDataCache>, IPollerDataCache
    {
        public PollerDataCacheClient()
        {
        }

        /// <summary>
        /// sets data cache entry by entry key, ttl and serialized cache entry
        /// </summary>
        /// <param name="entryKey"> entry key </param>
        /// <param name="ttl"> time to live </param>
        /// <param name="entry"> serialized cache entry </param>
        public void SetDataCacheEntry(string entryKey, TimeSpan ttl, SerializedCacheEntry entry)
        {
            Channel.SetDataCacheEntry(entryKey, ttl, entry);
        }

        /// <summary>
        /// gets data cache entry by entry key and ttl
        /// </summary>
        /// <param name="entryKey"> entry key</param>
        /// <param name="ttl"> time to live </param>
        /// <returns></returns>
        public SerializedCacheEntry GetDataCacheEntry(string entryKey, TimeSpan ttl = default)
        {
            return Channel.GetDataCacheEntry(entryKey, ttl);
        }
    }
}