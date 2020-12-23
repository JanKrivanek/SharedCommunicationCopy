using System.Runtime.Serialization;

namespace SolarWinds.SharedCommunication.Contracts.DataCache
{
    /// <summary>
    /// abstract class for cache entry base
    /// </summary>
    [DataContract]
    [KnownType(typeof(SerializedCacheEntry))]
    [KnownType(typeof(StringCacheEntry))]
    public abstract class CacheEntryBase : ICacheEntry
    { 
    } 
}
