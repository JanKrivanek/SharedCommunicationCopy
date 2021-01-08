using System;
using System.Threading;
using System.Threading.Tasks;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache
{
    ///<inheritdoc/>
    public class SharedMemoryDataCache<T> : IDataCache<T> where T : ICacheEntry
    {
        //Semaphore and memory segments are named - so we are fine recreating them in a same process
        private readonly IAsyncSemaphore asyncSemaphore;
        private readonly ISharedMemorySegment memorySegment;
        private readonly TimeSpan ttl;
        private readonly IDateTime dateTime;

        public SharedMemoryDataCache(
            DataCacheSettings settings, 
            IDateTime dateTime,
            IAsyncSemaphoreFactory semaphoreFactory,
            IKernelObjectsPrivilegesChecker kernelObjectsPrivilegesChecker)
            : this(settings.CacheName, settings.Ttl, dateTime, semaphoreFactory, kernelObjectsPrivilegesChecker)
        {  }

        public SharedMemoryDataCache(
            string cacheName, 
            TimeSpan ttl, 
            IDateTime dateTime, 
            IAsyncSemaphoreFactory semaphoreFactory,
            IKernelObjectsPrivilegesChecker kernelObjectsPrivilegesChecker)
        {
            asyncSemaphore = semaphoreFactory.Create(cacheName + "_MTX", kernelObjectsPrivilegesChecker);
            memorySegment = new SharedMemorySegment(cacheName + "_MMF", kernelObjectsPrivilegesChecker);
            this.ttl = ttl;
            this.dateTime = dateTime;
        }

        ///<inheritdoc/>
        public async Task<T> GetDataAsync(Func<Task<T>> asyncDataFactory, CancellationToken token = default)
        {

            using (await asyncSemaphore.LockAsync(token).ConfigureAwait(false))
            {
                bool hasData = memorySegment.LastChangedUtc >= dateTime.UtcNow - ttl;
                T data;
                if (hasData)
                {
                    data = memorySegment.ReadData<T>();
                }
                else
                {
                    data = await asyncDataFactory().ConfigureAwait(false);
                    memorySegment.WriteData(data);
                }

                return data;
            }
        }

        ///<inheritdoc/>
        public void EraseData()
        {
            //synchronisation is needed, as erasing is multistep and one of the steps is clearing the memory.
            //parallel writers could then write to deallocated memory segment
            EraseDataAsync().Wait();
        }

        ///<inheritdoc/>
        public async Task SetDataAsync(T data, CancellationToken token = default)
        {
            using (await asyncSemaphore.LockAsync(token).ConfigureAwait(false))
            {
                memorySegment.WriteData(data);
            }
        }

        public void Dispose()
        {
            memorySegment.Dispose();
            asyncSemaphore.Dispose();
        }

        private async Task EraseDataAsync()
        {
            using (await asyncSemaphore.LockAsync().ConfigureAwait(false))
            {
                memorySegment.Clear();
            }
        }
    }
}