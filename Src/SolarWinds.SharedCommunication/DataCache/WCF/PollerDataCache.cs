using System;
using System.Collections.Concurrent;
using System.Linq;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    /// <summary>
    /// A class that representd poller data cache.
    /// </summary>
    public class PollerDataCache : IPollerDataCache
    {
        /// <summary>
        /// A class that represents internal entry.
        /// </summary>
        private class InternalEntry
        {
            public InternalEntry(SerializedCacheEntry data, TimeSpan ttl, DateTime insertedUtc)
            {
                Data = data;
                Ttl = ttl;
                InsertedUtc = insertedUtc;
            }

            public SerializedCacheEntry Data { get; private set; }
            public TimeSpan Ttl { get; private set; }
            public DateTime InsertedUtc { get; private set; }

            /// <summary>
            /// Gets remaining time to live based on explicit time to live.
            /// </summary>
            /// <param name="utcNow">Current datetime in UTC.</param>
            /// <param name="explicitTtl">Explicit time to live.</param>
            public TimeSpan RemainingTtl(DateTime utcNow, TimeSpan explicitTtl = default)
            {
                return (explicitTtl != TimeSpan.Zero ? explicitTtl : Ttl) - (utcNow - InsertedUtc);
            }
        }

        private readonly ConcurrentDictionary<string, InternalEntry> cache = new ConcurrentDictionary<string, InternalEntry>();
        private readonly IDateTime dateTime;

        public PollerDataCache(IDateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        /// <summary>
        /// Gets data cache entry by entry key and TTL.
        /// </summary>
        /// <param name="entryKey">Entry key.</param>
        /// <param name="ttl">Time to live.</param>
        public SerializedCacheEntry GetDataCacheEntry(string entryKey, TimeSpan ttl = default)
        {
            InternalEntry entry;
            if (cache.TryGetValue(entryKey, out entry) && entry.RemainingTtl(dateTime.UtcNow, ttl) < TimeSpan.Zero)
            {
                entry = null;
            }

            return entry?.Data;
        }

        /// <summary>
        /// Sets data cache entry based on entry key, ttl and serialized cache entry.
        /// </summary>
        /// <param name="entryKey">Entry key.</param>
        /// <param name="ttl">Time to live.</param>
        /// <param name="entry">Serialized cache entry.</param>
        public void SetDataCacheEntry(string entryKey, TimeSpan ttl, SerializedCacheEntry entry)
        {
            if (entry?.SerializedData == null)
            {
                cache.TryRemove(entryKey, out _);
                return;
            }

            InternalEntry ie = new InternalEntry(entry, ttl, dateTime.UtcNow);
            cache[entryKey] = ie;
        }

        /// <summary>
        /// Gets cache size.
        /// </summary>
        public long GetCacheSize()
        {
            return cache.Values.Sum(v => v.Data.SerializedData.Length);
        }
    }
}
