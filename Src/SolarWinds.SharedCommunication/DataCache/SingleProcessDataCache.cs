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
    public class SingleProcessDataCache<T> : DelayedDisposalSharedObject<SingleProcessDataCache<T>>, IDataCache<T> //where T : ICacheEntry
    {
        //SemaphoreSlim cannot be created from handle - so we need to make sure to create single
        private static ConcurrentDictionary<string, IDataCache<T>> _instances = new ConcurrentDictionary<string, IDataCache<T>>();
        private readonly SemaphoreSlim _sp;
        private readonly TimeSpan _ttl;
        private readonly IDateTime _dateTime;

        private T _data;
        private DateTime _lastChangedUtc;

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

        private SingleProcessDataCache(TimeSpan ttl, IDateTime dateTime)
        {
            _sp = new SemaphoreSlim(1, 1);
            _ttl = ttl;
            _dateTime = dateTime;
        }

        ///<inheritdoc/>
        public async Task<T> GetData(Func<Task<T>> asyncDataFactory, CancellationToken token = default)
        {
            await _sp.WaitAsync(token);
            //on cancellation exception would be thrown and we won't get here
            try
            {
                bool hasData = _lastChangedUtc >= _dateTime.UtcNow - _ttl;
                if (!hasData)
                {
                    _data = await asyncDataFactory();
                    _lastChangedUtc = _dateTime.UtcNow;
                }

                return _data;
            }
            finally
            {
                _sp.Release();
            }
        }

        ///<inheritdoc/>
        public void EraseData()
        {
            _lastChangedUtc = DateTime.MinValue;
            _data = default(T);
        }

        ///<inheritdoc/>
        public Task SetData(T data, CancellationToken token = default)
        {
            //no need to synchronize - individual fields are references - no possibility of torn reads/writes of those in .NET
            // the time flag and data writes cannot be reordered (again - thanks to .net memory model), so in a worst case
            // we can happen to have race of 2 writes where data is from one write and timestamp from other - but since it was race,
            // the timestamps will be very close together - so no harm in mixing two
            _data = data;
            _lastChangedUtc = _dateTime.UtcNow;

            return Task.CompletedTask;
        }


        public void Dispose()
        {
            this.Release();
        }

        protected override void DisposeImpl()
        {
            _sp.Dispose();
        }
    }
}
