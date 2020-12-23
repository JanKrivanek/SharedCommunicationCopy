using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    ///<inheritdoc/>
    internal class DataCacheServiceClient<T> : DelayedDisposalSharedObject<DataCacheServiceClient<T>>, IDataCache<T>
    {
        private readonly PollerDataCacheClient cacheClient = new PollerDataCacheClient();

        //TODO: this must be done differently - different types have different ttl (e.g. topology has longer polling interval)
        // ttl can change during runtime. We might need to expose it through the individual calls
        private readonly TimeSpan ttl;
        private readonly IAsyncSemaphore asyncSemaphore;
        private readonly string key;
        private readonly DataContractSerializer serializer = new DataContractSerializer(typeof(T));

        /// <summary>
        /// creates a data cache service client based on name, ttl and semaphore factory
        /// </summary>
        /// <param name="cacheName"> name of cache </param>
        /// <param name="ttl"> time to live </param>
        /// <param name="semaphoreFactory"> semaphore factory </param>
        /// <returns></returns>
        public static DataCacheServiceClient<T> Create(string cacheName, TimeSpan ttl,
            IAsyncSemaphoreFactory semaphoreFactory)
        {
            return Acquire(cacheName, name => new DataCacheServiceClient<T>(name, ttl, semaphoreFactory));
        }

        private DataCacheServiceClient(string cacheName, TimeSpan ttl, IAsyncSemaphoreFactory semaphoreFactory)
        {
            this.ttl = ttl;
            asyncSemaphore = semaphoreFactory.Create(cacheName + "_MTX");
            key = cacheName;
        }
        
        ///<inheritdoc/>
        public async Task<T> GetDataAsync(Func<Task<T>> asyncDataFactory, CancellationToken token = default)
        {
            using (await asyncSemaphore.LockAsync(token).ConfigureAwait(false))
            {
                SerializedCacheEntry entry = cacheClient.GetDataCacheEntry(key, ttl);

                bool hasData = entry != null;
                T data;
                if (hasData)
                {
                    data = ToData(entry);
                }
                else
                {
                    token.ThrowIfCancellationRequested();
                    data = await asyncDataFactory().ConfigureAwait(false);
                    entry = FromData(data);
                    cacheClient.SetDataCacheEntry(key, ttl, entry);
                }

                return data;
            }
        }

        ///<inheritdoc/>
        public void EraseData()
        {
            //no need to synchronize - the server side concurrent dict will take care about serializing access
            cacheClient.SetDataCacheEntry(key, ttl, null);
        }

        ///<inheritdoc/>
        public Task SetDataAsync(T data, CancellationToken token = default)
        {
            if (data == null)
            {
                EraseData();
            }
            else
            {
                //no need to synchronize - the server side concurrent dict will take care about serializing access
                cacheClient.SetDataCacheEntry(key, ttl, FromData(data));
            }

            return Task.CompletedTask;
        }

        private T ToData(SerializedCacheEntry entry)
        {
            if (entry?.SerializedData == null) return default;

            MemoryStream ms = new MemoryStream(entry.SerializedData);

            var ds = new DataContractSerializer(typeof(T));
            return (T)ds.ReadObject(ms);
        }

        private SerializedCacheEntry FromData(T data)
        {
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, data);
            byte[] bytes = ms.ToArray();
            return new SerializedCacheEntry(bytes);
        }

        public void Dispose()
        {
            this.Release();
        }

        protected override void DisposeImpl()
        {
            cacheClient.Close();
            asyncSemaphore.Dispose();
        }
    }
}