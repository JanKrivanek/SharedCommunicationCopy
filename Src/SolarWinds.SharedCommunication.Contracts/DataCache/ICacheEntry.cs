namespace SolarWinds.SharedCommunication.Contracts.DataCache
{
    /// <summary>
    /// Just empty interface
    /// used to enforce type whitelist on deserialization of data coming from possible insecure surfaces (shared memory, remoting etc.)
    /// </summary>
    public interface ICacheEntry
    { 
    }
}
