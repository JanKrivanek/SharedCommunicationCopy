using System;

namespace SolarWinds.SharedCommunication.DataCache
{
    /// <summary>
    /// a class for data cache settings
    /// </summary>
    public class DataCacheSettings
    {
        public TimeSpan Ttl { get; set; }
        public string CacheName { get; set; }
    }
}