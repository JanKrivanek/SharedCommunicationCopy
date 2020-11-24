using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolarWinds.SharedCommunication.DataCache
{
    public static class DelayedCacheDisposingSetting
    {
        public static TimeSpan DestroyDelay { get; set; } = TimeSpan.FromMinutes(10);
    }

    public abstract class DelayedDisposalSharedObject<T> where T : DelayedDisposalSharedObject<T>
    {
        //the type is intentionaly generic to ensure separate instances dictionaries per type
        private static readonly ConcurrentDictionary<string, T> _instances = new ConcurrentDictionary<string, T>();
        private int _refCount = 0;

        protected static T Acquire(string key, Func<string, T> factory)
        {
            T instance = _instances.GetOrAdd(key, factory);
            Interlocked.Increment(ref instance._refCount);
            return instance;
        }

        protected abstract void DisposeImpl();

        protected void Release()
        {
            //Cannot dispose unconditionally here, as there might be other users of the cache
            if (Interlocked.Decrement(ref _refCount) == 0)
            {
                //waiting a delay if there is no other need for the item in the meantime
                Task.Delay(DelayedCacheDisposingSetting.DestroyDelay).ContinueWith(t =>
                {
                    //try to see if item is still present (it might have been removed already)
                    string key = _instances.FirstOrDefault(kp => kp.Value == this).Key;
                    //and if there is no other user active at this point - we can remove now
                    if (!string.IsNullOrEmpty(key) && Interlocked.CompareExchange(ref _refCount, -10, 0) == 0)
                    {
                        T _;
                        //there is a chance for ABA concurrency problem (acquire and release from other thread during the delay - so refCount still 0)
                        // that's why we call DisposeImpl only if we really removed the item
                        if (_instances.TryRemove(key, out _)) DisposeImpl();
                    }
                });
            }
        }
    }
}
