using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache
{
    ///<inheritdoc/>
    public class SingleProcessDataCache<T> : DelayedDisposalSharedObject<SingleProcessDataCache<T>>, IDataCache<T>
    {
        //SemaphoreSlim cannot be created from handle - so we need to make sure to create single
        private readonly SemaphoreSlim sp;
        private readonly TimeSpan ttl;
        private readonly IDateTime dateTime;

        private T data;
        private DateTime lastChangedUtc;

        public static IDataCache<T> Create(string cacheName, TimeSpan ttl, IDateTime dateTime)
        {
            return Acquire(cacheName, name => new SingleProcessDataCache<T>(ttl, dateTime));
        }

        public static IDataCache<T> Create(TimeSpan ttl, IDateTime dateTime)
        {
            return Create(typeof(T).Name, ttl, dateTime);
        }

        public static IDataCache<T> Create(DataCacheSettings settings, IDateTime dateTime)
        {
            return Create(settings.CacheName, settings.Ttl, dateTime);
        }

        ///<inheritdoc/>
        public async Task<T> GetDataAsync(Func<Task<T>> asyncDataFactory, CancellationToken token = default)
        {
            await sp.WaitAsync(token);
            //on cancellation exception would be thrown and we won't get here
            try
            {
                bool hasData = lastChangedUtc >= dateTime.UtcNow - ttl;
                if (!hasData)
                {
                    data = await asyncDataFactory().ConfigureAwait(false);
                    lastChangedUtc = dateTime.UtcNow;
                }

                return data;
            }
            finally
            {
                sp.Release();
            }
        }

        ///<inheritdoc/>
        public void EraseData()
        {
            lastChangedUtc = DateTime.MinValue;
            data = default(T);
        }

        ///<inheritdoc/>
        public Task SetDataAsync(T data, CancellationToken token = default)
        {
            //no need to synchronize - individual fields are references - no possibility of torn reads/writes of those in .NET
            // the time flag and data writes cannot be reordered (again - thanks to .net memory model), so in a worst case
            // we can happen to have race of 2 writes where data is from one write and timestamp from other - but since it was race,
            // the timestamps will be very close together - so no harm in mixing two
            this.data = data;
            lastChangedUtc = dateTime.UtcNow;

            return Task.CompletedTask;
        }


        public void Dispose()
        {
            this.Release();
        }

        protected override void DisposeImpl()
        {
            sp.Dispose();
        }

        private SingleProcessDataCache(TimeSpan ttl, IDateTime dateTime)
        {
            sp = new SemaphoreSlim(1, 1);
            this.ttl = ttl;
            this.dateTime = dateTime;
        }
    }
}
