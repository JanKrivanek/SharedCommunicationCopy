namespace SolarWinds.SharedCommunication.Contracts.DataCache
{
    //Just empty interface
    // used to enforce type whitelist on deserialization of data coming from possible insecure surfaces (shared memory, remoting etc.)
    public interface ICacheEntry
    { 
    
    }
}
