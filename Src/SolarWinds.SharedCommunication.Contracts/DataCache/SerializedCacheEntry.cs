using System.Runtime.Serialization;

namespace SolarWinds.SharedCommunication.Contracts.DataCache
{
    /// <summary>
    /// A class for serialized cache entry.
    /// </summary>
    [DataContract]
    public class SerializedCacheEntry : CacheEntryBase
    {
        public SerializedCacheEntry(byte[] serializedData)
        {
            SerializedData = serializedData;
        }

        [DataMember]
        public byte[] SerializedData { get; private set; }

    }
}
