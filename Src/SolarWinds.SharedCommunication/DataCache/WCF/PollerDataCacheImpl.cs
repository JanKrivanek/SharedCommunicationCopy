using System;
using System.Collections.Concurrent;
using System.Linq;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    public class PollerDataCacheImpl : IPollerDataCache
    {
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

            public TimeSpan RemainingTtl(DateTime utcNow, TimeSpan explicitTtl = default)
            {
                return (explicitTtl != TimeSpan.Zero ? explicitTtl : Ttl) - (utcNow - InsertedUtc);
            }
        }

        private readonly ConcurrentDictionary<string, InternalEntry> cache = new ConcurrentDictionary<string, InternalEntry>();
        private readonly IDateTime dateTime;

        public PollerDataCacheImpl(IDateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        public SerializedCacheEntry GetDataCacheEntry(string entryKey, TimeSpan ttl = default)
        {
            InternalEntry entry;
            if (cache.TryGetValue(entryKey, out entry) && entry.RemainingTtl(dateTime.UtcNow, ttl) < TimeSpan.Zero)
            {
                entry = null;
            }

            return entry?.Data;
        }

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
