using System.Runtime.Serialization;

namespace SolarWinds.SharedCommunication.Contracts.DataCache
{
    /// <summary>
    /// class for string cache entry
    /// </summary>
    [DataContract]
    public class StringCacheEntry : CacheEntryBase
    {
        public StringCacheEntry(string value)
        {
            Value = value;
        }

        [DataMember]
        public string Value { get; private set; }

        public static implicit operator string(StringCacheEntry entry) => entry.Value;
        public static implicit operator StringCacheEntry(string s) => new StringCacheEntry(s);
    }
}
