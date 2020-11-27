using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolarWinds.SharedCommunication.Contracts.DataCache
{
    //TTL globally - or maybe per type, or maybe per call as in the WCF cache case
    // Type constraint used to enforce type whitelist on deserialization of data coming from possible insecure surfaces (shared memory, remoting etc.)
    
    /// <summary>
    /// Data caching abstraction representing single slot of data
    /// The data has specific fixed addressing and Ttl
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataCache<T>: IDisposable //where T: ICacheEntry
    {
        /// <summary>
        /// Request the data slot from the cache. If cache has existing data with acceptable age (given by Ttl) it will be returned
        ///  and asyncDataFactory won't be called
        /// If the cache doesn't have data or it's stale, asyncDataFactory will be called, data stored into cache and returned
        ///
        /// Exception handling is up to client code! For wrappers - exception should be logged and null data returned to client.
        ///  Cache will be in consistent state (just the currently factoried on data might or might not be there)
        /// </summary>
        /// <param name="asyncDataFactory">Factory producing the data</param>
        /// <param name="token">cancellation token</param>
        /// <returns>Either pre-existing data from cache or newly created data from asyncDataFactory</returns>
        Task<T> GetDataAsync(Func<Task<T>> asyncDataFactory, CancellationToken token = default);

        /// <summary>
        /// Removes the data slot from the cache
        /// </summary>
        void EraseData();

        /// <summary>
        /// Force set the give data into cache slot. If there is any pre-existing data, it will be erased first
        /// </summary>
        /// <param name="data">Data to be pushed to cache</param>
        /// <param name="token">cancellation token</param>
        Task SetDataAsync(T data, CancellationToken token = default);
    }
}
