using System;
using System.Collections.Concurrent;
using System.Linq;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    /// <summary>
    /// a class that representd poller data cache
    /// </summary>
    public class PollerDataCache : IPollerDataCache
    {
        /// <summary>
        /// a class that represents internal entry
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
            /// getr remaining time to live base on explitic time to live
            /// </summary>
            /// <param name="utcNow"> current datetime in UTC </param>
            /// <param name="explicitTtl"> explicit time to live </param>
            /// <returns></returns>
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
        /// gets data cache entry by entry key and ttl
        /// </summary>
        /// <param name="entryKey"> entry key</param>
        /// <param name="ttl"> time to live </param>
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
        /// sets data cache entry by entry key, ttl and serialized cache entry
        /// </summary>
        /// <param name="entryKey"> entry key </param>
        /// <param name="ttl"> time to live </param>
        /// <param name="entry"> serialized cache entry </param>
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

        public long GetCacheSize()
        {
            return cache.Values.Sum(v => v.Data.SerializedData.Length);
        }
    }
}
