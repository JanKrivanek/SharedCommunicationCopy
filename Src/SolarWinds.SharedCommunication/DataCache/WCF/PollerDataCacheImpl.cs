using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
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

        private readonly ConcurrentDictionary<string, InternalEntry> _cache = new ConcurrentDictionary<string, InternalEntry>();
        private readonly IDateTime _dateTime;

        public PollerDataCacheImpl(IDateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public SerializedCacheEntry GetDataCacheEntry(string entryKey, TimeSpan ttl = default)
        {
            InternalEntry entry;
            if (_cache.TryGetValue(entryKey, out entry) && entry.RemainingTtl(_dateTime.UtcNow, ttl) < TimeSpan.Zero)
            {
                entry = null;
            }

            return entry?.Data;
        }

        public void SetDataCacheEntry(string entryKey, TimeSpan ttl, SerializedCacheEntry entry)
        {
            if (entry?.SerializedData == null)
            {
                _cache.TryRemove(entryKey, out _);
                return;
            }

            InternalEntry ie = new InternalEntry(entry, ttl, _dateTime.UtcNow);
            _cache[entryKey] = ie;
        }

        public long GetCacheSize()
        {
            return _cache.Values.Sum(v => v.Data.SerializedData.Length);
        }
    }
}
